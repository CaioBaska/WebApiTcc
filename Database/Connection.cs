using API_TCC.Model;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;

namespace API_TCC.Database
{
    public class MyDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public MyDbContext(DbContextOptions<MyDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<UsuarioModel> UsuarioModel { get; set; }
        public DbSet<MonitoramentoModel> MonitoramentoModel { get; set; }
        public DbSet<PlantasModel> PlantasModel { get; set; }

        public void AtualizaPlantas(string temperatura, string umidade, string nitrogenio, string fosforo, string pH, string potassio, string luminosidade, string nomePlanta)
        {
            Database.ExecuteSqlRaw(
                "BEGIN TCC.ATUALIZA_PLANTAS (" +
                ":p_TEMPERATURA, :p_UMIDADE, :p_NITROGENIO, :p_FOSFORO, " +
                ":p_PH, :p_POTASSIO, :p_LUMINOSIDADE, :p_nome_planta); END;",
                new OracleParameter("p_TEMPERATURA", temperatura),
                new OracleParameter("p_UMIDADE", umidade),
                new OracleParameter("p_NITROGENIO", nitrogenio),
                new OracleParameter("p_FOSFORO", fosforo),
                new OracleParameter("p_PH", pH),
                new OracleParameter("p_POTASSIO", potassio),
                new OracleParameter("p_LUMINOSIDADE", luminosidade),
                new OracleParameter("p_nome_planta", nomePlanta));
        }



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseOracle(_configuration.GetConnectionString("OracleConnection"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("TCC");
            modelBuilder.Entity<UsuarioModel>().ToTable("USUARIOS");
            modelBuilder.Entity<MonitoramentoModel>().ToTable("MONITORAMENTO");
            modelBuilder.Entity<PlantasModel>().ToTable("PLANTAS");
        }
    }
}
