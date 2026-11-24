using LiteArt.Poo.Models.ViewModels;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    public class MateriaController : Controller
    {
        private readonly IMateriaRepository _materiaRepository;

        public MateriaController(IMateriaRepository materiaRepository)
        {
            _materiaRepository = materiaRepository;
        }

        // Lista todas as matérias cadastradas.
        public async Task<IActionResult> Index()
        {
            var materias = await _materiaRepository.ListarTodos();

            return View("~/Views/Materias/Index.cshtml", materias);
        }

        // Abre a tela de cadastro.
        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Materias/Create.cshtml", new MateriaFormularioViewModel());
        }

        // Recebe os dados do formulário e cadastra a matéria.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Materia materia)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Materias/Create.cshtml", materia);
            }

            await _materiaRepository.Inserir(materia);

            return RedirectToAction(nameof(Index));
        }

        
        // Abre a tela de edição com os dados atuais da matéria.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var materia = await _materiaRepository.BuscarPorId(id);

            if (materia == null)
            {
                return NotFound();
            }

            var modelo = new MateriaFormularioViewModel
            {
                IdMateria = materia.IdMateria,
                Nome = materia.Nome,
                Descricao = materia.Descricao,
                IdAdministrador = materia.Administrador.IdAdministrador
            };

            return View("~/Views/Materias/Edit.cshtml", modelo);
        }

        // Recebe os dados alterados e atualiza a matéria no banco.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Materia materia)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/Materias/Edit.cshtml", materia);
            }

            var atualizado = await _materiaRepository.Atualizar(materia);

            if (!atualizado)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }


        // Recebe a solicitação de exclusão de uma matéria.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var excluido = await _materiaRepository.Excluir(id);

            if (!excluido)
            {
                TempData["Erro"] = "Não foi possível excluir a matéria. Verifique se existem questões vinculadas.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}
