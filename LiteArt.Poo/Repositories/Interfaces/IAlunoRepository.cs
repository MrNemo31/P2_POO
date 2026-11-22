
using LiteArt.Poo.Models.Entities;

namespace LiteArt.Poo.Repositories.Interfaces
{
    public interface IAlunoRepository
    {
        Task<IEnumerable<Aluno>> ListarTodos();

        Task<Aluno?> BuscarPorId(int id);

        Task<int> Inserir(Aluno aluno);

        Task<bool> Atualizar(Aluno aluno);

        Task<bool> Excluir(int id);
    }
}
