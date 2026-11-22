
using LiteArt.Poo.Models.Entities;

namespace LiteArt.Poo.Repositories.Interfaces
{
    public interface IQuestaoRepository
    {
        // Lista todas as questões.
        Task<IEnumerable<Questao>> ListarTodas();

        // Busca uma questão pelo ID.
        Task<Questao?> BuscarPorId(int id);

        // Cadastra uma nova questão.
        Task<int> Inserir(Questao questao);

        // Atualiza uma questão existente.
        Task<bool> Atualizar(Questao questao);

        // Exclui uma questão.
        Task<bool> Excluir(int id);
    }
}
