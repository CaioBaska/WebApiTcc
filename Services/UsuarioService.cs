using API_TCC.Database;
using API_TCC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace API_TCC.Services
{
    public class UsuarioService : IUsuarioRepository
    {
        private readonly MyDbContext _context;

        public UsuarioService(MyDbContext context)
        {
            _context = context;
        }

        public bool ValidarLogin(string login, string senha)
        {
            try
            {
                string query = $@"SELECT 1 FROM TCC.usuarios WHERE LOGIN = '{login}' AND SENHA = '{senha}'";

                bool result = _context.UsuarioModel.FromSqlRaw(query).Any();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na consulta: {ex.Message}");
                return false;
            }
        }

        public void CriaLogin(string nome, string login, string senha)
        {
            try
            {
                string query = $@"INSERT INTO TCC.usuarios (NOME, LOGIN, SENHA) VALUES ('{nome}', '{login}', '{senha}')";

                _context.Database.ExecuteSqlRaw(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar login: {ex.Message}");
            }
        }
        public void AlteraSenha(string login, string senha)
        {
            try
            {
                string query = $"UPDATE TCC.USUARIOS SET SENHA='{senha}' where LOGIN='{login}'";
                _context.Database.ExecuteSqlRaw(query);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar login: {ex.Message}");
            }
        }

        public bool VerificaLogin(string login)
        {
            try
            {
                string query = $"SELECT 1 FROM TCC.USUARIOS WHERE LOGIN='{login}'";
                bool result = _context.UsuarioModel.FromSqlRaw(query).Any();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao procurar login: {ex.Message}");
                return false;
            }
        }


    }
}
