using System.ComponentModel.DataAnnotations;

namespace API_TCC.Model
{
    public class PlantasModel
    {
        [Key]
        public int ID { get; set; }
        public string NOME_PLANTA { get; set; } = "";
        public string UMIDADE { get; set; } = "";
        public string TEMPERATURA { get; set; } = "";
        public string POTASSIO { get; set; } = "";
        public string PH { get; set; } = "";
        public string NITROGENIO { get; set; } = "";

        public string FOSFORO { get; set; } = "";

        public string LUMINOSIDADE { get; set; } = "";
    }
}
