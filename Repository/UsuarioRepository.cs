namespace API_TCC.Repositories
{
    public interface IUsuarioRepository
    {
        bool ValidarLogin(string login, string senha);
        void CriaLogin(string nome, string login, string senha);

        void AlteraSenha(string login, string senha);

        bool VerificaLogin(string login);
    }
}
