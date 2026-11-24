using System.Data.Common;
using System.Globalization;
using LiteArt.Poo.Exceptions;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Models.ViewModels;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    [Route("Estudo")]
    public class EstudoController : Controller
    {
        private readonly IQuestaoRepository _questaoRepository;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IMateriaRepository _materiaRepository;
        private readonly IRegistroTentativa _registroTentativa;
        private readonly ILogger<EstudoController> _logger;

        public EstudoController(
            IQuestaoRepository questaoRepository,
            IAlunoRepository alunoRepository,
            IMateriaRepository materiaRepository,
            IRegistroTentativa registroTentativa,
            ILogger<EstudoController> logger)
        {
            _questaoRepository = questaoRepository;
            _alunoRepository = alunoRepository;
            _materiaRepository = materiaRepository;
            _registroTentativa = registroTentativa;
            _logger = logger;
        }

        [HttpGet("", Name = "EstudoIndex")]
        public async Task<IActionResult> Index(int? alunoId, int? materiaId)
        {
            try
            {
                var alunos = await _alunoRepository.ListarTodos();
                var materias = await _materiaRepository.ListarTodos();
                var questoes = (await _questaoRepository.ListarTodos()).Where(q => q.Ativa);

                if (materiaId.HasValue)
                {
                    questoes = questoes.Where(q => q.Materias.Any(m => m.IdMateria == materiaId.Value));
                }

                return View("~/Views/Estudo/Index.cshtml", new EstudoViewModel
                {
                    AlunoId = alunoId,
                    MateriaId = materiaId,
                    Alunos = alunos.Select(a => new AlunoOpcao(a.IdAluno, a.Nome)).ToList(),
                    Materias = materias.Select(m => new MateriaOpcao(m.IdMateria, m.Nome)).ToList(),
                    Questoes = questoes.Select(q => new QuestaoPreviaViewModel
                    {
                        Id = q.IdQuestao,
                        Enunciado = q.Enunciado,
                        Materias = q.Materias.Select(m => new MateriaOpcao(m.IdMateria, m.Nome)).ToList(),
                        Tipo = q is MultiplaEscolha ? "MultiplaEscolha" : "VerdadeiroFalso"
                    }).ToList()
                });
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao carregar questões para resolução.");
                ViewData["ErroConsulta"] = "Não foi possível carregar os dados para resolução. Tente novamente.";
                return View("~/Views/Estudo/Index.cshtml", new EstudoViewModel
                {
                    AlunoId = alunoId,
                    MateriaId = materiaId
                });
            }
        }

        [HttpGet("Resolver/{id:int}", Name = "EstudoResolver")]
        public async Task<IActionResult> Resolver([FromRoute] int id, int? alunoId, int? materiaId)
        {
            var modelo = new ResolverViewModel { Id = id, AlunoId = alunoId, MateriaId = materiaId };
            try
            {
                var questao = await _questaoRepository.BuscarPorId(id);
                if (questao == null)
                {
                    return NotFound();
                }

                // O resultado vem apenas de um POST concluído e pertence a esta questão/aluno.
                var questaoResultado = TempData["ResolucaoQuestaoId"];
                var alunoResultado = TempData["ResolucaoAlunoId"];
                var acertou = TempData["ResolucaoAcertou"];
                var resposta = TempData["ResolucaoResposta"];
                if (questaoResultado is int questaoId && questaoId == id &&
                    alunoResultado is int alunoIdResultado && alunoIdResultado == alunoId &&
                    acertou is bool resultado)
                {
                    modelo.Acertou = resultado;
                    modelo.Resposta = resposta as string;
                }

                return await MostrarResolucao(modelo, questao);
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao carregar resolução da questão {Id}.", id);
                ModelState.AddModelError(string.Empty, "Não foi possível carregar a questão. Tente novamente.");
                return View("~/Views/Estudo/Resolver.cshtml", modelo);
            }
        }

        [HttpPost("Resolver/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Resolver([FromRoute] int id, ResolverViewModel modelo)
        {
            if (id <= 0 || modelo.Id != id)
            {
                return BadRequest();
            }

            modelo.Acertou = null;
            Questao? questao = null;
            try
            {
                questao = await _questaoRepository.BuscarPorId(id);
                if (questao == null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    return await MostrarResolucao(modelo, questao);
                }

                var aluno = await _alunoRepository.BuscarPorId(modelo.AlunoId!.Value);
                if (aluno == null)
                {
                    ModelState.AddModelError(nameof(modelo.AlunoId), "O aluno informado não existe.");
                }

                if (!NormalizarResposta(questao, modelo.Resposta, out var resposta))
                {
                    ModelState.AddModelError(nameof(modelo.Resposta), questao is MultiplaEscolha
                        ? "Selecione uma alternativa desta questão."
                        : "Selecione verdadeiro ou falso.");
                }

                if (!ModelState.IsValid)
                {
                    return await MostrarResolucao(modelo, questao);
                }

                var tentativa = new Tentativa(aluno!, questao, resposta);
                aluno!.AdicionarTentativa(tentativa);
                await _registroTentativa.Registrar(tentativa);

                TempData["ResolucaoQuestaoId"] = id;
                TempData["ResolucaoAlunoId"] = aluno.IdAluno;
                TempData["ResolucaoAcertou"] = tentativa.Acertou;
                TempData["ResolucaoResposta"] = tentativa.Resposta;

                return RedirectToAction(nameof(Resolver), new
                {
                    id,
                    alunoId = aluno.IdAluno,
                    materiaId = modelo.MateriaId
                });
            }
            catch (TentativaQuestaoInativaException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao registrar tentativa da questão {Id}.", id);
                ModelState.AddModelError(string.Empty, "Não foi possível registrar a tentativa. Tente novamente.");
            }

            return await MostrarResolucao(modelo, questao);
        }

        private async Task<IActionResult> MostrarResolucao(ResolverViewModel modelo, Questao? questao)
        {
            if (questao != null)
            {
                modelo.Questao = new QuestaoResolucaoViewModel
                {
                    Id = questao.IdQuestao,
                    Enunciado = questao.Enunciado,
                    Ativa = questao.Ativa,
                    Tipo = questao is MultiplaEscolha ? "MultiplaEscolha" : "VerdadeiroFalso",
                    Materias = questao.Materias.Select(m => new MateriaOpcao(m.IdMateria, m.Nome)).ToList(),
                    Alternativas = questao is MultiplaEscolha mc
                        ? mc.Opcoes.Select(a => new AlternativaResposta(a.IdAlternativa, a.Texto)).ToList()
                        : []
                };
            }

            try
            {
                var alunos = await _alunoRepository.ListarTodos();
                modelo.Alunos = alunos.Select(a => new AlunoOpcao(a.IdAluno, a.Nome)).ToList();
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException)
            {
                _logger.LogError(ex, "Erro ao carregar alunos para resolução.");
                ModelState.AddModelError(string.Empty, "Não foi possível carregar os alunos. Tente novamente.");
            }

            return View("~/Views/Estudo/Resolver.cshtml", modelo);
        }

        private static bool NormalizarResposta(Questao questao, string? resposta, out string normalizada)
        {
            normalizada = string.Empty;
            if (questao is MultiplaEscolha mc && int.TryParse(resposta, out var idAlternativa) &&
                mc.Opcoes.Any(a => a.IdAlternativa == idAlternativa))
            {
                normalizada = idAlternativa.ToString(CultureInfo.InvariantCulture);
                return true;
            }

            if (questao is VerdadeiroFalso && bool.TryParse(resposta, out var valor))
            {
                normalizada = valor ? "true" : "false";
                return true;
            }

            return false;
        }
    }
}
