using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew
{
    public partial class FormSincronizacaoSoftcomShop : Form
    {
        private ConfiguracaoSistema _config;
        private SoftcomShopDataManager _dataManager;
        private string _connectionString;

        public FormSincronizacaoSoftcomShop()
        {
            InitializeComponent();
        }

        private void FormSincronizacaoSoftcomShop_Load(object sender, EventArgs e)
        {
            // Carregar configurações
            _config = ConfiguracaoSistema.Carregar();

            // Verificar se SoftcomShop está configurado
            if (!_config.SoftcomShopConfigurado())
            {
                MessageBox.Show(
                    "SoftcomShop não está configurado!\n\n" +
                    "Configure em: Menu > Configurações",
                    "Atenção",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                
                this.Close();
                return;
            }

            // Obter connection string do SQLite
            _connectionString = LocalDatabaseManager.GetConnectionString();

            // Criar gerenciador de dados
            _dataManager = new SoftcomShopDataManager(_config.SoftcomShop, _connectionString);

            // Atualizar status
            AtualizarStatus();
        }

        private void AtualizarStatus()
        {
            if (_config.SoftcomShop.DataSync != null && !string.IsNullOrEmpty(_config.SoftcomShop.DataSync))
            {
                lblUltimaSinc.Text = $"Última sincronização: {_config.SoftcomShop.DataSync}";
            }
            else
            {
                lblUltimaSinc.Text = "Nenhuma sincronização realizada";
            }
        }

        #region Eventos dos Botões

        private async void btnSincronizarProdutos_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Deseja sincronizar TODOS os produtos?\n\n" +
                "Isso pode demorar alguns minutos dependendo da quantidade de produtos.",
                "Confirmar Sincronização",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            await SincronizarProdutosAsync();
        }

        private async Task SincronizarProdutosAsync()
        {
            try
            {
                // Desabilitar botões
                HabilitarBotoes(false);
                
                // Criar progress
                var progress = new Progress<string>(mensagem =>
                {
                    lblStatus.Text = mensagem;
                    Application.DoEvents();
                });

                lblStatus.Text = "Sincronizando produtos...";
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.Visible = true;

                // Sincronizar
                var syncResult = await _dataManager.SincronizarProdutosAsync("v2", progress);

                // Mostrar resultado
                progressBar.Visible = false;
                
                if (syncResult.Sucesso)
                {
                    MessageBox.Show(
                        $"Sincronização concluída com sucesso!\n\n" +
                        $"Produtos sincronizados: {syncResult.ProdutosAdicionados}",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    AtualizarStatus();
                    lblStatus.Text = "Pronto";
                }
                else
                {
                    MessageBox.Show(
                        $"Erro ao sincronizar:\n\n{syncResult.MensagemErro}",
                        "Erro",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    
                    lblStatus.Text = "Erro na sincronização";
                }
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                MessageBox.Show(
                    $"Erro inesperado:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                lblStatus.Text = "Erro";
            }
            finally
            {
                HabilitarBotoes(true);
            }
        }

        private async void btnBuscarNotaFiscal_Click(object sender, EventArgs e)
        {
            // Criar formulário de entrada
            using (var form = new FormBuscarNotaFiscal())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    await BuscarNotaFiscalAsync(form.DataEntrada, form.NumeroNota);
                }
            }
        }

        private async Task BuscarNotaFiscalAsync(DateTime dataEntrada, int numeroNota)
        {
            try
            {
                HabilitarBotoes(false);

                var progress = new Progress<string>(mensagem =>
                {
                    lblStatus.Text = mensagem;
                    Application.DoEvents();
                });

                lblStatus.Text = "Buscando nota fiscal...";
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.Visible = true;

                var syncResult = await _dataManager.BuscarPorNotaFiscalAsync(dataEntrada, numeroNota, "v2", progress);

                progressBar.Visible = false;

                if (syncResult.Sucesso)
                {
                    MessageBox.Show(
                        $"Produtos carregados com sucesso!\n\n" +
                        $"Total: {syncResult.ProdutosAdicionados} produtos\n\n" +
                        $"Os produtos foram marcados para impressão de etiquetas.",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    lblStatus.Text = "Pronto";
                }
                else
                {
                    MessageBox.Show(
                        syncResult.MensagemErro,
                        "Atenção",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    
                    lblStatus.Text = "Nenhum produto encontrado";
                }
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                MessageBox.Show(
                    $"Erro ao buscar nota fiscal:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                lblStatus.Text = "Erro";
            }
            finally
            {
                HabilitarBotoes(true);
            }
        }

        private async void btnBuscarVenda_Click(object sender, EventArgs e)
        {
            string input = Prompt.ShowDialog("Informe o número da venda:", "Buscar Venda");

            if (string.IsNullOrWhiteSpace(input))
                return;

            if (!int.TryParse(input, out int numeroVenda))
            {
                MessageBox.Show("Número inválido!", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await BuscarVendaAsync(numeroVenda);
        }

        private async Task BuscarVendaAsync(int numeroVenda)
        {
            try
            {
                HabilitarBotoes(false);

                var progress = new Progress<string>(mensagem =>
                {
                    lblStatus.Text = mensagem;
                    Application.DoEvents();
                });

                lblStatus.Text = $"Buscando venda {numeroVenda}...";
                progressBar.Style = ProgressBarStyle.Marquee;
                progressBar.Visible = true;

                var syncResult = await _dataManager.BuscarPorVendaAsync(numeroVenda, progress);

                progressBar.Visible = false;

                if (syncResult.Sucesso)
                {
                    MessageBox.Show(
                        $"Produtos da venda carregados com sucesso!\n\n" +
                        $"Total: {syncResult.ProdutosAdicionados} produtos\n\n" +
                        $"Os produtos foram marcados para impressão de etiquetas.",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    lblStatus.Text = "Pronto";
                }
                else
                {
                    MessageBox.Show(
                        syncResult.MensagemErro,
                        "Atenção",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    
                    lblStatus.Text = "Venda não encontrada";
                }
            }
            catch (Exception ex)
            {
                progressBar.Visible = false;
                MessageBox.Show(
                    $"Erro ao buscar venda:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                
                lblStatus.Text = "Erro";
            }
            finally
            {
                HabilitarBotoes(true);
            }
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Métodos Auxiliares

        private void HabilitarBotoes(bool habilitar)
        {
            btnSincronizarProdutos.Enabled = habilitar;
            btnBuscarNotaFiscal.Enabled = habilitar;
            btnBuscarVenda.Enabled = habilitar;
            btnFechar.Enabled = habilitar;
        }

        #endregion
    }

    /// <summary>
    /// Classe auxiliar para InputBox simples
    /// </summary>
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 400,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label textLabel = new Label() { Left = 20, Top = 20, Text = text, Width = 350 };
            TextBox textBox = new TextBox() { Left = 20, Top = 50, Width = 350 };
            Button confirmation = new Button() { Text = "OK", Left = 220, Width = 70, Top = 80, DialogResult = DialogResult.OK };
            Button cancel = new Button() { Text = "Cancelar", Left = 300, Width = 70, Top = 80, DialogResult = DialogResult.Cancel };

            confirmation.Click += (sender, e) => { prompt.Close(); };
            cancel.Click += (sender, e) => { prompt.Close(); };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(cancel);
            prompt.AcceptButton = confirmation;
            prompt.CancelButton = cancel;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
        }
    }

    /// <summary>
    /// Formulário auxiliar para entrada de dados da nota fiscal
    /// </summary>
    public class FormBuscarNotaFiscal : Form
    {
        private DateTimePicker dtpDataEntrada;
        private TextBox txtNumeroNota;
        private Button btnOK;
        private Button btnCancelar;
        private Label lblData;
        private Label lblNumero;

        public DateTime DataEntrada { get; private set; }
        public int NumeroNota { get; private set; }

        public FormBuscarNotaFiscal()
        {
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            // Form
            this.Text = "Buscar Nota Fiscal";
            this.Size = new System.Drawing.Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Label Data
            lblData = new Label();
            lblData.Text = "Data de Entrada:";
            lblData.Location = new System.Drawing.Point(20, 20);
            lblData.Size = new System.Drawing.Size(120, 20);
            this.Controls.Add(lblData);

            // DateTimePicker
            dtpDataEntrada = new DateTimePicker();
            dtpDataEntrada.Location = new System.Drawing.Point(20, 45);
            dtpDataEntrada.Size = new System.Drawing.Size(340, 20);
            dtpDataEntrada.Format = DateTimePickerFormat.Short;
            this.Controls.Add(dtpDataEntrada);

            // Label Numero
            lblNumero = new Label();
            lblNumero.Text = "Número da Nota (opcional):";
            lblNumero.Location = new System.Drawing.Point(20, 80);
            lblNumero.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(lblNumero);

            // TextBox Numero
            txtNumeroNota = new TextBox();
            txtNumeroNota.Location = new System.Drawing.Point(20, 105);
            txtNumeroNota.Size = new System.Drawing.Size(200, 20);
            this.Controls.Add(txtNumeroNota);

            // Botão OK
            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Location = new System.Drawing.Point(195, 135);
            btnOK.Size = new System.Drawing.Size(80, 25);
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            // Botão Cancelar
            btnCancelar = new Button();
            btnCancelar.Text = "Cancelar";
            btnCancelar.Location = new System.Drawing.Point(280, 135);
            btnCancelar.Size = new System.Drawing.Size(80, 25);
            btnCancelar.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancelar);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancelar;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            DataEntrada = dtpDataEntrada.Value;
            
            if (!string.IsNullOrWhiteSpace(txtNumeroNota.Text))
            {
                if (int.TryParse(txtNumeroNota.Text, out int numero))
                {
                    NumeroNota = numero;
                }
            }
        }
    }
}
