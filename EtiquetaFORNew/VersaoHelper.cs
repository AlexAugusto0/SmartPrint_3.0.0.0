using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Classe auxiliar para gerenciamento de versionamento da aplicação
    /// </summary>
    public static class VersaoHelper
    {
        private static string _versaoCache = null;

        /// <summary>
        /// Obtém a versão atual da aplicação
        /// </summary>
        public static string ObterVersao()
        {
            if (_versaoCache == null)
            {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var fileInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
                    _versaoCache = fileInfo.FileVersion;
                }
                catch
                {
                    _versaoCache = "1.0.0.0";
                }
            }
            return _versaoCache;
        }

        /// <summary>
        /// Obtém a versão formatada (ex: "v1.0.1.0")
        /// </summary>
        public static string ObterVersaoFormatada()
        {
            return $"v{ObterVersao()}";
        }

        /// <summary>
        /// Define o título do form com a versão automaticamente
        /// Uso: VersaoHelper.DefinirTituloComVersao(this, "Menu Principal");
        /// Resultado: "SmartPrint v1.0.1.0 - Menu Principal"
        /// </summary>
        public static void DefinirTituloComVersao(Form form, string tituloBase)
        {
            form.Text = $"SmartPrint {ObterVersaoFormatada()} - {tituloBase}";
        }

        /// <summary>
        /// Define apenas "SmartPrint v1.0.1.0" sem subtítulo
        /// </summary>
        public static void DefinirTituloComVersao(Form form)
        {
            form.Text = $"SmartPrint {ObterVersaoFormatada()}";
        }

        /// <summary>
        /// Aplica a versão a uma Label específica
        /// </summary>
        public static void AplicarVersaoLabel(Label label)
        {
            label.Text = ObterVersao();
        }
    }
}
