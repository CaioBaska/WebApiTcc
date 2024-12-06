using System.ComponentModel.DataAnnotations;

namespace API_TCC.DTO
{
    public class UsuarioDTO
    {
        public int ID { get; set; }
        public string NOME { get; set; } = "";

        public string Login { get; set; } = "";

        public string Senha { get; set; } = "";
    }
}
