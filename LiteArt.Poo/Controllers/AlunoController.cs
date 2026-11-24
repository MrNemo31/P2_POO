using System.Data.Common;
using LiteArt.Poo.Models.ViewModels;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    public class AlunoController : Controller
    {
        private readonly IAlunoRepository _alunoRepository;
        private readonly ILogger<AlunoController> _logger;

        public AlunoController(
            IAlunoRepository alunoRepository,
            ILogger<AlunoController> logger)
        {
            _alunoRepository = alunoRepository;
            _logger = logger;
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
            return View(
                "~/Views/Alunos/Create.cshtml",
                new AlunoFormularioViewModel()
            );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            AlunoFormularioViewModel modelo)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Alunos/Create.cshtml", modelo);
            }

            try
            {
                var aluno = new Aluno(modelo.Nome, modelo.Email);

                await _alunoRepository.Inserir(aluno);

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar aluno.");

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível cadastrar o aluno. Tente novamente.");
            }

            return View("~/Views/Alunos/Create.cshtml", modelo);
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

            var modelo = new AlunoFormularioViewModel
            {
                IdAluno = aluno.IdAluno,
                Nome = aluno.Nome,
                Email = aluno.Email
            };

            return View("~/Views/Alunos/Edit.cshtml", modelo);
        }

        // Atualiza os dados do aluno.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            AlunoFormularioViewModel modelo)
        {
            if (modelo.IdAluno <= 0)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Alunos/Edit.cshtml", modelo);
            }

            try
            {
                var aluno = await _alunoRepository.BuscarPorId(
                    modelo.IdAluno);

                if (aluno == null)
                {
                    return NotFound();
                }

                aluno.EditarDados(modelo.Nome, modelo.Email);

                var atualizado = await _alunoRepository.Atualizar(aluno);

                if (!atualizado)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            catch (DbException ex)
            {
                _logger.LogError(ex, "Erro ao atualizar aluno.");

                ModelState.AddModelError(
                    string.Empty,
                    "Não foi possível atualizar o aluno. Tente novamente.");
            }

            return View("~/Views/Alunos/Edit.cshtml", modelo);
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
