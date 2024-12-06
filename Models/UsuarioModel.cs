using System.ComponentModel.DataAnnotations;

namespace API_TCC.Model
{
    public class UsuarioModel
    {
        [Key]
        public int ID { get; set; }
        public string NOME { get; set; } = "";

        public string Login { get; set; } = "";

        public string Senha { get; set; } = "";
    }

}