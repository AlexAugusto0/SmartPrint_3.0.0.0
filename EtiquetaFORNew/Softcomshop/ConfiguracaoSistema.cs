using System;
using System.IO;
using Newtonsoft.Json;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Enumeração dos tipos de conexão disponíveis
    /// </summary>
    public enum TipoConexao
    {
        SqlServer,
        SoftcomShop
    }

    /// <summary>
    /// Gerenciador de configurações do sistema
    /// </summary>
    public class ConfiguracaoSistema
    {
        private const string CONFIG_FILE = "config_sistema.json";
        
        public TipoConexao TipoConexaoAtiva { get; set; }
        public SoftcomShopConfig SoftcomShop { get; set; }
        
        // Configurações SQL Server existentes (mantidas para compatibilidade)
        public string SqlServerConnectionString { get; set; }

        public ConfiguracaoSistema()
        {
            TipoConexaoAtiva = TipoConexao.SqlServer;
            SoftcomShop = new SoftcomShopConfig();
            SqlServerConnectionString = "";
        }

        /// <summary>
        /// Salva as configurações em arquivo JSON
        /// </summary>
        public void Salvar()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this, Formatting.Indented);
                string caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE);
                File.WriteAllText(caminho, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar configurações: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Carrega as configurações do arquivo JSON
        /// </summary>
        public static ConfiguracaoSistema Carregar()
        {
            try
            {
                string caminho = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, CONFIG_FILE);
                
                if (File.Exists(caminho))
                {
                    string json = File.ReadAllText(caminho);
                    return JsonConvert.DeserializeObject<ConfiguracaoSistema>(json);
                }
                
                return new ConfiguracaoSistema();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar configurações: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Verifica se o SoftcomShop está configurado
        /// </summary>
        public bool SoftcomShopConfigurado()
        {
            return SoftcomShop != null && SoftcomShop.IsValid();
        }

        /// <summary>
        /// Verifica se SQL Server está configurado
        /// </summary>
        public bool SqlServerConfigurado()
        {
            return !string.IsNullOrWhiteSpace(SqlServerConnectionString);
        }

        /// <summary>
        /// Obtém a string de conexão ativa (para SQL Server)
        /// </summary>
        public string GetConnectionStringAtiva()
        {
            if (TipoConexaoAtiva == TipoConexao.SqlServer)
            {
                return SqlServerConnectionString;
            }
            
            // SoftcomShop usa API, não connection string
            return null;
        }
    }
}
