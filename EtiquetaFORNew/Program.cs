using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// Suporta:
        /// - Modo Normal: SmartPrint.exe (uso padrão com login)
        /// - Modo SoftcomShop: Pula login e vai direto para FormPrincipal
        /// - Modo Importação: SmartPrint.exe "caminho\arquivo.json" (Softshop Access)
        /// - Modo API: SmartPrint.exe --api-import:dados (futuro - sistema web)
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ========================================
            // ⭐ INICIALIZAR BANCO LOCAL PRIMEIRO
            // ========================================
            InicializarBancoLocal();

            // ========================================
            // ⭐ VERIFICAR SE É MODO SOFTCOMSHOP
            // ========================================
            if (VerificarModoSoftcomShop())
            {
                // Modo SoftcomShop - pula login e vai direto para FormPrincipal
                Application.Run(new FormPrincipal());
                return;
            }

            // ========================================
            // 🔹 DETECTAR TIPO DE INICIALIZAÇÃO
            // ========================================
            var tipoImportacao = IntegracaoExterna.DetectarTipoImportacao(args);

            switch (tipoImportacao)
            {
                case IntegracaoExterna.TipoImportacao.Nenhuma:
                    // ✅ USO NORMAL - Abre tela de login (comportamento original)
                    Application.Run(new Main());
                    break;

                case IntegracaoExterna.TipoImportacao.ArquivoJSON:
                    // ✅ IMPORTAÇÃO SOFTSHOP - Processa arquivo e abre FormPrincipal direto
                    IniciarComImportacao(args[0], tipoImportacao);
                    break;

                case IntegracaoExterna.TipoImportacao.ArquivoXML:
                    // 📜 FUTURO: Importação XML se necessário
                    IniciarComImportacao(args[0], tipoImportacao);
                    break;

                case IntegracaoExterna.TipoImportacao.WebAPI:
                    // 📜 FUTURO: Importação via API REST
                    IniciarComImportacao(args[0], tipoImportacao);
                    break;
            }
            //testeGit
        }

        /// <summary>
        /// ⭐ NOVO: Inicializa o banco local SQLite
        /// Trata erro de migração caso banco antigo não tenha os campos necessários
        /// </summary>
        private static void InicializarBancoLocal()
        {
            try
            {
                LocalDatabaseManager.InicializarBanco();
            }
            catch (Exception ex)
            {
                // Se erro é relacionado a coluna faltando, tentar migração
                if (ex.Message.Contains("no such column") ||
                    ex.Message.Contains("ID_SoftcomShop"))
                {
                    try
                    {
                        // Deletar banco antigo e recriar
                        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalData.db");

                        if (File.Exists(dbPath))
                        {
                            File.Delete(dbPath);
                        }

                        // Tentar criar novamente
                        LocalDatabaseManager.InicializarBanco();
                    }
                    catch (Exception ex2)
                    {
                        MessageBox.Show(
                            $"Erro ao inicializar banco de dados local:\n\n{ex2.Message}\n\n" +
                            "O sistema pode não funcionar corretamente.",
                            "Erro Crítico",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                else
                {
                    // Outro tipo de erro
                    MessageBox.Show(
                        $"Aviso ao inicializar banco local:\n\n{ex.Message}\n\n" +
                        "O sistema continuará funcionando, mas alguns recursos podem estar limitados.",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// ⭐ NOVO: Verifica se o sistema está configurado para modo SoftcomShop
        /// </summary>
        private static bool VerificarModoSoftcomShop()
        {
            try
            {
                var config = ConfiguracaoSistema.Carregar();

                // Se tipo de conexão ativa é SoftcomShop E está configurado
                return config.TipoConexaoAtiva == TipoConexao.SoftcomShop &&
                       config.SoftcomShopConfigurado();
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Inicia SmartPrint com dados importados de sistema externo
        /// </summary>
        private static void IniciarComImportacao(string parametro, IntegracaoExterna.TipoImportacao tipo)
        {
            try
            {
                DadosImportacao dadosImportados = null;

                // Processar conforme tipo
                switch (tipo)
                {
                    case IntegracaoExterna.TipoImportacao.ArquivoJSON:
                        dadosImportados = IntegracaoExterna.ProcessarImportacaoJSON(parametro);
                        break;

                    case IntegracaoExterna.TipoImportacao.ArquivoXML:
                        dadosImportados = IntegracaoExterna.ProcessarImportacaoXML(parametro);
                        break;

                    case IntegracaoExterna.TipoImportacao.WebAPI:
                        dadosImportados = IntegracaoExterna.ProcessarImportacaoWebAPI(parametro);
                        break;
                }

                if (dadosImportados != null && dadosImportados.Itens.Count > 0)
                {
                    // Abrir FormPrincipal com dados importados
                    var formPrincipal = new FormPrincipal(dadosImportados);
                    Application.Run(formPrincipal);

                    // Limpar arquivo temporário após fechar o formulário
                    if (tipo == IntegracaoExterna.TipoImportacao.ArquivoJSON ||
                        tipo == IntegracaoExterna.TipoImportacao.ArquivoXML)
                    {
                        IntegracaoExterna.LimparArquivoTemporario(parametro);
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Nenhum item foi importado do arquivo fornecido.\n\n" +
                        "O SmartPrint será aberto no modo normal.",
                        "Importação Vazia",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    Application.Run(new Main());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao processar importação externa:\n\n{ex.Message}\n\n" +
                    "O SmartPrint será aberto no modo normal.",
                    "Erro de Importação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                // Em caso de erro, abrir normalmente
                Application.Run(new Main());
            }
        }
    }
}