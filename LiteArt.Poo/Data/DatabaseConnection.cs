
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
                ?? throw new InvalidOperationException(
                    "String de conexão não encontrada."
                );
        }

        public NpgsqlConnection CriarConexao()
        {
            return new NpgsqlConnection(_connectionString);
        }
    }
}
