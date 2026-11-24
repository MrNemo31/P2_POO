using Dapper;
using LiteArt.Poo.Data;
using LiteArt.Poo.Models.Entities;
using LiteArt.Poo.Repositories.Interfaces;

namespace LiteArt.Poo.Repositories.Dapper
{
    public class ConsultaTentativas : IConsultaTentativas
    {
        private readonly DatabaseConnection _databaseConnection;
        private readonly IAlunoRepository _alunoRepository;
        private readonly IQuestaoRepository _questaoRepository;

        public ConsultaTentativas(
            DatabaseConnection databaseConnection,
            IAlunoRepository alunoRepository,
            IQuestaoRepository questaoRepository)
        {
            _databaseConnection = databaseConnection;
            _alunoRepository = alunoRepository;
            _questaoRepository = questaoRepository;
        }

        public async Task<IReadOnlyList<Tentativa>> ListarPorAluno(int alunoId)
        {
            const string sql = @"
                SELECT
                    id_tentativa AS IdTentativa,
                    id_questao AS IdQuestao,
                    resposta AS Resposta,
                    acertou AS Acertou,
                    data_hora AS DataHora
                FROM tentativas
                WHERE id_aluno = @AlunoId
                ORDER BY data_hora DESC, id_tentativa DESC;
            ";
            using var conexao = _databaseConnection.CriarConexao();
            var registros = (await conexao.QueryAsync<TentativaRegistro>(
                sql, new { AlunoId = alunoId })).ToList();

            if (registros.Count == 0)
            {
                return Array.Empty<Tentativa>();
            }

            var aluno = await _alunoRepository.BuscarPorId(alunoId)
                ?? throw new InvalidOperationException("O aluno do histórico não foi encontrado.");
            var questoes = (await _questaoRepository.ListarTodos())
                .ToDictionary(q => q.IdQuestao);
            var tentativas = new List<Tentativa>();

            foreach (var registro in registros)
            {
                if (!questoes.TryGetValue(registro.IdQuestao, out var questao))
                {
                    throw new InvalidOperationException("Uma questão do histórico não foi encontrada.");
                }

                // Reconstrução preserva o resultado original, sem avaliar a resposta novamente.
                var tentativa = new Tentativa(
                    aluno, questao, registro.Resposta, registro.IdTentativa,
                    registro.DataHora, registro.Acertou);
                aluno.AdicionarTentativa(tentativa);
                tentativas.Add(tentativa);
            }

            return tentativas;
        }

        private sealed record TentativaRegistro(
            int IdTentativa,
            int IdQuestao,
            string Resposta,
            bool Acertou,
            DateTime DataHora
        );
    }
}
