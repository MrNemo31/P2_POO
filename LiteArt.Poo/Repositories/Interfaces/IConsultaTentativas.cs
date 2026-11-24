using LiteArt.Poo.Models.Entities;

namespace LiteArt.Poo.Repositories.Interfaces
{
    public interface IConsultaTentativas
    {
        Task<IReadOnlyList<Tentativa>> ListarPorAluno(int alunoId);
    }
}
