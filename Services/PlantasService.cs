using API_TCC.Database;
using API_TCC.DTO;
using API_TCC.Repository;
using Microsoft.EntityFrameworkCore;

namespace API_TCC.Services
{
    public class PlantasService : IPlantasRepository
    {

        private readonly MyDbContext _context;

        public PlantasService(MyDbContext context)
        {
            _context = context;
        }

        public List<PlantasDTO> GetDadosPlantas(string nomePlanta)
        {
            try
            {
                string query = $"SELECT TEMPERATURA, UMIDADE, NITROGENIO, FOSFORO, PH, POTASSIO FROM TCC.PLANTAS WHERE NOME_PLANTA='{nomePlanta}'";

                List<PlantasDTO> result = _context.PlantasModel
                    .FromSqlRaw(query)
                    .Select(p => new PlantasDTO
                    {
                        TEMPERATURA = p.TEMPERATURA,
                        UMIDADE = p.UMIDADE,
                        NITROGENIO = p.NITROGENIO,
                        FOSFORO = p.FOSFORO,
                        PH = p.PH,
                        POTASSIO = p.POTASSIO,
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na consulta: {ex.Message}");
                return new List<PlantasDTO>();
            }
        }

        public void CreateDadosPlantas(string nomePlanta, string temperatura, string umidade, string nitrogenio, string fosforo, string pH, string potassio, string luminosidade)
        {
            try
            {
                _context.AtualizaPlantas(temperatura, umidade, nitrogenio, fosforo, pH, potassio, luminosidade, nomePlanta);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na consulta: {ex.Message}");
            }
        }
        public List<string> GetPlantasPersonalizadas()
        {
            try
            {
                string query = $"SELECT NOME_PLANTA FROM TCC.PLANTAS where NOME_PLANTA NOT IN ('BATATA','REPOLHO','TOMATE','HORTELA') ";


                List<string> result = _context.PlantasModel.FromSqlRaw(query).Select(p => p.NOME_PLANTA).ToList();

                return result;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na consulta: {ex.Message}");
                return new List<string>();
            }
        }

    }
}
