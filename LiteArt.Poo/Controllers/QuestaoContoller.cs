
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    public class QuestaoController : Controller
    {
        private readonly IQuestaoRepository _questaoRepository;

        public QuestaoController(IQuestaoRepository questaoRepository)
        {
            _questaoRepository = questaoRepository;
        }

        // Lista todas as questões.
        public async Task<IActionResult> Index()
        {
            var questoes = await _questaoRepository.ListarTodas();

            return View("~/Views/Questoes/Index.cshtml", questoes);
        }

        // Exibe o formulário de cadastro.
        [HttpGet]
        public IActionResult Nova()
        {
            return View("~/Views/Questoes/Nova.cshtml");
        }

        // Recebe os dados e cadastra uma questão.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Nova(Questao questao)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Questoes/Nova.cshtml", questao);
            }

            await _questaoRepository.Inserir(questao);

            return RedirectToAction(nameof(Index));
        }

        // Busca uma questão para edição.
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var questao = await _questaoRepository.BuscarPorId(id);

            if (questao == null)
            {
                return NotFound();
            }

            return View("~/Views/Questoes/Editar.cshtml", questao);
        }

        // Atualiza uma questão.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(Questao questao)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Questoes/Editar.cshtml", questao);
            }

            var atualizado = await _questaoRepository.Atualizar(questao);

            if (!atualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // Exclui uma questão, caso não possua tentativas.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var excluido = await _questaoRepository.Excluir(id);

            if (!excluido)
            {
                TempData["Erro"] = "Não foi possível excluir a questão. Ela pode possuir tentativas registradas.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
