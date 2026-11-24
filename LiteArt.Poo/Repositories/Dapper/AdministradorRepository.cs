using Dapper;
using LiteArt.Poo.Data;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;

namespace LiteArt.Poo.Repositories.Dapper;

public class AdministradorRepository : AdministradorConsulta, IAdministradorRepository
{
    private readonly DatabaseConnection _databaseConnection;

    public AdministradorRepository(DatabaseConnection databaseConnection) : base(databaseConnection)
    {
        _databaseConnection = databaseConnection;
    }

    public async Task<IEnumerable<Administrador>> ListarTodos()
    {
        const string sql = @"
            SELECT nome AS Nome, email AS Email, id_administrador AS IdAdministrador
            FROM administradores
            ORDER BY id_administrador;";

        using var conexao = _databaseConnection.CriarConexao();
        return await conexao.QueryAsync<Administrador>(sql);
    }

    public async Task<int> Inserir(Administrador administrador)
    {
        const string sql = @"
            INSERT INTO administradores (nome, email)
            VALUES (@Nome, @Email)
            RETURNING id_administrador;";

        using var conexao = _databaseConnection.CriarConexao();
        return await conexao.QuerySingleAsync<int>(sql, new { administrador.Nome, administrador.Email });
    }

    public async Task<bool> Atualizar(Administrador administrador)
    {
        const string sql = @"
            UPDATE administradores
            SET nome = @Nome, email = @Email
            WHERE id_administrador = @IdAdministrador;";

        using var conexao = _databaseConnection.CriarConexao();
        var linhas = await conexao.ExecuteAsync(sql, new
        {
            administrador.IdAdministrador,
            administrador.Nome,
            administrador.Email
        });
        return linhas > 0;
    }

    public async Task<bool> Excluir(int id)
    {
        // Exigido pelo contrato genérico; não há ação de exclusão nas telas.
        const string sql = @"
            DELETE FROM administradores a
            WHERE a.id_administrador = @Id
              AND NOT EXISTS (SELECT 1 FROM materias m WHERE m.id_administrador = a.id_administrador)
              AND NOT EXISTS (SELECT 1 FROM questoes q WHERE q.id_administrador = a.id_administrador);";

        using var conexao = _databaseConnection.CriarConexao();
        return await conexao.ExecuteAsync(sql, new { Id = id }) > 0;
    }
}
