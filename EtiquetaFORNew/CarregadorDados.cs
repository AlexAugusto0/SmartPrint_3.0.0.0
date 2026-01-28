using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;

namespace EtiquetaFORNew.Data
{
    /// <summary>
    /// Gerenciador de carregamento de produtos por diferentes tipos
    /// Equivalente às queries do SoftShop: GeradordeEtiquetas_Carregar*
    /// </summary>
    public static class CarregadorDados
    {
        // ⭐ ConnectionString local (mesmo do LocalDatabaseManager)
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "LocalData.db");
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";

        // ========================================
        // 🔹 CARREGAMENTO POR TIPO
        // ========================================

        /// <summary>
        /// Carrega produtos baseado no tipo e filtros fornecidos
        /// </summary>
        public static DataTable CarregarProdutosPorTipo(
            string tipo,
            string documento = null,
            DateTime? dataInicial = null,
            DateTime? dataFinal = null,
            string grupo = null,
            string subGrupo = null,
            string fabricante = null,
            string fornecedor = null,
            string produto = null,
            bool isConfeccao = false,
            int? idPromocao = null) // ⭐ NOVO parâmetro
        {
            switch (tipo.ToUpper())
            {
                case "AJUSTES":
                    return CarregarAjustes(documento, dataInicial, dataFinal);

                case "BALANÇOS":
                    return CarregarBalancos(documento, dataInicial, dataFinal);

                case "NOTAS ENTRADA":
                    return CarregarNotasEntrada(documento, dataInicial, dataFinal);

                case "PREÇOS ALTERADOS":
                    return CarregarPrecosAlterados(dataInicial.Value, dataFinal.Value);

                case "PROMOÇÕES":
                    // ⭐ Usa o método do PromocoesManager com ID da promoção
                    if (idPromocao.HasValue)
                    {
                        return PromocoesManager.BuscarProdutosDaPromocao(
                            idPromocao.Value,
                            null, // loja (usa padrão)
                            produto,
                            grupo,
                            subGrupo,
                            fabricante,
                            fornecedor);
                    }
                    else
                    {
                        throw new Exception("ID da promoção não foi informado!");
                    }

                case "FILTROS MANUAIS":
                default:
                    // Para filtros manuais, usa o método existente do LocalDatabaseManager
                    // que aceita: grupo, fabricante, fornecedor, isConfeccao
                    return LocalDatabaseManager.BuscarMercadoriasPorFiltros(
                        grupo,
                        fabricante,
                        fornecedor,
                        isConfeccao);
            }
        }

