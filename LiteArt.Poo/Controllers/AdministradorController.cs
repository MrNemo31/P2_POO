
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiteArt.Poo.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly IAdministradorConsulta _administradorConsulta;

        public AdministradorController(
            IAdministradorConsulta administradorConsulta)
        {
            _administradorConsulta = administradorConsulta;
        }

        // Busca um administrador pelo ID.
        [HttpGet]
        public async Task<IActionResult> BuscarPorId(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID do administrador inválido.");
            }

            var administrador = await _administradorConsulta.BuscarPorId(id);

            if (administrador == null)
            {
                return NotFound("Administrador não encontrado.");
            }

            return Json(administrador);
        }
    }
}
