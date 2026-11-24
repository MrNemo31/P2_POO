namespace LiteArt.Poo.Repositories.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> ListarTodos();

        Task<T?> BuscarPorId(int id);

        Task<int> Inserir(T entidade);

        Task<bool> Atualizar(T entidade);

        Task<bool> Excluir(int id);
    }
}