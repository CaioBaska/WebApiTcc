using API_TCC.DTO;

namespace API_TCC.Repositories
{
    public interface IMonitoramentoRepository
    {
        public List<MonitoramentoDTO> GetAllDados();
        public void CadastrarDados(string json);

        public List<RelatorioDTO> GetDadosByData(DateTime dataInicial, DateTime dataFinal);

        public string GerarConteudoCSV(List<RelatorioDTO> dadosRelatorio);

        public void EnviarEmail(string destinatario, string anexoCsv);



    }
}
