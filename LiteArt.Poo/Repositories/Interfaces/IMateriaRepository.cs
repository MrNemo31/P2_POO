
using LiteArt.Poo.Models.Entities;

namespace LiteArt.Poo.Repositories.Interfaces
{
    public interface IMateriaRepository
    {
        Task<IEnumerable<Materia>> ListarTodas();

        Task<Materia?> BuscarPorId(int id);

        Task<int> Inserir(Materia materia);

        Task<bool> Atualizar(Materia materia);

        Task<bool> Excluir(int id);
    }
}
