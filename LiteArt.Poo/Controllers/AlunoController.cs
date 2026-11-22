
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    public class AlunoController : Controller
    {
        private readonly IAlunoRepository _alunoRepository;

        public AlunoController(IAlunoRepository alunoRepository)
        {
            _alunoRepository = alunoRepository;
        }

        // Lista todos os alunos.
        public async Task<IActionResult> Index()
        {
            var alunos = await _alunoRepository.ListarTodos();

            return View("~/Views/Alunos/Index.cshtml", alunos);
        }

        // Exibe o formulário de cadastro.
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Alunos/Create.cshtml");
        }

        // Recebe os dados e cadastra o aluno.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Aluno aluno)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Alunos/Create.cshtml", aluno);
            }

            await _alunoRepository.Inserir(aluno);

            return RedirectToAction(nameof(Index));
        }

        // Busca o aluno para edição.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var aluno = await _alunoRepository.BuscarPorId(id);

            if (aluno == null)
            {
                return NotFound();
            }

            return View("~/Views/Alunos/Edit.cshtml", aluno);
        }

        // Atualiza os dados do aluno.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Aluno aluno)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Alunos/Edit.cshtml", aluno);
            }

            var atualizado = await _alunoRepository.Atualizar(aluno);

            if (!atualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }

        // Exclui o aluno, caso não possua tentativas registradas.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var excluido = await _alunoRepository.Excluir(id);

            if (!excluido)
            {
                TempData["Erro"] = "Não foi possível excluir o aluno. Ele pode possuir tentativas registradas.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