        // ========================================
        // 🔹 AJUSTES DE ESTOQUE
        // ========================================
        /// <summary>
        /// Carrega produtos de ajustes de estoque
        /// Equivalente: GeradordeEtiquetas_CarregarAjustes
        /// </summary>
        private static DataTable CarregarAjustes(string numeroAjuste, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    List<string> condicoes = new List<string>();
                    var parametros = new List<SQLiteParameter>();

                    // Filtro por número do ajuste (se implementado em campo específico)
                    if (!string.IsNullOrEmpty(numeroAjuste))
                    {
                        // TODO: Implementar quando houver campo de controle de ajustes
                        // condicoes.Add("m.NumeroAjuste = @numeroAjuste");
                        // parametros.Add(new SQLiteParameter("@numeroAjuste", numeroAjuste));
                    }

                    // Filtro por data
                    if (dataInicial.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de ajuste
                        // condicoes.Add("DATE(m.DataAjuste) >= DATE(@dataInicial)");
                        // parametros.Add(new SQLiteParameter("@dataInicial", dataInicial.Value.ToString("yyyy-MM-dd")));
                    }

                    if (dataFinal.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de ajuste
                        // condicoes.Add("DATE(m.DataAjuste) <= DATE(@dataFinal)");
                        // parametros.Add(new SQLiteParameter("@dataFinal", dataFinal.Value.ToString("yyyy-MM-dd")));
                    }

                    if (condicoes.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", condicoes);
                    }

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        foreach (var param in parametros)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar ajustes: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 BALANÇOS
        // ========================================
        /// <summary>
        /// Carrega produtos de balanços de estoque
        /// Equivalente: GeradordeEtiquetas_CarregarBalancos
        /// </summary>
        private static DataTable CarregarBalancos(string numeroBalanco, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    List<string> condicoes = new List<string>();
                    var parametros = new List<SQLiteParameter>();

                    // Filtro por número do balanço
                    if (!string.IsNullOrEmpty(numeroBalanco))
                    {
                        // TODO: Implementar quando houver campo de controle de balanços
                        // condicoes.Add("m.NumeroBalanco = @numeroBalanco");
                        // parametros.Add(new SQLiteParameter("@numeroBalanco", numeroBalanco));
                    }

                    // Filtro por data
                    if (dataInicial.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de balanço
                    }

                    if (dataFinal.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de balanço
                    }

                    if (condicoes.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", condicoes);
                    }

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        foreach (var param in parametros)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar balanços: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 NOTAS DE ENTRADA
        // ========================================
        /// <summary>
        /// Carrega produtos de notas de entrada consultando memoria_NF_Entrada no SQL Server
        /// Busca os itens pela tabela memoria_NF_Entrada do SQL Server e combina com dados locais
        /// </summary>
        private static DataTable CarregarNotasEntrada(string numeroNF, DateTime? dataInicial, DateTime? dataFinal)
        {
            try
            {
                // ⭐ STEP 1: Buscar itens da nota fiscal no SQL Server
                DataTable itensNF = BuscarItensNotaFiscalSQL(numeroNF);

                if (itensNF == null || itensNF.Rows.Count == 0)
                {
                    throw new Exception($"Nenhum item encontrado para a Nota Fiscal: {numeroNF}");
                }

                // ⭐ STEP 2: Criar DataTable de resultado com estrutura padrão
                DataTable resultado = CriarTabelaResultadoPadrao();

                // ⭐ STEP 3: Para cada item da NF, buscar dados completos no SQLite local
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    foreach (DataRow itemNF in itensNF.Rows)
                    {
                        string codigoMercadoria = itemNF["Codigo_Mercadoria"]?.ToString();
                        decimal precoItem = itemNF["Preco_Item"] != DBNull.Value
                            ? Convert.ToDecimal(itemNF["Preco_Item"])
                            : 0m;
                        int quantidadeItem = itemNF["Quantidade_Item"] != DBNull.Value
                            ? Convert.ToInt32(itemNF["Quantidade_Item"])
                            : 1;

                        if (string.IsNullOrEmpty(codigoMercadoria)) continue;

                        // ⭐ Buscar dados completos da mercadoria no SQLite local (SELECT *)
                        string queryLocal = "SELECT * FROM Mercadorias WHERE CodigoMercadoria = @codigo";

                        using (var cmd = new SQLiteCommand(queryLocal, conn))
                        {
                            cmd.Parameters.AddWithValue("@codigo", codigoMercadoria);

                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Adicionar produto ao resultado com dados da NF + dados locais
                                    DataRow novaLinha = resultado.NewRow();

                                    // ⭐ CAMPOS OBRIGATÓRIOS
                                    novaLinha["CodigoMercadoria"] = LerCampoSeguro(reader, "CodigoMercadoria");
                                    novaLinha["Mercadoria"] = LerCampoSeguro(reader, "Mercadoria");

                                    // ⭐ Usar preço da NF se disponível, senão usar preço local
                                    novaLinha["PrecoVenda"] = precoItem > 0 ? precoItem :
                                        LerCampoDecimal(reader, "PrecoVenda");

                                    // ⭐ CAMPOS OPCIONAIS - só preenche se existirem
                                    novaLinha["Grupo"] = LerCampoSeguro(reader, "Grupo");
                                    novaLinha["SubGrupo"] = LerCampoSeguro(reader, "SubGrupo");
                                    novaLinha["Fabricante"] = LerCampoSeguro(reader, "Fabricante");
                                    novaLinha["Fornecedor"] = LerCampoSeguro(reader, "Fornecedor");
                                    novaLinha["CodBarras"] = LerCampoSeguro(reader, "CodBarras");
                                    novaLinha["CodFabricante"] = LerCampoSeguro(reader, "CodFabricante");
                                    novaLinha["Tam"] = LerCampoSeguro(reader, "Tam");
                                    novaLinha["Cores"] = LerCampoSeguro(reader, "Cores");
                                    novaLinha["CodBarras_Grade"] = LerCampoSeguro(reader, "CodBarras_Grade");
                                    novaLinha["Registro"] = LerCampoInt(reader, "Registro");

                                    // ⭐ Usar quantidade da NF
                                    novaLinha["Quantidade"] = quantidadeItem;

                                    // ⭐ Campos adicionais de preço (opcionais)
                                    if (resultado.Columns.Contains("VendaA"))
                                        novaLinha["VendaA"] = LerCampoDecimal(reader, "VendaA");
                                    if (resultado.Columns.Contains("VendaB"))
                                        novaLinha["VendaB"] = LerCampoDecimal(reader, "VendaB");
                                    if (resultado.Columns.Contains("VendaC"))
                                        novaLinha["VendaC"] = LerCampoDecimal(reader, "VendaC");
                                    if (resultado.Columns.Contains("VendaD"))
                                        novaLinha["VendaD"] = LerCampoDecimal(reader, "VendaD");
                                    if (resultado.Columns.Contains("VendaE"))
                                        novaLinha["VendaE"] = LerCampoDecimal(reader, "VendaE");
                                    if (resultado.Columns.Contains("Prateleira"))
                                        novaLinha["Prateleira"] = LerCampoSeguro(reader, "Prateleira");
                                    if (resultado.Columns.Contains("Garantia"))
                                        novaLinha["Garantia"] = LerCampoSeguro(reader, "Garantia");

                                    resultado.Rows.Add(novaLinha);
                                }
                                else
                                {
                                    // ⭐ Mercadoria não encontrada localmente - criar registro básico com dados da NF
                                    DataRow novaLinha = resultado.NewRow();
                                    novaLinha["CodigoMercadoria"] = codigoMercadoria;
                                    novaLinha["Mercadoria"] = itemNF["Mercadoria"]?.ToString() ?? $"Produto {codigoMercadoria}";
                                    novaLinha["PrecoVenda"] = precoItem;
                                    novaLinha["Quantidade"] = quantidadeItem;
                                    resultado.Rows.Add(novaLinha);
                                }
                            }
                        }
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar notas de entrada: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ⭐ NOVO: Busca itens da Nota Fiscal no SQL Server (tabela memoria_NF_Entrada)
        /// </summary>
        private static DataTable BuscarItensNotaFiscalSQL(string numeroNota)
        {
            try
            {
                string connectionString = DatabaseConfig.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("Conexão com SQL Server não configurada!");
                }

                using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            [Código da Mercadoria] AS Codigo_Mercadoria,
                            Mercadoria,
                            Preco_Item,
                            Quantidade_Item
                        FROM memoria_NF_Entrada
                        WHERE [Nº Nota Fiscal] = @numeroNota
                        ORDER BY [Código da Mercadoria]
                    ";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@numeroNota", numeroNota);

                        using (var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao buscar itens da NF no SQL Server: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// ⭐ NOVO: Cria DataTable com estrutura padrão para resultado
        /// </summary>
        private static DataTable CriarTabelaResultadoPadrao()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CodigoMercadoria", typeof(string));
            dt.Columns.Add("Mercadoria", typeof(string));
            dt.Columns.Add("PrecoVenda", typeof(decimal));
            dt.Columns.Add("Grupo", typeof(string));
            dt.Columns.Add("SubGrupo", typeof(string));
            dt.Columns.Add("Fabricante", typeof(string));
            dt.Columns.Add("Fornecedor", typeof(string));
            dt.Columns.Add("CodBarras", typeof(string));
            dt.Columns.Add("CodFabricante", typeof(string));
            dt.Columns.Add("Tam", typeof(string));
            dt.Columns.Add("Cores", typeof(string));
            dt.Columns.Add("CodBarras_Grade", typeof(string));
            dt.Columns.Add("Registro", typeof(int));
            dt.Columns.Add("Quantidade", typeof(int));
            dt.Columns.Add("VendaA", typeof(decimal));
            dt.Columns.Add("VendaB", typeof(decimal));
            dt.Columns.Add("VendaC", typeof(decimal));
            dt.Columns.Add("VendaD", typeof(decimal));
            dt.Columns.Add("VendaE", typeof(decimal));
            dt.Columns.Add("Prateleira", typeof(string));
            dt.Columns.Add("Garantia", typeof(string));
            return dt;
        }

        // ========================================
        // 🔹 PREÇOS ALTERADOS
        // ========================================
        /// <summary>
        /// Carrega produtos com preços alterados no período
        /// Equivalente: GeradordeEtiquetas_CarregarAlteracaoPrecos
        /// </summary>
        private static DataTable CarregarPrecosAlterados(DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    // TODO: Implementar quando houver campo de data de alteração de preço
                    // query += @"
                    //     AND DATE(m.DataAlteracaoPreco) >= DATE(@dataInicial)
                    //     AND DATE(m.DataAlteracaoPreco) <= DATE(@dataFinal)
                    // ";

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dataInicial", dataInicial.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@dataFinal", dataFinal.ToString("yyyy-MM-dd"));

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar preços alterados: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 PROMOÇÕES
        // ========================================
        /// <summary>
        /// Carrega produtos em promoção com filtros específicos
        /// Equivalente: Promocoes_GeradorEtiquetasAnexar
        /// </summary>
        private static DataTable CarregarPromocoes(
            string grupo = null,
            string subGrupo = null,
            string fabricante = null,
            string fornecedor = null,
            string produto = null)
        {
            try
            {
                using (var conn = new SQLiteConnection(ConnectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT DISTINCT
                            m.CodigoMercadoria,
                            m.Mercadoria,
                            m.PrecoVenda,
                            m.Grupo,
                            m.Fabricante,
                            m.Fornecedor,
                            m.CodBarras,
                            m.CodFabricante,
                            m.Tam,
                            m.Cores,
                            m.CodBarras_Grade,
                            m.Registro,
                            1 as Quantidade
                        FROM Mercadorias m
                        WHERE 1=1
                    ";

                    // TODO: Quando houver tabela de promoções:
                    // query += " INNER JOIN Promocoes p ON m.CodigoMercadoria = p.CodigoMercadoria";
                    // query += " WHERE p.Ativa = 1";

                    List<string> condicoes = new List<string>();
                    var parametros = new List<SQLiteParameter>();

                    if (!string.IsNullOrEmpty(grupo))
                    {
                        condicoes.Add("m.Grupo = @grupo");
                        parametros.Add(new SQLiteParameter("@grupo", grupo));
                    }

                    if (!string.IsNullOrEmpty(subGrupo))
                    {
                        condicoes.Add("m.SubGrupo = @subGrupo");
                        parametros.Add(new SQLiteParameter("@subGrupo", subGrupo));
                    }

                    if (!string.IsNullOrEmpty(fabricante))
                    {
                        condicoes.Add("m.Fabricante = @fabricante");
                        parametros.Add(new SQLiteParameter("@fabricante", fabricante));
                    }

                    if (!string.IsNullOrEmpty(fornecedor))
                    {
                        condicoes.Add("m.Fornecedor = @fornecedor");
                        parametros.Add(new SQLiteParameter("@fornecedor", fornecedor));
                    }

                    if (!string.IsNullOrEmpty(produto))
                    {
                        condicoes.Add("m.Mercadoria LIKE @produto");
                        parametros.Add(new SQLiteParameter("@produto", $"%{produto}%"));
                    }

                    if (condicoes.Count > 0)
                    {
                        query += " AND " + string.Join(" AND ", condicoes);
                    }

                    query += " ORDER BY m.Mercadoria";

                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        foreach (var param in parametros)
                        {
                            cmd.Parameters.Add(param);
                        }

                        using (var adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar promoções: {ex.Message}", ex);
            }
        }

        // ========================================
        // 🔹 LIMPAR ETIQUETAS EXISTENTES
        // ========================================
        /// <summary>
        /// Limpa produtos já carregados (equivalente ao DELETE no SoftShop)
        /// </summary>
        public static bool LimparEtiquetasCarregadas()
        {
            // Esta funcionalidade pode ser implementada se houver
            // uma "área de staging" para produtos carregados
            // Por enquanto, apenas retorna true
            return true;
        }

        // ========================================
        // 🔹 MÉTODOS AUXILIARES - LEITURA SEGURA
        // ========================================

        /// <summary>
        /// ⭐ NOVO: Lê campo string do reader verificando se existe
        /// </summary>
        private static object LerCampoSeguro(SQLiteDataReader reader, string nomeCampo)
        {
            try
            {
                int ordinal = reader.GetOrdinal(nomeCampo);
                if (reader.IsDBNull(ordinal))
                    return DBNull.Value;
                return reader.GetValue(ordinal);
            }
            catch
            {
                return DBNull.Value;
            }
        }

        /// <summary>
        /// ⭐ NOVO: Lê campo decimal do reader verificando se existe
        /// </summary>
        private static object LerCampoDecimal(SQLiteDataReader reader, string nomeCampo)
        {
            try
            {
                int ordinal = reader.GetOrdinal(nomeCampo);
                if (reader.IsDBNull(ordinal))
                    return DBNull.Value;
                return reader.GetDecimal(ordinal);
            }
            catch
            {
                return DBNull.Value;
            }
        }

        /// <summary>
        /// ⭐ NOVO: Lê campo int do reader verificando se existe
        /// </summary>
        private static object LerCampoInt(SQLiteDataReader reader, string nomeCampo)
        {
            try
            {
                int ordinal = reader.GetOrdinal(nomeCampo);
                if (reader.IsDBNull(ordinal))
                    return DBNull.Value;
                return reader.GetInt32(ordinal);
            }
            catch
            {
                return DBNull.Value;
            }
        }
    }
}