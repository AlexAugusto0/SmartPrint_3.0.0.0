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
    /// Equivalente ÃƒÂ s queries do SoftShop: GeradordeEtiquetas_Carregar*
    /// </summary>
    public static class CarregadorDados
    {
        // Ã¢Â­Â ConnectionString local (mesmo do LocalDatabaseManager)
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "LocalData.db");
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";
        // ========================================
        // Ã°Å¸â€Â¹ CARREGAMENTO POR TIPO
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
            int? idPromocao = null) // Ã¢Â­Â NOVO parÃƒÂ¢metro
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
                    // Ã¢Â­Â Usa o mÃƒÂ©todo do PromocoesManager com ID da promoçÃƒÂ£o
                    if (idPromocao.HasValue)
                    {
                        return PromocoesManager.BuscarProdutosDaPromocao(
                            idPromocao.Value,
                            null, // loja (usa padrÃƒÂ£o)
                            produto,
                            grupo,
                            subGrupo,
                            fabricante,
                            fornecedor);
                    }
                    else
                    {
                        throw new Exception("ID da promoçÃƒÂ£o nÃƒÂ£o foi informado!");
                    }

                case "FILTROS MANUAIS":
                default:
                    // Para filtros manuais, usa o mÃƒÂ©todo existente do LocalDatabaseManager
                    // que aceita: grupo, fabricante, fornecedor, isConfeccao
                    return LocalDatabaseManager.BuscarMercadoriasPorFiltros(
                        grupo,
                        fabricante,
                        fornecedor,
                        isConfeccao);
            }
        }

        // ========================================
        // Ã°Å¸â€Â¹ AJUSTES DE ESTOQUE
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
                            m.SubGrupo,
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

                    // Filtro por número do ajuste (se implementado em campo especÃƒÂ­fico)
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
        // Ã°Å¸â€Â¹ BALANÇOS
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
                            m.SubGrupo,
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
        // Ã°Å¸â€Â¹ NOTAS DE ENTRADA
        // ========================================
        /// <summary>
        /// Carrega produtos de notas fiscais de entrada
        /// Equivalente: GeradordeEtiquetas_CarregarCompras
        /// </summary>
        private static DataTable CarregarNotasEntrada(string numeroNF, DateTime? dataInicial, DateTime? dataFinal)
        {
            DataTable itensNF = BuscarItensNotaFiscalSQL(numeroNF);
            DataTable resultado = CriarTabelaResultadoPadrao();

            if (itensNF == null || itensNF.Rows.Count == 0) return resultado;

            using (var conn = new SQLiteConnection(ConnectionString))
            {
                conn.Open();
                foreach (DataRow itemNF in itensNF.Rows)
                {
                    string codBarrasNF = itemNF["CodBarras"]?.ToString()?.Trim() ?? "";
                    string codigoMestre = itemNF["Codigo_Mercadoria"]?.ToString()?.Trim() ?? "";
                    int qtd = itemNF["Quantidade_Item"] != DBNull.Value ? Convert.ToInt32(itemNF["Quantidade_Item"]) : 1;
                    decimal precoNF = itemNF["Preco_Item"] != DBNull.Value ? Convert.ToDecimal(itemNF["Preco_Item"]) : 0m;

                    bool encontrouVariação = false;

                    // 1. TENTA BUSCAR PELA GRADE (CODBARRAS)
                    if (!string.IsNullOrEmpty(codBarrasNF))
                    {
                        string queryGrade = "SELECT * FROM Mercadorias WHERE CodBarras_Grade = @cb OR CodBarras = @cb LIMIT 1";
                        using (var cmd = new SQLiteCommand(queryGrade, conn))
                        {
                            cmd.Parameters.AddWithValue("@cb", codBarrasNF);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string tam = reader["Tam"]?.ToString() ?? "";
                                    string cor = reader["Cores"]?.ToString() ?? "";

                                    // SE HOUVER TAMANHO OU COR, É UM ITEM DE GRADE (CONFECÇÃO)
                                    if (!string.IsNullOrWhiteSpace(tam) || !string.IsNullOrWhiteSpace(cor))
                                    {
                                        AdicionarRow(resultado, reader, codBarrasNF, qtd, precoNF);
                                        encontrouVariação = true;
                                    }
                                }
                            }
                        }
                    }

                    // 2. SE NÃO ACHOU VARIAÇÃO, BUSCA PELO CÓDIGO DA MERCADORIA (PADRÃO ORIGINAL)
                    if (!encontrouVariação && !string.IsNullOrEmpty(codigoMestre))
                    {
                        string queryPadrao = "SELECT * FROM Mercadorias WHERE CodigoMercadoria = @mestre LIMIT 1";
                        using (var cmd = new SQLiteCommand(queryPadrao, conn))
                        {
                            cmd.Parameters.AddWithValue("@mestre", codigoMestre);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    AdicionarRow(resultado, reader, codBarrasNF, qtd, precoNF);
                                }
                            }
                        }
                    }
                }
            }
            return resultado;
        }

        // Método auxiliar para evitar repetição e erros de ambiguidade
        private static void AdicionarRow(DataTable dt, SQLiteDataReader reader, string cbNF, int qtd, decimal preco)
        {
            DataRow row = dt.NewRow();
            row["CodigoMercadoria"] = reader["CodigoMercadoria"]?.ToString() ?? "";
            row["Mercadoria"] = reader["Mercadoria"]?.ToString() ?? "";
            row["Tam"] = reader["Tam"]?.ToString() ?? "PADRAO";
            row["Cores"] = reader["Cores"]?.ToString() ?? "PADRAO";
            row["CodBarras"] = !string.IsNullOrEmpty(cbNF) ? cbNF : (reader["CodBarras"]?.ToString() ?? "");
            row["Quantidade"] = qtd;
            row["PrecoVenda"] = preco;
            row["CodFabricante"] = reader["CodFabricante"]?.ToString() ?? "";
            row["Fabricante"] = reader["Fabricante"]?.ToString() ?? "";

            if (dt.Columns.Contains("CodBarras_Grade"))
                row["CodBarras_Grade"] = reader["CodBarras_Grade"]?.ToString() ?? "";

            dt.Rows.Add(row);
        }


        private static DataTable BuscarItensNotaFiscalSQL(string numeroNota)
        {
            try
            {
                string connectionString = DatabaseConfig.GetConnectionString();

                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new Exception("ConexÃ£o com SQL Server nÃ£o configurada!");
                }

                using (var conn = new System.Data.SqlClient.SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            [Código da Mercadoria] AS Codigo_Mercadoria,
                            Mercadoria,
                            Preco_Item,
                            Quantidade_Item,
                            CODBARRAS AS CodBarras
                        FROM memoria_NF_Entrada
                        WHERE [Nº Nota Fiscal] = @numeroNota
                        ORDER BY [Código da Mercadoria], CODBARRAS
                    ";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@numeroNota", numeroNota);

                        using (var adapter = new System.Data.SqlClient.SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            adapter.Fill(dt);

                            // ⭐ DEBUG: Log quantos registros foram encontrados
                            System.Diagnostics.Debug.WriteLine($"[BuscarItensNF] NF: {numeroNota} - Registros encontrados: {dt.Rows.Count}");
                            foreach (DataRow row in dt.Rows)
                            {
                                System.Diagnostics.Debug.WriteLine($"  - Cod: {row["Codigo_Mercadoria"]}, CodBarras: {row["CodBarras"]}, Qtd: {row["Quantidade_Item"]}");
                            }

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
        // Ã°Å¸â€Â¹ PREÇOS ALTERADOS
        // ========================================
        /// <summary>
        /// Carrega produtos com preços alterados no perÃƒÂ­odo
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
                            m.SubGrupo,
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

                    // TODO: Implementar quando houver campo de data de alteraçÃƒÂ£o de preço
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
        // Ã°Å¸â€Â¹ PROMOÇÕES
        // ========================================
        /// <summary>
        /// Carrega produtos em promoçÃƒÂ£o com filtros especÃƒÂ­ficos
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
                            m.SubGrupo,
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

                    // TODO: Quando houver tabela de promoçÃƒÂµes:
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
                throw new Exception($"Erro ao carregar promoçÃƒÂµes: {ex.Message}", ex);
            }
        }

        // ========================================
        // Ã°Å¸â€Â¹ LIMPAR ETIQUETAS EXISTENTES
        // ========================================
        /// <summary>
        /// Limpa produtos jÃƒÂ¡ carregados (equivalente ao DELETE no SoftShop)
        /// </summary>
        public static bool LimparEtiquetasCarregadas()
        {
            // Esta funcionalidade pode ser implementada se houver
            // uma "ÃƒÂ¡rea de staging" para produtos carregados
            // Por enquanto, apenas retorna true
            return true;
        }

        // ========================================
        // ðŸ”¹ MÃ‰TODOS AUXILIARES - LEITURA SEGURA
        // ========================================

        /// <summary>
        /// LÃª campo string do reader verificando se existe
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
        /// LÃª campo decimal do reader verificando se existe
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
        /// LÃª campo int do reader verificando se existe
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