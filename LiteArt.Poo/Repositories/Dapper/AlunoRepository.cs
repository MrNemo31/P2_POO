
using Dapper;
using LiteArt.Poo.Data;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;

namespace LiteArt.Poo.Repositories.Dapper
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly DatabaseConnection _databaseConnection;

        public AlunoRepository(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        // Lista todos os alunos cadastrados.
        public async Task<IEnumerable<Aluno>> ListarTodos()
        {
            const string sql = @"
                SELECT
                    nome AS Nome,
                    email AS Email,
                    id_aluno AS IdAluno
                FROM alunos
                ORDER BY id_aluno;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QueryAsync<Aluno>(sql);
        }

        // Busca um aluno pelo ID.
        public async Task<Aluno?> BuscarPorId(int id)
        {
            const string sql = @"
                SELECT
                    nome AS Nome,
                    email AS Email,
                    id_aluno AS IdAluno
                FROM alunos
                WHERE id_aluno = @Id;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QueryFirstOrDefaultAsync<Aluno>(
                sql,
                new { Id = id }
            );
        }

        // Cadastra um novo aluno.
        public async Task<int> Inserir(Aluno aluno)
        {
            const string sql = @"
                INSERT INTO alunos (nome, email)
                VALUES (@Nome, @Email)
                RETURNING id_aluno;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QuerySingleAsync<int>(sql, aluno);
        }

        // Atualiza os dados de um aluno.
        public async Task<bool> Atualizar(Aluno aluno)
        {
            const string sql = @"
                UPDATE alunos
                SET
                    nome = @Nome,
                    email = @Email
                WHERE id_aluno = @IdAluno;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            var linhasAfetadas = await conexao.ExecuteAsync(sql, aluno);

            return linhasAfetadas > 0;
        }

        // Exclui um aluno somente se ele não possuir tentativas registradas.
        public async Task<bool> Excluir(int id)
        {
            const string sql = @"
                DELETE FROM alunos
                WHERE id_aluno = @Id
                AND NOT EXISTS (
                    SELECT 1
                    FROM tentativas
                    WHERE id_aluno = @Id
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
