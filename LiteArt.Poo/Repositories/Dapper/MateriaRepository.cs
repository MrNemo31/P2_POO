using Dapper;
using LiteArt.Poo.Data;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;

namespace LiteArt.Poo.Repositories.Dapper
{
    public class MateriaRepository : IMateriaRepository
    {
        private readonly DatabaseConnection _databaseConnection;

        public MateriaRepository(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<IEnumerable<Materia>> ListarTodos()
        {
            const string sql = @"
                SELECT
                    id_materia AS IdMateria,
                    nome AS Nome,
                    descricao AS Descricao,
                    id_administrador AS IdAdministrador
                FROM materias
                ORDER BY id_materia;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QueryAsync<Materia>(sql);
        }

        public async Task<Materia?> BuscarPorId(int id)
        {
            const string sql = @"
                SELECT
                    id_materia AS IdMateria,
                    nome AS Nome,
                    descricao AS Descricao,
                    id_administrador AS IdAdministrador
                FROM materias
                WHERE id_materia = @Id;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QueryFirstOrDefaultAsync<Materia>(
                sql,
                new { Id = id }
            );
        }

        public async Task<int> Inserir(Materia materia)
        {
            const string sql = @"
                INSERT INTO materias
                    (nome, descricao, id_administrador)
                VALUES
                    (@Nome, @Descricao, @IdAdministrador)
                RETURNING id_materia;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QuerySingleAsync<int>(sql, materia);
        }

        public async Task<bool> Atualizar(Materia materia)
        {
            const string sql = @"
                UPDATE materias
                SET
                    nome = @Nome,
                    descricao = @Descricao,
                    id_administrador = @IdAdministrador
                WHERE id_materia = @IdMateria;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            var linhasAfetadas = await conexao.ExecuteAsync(sql, materia);

            return linhasAfetadas > 0;
        }

        public async Task<bool> Excluir(int id)
        {
            const string sql = @"
                DELETE FROM materias
                WHERE id_materia = @Id
                AND NOT EXISTS (
                    SELECT 1
                    FROM questoes_materias
                    WHERE id_materia = @Id
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