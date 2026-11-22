// Arquivo responsavel por receber as solicitações relacionadas as materias(Recebidas pelo MateriaRepository)
// e utilizar nosso repositório para executar as operações necessarias

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

        public async Task<IActionResult> Index()
        {
            var materias = await _materiaRepository.ListarTodas();

            return View("~/Views/Materias/Index.cshtml", materias);
        }
    }
}