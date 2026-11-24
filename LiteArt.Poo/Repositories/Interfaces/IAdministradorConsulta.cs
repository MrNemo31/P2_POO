using LiteArt.Poo.Models.Entities;

namespace LiteArt.Poo.Repositories.Interfaces
{
    public interface IAdministradorConsulta
    {
        Task<Administrador?> BuscarPorId(int id);
    }
}