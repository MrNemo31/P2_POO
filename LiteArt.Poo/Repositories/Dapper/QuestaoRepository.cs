using System.Data;
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
            var parametros = CriarParametros(questao);
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
            await conexao.OpenAsync();
            await using var transacao = await conexao.BeginTransactionAsync();

            try
            {
                var id = await conexao.QuerySingleAsync<int>(sql, parametros, transacao);

                await InserirMaterias(conexao, transacao, id, questao.Materias);

                if (questao is MultiplaEscolha multiplaEscolha)
                {
                    await InserirAlternativas(conexao, transacao, id, multiplaEscolha.Opcoes);
                }

                await transacao.CommitAsync();
                return id;
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
        }

        // Atualiza uma questão existente.
        public async Task<bool> Atualizar(Questao questao)
        {
            var parametros = CriarParametros(questao);
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
            await conexao.OpenAsync();
            await using var transacao = await conexao.BeginTransactionAsync();

            try
            {
                // Serializa a edição da questão e protege a verificação do histórico.
                var id = await conexao.QuerySingleOrDefaultAsync<int?>(
                    "SELECT id_questao FROM questoes WHERE id_questao = @Id FOR UPDATE;",
                    new { Id = questao.IdQuestao },
                    transacao
                );

                if (!id.HasValue)
                {
                    await transacao.RollbackAsync();
                    return false;
                }

                await conexao.ExecuteAsync(sql, parametros, transacao);

                await conexao.ExecuteAsync(
                    "DELETE FROM questoes_materias WHERE id_questao = @Id;",
                    new { Id = questao.IdQuestao },
                    transacao
                );
                await InserirMaterias(conexao, transacao, questao.IdQuestao, questao.Materias);
                await AtualizarAlternativas(conexao, transacao, questao);

                await transacao.CommitAsync();
                return true;
            }
            catch
            {
                await transacao.RollbackAsync();
                throw;
            }
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
        private static object CriarParametros(Questao questao)
        {
            ArgumentNullException.ThrowIfNull(questao);

            var (tipo, respostaCorreta) = questao switch
            {
                MultiplaEscolha => ("MULTIPLA_ESCOLHA", (bool?)null),
                VerdadeiroFalso vf => ("VERDADEIRO_FALSO", (bool?)vf.RespostaCorreta),
                _ => throw new ArgumentException("Tipo de questão não suportado.", nameof(questao))
            };

            return new
            {
                questao.IdQuestao,
                questao.Enunciado,
                questao.Ativa,
                TipoQuestao = tipo,
                RespostaCorreta = respostaCorreta,
                IdAdministrador = questao.Administrador.IdAdministrador
            };
        }

        private static async Task InserirMaterias(
            IDbConnection conexao,
            IDbTransaction transacao,
            int idQuestao,
            IReadOnlyList<Materia> materias)
        {
            const string sql = @"
                INSERT INTO questoes_materias (id_questao, id_materia)
                VALUES (@IdQuestao, @IdMateria);
            ";

            var parametros = materias.Select(m => m.IdMateria)
                .Distinct()
                .Select(id => new { IdQuestao = idQuestao, IdMateria = id });

            await conexao.ExecuteAsync(sql, parametros, transacao);
        }

        private static async Task InserirAlternativas(
            IDbConnection conexao,
            IDbTransaction transacao,
            int idQuestao,
            IEnumerable<Alternativa> alternativas)
        {
            const string sql = @"
                INSERT INTO alternativas (id_questao, texto, correta)
                VALUES (@IdQuestao, @Texto, @Correta);
            ";

            var parametros = alternativas.Select(a => new
            {
                IdQuestao = idQuestao,
                a.Texto,
                a.Correta
            });

            await conexao.ExecuteAsync(sql, parametros, transacao);
        }

        private static async Task AtualizarAlternativas(
            IDbConnection conexao,
            IDbTransaction transacao,
            Questao questao)
        {
            IReadOnlyList<Alternativa> alternativas = questao is MultiplaEscolha multiplaEscolha
                ? multiplaEscolha.Opcoes
                : Array.Empty<Alternativa>();

            var idsMantidos = alternativas.Where(a => a.IdAlternativa > 0)
                .Select(a => a.IdAlternativa).ToArray();
            var idsExistentes = (await conexao.QueryAsync<int>(
                "SELECT id_alternativa FROM alternativas WHERE id_questao = @Id;",
                new { Id = questao.IdQuestao },
                transacao
            )).ToArray();

            if (idsMantidos.Distinct().Count() != idsMantidos.Length ||
                idsMantidos.Except(idsExistentes).Any())
            {
                throw new ArgumentException(
                    "As alternativas devem ter IDs únicos e pertencer à questão editada.",
                    nameof(questao));
            }

            var idsRemovidos = idsExistentes.Except(idsMantidos).ToArray();

            if (idsRemovidos.Length > 0)
            {
                const string consultaTentativas = @"
                    SELECT EXISTS (
                        SELECT 1
                        FROM alternativas a
                        INNER JOIN tentativas t
                            ON t.id_questao = a.id_questao
                            AND t.resposta = a.id_alternativa::text
                        WHERE a.id_questao = @IdQuestao
                            AND a.id_alternativa = ANY(@Ids)
                    );
                ";
                var parametros = new { questao.IdQuestao, Ids = idsRemovidos };
                var possuiTentativas = await conexao.QuerySingleAsync<bool>(
                    consultaTentativas, parametros, transacao);

                if (possuiTentativas)
                {
                    throw new ArgumentException(
                        "Não é possível remover alternativas usadas em tentativas registradas.",
                        nameof(questao));
                }

                await conexao.ExecuteAsync(
                    "DELETE FROM alternativas WHERE id_questao = @IdQuestao AND id_alternativa = ANY(@Ids);",
                    parametros,
                    transacao
                );
            }

            foreach (var alternativa in alternativas.Where(a => a.IdAlternativa > 0))
            {
                const string sql = @"
                    UPDATE alternativas
                    SET texto = @Texto, correta = @Correta
                    WHERE id_questao = @IdQuestao AND id_alternativa = @IdAlternativa;
                ";
                await conexao.ExecuteAsync(sql, new
                {
                    questao.IdQuestao,
                    alternativa.IdAlternativa,
                    alternativa.Texto,
                    alternativa.Correta
                }, transacao);
            }

            await InserirAlternativas(conexao, transacao, questao.IdQuestao,
                alternativas.Where(a => a.IdAlternativa == 0));
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
