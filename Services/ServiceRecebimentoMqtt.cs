using API_TCC.DTO;
using API_TCC.Repositories;
using MQTTnet;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System.Globalization;
using System.Text;

namespace API_TCC.Services
{
    public class ServiceRecebimentoMqtt : BackgroundService
    {
        private readonly ManagedMqttClientOptions _mqttClientOptions;
        private readonly IManagedMqttClient _mqttClient;
        private readonly IMonitoramentoRepository _repository;

        public ServiceRecebimentoMqtt(IMonitoramentoRepository repository)
        {
           
            _repository = repository;

            var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithClientId("Your mqtt Client Id")
                .WithTcpServer("Your mqtt Server Address", 0000)
                .WithCredentials("Your Credentials", "Your Credentials").Build();

            _mqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithAutoReconnectDelay(TimeSpan.FromSeconds(60))
                .WithClientOptions(mqttClientOptions)
                .Build();

            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            ConfigureMqttHandlers();


        }

        private void ConfigureMqttHandlers()
        {
            _mqttClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(OnConnected);
            _mqttClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(OnDisconnected);
            _mqttClient.ConnectingFailedHandler = new ConnectingFailedHandlerDelegate(OnConnectingFailed);

            _mqttClient.UseApplicationMessageReceivedHandler(HandleReceivedMessage);
        }

        private void HandleReceivedMessage(MqttApplicationMessageReceivedEventArgs args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            string diretorio = config["PythonArchivesFolder"];
            if (string.IsNullOrEmpty(diretorio))
            {
                Console.WriteLine("Diretório PythonArchivesFolder não configurado.");
                return;
            }

            var payloadText = Encoding.UTF8.GetString(args.ApplicationMessage.Payload ?? Array.Empty<byte>());

            string[] valores = payloadText.Split(',');

            if (valores.Length == 3)
            {
                string primeiroValor = valores[0].Trim();
                string segundoValor = valores[1].Trim();
                string terceiroValor = valores[2].Trim();

                if (DateTime.TryParseExact(primeiroValor, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataInicial) &&
                    DateTime.TryParseExact(segundoValor, "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dataFinal))
                {
                    List<RelatorioDTO> dados = _repository.GetDadosByData(dataInicial, dataFinal);

                    var csvContent = _repository.GerarConteudoCSV(dados);

                    string caminhoArquivo = Path.Combine(diretorio, $"{terceiroValor}.csv");

                    File.WriteAllText(caminhoArquivo, csvContent);

                    Console.WriteLine($"Arquivo CSV gerado em: {caminhoArquivo}");
                }
                else
                {
                    Console.WriteLine("Formato de data inválido. Esperado: 'dd/MM/yyyy HH:mm:ss'");
                }
            }
            else
            {
                Console.WriteLine("Formato de string inválido. Esperado: 'valor1,valor2,valor3'");
            }
        }



        private void OnConnected(MqttClientConnectedEventArgs obj)
        {
            Console.WriteLine(@"Successfully connected.");
        }

        private void OnConnectingFailed(ManagedProcessFailedEventArgs obj)
        {
            Console.WriteLine("Couldn't connect to broker.");
        }

        private void OnDisconnected(MqttClientDisconnectedEventArgs obj)
        {
            Console.WriteLine("Successfully disconnected.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _mqttClient.StartAsync(_mqttClientOptions);

            await _mqttClient.SubscribeAsync(
                new MqttTopicFilter
                {
                    Topic = "EnviaRelatorio"
                }
            );

            while (!stoppingToken.IsCancellationRequested)
            {

                await Task.Delay(1000, stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _mqttClient.StopAsync();
            await base.StopAsync(cancellationToken);
        }
    }
}
