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
            const string sql = ConsultaMateria + " ORDER BY m.id_materia;";

            using var conexao = _databaseConnection.CriarConexao();

            var registros = await conexao.QueryAsync<MateriaRegistro>(sql);

            return registros.Select(MapearMateria).ToList();
        }

        public async Task<Materia?> BuscarPorId(int id)
        {
            const string sql = ConsultaMateria + " WHERE m.id_materia = @Id;";

            using var conexao = _databaseConnection.CriarConexao();

            var registro = await conexao.QuerySingleOrDefaultAsync<MateriaRegistro>(
                sql,
                new { Id = id }
            );

            return registro is null ? null : MapearMateria(registro);
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

            var parametros = new
            {
                materia.Nome,
                materia.Descricao,
                IdAdministrador = materia.Administrador.IdAdministrador
            };

            return await conexao.QuerySingleAsync<int>(sql, parametros);
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

            var parametros = new
            {
                materia.IdMateria,
                materia.Nome,
                materia.Descricao,
                IdAdministrador = materia.Administrador.IdAdministrador
            };

            var linhasAfetadas = await conexao.ExecuteAsync(sql, parametros);

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

        private sealed record MateriaRegistro(
            int IdMateria,
            string Nome,
            string? Descricao,
            int IdAdministrador,
            string NomeAdministrador,
            string EmailAdministrador
        );

        private const string ConsultaMateria = @"
            SELECT
                m.id_materia AS IdMateria,
                m.nome AS Nome,
                m.descricao AS Descricao,
                a.id_administrador AS IdAdministrador,
                a.nome AS NomeAdministrador,
                a.email AS EmailAdministrador
            FROM materias m
            INNER JOIN administradores a
                ON a.id_administrador = m.id_administrador
        ";

        private static Materia MapearMateria(MateriaRegistro registro)
        {
            var administrador = new Administrador(
                registro.NomeAdministrador,
                registro.EmailAdministrador,
                registro.IdAdministrador
            );

            return new Materia(
                registro.Nome,
                registro.Descricao,
                administrador,
                registro.IdMateria
            );
        }
    }
}
