namespace API_TCC.Repository
{
    public interface ServiceEnvioMqttRepository
    {
        void PublicarMensagem(string mensagem);
    }
}
