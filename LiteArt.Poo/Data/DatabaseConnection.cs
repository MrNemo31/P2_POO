// Responsavel pela conecção com o banco de dados(Postgre)
using Npgsql;

namespace LiteArt.Poo.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration
                .GetConnectionString("DefaultConnection")
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(_connectionString))
            {
                throw new InvalidOperationException(
                    "String de conexão não encontrada ou vazia. Verifique o appsettings.dev.json."
                );
            }
        }

        public NpgsqlConnection CriarConexao()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
