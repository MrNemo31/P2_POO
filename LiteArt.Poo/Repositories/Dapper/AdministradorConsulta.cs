using Dapper;
using LiteArt.Poo.Data;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;

namespace LiteArt.Poo.Repositories.Dapper
{
    public class AdministradorConsulta : IAdministradorConsulta
    {
        private readonly DatabaseConnection _databaseConnection;

        public AdministradorConsulta(DatabaseConnection databaseConnection)
        {
            _databaseConnection = databaseConnection;
        }

        public async Task<Administrador?> BuscarPorId(int id)
        {
            const string sql = @"
                SELECT
                    nome AS Nome,
                    email AS Email,
                    id_administrador AS IdAdministrador
                FROM administradores
                WHERE id_administrador = @Id;
            ";

            using var conexao = _databaseConnection.CriarConexao();

            return await conexao.QuerySingleOrDefaultAsync<Administrador>(
                sql,
                new { Id = id }
            );
        }
    }
}