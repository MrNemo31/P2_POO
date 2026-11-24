using Dapper;
using LiteArt.Poo.Data;
using LiteArt.Poo.Exceptions;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;

namespace LiteArt.Poo.Repositories.Dapper
{
    public class RegistroTentativa : IRegistroTentativa
    {
        private readonly DatabaseConnection _databaseConnection;

        public RegistroTentativa(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<int> Registrar(Tentativa tentativa)
        {
            ArgumentNullException.ThrowIfNull(tentativa);

            const string sql = @"
                INSERT INTO tentativas (id_aluno, id_questao, resposta, acertou, data_hora)
                SELECT @IdAluno, q.id_questao, @Resposta, @Acertou, @DataHora
                FROM questoes q
                WHERE q.id_questao = @IdQuestao AND q.ativa
                RETURNING id_tentativa;
            ";
            using var conexao = _databaseConnection.CriarConexao();
            var id = await conexao.QuerySingleOrDefaultAsync<int?>(sql, new
            {
                IdAluno = tentativa.Aluno.IdAluno,
                IdQuestao = tentativa.Questao.IdQuestao,
                tentativa.Resposta,
                tentativa.Acertou,
                // A coluna TIMESTAMP preserva a data/hora local, sem informação de fuso.
                DataHora = DateTime.SpecifyKind(tentativa.DataHora, DateTimeKind.Unspecified)
            });

            return id ?? throw new TentativaQuestaoInativaException(
                "A questão está inativa ou indisponível. A tentativa não foi registrada.");
        }
    }
}
