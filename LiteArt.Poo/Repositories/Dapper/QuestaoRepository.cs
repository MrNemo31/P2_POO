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
            return await CarregarQuestoes(null);
        }

        // Busca uma questão pelo ID.
        public async Task<Questao?> BuscarPorId(int id)
        {
            var questoes = await CarregarQuestoes(id);

            return questoes.SingleOrDefault();
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
        private sealed record QuestaoRegistro(
            int IdQuestao,
            string Enunciado,
            bool Ativa,
            string TipoQuestao,
            bool? RespostaCorreta,
            int IdAdministrador,
            string NomeAdministrador,
            string EmailAdministrador
        );

        private sealed record MateriaQuestaoRegistro(
            int IdQuestao,
            int IdMateria,
            string Nome,
            string? Descricao,
            int IdAdministrador,
            string NomeAdministrador,
            string EmailAdministrador
        );

        private sealed record AlternativaQuestaoRegistro(
            int IdQuestao,
            int IdAlternativa,
            string Texto,
            bool Correta
        );

        private const string ConsultaQuestoes = @"
            SELECT
                q.id_questao AS IdQuestao,
                q.enunciado AS Enunciado,
                q.ativa AS Ativa,
                q.tipo_questao AS TipoQuestao,
                q.resposta_correta AS RespostaCorreta,
                a.id_administrador AS IdAdministrador,
                a.nome AS NomeAdministrador,
                a.email AS EmailAdministrador
            FROM questoes q
            INNER JOIN administradores a
                ON a.id_administrador = q.id_administrador
            WHERE q.id_questao = COALESCE(@Id, q.id_questao)
            ORDER BY q.id_questao;

            SELECT
                qm.id_questao AS IdQuestao,
                m.id_materia AS IdMateria,
                m.nome AS Nome,
                m.descricao AS Descricao,
                a.id_administrador AS IdAdministrador,
                a.nome AS NomeAdministrador,
                a.email AS EmailAdministrador
            FROM questoes_materias qm
            INNER JOIN materias m
                ON m.id_materia = qm.id_materia
            INNER JOIN administradores a
                ON a.id_administrador = m.id_administrador
            WHERE qm.id_questao = COALESCE(@Id, qm.id_questao)
            ORDER BY qm.id_questao, m.id_materia;

            SELECT
                id_questao AS IdQuestao,
                id_alternativa AS IdAlternativa,
                texto AS Texto,
                correta AS Correta
            FROM alternativas
            WHERE id_questao = COALESCE(@Id, id_questao)
            ORDER BY id_questao, id_alternativa;
        ";

        private static Questao MontarQuestao(
            QuestaoRegistro registro,
            List<Materia> materias,
            List<Alternativa> alternativas)
        {
            var administrador = new Administrador(
                registro.NomeAdministrador,
                registro.EmailAdministrador,
                registro.IdAdministrador
            );

            Questao questao;

            switch (registro.TipoQuestao)
            {
                case "MULTIPLA_ESCOLHA":
                    questao = new MultiplaEscolha(
                        registro.Enunciado,
                        alternativas,
                        administrador,
                        materias,
                        registro.IdQuestao
                    );
                    break;

                case "VERDADEIRO_FALSO":
                    if (!registro.RespostaCorreta.HasValue)
                    {
                        throw new InvalidOperationException(
                            "Questão de verdadeiro/falso sem gabarito.");
                    }

                    questao = new VerdadeiroFalso(
                        registro.Enunciado,
                        registro.RespostaCorreta.Value,
                        administrador,
                        materias,
                        registro.IdQuestao
                    );
                    break;

                default:
                    throw new InvalidOperationException(
                        "Tipo de questão não reconhecido no banco.");
            }

            if (!registro.Ativa)
            {
                questao.Desativar();
            }

            return questao;
        }

        private async Task<List<Questao>> CarregarQuestoes(int? id)
        {
            using var conexao = _databaseConnection.CriarConexao();

            using var resultados = await conexao.QueryMultipleAsync(
                ConsultaQuestoes,
                new { Id = id }
            );

            var registros = (
                await resultados.ReadAsync<QuestaoRegistro>()
            ).ToList();

            var materiasPorQuestao = (
                await resultados.ReadAsync<MateriaQuestaoRegistro>()
            ).ToLookup(m => m.IdQuestao);

            var alternativasPorQuestao = (
                await resultados.ReadAsync<AlternativaQuestaoRegistro>()
            ).ToLookup(a => a.IdQuestao);

            var questoes = new List<Questao>();

            foreach (var registro in registros)
            {
                var materias = materiasPorQuestao[registro.IdQuestao]
                    .Select(m => new Materia(
                        m.Nome,
                        m.Descricao,
                        new Administrador(
                            m.NomeAdministrador,
                            m.EmailAdministrador,
                            m.IdAdministrador
                        ),
                        m.IdMateria
                    ))
                    .ToList();

                var alternativas = alternativasPorQuestao[registro.IdQuestao]
                    .Select(a => new Alternativa(
                        a.Texto,
                        a.Correta,
                        a.IdAlternativa
                    ))
                    .ToList();

                questoes.Add(MontarQuestao(
                    registro,
                    materias,
                    alternativas
                ));
            }

            return questoes;
        }
    }
}
