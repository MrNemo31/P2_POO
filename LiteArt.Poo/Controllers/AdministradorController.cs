using System.Data.Common;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Models.ViewModels;
using LiteArt.Poo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace LiteArt.Poo.Controllers;

public class AdministradorController : Controller
{
    private readonly IAdministradorRepository _administradorRepository;
    private readonly ILogger<AdministradorController> _logger;

    public AdministradorController(
        IAdministradorRepository administradorRepository,
        ILogger<AdministradorController> logger)
    {
        _administradorRepository = administradorRepository;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        try
        {
            return View("~/Views/Administradores/Index.cshtml", await _administradorRepository.ListarTodos());
        }
        catch (Exception ex) when (ex is DbException or InvalidOperationException)
        {
            _logger.LogError(ex, "Erro ao listar administradores.");
            ViewData["ErroConsulta"] = "Não foi possível carregar os administradores. Tente novamente.";
            return View("~/Views/Administradores/Index.cshtml", Array.Empty<Administrador>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("~/Views/Administradores/Create.cshtml", new AdministradorFormularioViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Create(AdministradorFormularioViewModel modelo)
    {
        return Salvar(modelo, editar: false);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        if (id <= 0)
        {
            return BadRequest();
        }

        try
        {
            var administrador = await _administradorRepository.BuscarPorId(id);
            if (administrador == null)
            {
                return NotFound();
            }

            return View("~/Views/Administradores/Edit.cshtml", new AdministradorFormularioViewModel
            {
                IdAdministrador = administrador.IdAdministrador,
                Nome = administrador.Nome,
                Email = administrador.Email
            });
        }
        catch (Exception ex) when (ex is DbException or InvalidOperationException)
        {
            _logger.LogError(ex, "Erro ao carregar administrador para edição.");
            TempData["Erro"] = "Não foi possível carregar o administrador. Tente novamente.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Edit(AdministradorFormularioViewModel modelo)
    {
        if (modelo.IdAdministrador <= 0)
        {
            return Task.FromResult<IActionResult>(BadRequest());
        }

        return Salvar(modelo, editar: true);
    }

    private async Task<IActionResult> Salvar(AdministradorFormularioViewModel modelo, bool editar)
    {
        var pagina = editar ? "~/Views/Administradores/Edit.cshtml" : "~/Views/Administradores/Create.cshtml";
        if (!ModelState.IsValid)
        {
            return View(pagina, modelo);
        }

        try
        {
            if (editar)
            {
                var administrador = await _administradorRepository.BuscarPorId(modelo.IdAdministrador);
                if (administrador == null)
                {
                    return NotFound();
                }

                administrador.EditarDados(modelo.Nome, modelo.Email);
                if (!await _administradorRepository.Atualizar(administrador))
                {
                    return NotFound();
                }
            }
            else
            {
                var administrador = new Administrador(modelo.Nome, modelo.Email);
                await _administradorRepository.Inserir(administrador);
            }

            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
        }
        catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
        {
            ModelState.AddModelError(nameof(modelo.Email), "Já existe um administrador com este e-mail.");
        }
        catch (Exception ex) when (ex is DbException or InvalidOperationException)
        {
            _logger.LogError(ex, "Erro ao salvar administrador.");
            ModelState.AddModelError(string.Empty, "Não foi possível salvar o administrador. Tente novamente.");
        }

        return View(pagina, modelo);
    }

    [HttpGet]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        if (id <= 0)
        {
            return BadRequest("ID do administrador inválido.");
        }

        try
        {
            var administrador = await _administradorRepository.BuscarPorId(id);
            return administrador == null
                ? NotFound("Administrador não encontrado.")
                : Json(administrador);
        }
        catch (Exception ex) when (ex is DbException or InvalidOperationException)
        {
            _logger.LogError(ex, "Erro ao consultar administrador.");
            return StatusCode(503, "Não foi possível consultar o administrador. Tente novamente.");
        }
    }
}
