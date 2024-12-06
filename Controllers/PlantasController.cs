using API_TCC.Database;
using API_TCC.DTO;
using API_TCC.Model;
using API_TCC.Repository;
using API_TCC.Services;
using Microsoft.AspNetCore.Mvc;

namespace API_TCC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlantasController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly PlantasService _plantasService;
        private readonly IPlantasRepository _repository;

        public PlantasController(MyDbContext context, IPlantasRepository repository, PlantasService plantasService)
        {
            _context = context;
            _repository = repository;
            _plantasService = plantasService;
        }

        [HttpGet("obterDadosPlantas")]
        public IActionResult GetPlantaDados(PlantasDTO PlantasDTO)
        {


            List<PlantasDTO> dados = _plantasService.GetDadosPlantas(PlantasDTO.NOME_PLANTA);
            return Ok(dados);
        }

        [HttpDelete("deletarDadosPlantasPorNome")]
        public IActionResult DeletePlantaByNome(PlantasDTO plantasDTO)
        {
            if (plantasDTO.NOME_PLANTA != "HORTELA" && plantasDTO.NOME_PLANTA != "TOMATE" && plantasDTO.NOME_PLANTA != "REPOLHO" && plantasDTO.NOME_PLANTA != "BATATA")
            {
                var planta = _context.PlantasModel.FirstOrDefault(p => p.NOME_PLANTA == plantasDTO.NOME_PLANTA);
                if (planta == null)
                {
                    return NotFound();
                }

                _context.PlantasModel.Remove(planta);
                _context.SaveChanges();

                return NoContent();
            }
            else
            {
                return BadRequest("Não é possível deletar os valores padrão");
            }

        }

        [HttpGet("cadastrarPlanta")]
        public IActionResult CreatePlantaDados(PlantasDTO PlantasDTO)
        {
            if (string.IsNullOrEmpty(PlantasDTO.NOME_PLANTA) ||
                string.IsNullOrEmpty(PlantasDTO.TEMPERATURA) ||
                string.IsNullOrEmpty(PlantasDTO.UMIDADE) ||
                string.IsNullOrEmpty(PlantasDTO.NITROGENIO) ||
                string.IsNullOrEmpty(PlantasDTO.FOSFORO) ||
                string.IsNullOrEmpty(PlantasDTO.PH) ||
                string.IsNullOrEmpty(PlantasDTO.POTASSIO) ||
                string.IsNullOrEmpty(PlantasDTO.LUMINOSIDADE))
            {
                return BadRequest("Todos os parâmetros devem ser fornecidos.");
            }

            _plantasService.CreateDadosPlantas(
                PlantasDTO.NOME_PLANTA,
                PlantasDTO.TEMPERATURA,
                PlantasDTO.UMIDADE,
                PlantasDTO.NITROGENIO,
                PlantasDTO.FOSFORO,
                PlantasDTO.PH,
                PlantasDTO.POTASSIO,
                PlantasDTO.LUMINOSIDADE
            );

            return Ok();
        }


        [HttpGet("obterTodasPlantasPersonalizadas")]
        public IActionResult GetAllPlantasPersonalizadas()
        {

            List<string> dados = _plantasService.GetPlantasPersonalizadas();
            return Ok(dados);

        }

    }

}