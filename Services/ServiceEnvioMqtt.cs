using API_TCC.Repository;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;

namespace API_TCC.Services
{
    public class ServiceEnvioMqtt : ServiceEnvioMqttRepository
    {
        private IMqttClient _mqttClient;
        public ServiceEnvioMqtt()
        {
            Connect_Client().Wait();
        }

        public async Task Connect_Client()
        {
            var mqttFactory = new MqttFactory();

            _mqttClient = mqttFactory.CreateMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithClientId("Your mqtt Client Id")
                .WithTcpServer("Your mqtt Server Address", 0000)
                .WithCredentials("Your Credentials", "Your Credentials").Build();

            var response = await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

        }

        public async void PublicarMensagem(string mensagem)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("EnviaSmart")
                .WithPayload(mensagem)
                .Build();

            await _mqttClient.PublishAsync(applicationMessage);
        }

        public async void PublicarRelatorio(string mensagem)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic("EnviaRelatorio")
                .WithPayload(mensagem)
                .Build();

            await _mqttClient.PublishAsync(applicationMessage);
        }
    }
}
