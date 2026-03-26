using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace EtiquetaFORNew.Data
{
    public class DatabaseConfig

    {
        private static readonly string ConfigFilePath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "config.json");


        public class ConfigData
        {
            public string Servidor { get; set; }
            public string Porta { get; set; }
            public string Banco { get; set; }
            public string Usuario { get; set; }
            public string Senha { get; set; }
            public string Timeout { get; set; }
            public string Loja { get; set; }
            public string ModuloApp { get; set; }
            public string ModuloAppWeb { get; set; } // Valor vindo da sua ComboBox na tela Web
        }

        public static bool IsConfigured()
        {
            try
            {
                return File.Exists(ConfigFilePath) && !string.IsNullOrEmpty(GetConnectionString());
            }
            catch
            {
                return false;
            }
        }

        public static string GetConnectionString()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                    return string.Empty;

                string json = File.ReadAllText(ConfigFilePath);
                ConfigData config = JsonConvert.DeserializeObject<ConfigData>(json);

                if (config == null || string.IsNullOrEmpty(config.Servidor) || string.IsNullOrEmpty(config.Banco))
                    return string.Empty;

                string servidor = config.Servidor;

                // Adicionar porta se informada e diferente da padrão
                if (!string.IsNullOrEmpty(config.Porta) && config.Porta != "1433")
                {
                    servidor = $"{servidor},{config.Porta}";
                }

                string connStr = $"Server={servidor};Database={config.Banco};";

                // Autenticação
                if (!string.IsNullOrEmpty(config.Usuario))
                {
                    connStr += $"User Id={config.Usuario};Password={config.Senha};";
                }
                else
                {
                    connStr += "Integrated Security=true;";
                }

                // Timeout
                if (!string.IsNullOrEmpty(config.Timeout) && config.Timeout != "15")
                {
                    connStr += $"Connection Timeout={config.Timeout};";
                }

                return connStr;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SaveConnectionString(string connectionString)
        {
            // Este método mantido para compatibilidade, mas não faz nada
            // Use SaveConfiguration ao invés
        }

        public static void SaveConfiguration(string servidor, string porta, string bancoDados,
            string usuario, string senha, string timeout, string loja = "", string moduloApp = "", string moduloAppWeb = "")
        {
            try
            {
                ConfigData configExistente = LoadConfiguration();
                ConfigData config = new ConfigData
                {
                    Servidor = servidor,
                    Porta = porta,
                    Banco = bancoDados,
                    Usuario = usuario,
                    Senha = senha,
                    Timeout = timeout,
                    Loja = loja,
                    ModuloApp = moduloApp,
                    ModuloAppWeb = string.IsNullOrEmpty(moduloAppWeb) ? configExistente.ModuloAppWeb : moduloAppWeb
                };

                string json = JsonConvert.SerializeObject(config, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar configuração: {ex.Message}");
            }
        }

        public static ConfigData LoadConfiguration()
        {
            try
            {
                if (!File.Exists(ConfigFilePath))
                    return new ConfigData();

                string json = File.ReadAllText(ConfigFilePath);
                return JsonConvert.DeserializeObject<ConfigData>(json) ?? new ConfigData();
            }
            catch
            {
                return new ConfigData();
            }
        }

        public static string GetConfigFilePath()
        {
            return ConfigFilePath;
        }

        public static async Task<string> GetSetRegistroJsonAsync(string codigoSuporte, string cnpj, string fantasia)
        {
            string url = "http://softcomdevelop.com.br/webService/wsRegistro.asmx";

            // Obter a versão do sistema

            var assembly = Assembly.GetExecutingAssembly();
            var fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        

            string versaoSistema = fileInfo.FileVersion;//System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string nomeAppComVersao = $"Smart Print v{versaoSistema}";

            string parametrosJson = "{"
                + "\"CNPJ\":\"" + cnpj.Replace("\"", "\\\"") + "\","
                + "\"Empresa\":\"" + fantasia.Replace("\"", "\\\"") + "\","
                + "\"CodigoApp\":\"41\","
                + "\"App\":\"" + nomeAppComVersao.Replace("\"", "\\\"") + "\","
                + "\"Token\":\"{EFAC2E35-9FCC-480E-80AC-5EDDA66E8A9F}\","
                + "\"CodigoSuporte\":\"" + codigoSuporte.Replace("\"", "\\\"") + "\","
                + "\"ConfigApp\":\"{}\"" // <- Colocar aqui o link de download do backup em nuvem
                + "}";


            //string parametrosJson = "{"
            //    + "\"CNPJ\":\"" + cnpj.Replace("\"", "\\\"") + "\","
            //    + "\"Empresa\":\"" + fantasia.Replace("\"", "\\\"") + "\","
            //    + "\"CodigoApp\":\"41\","
            //    + "\"App\":\"Smart Print\","
            //    + "\"Token\":\"{EFAC2E35-9FCC-480E-80AC-5EDDA66E8A9F}\","
            //    + "\"CodigoSuporte\":\"" + codigoSuporte.Replace("\"", "\\\"") + "\","
            //    + "\"ConfigApp\":\"{}\"" // <- Colocar aqui o link de download do backup em nuvem
            //    + "}";

            string jsonEscapadoXml = System.Security.SecurityElement.Escape(parametrosJson);

            string soap =
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>"
              + "<soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\">"
              + "  <soap:Body>"
              + "    <getSetRegistroJson xmlns=\"http://tempuri.org/\">"
              + "      <parametrosJson>" + jsonEscapadoXml + "</parametrosJson>"
              + "    </getSetRegistroJson>"
              + "  </soap:Body>"
              + "</soap:Envelope>";

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "text/xml; charset=utf-8";
            req.Headers.Add("SOAPAction", "http://tempuri.org/getSetRegistroJson");

            byte[] data = Encoding.UTF8.GetBytes(soap);
            using (var reqStream = await req.GetRequestStreamAsync())
                await reqStream.WriteAsync(data, 0, data.Length);

            using (var resp = (HttpWebResponse)await req.GetResponseAsync())
            using (var reader = new StreamReader(resp.GetResponseStream()))
                return await reader.ReadToEndAsync();
        }

        public static void SaveConfiguration(ConfigData config)
        {
            // Ele apenas repassa os dados para o método acima
            SaveConfiguration(
                config.Servidor,
                config.Porta,
                config.Banco,
                config.Usuario,
                config.Senha,
                config.Timeout,
                config.Loja,
                config.ModuloApp,
                config.ModuloAppWeb // Aqui garantimos que o valor do combo Web seja passado
            );
        }

    }
}