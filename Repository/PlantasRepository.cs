using API_TCC.DTO;

namespace API_TCC.Repository
{
    public interface IPlantasRepository
    {
        public List<PlantasDTO> GetDadosPlantas(string nomePlanta);
    }
}
