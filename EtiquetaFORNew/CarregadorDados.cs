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
    /// Equivalente ÃƒÆ’Ã‚Â s queries do SoftShop: GeradordeEtiquetas_Carregar*
    /// </summary>
    public static class CarregadorDados
    {
        // ÃƒÂ¢Ã‚Â­Ã‚Â ConnectionString local (mesmo do LocalDatabaseManager)
        private static readonly string DbPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "LocalData.db");
        private static readonly string ConnectionString = $"Data Source={DbPath};Version=3;";
        // ========================================
        // ÃƒÂ°Ã…Â¸Ã¢â‚¬ÂÃ‚Â¹ CARREGAMENTO POR TIPO
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
            int? idPromocao = null) // ÃƒÂ¢Ã‚Â­Ã‚Â NOVO parÃƒÆ’Ã‚Â¢metro
        {
            switch (tipo.ToUpper())
            {
                case "AJUSTES":
                    return CarregarAjustes(documento, dataInicial, dataFinal);

                case "BALANÃ‡OS":
                    return CarregarBalancos(documento, dataInicial, dataFinal);

                case "NOTAS ENTRADA":
                    return CarregarNotasEntrada(documento, dataInicial, dataFinal);

                case "PREÃ‡OS ALTERADOS":
                    return CarregarPrecosAlterados(dataInicial.Value, dataFinal.Value);

                case "PROMOÃ‡Ã•ES":
                    // ÃƒÂ¢Ã‚Â­Ã‚Â Usa o mÃƒÆ’Ã‚Â©todo do PromocoesManager com ID da promoÃ§ÃƒÆ’Ã‚Â£o
                    if (idPromocao.HasValue)
                    {
                        return PromocoesManager.BuscarProdutosDaPromocao(
                            idPromocao.Value,
                            null, // loja (usa padrÃƒÆ’Ã‚Â£o)
                            produto,
                            grupo,
                            subGrupo,
                            fabricante,
                            fornecedor);
                    }
                    else
                    {
                        throw new Exception("ID da promoÃ§ÃƒÆ’Ã‚Â£o nÃƒÆ’Ã‚Â£o foi informado!");
                    }

                case "FILTROS MANUAIS":
                default:
                    // Para filtros manuais, usa o mÃƒÆ’Ã‚Â©todo existente do LocalDatabaseManager
                    // que aceita: grupo, fabricante, fornecedor, isConfeccao
                    return LocalDatabaseManager.BuscarMercadoriasPorFiltros(
                        grupo,
                        fabricante,
                        fornecedor,
                        isConfeccao);
            }
        }

        // ========================================
        // ÃƒÂ°Ã…Â¸Ã¢â‚¬ÂÃ‚Â¹ AJUSTES DE ESTOQUE
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

                    // Filtro por nÃºmero do ajuste (se implementado em campo especÃƒÆ’Ã‚Â­fico)
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
        // ÃƒÂ°Ã…Â¸Ã¢â‚¬ÂÃ‚Â¹ BALANÃ‡OS
        // ========================================
        /// <summary>
        /// Carrega produtos de balanÃ§os de estoque
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

                    // Filtro por nÃºmero do balanÃ§o
                    if (!string.IsNullOrEmpty(numeroBalanco))
                    {
                        // TODO: Implementar quando houver campo de controle de balanÃ§os
                        // condicoes.Add("m.NumeroBalanco = @numeroBalanco");
                        // parametros.Add(new SQLiteParameter("@numeroBalanco", numeroBalanco));
                    }

                    // Filtro por data
                    if (dataInicial.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de balanÃ§o
                    }

                    if (dataFinal.HasValue)
                    {
                        // TODO: Implementar quando houver campo de data de balanÃ§o
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
                throw new Exception($"Erro ao carregar balanÃ§os: {ex.Message}", ex);
            }
        }


        // ========================================
        // 🔹 NOTAS DE ENTRADA
        // ========================================
        /// <summary>
        /// Carrega produtos de notas fiscais de entrada
        /// Equivalente: GeradordeEtiquetas_CarregarCompras
        /// </summary>
        private static DataTable CarregarNotasEntrada(string numeroNF, DateTime? dataInicial, DateTime? dataFinal)
        {
            DataTable resultado = CriarTabelaResultadoPadrao();

            try
            {
                string connectionStringSQLServer = DatabaseConfig.GetConnectionString();
                if (string.IsNullOrEmpty(connectionStringSQLServer))
                    throw new Exception("Conexão SQL Server não configurada!");

                using (var connSQL = new System.Data.SqlClient.SqlConnection(connectionStringSQLServer))
                using (var connLocal = new SQLiteConnection(ConnectionString))
                {
                    connSQL.Open();
                    connLocal.Open();

                    // BUSCAR ITENS DA NF
                    string queryNF = @"
                        SELECT 
                            [Código da Mercadoria] AS Codigo_Mercadoria,
                            CODBARRAS AS CodBarras,
                            Quantidade_Item
                        FROM memoria_NF_Entrada
                        WHERE [Nº Nota Fiscal] = @numeroNota";

                    using (var cmd = new System.Data.SqlClient.SqlCommand(queryNF, connSQL))
                    {
                        cmd.Parameters.AddWithValue("@numeroNota", numeroNF);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string codigoMercadoria = reader["Codigo_Mercadoria"]?.ToString()?.Trim() ?? "";
                                string codBarras = reader["CodBarras"]?.ToString()?.Trim() ?? "";
                                int qtd = reader["Quantidade_Item"] != DBNull.Value
                                    ? Convert.ToInt32(reader["Quantidade_Item"])
                                    : 1;

                                bool encontrado = false;

                                // CONFECÇÃO: Buscar por CodBarras_Grade
                                if (!string.IsNullOrEmpty(codBarras))
                                {
                                    string queryGrade = "SELECT * FROM Mercadorias WHERE CodBarras_Grade = @cod LIMIT 1";
                                    using (var cmdLocal = new SQLiteCommand(queryGrade, connLocal))
                                    {
                                        cmdLocal.Parameters.AddWithValue("@cod", codBarras);
                                        using (var readerLocal = cmdLocal.ExecuteReader())
                                        {
                                            if (readerLocal.Read())
                                            {
                                                AdicionarRowCompleto(resultado, readerLocal, qtd);
                                                encontrado = true;
                                            }
                                        }
                                    }
                                }

                                // PADRÃO: Buscar por CodigoMercadoria
                                if (!encontrado && !string.IsNullOrEmpty(codigoMercadoria))
                                {
                                    string queryPadrao = "SELECT * FROM Mercadorias WHERE CodigoMercadoria = @cod LIMIT 1";
                                    using (var cmdLocal = new SQLiteCommand(queryPadrao, connLocal))
                                    {
                                        cmdLocal.Parameters.AddWithValue("@cod", codigoMercadoria);
                                        using (var readerLocal = cmdLocal.ExecuteReader())
                                        {
                                            if (readerLocal.Read())
                                            {
                                                AdicionarRowCompleto(resultado, readerLocal, qtd);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao carregar NF {numeroNF}: {ex.Message}", ex);
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

        // Método NOVO para Notas de Entrada - pega TUDO do banco local
        private static void AdicionarRowCompleto(DataTable dt, SQLiteDataReader reader, int quantidade)
        {
            DataRow row = dt.NewRow();

            // Função helper
            T GetValue<T>(string columnName, T defaultValue = default(T))
            {
                try
                {
                    if (reader[columnName] != DBNull.Value)
                        return (T)Convert.ChangeType(reader[columnName], typeof(T));
                }
                catch { }
                return defaultValue;
            }

            row["CodigoMercadoria"] = GetValue<string>("CodigoMercadoria", "");
            row["Mercadoria"] = GetValue<string>("Mercadoria", "");
            row["PrecoVenda"] = GetValue<decimal>("PrecoVenda", 0m);
            row["VendaA"] = GetValue<decimal>("VendaA", 0m);
            row["VendaB"] = GetValue<decimal>("VendaB", 0m);
            row["VendaC"] = GetValue<decimal>("VendaC", 0m);
            row["VendaD"] = GetValue<decimal>("VendaD", 0m);
            row["VendaE"] = GetValue<decimal>("VendaE", 0m);
            row["Grupo"] = GetValue<string>("Grupo", "");
            row["SubGrupo"] = GetValue<string>("SubGrupo", "");
            row["Fabricante"] = GetValue<string>("Fabricante", "");
            row["Fornecedor"] = GetValue<string>("Fornecedor", "");
            row["CodBarras"] = GetValue<string>("CodBarras", "");
            row["CodFabricante"] = GetValue<string>("CodFabricante", "");
            row["Tam"] = GetValue<string>("Tam", "");
            row["Cores"] = GetValue<string>("Cores", "");
            row["CodBarras_Grade"] = GetValue<string>("CodBarras_Grade", "");
            row["Prateleira"] = GetValue<string>("Prateleira", "");
            row["Garantia"] = GetValue<string>("Garantia", "");
            row["Registro"] = GetValue<int>("Registro", 0);
            row["Quantidade"] = quantidade;

            dt.Rows.Add(row);
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
        // ÃƒÂ°Ã…Â¸Ã¢â‚¬ÂÃ‚Â¹ PREÃ‡OS ALTERADOS
        // ========================================
        /// <summary>
        /// Carrega produtos com preÃ§os alterados no perÃƒÆ’Ã‚Â­odo
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

                    // TODO: Implementar quando houver campo de data de alteraÃ§ÃƒÆ’Ã‚Â£o de preÃ§o
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
                throw new Exception($"Erro ao carregar preÃ§os alterados: {ex.Message}", ex);
            }
        }

        // ========================================
        // ÃƒÂ°Ã…Â¸Ã¢â‚¬ÂÃ‚Â¹ PROMOÃ‡Ã•ES
        // ========================================
        /// <summary>
        /// Carrega produtos em promoÃ§ÃƒÆ’Ã‚Â£o com filtros especÃƒÆ’Ã‚Â­ficos
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

                    // TODO: Quando houver tabela de promoÃ§ÃƒÆ’Ã‚Âµes:
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
                throw new Exception($"Erro ao carregar promoÃ§ÃƒÆ’Ã‚Âµes: {ex.Message}", ex);
            }
        }

        // ========================================
        // ÃƒÂ°Ã…Â¸Ã¢â‚¬ÂÃ‚Â¹ LIMPAR ETIQUETAS EXISTENTES
        // ========================================
        /// <summary>
        /// Limpa produtos jÃƒÆ’Ã‚Â¡ carregados (equivalente ao DELETE no SoftShop)
        /// </summary>
        public static bool LimparEtiquetasCarregadas()
        {
            // Esta funcionalidade pode ser implementada se houver
            // uma "ÃƒÆ’Ã‚Â¡rea de staging" para produtos carregados
            // Por enquanto, apenas retorna true
            return true;
        }

        // ========================================
        // Ã°Å¸â€Â¹ MÃƒâ€°TODOS AUXILIARES - LEITURA SEGURA
        // ========================================

        /// <summary>
        /// LÃƒÂª campo string do reader verificando se existe
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
        /// LÃƒÂª campo decimal do reader verificando se existe
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
        /// LÃƒÂª campo int do reader verificando se existe
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