using System.Data.Common;
using System.Globalization;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Models.ViewModels;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    [Route("Historico")]
    public class HistoricoController : Controller
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly IConsultaTentativas _consultaTentativas;
        private readonly ILogger<HistoricoController> _logger;

        public HistoricoController(
            IAlunoRepository alunoRepository,
            IConsultaTentativas consultaTentativas,
            ILogger<HistoricoController> logger)
        {
            _alunoRepository = alunoRepository;
            _consultaTentativas = consultaTentativas;
            _logger = logger;
        }

        [HttpGet("", Name = "HistoricoIndex")]
        public async Task<IActionResult> Index(int? alunoId)
        {
            IReadOnlyList<AlunoOpcao> alunos = [];
            try
            {
                alunos = (await _alunoRepository.ListarTodos())
                    .Select(a => new AlunoOpcao(a.IdAluno, a.Nome)).ToList();

                if (!ModelState.IsValid ||
                    (alunoId.HasValue && !alunos.Any(a => a.Id == alunoId.Value)))
                {
                    ViewData["ErroAluno"] = "O aluno informado não existe. Selecione um aluno da lista.";
                    return View("~/Views/Historico/Index.cshtml", new HistoricoViewModel { Alunos = alunos });
                }

                var tentativas = alunoId.HasValue
                    ? await _consultaTentativas.ListarPorAluno(alunoId.Value)
                    : Array.Empty<Tentativa>();

                return View("~/Views/Historico/Index.cshtml", new HistoricoViewModel
                {
                    Alunos = alunos,
                    AlunoId = alunoId,
                    Tentativas = tentativas.Select(t => new TentativaPreviaViewModel(
                        new QuestaoPreviaViewModel
                        {
                            Id = t.Questao.IdQuestao,
                            Enunciado = t.Questao.Enunciado,
                            Tipo = t.Questao is MultiplaEscolha ? "MultiplaEscolha" : "VerdadeiroFalso",
                            Materias = t.Questao.Materias.Select(m => new MateriaOpcao(m.IdMateria, m.Nome)).ToList()
                        },
                        t.Acertou,
                        t.DataHora.ToString("dd/MM/yyyy HH:mm:ss", CultureInfo.GetCultureInfo("pt-BR"))
                    ) { Id = t.IdTentativa }).ToList()
                });
            }
            catch (Exception ex) when (ex is DbException or InvalidOperationException or ArgumentException)
            {
                _logger.LogError(ex, "Erro ao consultar o histórico do aluno {AlunoId}.", alunoId);
                ViewData["ErroConsulta"] = "Não foi possível carregar o histórico. Tente novamente.";
                return View("~/Views/Historico/Index.cshtml", new HistoricoViewModel
                {
                    Alunos = alunos,
                    AlunoId = alunoId
                });
            }
        }
    }
}
