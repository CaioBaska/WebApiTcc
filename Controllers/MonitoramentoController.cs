using API_TCC.Database;
using API_TCC.DTO;
using API_TCC.Repositories;
using API_TCC.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace API_TCC.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MonitoramentoController : ControllerBase
    {
        private readonly MyDbContext _context;
        private readonly MonitoramentoService _monitoramentoService;
        private readonly IMonitoramentoRepository _repository;
        private readonly ServiceEnvioMqtt _meuServicoEnviaMqtt;

        public MonitoramentoController(MyDbContext context, IMonitoramentoRepository repository, MonitoramentoService monitoramentoService, ServiceEnvioMqtt meuServicoEnviaMqtt)
        {
            _context = context;
            _repository = repository;
            _monitoramentoService = monitoramentoService;
            _meuServicoEnviaMqtt = meuServicoEnviaMqtt;
        }

        [HttpGet("obterDados")]
        public IActionResult GetAllDados()
        {

            List<MonitoramentoDTO> dados = _monitoramentoService.GetAllDados();

            return Ok(dados);
        }

        [HttpGet("obterDadosByData")]
        public IActionResult GetDadosByData(MonitoramentoDTO monitoramentoDTO)
        {
            DateTime dataInicialFormatada, dataFinalFormatada;

            if (DateTime.TryParseExact(monitoramentoDTO.dataInicial, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataInicialFormatada) &&
                DateTime.TryParseExact(monitoramentoDTO.dataFinal, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataFinalFormatada))
            {

                List<RelatorioDTO> dados = _monitoramentoService.GetDadosByData(dataInicialFormatada, dataFinalFormatada);


                return Ok(dados);
            }
            else
            {
                return BadRequest("Formato de data inválido. Use o formato dd/MM/yyyy HH:mm:ss");
            }
        }



        [HttpGet("mandarTopicoMqtt")]
        public async Task<IActionResult> SendDadosPorTopico(MonitoramentoDTO monitoramentoDTO)
        {
            if (string.IsNullOrEmpty(monitoramentoDTO.mensagem))
            {
                return BadRequest("O parâmetro 'mensagem' não pode ser nulo ou vazio.");
            }

            _meuServicoEnviaMqtt.PublicarMensagem(monitoramentoDTO.mensagem);

            return Ok($"Dados enviados para o tópico smartgreen: {monitoramentoDTO.mensagem}");
        }

        [HttpGet("mandarRelatorioMqtt")]
        public async Task<IActionResult> SendRelatorioPorTopico(MonitoramentoDTO monitoramentoDTO)
        {
            if (string.IsNullOrEmpty(monitoramentoDTO.mensagem))
            {
                return BadRequest("O parâmetro 'mensagem' não pode ser nulo ou vazio.");
            }

            _meuServicoEnviaMqtt.PublicarRelatorio(monitoramentoDTO.mensagem);

            return Ok($"Dados enviados para o tópico smartgreen: {monitoramentoDTO.mensagem}");
        }

        [HttpGet("enviarRelatorioEmail")]
        public IActionResult SendEmailByData(MonitoramentoDTO monitoramentoDTO)
        {
            DateTime dataInicialFormatada, dataFinalFormatada;

            if (DateTime.TryParseExact(monitoramentoDTO.dataInicial, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataInicialFormatada) &&
                DateTime.TryParseExact(monitoramentoDTO.dataFinal, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out dataFinalFormatada))
            {

                List<RelatorioDTO> dados = _monitoramentoService.GetDadosByData(dataInicialFormatada, dataFinalFormatada);


                var csvContent = _monitoramentoService.GerarConteudoCSV(dados);

                _monitoramentoService.EnviarEmail(monitoramentoDTO.Destinatario, csvContent);


                return Ok("Email Enviado com Sucesso");
            }
            else
            {
                return BadRequest("Formato de data inválido. Use o formato dd/MM/yyyy HH:mm:ss");
            }
        }

    }
}
