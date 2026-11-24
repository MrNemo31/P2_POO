
using LiteArt.Poo.Models.Entities;

namespace LiteArt.Poo.Repositories.Interfaces
{
    public interface IQuestaoRepository : IRepository<Questao>
    {
        Task<bool> PossuiTentativas(int id);
    }
}
