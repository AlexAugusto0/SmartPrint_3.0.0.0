using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    internal static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// Suporta:
        /// - Modo Normal: SmartPrint.exe (uso padrão com login)
        /// - Modo Importação: SmartPrint.exe "caminho\arquivo.json" (Softshop Access)
        /// - Modo API: SmartPrint.exe --api-import:dados (futuro - sistema web)
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

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
                    // 🔜 FUTURO: Importação XML se necessário
                    IniciarComImportacao(args[0], tipoImportacao);
                    break;

                case IntegracaoExterna.TipoImportacao.WebAPI:
                    // 🔜 FUTURO: Importação via API REST
                    IniciarComImportacao(args[0], tipoImportacao);
                    break;
            }
            //testeGit
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
