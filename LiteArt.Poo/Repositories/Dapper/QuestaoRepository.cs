using Dapper;
using LiteArt.Poo.Data;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;

namespace LiteArt.Poo.Repositories.Dapper
{
    public class QuestaoRepository : IQuestaoRepository
    {
        private readonly DatabaseConnection _databaseConnection;

        public QuestaoRepository(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        // Lista todas as questões.
        public async Task<IEnumerable<Questao>> ListarTodos()
        {
            const string sql = @"
                SELECT
                    id_questao AS IdQuestao,
                    enunciado AS Enunciado,
                    ativa AS Ativa,
                    tipo_questao AS TipoQuestao,
                    resposta_correta AS RespostaCorreta,
                    id_administrador AS IdAdministrador
                FROM questoes
                ORDER BY id_questao;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QueryAsync<Questao>(sql);
        }

        // Busca uma questão pelo ID.
        public async Task<Questao?> BuscarPorId(int id)
        {
            const string sql = @"
                SELECT
                    id_questao AS IdQuestao,
                    enunciado AS Enunciado,
                    ativa AS Ativa,
                    tipo_questao AS TipoQuestao,
                    resposta_correta AS RespostaCorreta,
                    id_administrador AS IdAdministrador
                FROM questoes
                WHERE id_questao = @Id;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QueryFirstOrDefaultAsync<Questao>(
                sql,
                new { Id = id }
            );
        }

        // Cadastra uma nova questão.
        public async Task<int> Inserir(Questao questao)
        {
            const string sql = @"
                INSERT INTO questoes (
                    enunciado,
                    ativa,
                    tipo_questao,
                    resposta_correta,
                    id_administrador
                )
                VALUES (
                    @Enunciado,
                    @Ativa,
                    @TipoQuestao,
                    @RespostaCorreta,
                    @IdAdministrador
                )
                RETURNING id_questao;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QuerySingleAsync<int>(sql, questao);
        }

        // Atualiza uma questão existente.
        public async Task<bool> Atualizar(Questao questao)
        {
            const string sql = @"
                UPDATE questoes
                SET
                    enunciado = @Enunciado,
                    ativa = @Ativa,
                    tipo_questao = @TipoQuestao,
                    resposta_correta = @RespostaCorreta,
                    id_administrador = @IdAdministrador
                WHERE id_questao = @IdQuestao;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            var linhasAfetadas = await conexao.ExecuteAsync(sql, questao);

            return linhasAfetadas > 0;
        }

        // Exclui uma questão somente se não possuir tentativas.
        public async Task<bool> Excluir(int id)
        {
            const string sql = @"
                DELETE FROM questoes
                WHERE id_questao = @Id
                AND NOT EXISTS (
                    SELECT 1
                    FROM tentativas
                    WHERE id_questao = @Id
                );
            ";

            using var conexao = _databaseConnection.CriarConexao();

            var linhasAfetadas = await conexao.ExecuteAsync(
                sql,
                new { Id = id }
            );

            return linhasAfetadas > 0;
        }
    }
}
