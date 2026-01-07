using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EtiquetaFORNew.Forms
{
    public partial class FormMenuConfiguracao : Form
    {
        public FormMenuConfiguracao()
        {
            InitializeComponent();
            VersaoHelper.DefinirTituloComVersao(this, "Menu de Configurações");
            ConfigurarVisualForm();
        }
        private void ConfigurarVisualForm()
        {
            // Remove os botões Maximizar/Minimizar/Fechar e a borda de redimensionamento.
            this.FormBorderStyle = FormBorderStyle.FixedToolWindow;

            // Se quiser remover TUDO (incluindo o ícone e a barra de título):
            // this.FormBorderStyle = FormBorderStyle.None;

            // Garante que a janela inicie no centro da tela.
            this.StartPosition = FormStartPosition.CenterScreen;

            // Define a cor de fundo (sugestão do SmartPrint)
            //this.BackColor = System.Drawing.Color.White;

            // Ação principal: Remove toda a moldura, barra de título e botões.
            this.FormBorderStyle = FormBorderStyle.None;

            // As linhas MinimizeBox/MaximizeBox são redundantes com FormBorderStyle.None, mas não atrapalham.
            this.MinimizeBox = false;
            this.MaximizeBox = false;

            this.ControlBox = false; // Garante que nenhum controle de sistema seja exibido.
            this.Text = string.Empty; // Define o título como vazio, apenas por garantia.
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private void btnNovaConfig_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Yes; // Sinaliza NOVO
            this.Close();
        }

        private void btnCarregar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.No; // Sinaliza CARREGAR
            this.Close();
        }
    }

}