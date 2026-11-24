using LiteArt.Poo.Models.Entities;

namespace LiteArt.Poo.Repositories.Interfaces
{
    // Cada registro acrescenta uma tentativa ao histórico.
    public interface IRegistroTentativa
    {
        Task<int> Registrar(Tentativa tentativa);
    }
}
