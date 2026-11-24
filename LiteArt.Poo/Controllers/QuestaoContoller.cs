using System.Data.Common;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Models.ViewModels;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    [Route("Questoes")]
    public class QuestaoController : Controller
    {
        private readonly IQuestaoRepository _questaoRepository;
        private readonly IMateriaRepository _materiaRepository;
        private readonly IAdministradorConsulta _administradorConsulta;
        private readonly ILogger<QuestaoController> _logger;

        public QuestaoController(
            IQuestaoRepository questaoRepository,
            IMateriaRepository materiaRepository,
            IAdministradorConsulta administradorConsulta,
            ILogger<QuestaoController> logger)
        {
            _questaoRepository = questaoRepository;
            _materiaRepository = materiaRepository;
            _administradorConsulta = administradorConsulta;
            _logger = logger;
        }

        [HttpGet("", Name = "QuestoesIndex")]
        public async Task<IActionResult> Index(int? materiaId)
        {
            try
            {
                var materias = await _materiaRepository.ListarTodos();
                var questoes = await _questaoRepository.ListarTodos();
                if (materiaId.HasValue)
                {
                    questoes = questoes.Where(q => q.Materias.Any(m => m.IdMateria == materiaId.Value));
                }

                return View("~/Views/Questoes/Index.cshtml", new BancoQuestoesViewModel
                {
                    MateriaId = materiaId,
                    Materias = materias.Select(m => new MateriaOpcao(m.IdMateria, m.Nome)).ToList(),
                    Questoes = questoes.Select(q => CriarResumo(q)).ToList()
                });
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao consultar o banco de questões.");
                ViewData["ErroConsulta"] = "Não foi possível carregar as questões. Tente novamente.";
                return View("~/Views/Questoes/Index.cshtml", new BancoQuestoesViewModel { MateriaId = materiaId });
            }
        }

        [HttpGet("Nova", Name = "QuestoesNova")]
        public Task<IActionResult> Nova()
        {
            return MostrarFormulario("Nova", new QuestaoFormularioViewModel());
        }

        [HttpPost("Nova")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Nova(QuestaoFormularioViewModel modelo)
        {
            if (modelo.Id.HasValue)
            {
                return Task.FromResult<IActionResult>(BadRequest());
            }

            return Salvar(modelo, null);
        }

        [HttpGet("Editar/{id:int}", Name = "QuestoesEditar")]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                var questao = await _questaoRepository.BuscarPorId(id);
                if (questao == null)
                {
                    return NotFound();
                }

                var multipla = questao as MultiplaEscolha;
                var quantidade = Math.Max(4, (multipla?.Opcoes.Count ?? 0) + 1);
                var modelo = new QuestaoFormularioViewModel
                {
                    Id = questao.IdQuestao,
                    Enunciado = questao.Enunciado,
                    IdAdministrador = questao.Administrador.IdAdministrador,
                    MateriaIds = questao.Materias.Select(m => m.IdMateria).ToArray(),
                    Tipo = multipla != null ? "MultiplaEscolha" : "VerdadeiroFalso",
                    Alternativas = new string?[quantidade],
                    AlternativaIds = new int[quantidade],
                    GabaritoVf = questao is VerdadeiroFalso vf ? vf.RespostaCorreta : null
                };

                if (multipla != null)
                {
                    for (var i = 0; i < multipla.Opcoes.Count; i++)
                    {
                        modelo.Alternativas[i] = multipla.Opcoes[i].Texto;
                        modelo.AlternativaIds[i] = multipla.Opcoes[i].IdAlternativa;
                        if (multipla.Opcoes[i].Correta)
                        {
                            modelo.AlternativaCorreta = i;
                        }
                    }
                }

                return await MostrarFormulario("Editar", modelo);
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao carregar questão {Id}.", id);
                TempData["Erro"] = "Não foi possível carregar a questão. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Editar/{id:int}")]
        [ValidateAntiForgeryToken]
        public Task<IActionResult> Editar(int id, QuestaoFormularioViewModel modelo)
        {
            if (id <= 0 || modelo.Id != id)
            {
                return Task.FromResult<IActionResult>(BadRequest());
            }

            return Salvar(modelo, id);
        }

        private async Task<IActionResult> Salvar(QuestaoFormularioViewModel modelo, int? id)
        {
            var pagina = id.HasValue ? "Editar" : "Nova";
            if (!ModelState.IsValid)
            {
                return await MostrarFormulario(pagina, modelo);
            }

            try
            {
                var original = id.HasValue ? await _questaoRepository.BuscarPorId(id.Value) : null;
                if (id.HasValue && original == null)
                {
                    return NotFound();
                }

                var administrador = await _administradorConsulta.BuscarPorId(modelo.IdAdministrador);
                if (administrador == null)
                {
                    ModelState.AddModelError(nameof(modelo.IdAdministrador), "O administrador informado não existe.");
                }

                var materias = (await _materiaRepository.ListarTodos()).ToList();
                var idsMaterias = modelo.MateriaIds.Distinct().ToHashSet();
                var selecionadas = materias.Where(m => idsMaterias.Contains(m.IdMateria)).ToList();
                if (selecionadas.Count != idsMaterias.Count)
                {
                    ModelState.AddModelError(nameof(modelo.MateriaIds), "Uma ou mais matérias informadas não existem.");
                }

                if (modelo.Tipo == "MultiplaEscolha")
                {
                    var idsPermitidos = original is MultiplaEscolha mc
                        ? mc.Opcoes.Select(a => a.IdAlternativa).ToHashSet()
                        : new HashSet<int>();
                    if (modelo.AlternativaIds.Any(a => a > 0 && !idsPermitidos.Contains(a)))
                    {
                        ModelState.AddModelError(nameof(modelo.AlternativaIds), "As alternativas informadas não pertencem à questão editada.");
                    }
                }

                if (!ModelState.IsValid)
                {
                    return await MostrarFormulario(pagina, modelo);
                }

                Questao questao;
                if (modelo.Tipo == "MultiplaEscolha")
                {
                    var alternativas = new List<Alternativa>();
                    for (var i = 0; i < modelo.Alternativas.Length; i++)
                    {
                        var texto = modelo.Alternativas[i];
                        if (string.IsNullOrWhiteSpace(texto))
                        {
                            continue;
                        }

                        var correta = modelo.AlternativaCorreta == i;
                        var idAlternativa = modelo.AlternativaIds[i];
                        alternativas.Add(idAlternativa > 0
                            ? new Alternativa(texto, correta, idAlternativa)
                            : new Alternativa(texto, correta));
                    }

                    questao = id.HasValue
                        ? new MultiplaEscolha(modelo.Enunciado, alternativas, administrador!, selecionadas, id.Value)
                        : new MultiplaEscolha(modelo.Enunciado, alternativas, administrador!, selecionadas);
                }
                else
                {
                    questao = id.HasValue
                        ? new VerdadeiroFalso(modelo.Enunciado, modelo.GabaritoVf!.Value, administrador!, selecionadas, id.Value)
                        : new VerdadeiroFalso(modelo.Enunciado, modelo.GabaritoVf!.Value, administrador!, selecionadas);
                }

                if (original is { Ativa: false })
                {
                    questao.Desativar();
                }

                if (id.HasValue)
                {
                    if (!await _questaoRepository.Atualizar(questao))
                    {
                        return NotFound();
                    }
                }
                else
                {
                    await _questaoRepository.Inserir(questao);
                }

                TempData["Sucesso"] = id.HasValue ? "Questão atualizada." : "Questão cadastrada.";
                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao salvar questão {Id}.", id);
                ModelState.AddModelError(string.Empty, "Não foi possível salvar a questão. Tente novamente.");
            }

            return await MostrarFormulario(pagina, modelo);
        }

        private async Task<IActionResult> MostrarFormulario(string pagina, QuestaoFormularioViewModel modelo)
        {
            // Mantém os arrays alinhados ao renderizar novamente um formulário inválido.
            var textos = modelo.Alternativas ?? [];
            var ids = modelo.AlternativaIds ?? [];
            var quantidade = Math.Max(4, textos.Length);
            Array.Resize(ref textos, quantidade);
            Array.Resize(ref ids, quantidade);
            modelo.Alternativas = textos;
            modelo.AlternativaIds = ids;
            modelo.MateriaIds ??= [];

            try
            {
                var materias = await _materiaRepository.ListarTodos();
                modelo.Materias = materias.Select(m => new MateriaOpcao(m.IdMateria, m.Nome)).ToList();
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao carregar matérias do formulário.");
                ModelState.AddModelError(string.Empty, "Não foi possível carregar as matérias. Tente novamente.");
            }

            return View($"~/Views/Questoes/{pagina}.cshtml", modelo);
        }

        [HttpGet("Excluir/{id:int}", Name = "QuestoesExcluir")]
        public async Task<IActionResult> Excluir(int id)
        {
            try
            {
                var questao = await _questaoRepository.BuscarPorId(id);
                if (questao == null)
                {
                    return NotFound();
                }

                return View("~/Views/Questoes/Excluir.cshtml", CriarResumo(
                    questao, await _questaoRepository.PossuiTentativas(id)));
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao carregar exclusão da questão {Id}.", id);
                TempData["Erro"] = "Não foi possível carregar a questão. Tente novamente.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost("Excluir/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmarExclusao(int id)
        {
            try
            {
                if (await _questaoRepository.Excluir(id))
                {
                    TempData["Sucesso"] = "Questão excluída.";
                }
                else
                {
                    TempData["Erro"] = "Não foi possível excluir a questão. Ela pode possuir tentativas registradas ou já ter sido excluída.";
                }
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao excluir questão {Id}.", id);
                TempData["Erro"] = "Não foi possível excluir a questão. Tente novamente.";
            }

            return RedirectToAction(nameof(Index));
        }

        private static QuestaoPreviaViewModel CriarResumo(Questao questao, bool possuiTentativas = false)
        {
            var multipla = questao as MultiplaEscolha;
            return new QuestaoPreviaViewModel
            {
                Id = questao.IdQuestao,
                Enunciado = questao.Enunciado,
                Materias = questao.Materias.Select(m => new MateriaOpcao(m.IdMateria, m.Nome)).ToList(),
                Tipo = multipla != null ? "MultiplaEscolha" : "VerdadeiroFalso",
                Alternativas = multipla?.Opcoes.Select(a => a.Texto).ToList() ?? [],
                AlternativaCorreta = multipla?.Opcoes.ToList().FindIndex(a => a.Correta) ?? 0,
                GabaritoVf = questao is VerdadeiroFalso vf && vf.RespostaCorreta,
                TemTentativas = possuiTentativas
            };
        }
    }
}
