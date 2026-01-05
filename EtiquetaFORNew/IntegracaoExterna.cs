using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Gerencia importações de sistemas externos (Softshop Access, Web API, etc)
    /// sem modificar o funcionamento normal do SmartPrint
    /// </summary>
    public static class IntegracaoExterna
    {
        /// <summary>
        /// Tipos de fonte de importação suportados
        /// </summary>
        public enum TipoImportacao
        {
            Nenhuma,        // Uso normal do sistema
            ArquivoJSON,    // Softshop Access exporta JSON
            ArquivoXML,     // Softshop Access exporta XML
            WebAPI          // Futuro: Sistema web chama API REST
        }

        /// <summary>
        /// Detecta se há importação pendente e retorna o tipo
        /// </summary>
        public static TipoImportacao DetectarTipoImportacao(string[] args)
        {
            // Sem argumentos = uso normal
            if (args == null || args.Length == 0)
                return TipoImportacao.Nenhuma;

            string parametro = args[0];

            // Verificar se é arquivo válido
            if (File.Exists(parametro))
            {
                string extensao = Path.GetExtension(parametro).ToLower();
                
                if (extensao == ".json")
                    return TipoImportacao.ArquivoJSON;
                
                if (extensao == ".xml")
                    return TipoImportacao.ArquivoXML;
            }

            // Verificar se é chamada de API (futuro)
            if (parametro.StartsWith("--api-import:", StringComparison.OrdinalIgnoreCase))
            {
                return TipoImportacao.WebAPI;
            }

            return TipoImportacao.Nenhuma;
        }

        /// <summary>
        /// Processa arquivo de importação JSON do Softshop
        /// </summary>
        public static DadosImportacao ProcessarImportacaoJSON(string caminhoArquivo)
        {
            try
            {
                string json = File.ReadAllText(caminhoArquivo);
                var dados = JsonConvert.DeserializeObject<DadosImportacao>(json);

                // Validar dados
                if (dados == null || dados.Itens == null || dados.Itens.Count == 0)
                {
                    throw new Exception("Arquivo JSON não contém itens válidos para importação.");
                }

                // Marcar fonte
                dados.FonteImportacao = "Softshop Access";
                dados.DataImportacao = DateTime.Now;

                return dados;
            }
            catch (JsonException ex)
            {
                throw new Exception($"Erro ao processar arquivo JSON: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao ler arquivo de importação: {ex.Message}");
            }
        }

        /// <summary>
        /// Processa arquivo de importação XML do Softshop (futuro)
        /// </summary>
        public static DadosImportacao ProcessarImportacaoXML(string caminhoArquivo)
        {
            // TODO: Implementar se necessário
            throw new NotImplementedException("Importação XML será implementada conforme necessidade.");
        }

        /// <summary>
        /// Processa importação via Web API (futuro)
        /// </summary>
        public static DadosImportacao ProcessarImportacaoWebAPI(string parametroAPI)
        {
            // TODO: Implementar quando houver sistema web
            throw new NotImplementedException("Importação via Web API será implementada futuramente.");
        }

        /// <summary>
        /// Tenta deletar arquivo temporário após processamento
        /// </summary>
        public static void LimparArquivoTemporario(string caminhoArquivo)
        {
            try
            {
                if (File.Exists(caminhoArquivo))
                {
                    // Tentar deletar até 3 vezes (pode estar bloqueado)
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            File.Delete(caminhoArquivo);
                            return;
                        }
                        catch
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }
            }
            catch
            {
                // Ignorar erros de limpeza - não é crítico
            }
        }
    }

    #region Classes de Dados de Importação

    /// <summary>
    /// Estrutura principal de dados importados
    /// </summary>
    public class DadosImportacao
    {
        [JsonProperty("itens")]
        public List<ItemImportacao> Itens { get; set; }

        [JsonProperty("configuracao")]
        public ConfiguracaoImportacao Configuracao { get; set; }

        // Metadados (não vem no JSON, preenchido internamente)
        [JsonIgnore]
        public string FonteImportacao { get; set; }

        [JsonIgnore]
        public DateTime DataImportacao { get; set; }

        public DadosImportacao()
        {
            Itens = new List<ItemImportacao>();
            Configuracao = new ConfiguracaoImportacao();
        }
    }

    /// <summary>
    /// Item individual para importação
    /// </summary>
    public class ItemImportacao
    {
        // Campos obrigatórios
        [JsonProperty("codigo")]
        public string Codigo { get; set; }

        [JsonProperty("referencia")]
        public string Referencia { get; set; }

        [JsonProperty("mercadoria")]
        public string Mercadoria { get; set; }

        [JsonProperty("quantidade")]
        public int Quantidade { get; set; }

        [JsonProperty("gerar")]
        public bool Gerar { get; set; }

        // Campos opcionais (podem vir do Softshop ou não)
        [JsonProperty("preco")]
        public decimal? Preco { get; set; }

        [JsonProperty("codigoBarras")]
        public string CodigoBarras { get; set; }

        [JsonProperty("unidade")]
        public string Unidade { get; set; }

        // Campos para confecção (opcionais)
        [JsonProperty("tamanho")]
        public string Tamanho { get; set; }

        [JsonProperty("cor")]
        public string Cor { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }

        public ItemImportacao()
        {
            Quantidade = 1;
            Gerar = true;
        }
    }

    /// <summary>
    /// Configurações opcionais da importação
    /// </summary>
    public class ConfiguracaoImportacao
    {
        [JsonProperty("autoImprimir")]
        public bool AutoImprimir { get; set; }

        [JsonProperty("fecharAposImprimir")]
        public bool FecharAposImprimir { get; set; }

        [JsonProperty("impressoraSugerida")]
        public string ImpressoraSugerida { get; set; }

        [JsonProperty("templateSugerido")]
        public string TemplateSugerido { get; set; }

        public ConfiguracaoImportacao()
        {
            AutoImprimir = false;
            FecharAposImprimir = false;
        }
    }

    #endregion
}
