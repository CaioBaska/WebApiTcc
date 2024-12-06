using API_TCC.Database;
using API_TCC.DTO;
using API_TCC.Model;
using API_TCC.Repositories;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Net;
using System.Net.Mail;


namespace API_TCC.Services
{
    public class MonitoramentoService : IMonitoramentoRepository
    {
        private readonly MyDbContext _context;

        public MonitoramentoService(MyDbContext context)
        {
            _context = context;
        }
        public List<MonitoramentoDTO> GetAllDados()
        {
            try
            {
                string query = "SELECT * FROM (SELECT id, DATA, UMIDADE, TEMPERATURA, PH, NITROGENIO, FOSFORO, POTASSIO, LUMINOSIDADE FROM TCC.MONITORAMENTO ORDER BY DATA DESC ) WHERE ROWNUM = 1";

                List<MonitoramentoDTO> result = _context.MonitoramentoModel
                    .FromSqlRaw(query)
                    .Select(m => new MonitoramentoDTO
                    {
                        UMIDADE = m.UMIDADE,
                        TEMPERATURA = m.TEMPERATURA,
                        PH = m.PH,
                        NITROGENIO = m.NITROGENIO,
                        FOSFORO = m.FOSFORO,
                        POTASSIO = m.POTASSIO,
                        LUMINOSIDADE = m.LUMINOSIDADE
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na consulta: {ex.Message}");
                return new List<MonitoramentoDTO>();
            }
        }
        public void CadastrarDados(string json)
        {
            try
            {
                var valores = JObject.Parse(json);


                var cultura = new System.Globalization.CultureInfo("pt-BR");


                var novoMonitoramento = new MonitoramentoModel
                {
                    DATA = DateTime.Now,
                    UMIDADE = valores.ContainsKey("UMIDADE") ? valores["UMIDADE"].ToString() : "0",
                    TEMPERATURA = valores.ContainsKey("TEMPERATURA") ? valores["TEMPERATURA"].ToString() : "0",
                    POTASSIO = valores.ContainsKey("POTASSIO") ? valores["POTASSIO"].ToString() : "0",
                    PH = valores.ContainsKey("PH") ? valores["PH"].ToString() : "0",
                    NITROGENIO = valores.ContainsKey("NITROGENIO") ? valores["NITROGENIO"].ToString() : "0",
                    FOSFORO = valores.ContainsKey("FOSFORO") ? valores["FOSFORO"].ToString() : "0",
                    LUMINOSIDADE = valores.ContainsKey("LUMINOSIDADE") ? valores["LUMINOSIDADE"].ToString() : "0",
                };


                _context.MonitoramentoModel.Add(novoMonitoramento);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao cadastrar dados: {ex.Message}");
            }
        }
        public List<RelatorioDTO> GetDadosByData(DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                string query = $@"SELECT id, DATA, UMIDADE, TEMPERATURA, PH, NITROGENIO, FOSFORO, POTASSIO, LUMINOSIDADE 
                          FROM TCC.MONITORAMENTO 
                          WHERE DATA >= TO_DATE('{dataInicial.ToString("dd/MM/yyyy HH:mm:ss")}', 'DD/MM/YYYY HH24:MI:SS') 
                            AND DATA < TO_DATE('{dataFinal.ToString("dd/MM/yyyy HH:mm:ss")}', 'DD/MM/YYYY HH24:MI:SS') 
                          ORDER BY DATA DESC";

                List<RelatorioDTO> result = _context.MonitoramentoModel
                    .FromSqlRaw(query)
                    .Select(m => new RelatorioDTO
                    {
                        DataFormatada = m.DATA.ToString("dd/MM/yyyy HH:mm:ss"),
                        UMIDADE = m.UMIDADE,
                        TEMPERATURA = m.TEMPERATURA,
                        PH = m.PH,
                        NITROGENIO = m.NITROGENIO,
                        FOSFORO = m.FOSFORO,
                        POTASSIO = m.POTASSIO,
                        LUMINOSIDADE = m.LUMINOSIDADE
                    })
                    .ToList();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na consulta: {ex.Message}");
                return new List<RelatorioDTO>();
            }
        }
        public void EnviarEmail(string destinatario, string anexoCsv)
        {
            try
            {
                var fromEmail = "Your Configured Email";
                var password = "Your Configured Email Password";

                var message = new MailMessage();
                message.From = new MailAddress(fromEmail);
                message.Subject = "Relatório SmartGreen";
                message.To.Add(new MailAddress(destinatario));
                message.Body = "Em anexo está o relatório CSV gerado.";

                Attachment attachment = new Attachment(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(anexoCsv)), "relatorio.csv", "text/csv");
                message.Attachments.Add(attachment);

                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(fromEmail, password);
                    client.Send(message);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar o e-mail: {ex.Message}");
            }
        }
        public string GerarConteudoCSV(List<RelatorioDTO> dadosRelatorio)
        {
            using (var writer = new StringWriter())
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(dadosRelatorio);
                return writer.ToString();
            }
        }

    }
}
