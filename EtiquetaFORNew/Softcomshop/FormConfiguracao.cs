using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace EtiquetaFORNew
{
    public partial class FormConfiguracao : Form
    {
        private ConfiguracaoSistema _config;
        private SoftcomShopService _service;
        private ConfigForm _configFormSql; // ⭐ Instância do ConfigForm existente

        public FormConfiguracao()
        {
            InitializeComponent();
            CarregarConfiguracoes();

            // Limpar ConfigForm quando fechar
            this.FormClosing += FormConfiguracao_FormClosing;
        }

        private void FormConfiguracao_Load(object sender, EventArgs e)
        {
            // Configurar ComboBox de tipo de conexão
            cboTipoConexao.Items.Clear();
            cboTipoConexao.Items.Add("SQL Server");
            cboTipoConexao.Items.Add("SoftcomShop");

            // Selecionar tipo de conexão atual
            if (_config.TipoConexaoAtiva == TipoConexao.SqlServer)
                cboTipoConexao.SelectedIndex = 0;
            else
                cboTipoConexao.SelectedIndex = 1;

            // Atualizar visibilidade dos painéis
            AtualizarPaineis();
        }

        #region Carregar/Salvar Configurações

        private void CarregarConfiguracoes()
        {
            try
            {
                _config = ConfiguracaoSistema.Carregar();

                // Carregar configurações SoftcomShop
                if (_config.SoftcomShop != null)
                {
                    txtBaseURL.Text = _config.SoftcomShop.BaseURL;
                    txtClientId.Text = _config.SoftcomShop.ClientId;
                    txtClientSecret.Text = _config.SoftcomShop.ClientSecret;
                    txtEmpresaName.Text = _config.SoftcomShop.CompanyName;
                    txtEmpresaCNPJ.Text = _config.SoftcomShop.CompanyCNPJ;
                    txtDeviceName.Text = _config.SoftcomShop.DeviceName;
                    txtDeviceId.Text = _config.SoftcomShop.DeviceId;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar configurações: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SalvarConfiguracoes()
        {
            try
            {
                // Atualizar tipo de conexão
                _config.TipoConexaoAtiva = cboTipoConexao.SelectedIndex == 0
                    ? TipoConexao.SqlServer
                    : TipoConexao.SoftcomShop;

                // Salvar configurações SoftcomShop
                if (cboTipoConexao.SelectedIndex == 1) // SoftcomShop
                {
                    if (_config.SoftcomShop == null)
                        _config.SoftcomShop = new SoftcomShopConfig();

                    _config.SoftcomShop.BaseURL = txtBaseURL.Text.Trim();
                    _config.SoftcomShop.ClientId = txtClientId.Text.Trim();
                    _config.SoftcomShop.ClientSecret = txtClientSecret.Text.Trim();
                    _config.SoftcomShop.CompanyName = txtEmpresaName.Text.Trim();
                    _config.SoftcomShop.CompanyCNPJ = txtEmpresaCNPJ.Text.Trim();
                    _config.SoftcomShop.DeviceName = txtDeviceName.Text.Trim();
                    _config.SoftcomShop.DeviceId = txtDeviceId.Text.Trim();
                }

                // Salvar no arquivo
                _config.Salvar();

                MessageBox.Show("Configurações salvas com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar configurações: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Eventos dos Controles

        private void cboTipoConexao_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarPaineis();
        }

        private void AtualizarPaineis()
        {
            if (cboTipoConexao.SelectedIndex == 0) // SQL Server
            {
                panelSqlServer.Visible = true;
                panelSoftcomShop.Visible = false;

                // ⭐ CARREGAR ConfigForm dentro do painel
                CarregarConfigFormSql();
            }
            else // SoftcomShop
            {
                panelSqlServer.Visible = false;
                panelSoftcomShop.Visible = true;

                // ⭐ REMOVER ConfigForm se existir
                RemoverConfigFormSql();
            }
        }

        /// <summary>
        /// ⭐ NOVO: Carrega o ConfigForm existente dentro do painel SQL
        /// </summary>
        private void CarregarConfigFormSql()
        {
            // Se já existe, não cria de novo
            if (_configFormSql != null)
                return;

            // Criar instância do ConfigForm
            _configFormSql = new ConfigForm();
            _configFormSql.TopLevel = false;
            _configFormSql.FormBorderStyle = FormBorderStyle.None;
            _configFormSql.Dock = DockStyle.Fill;

            // Adicionar ao painel
            panelSqlServer.Controls.Clear();
            panelSqlServer.Controls.Add(_configFormSql);
            _configFormSql.Show();
        }

        /// <summary>
        /// ⭐ NOVO: Remove o ConfigForm quando trocar para SoftcomShop
        /// </summary>
        private void RemoverConfigFormSql()
        {
            if (_configFormSql != null)
            {
                panelSqlServer.Controls.Remove(_configFormSql);
                _configFormSql.Dispose();
                _configFormSql = null;
            }
        }

        private void btnGerarDeviceId_Click(object sender, EventArgs e)
        {
            txtDeviceId.Text = new SoftcomShopConfig().DeviceId;
            MessageBox.Show("Device ID gerado com sucesso!",
                "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            // 1. Se for SQL Server (Index 0)
            if (cboTipoConexao.SelectedIndex == 0)
            {
                if (_configFormSql != null)
                {
                    // Tentamos encontrar o botão salvar dentro do formulário SQL e disparar o clique
                    // Assumindo que o nome do botão no ConfigForm original seja 'btnSalvar'
                    Control[] bttns = _configFormSql.Controls.Find("btnSalvar", true);
                    if (bttns.Length > 0 && bttns[0] is Button btnSql)
                    {
                        btnSql.PerformClick();

                        // Se o formulário SQL fechar após salvar, precisamos fechar este também
                        // Mas geralmente, verificamos se o salvamento ocorreu bem:
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        // Fallback: Caso o botão não seja encontrado por nome, 
                        // você precisaria tornar o método btnSalvar_Click do ConfigForm 'public'
                        // e chamá-lo assim: _configFormSql.btnSalvar_Click(sender, e);
                        MessageBox.Show("Não foi possível acionar o salvamento do SQL Server automaticamente.", "Aviso");
                    }
                }
            }
            // 2. Se for SoftcomShop (Index 1)
            else if (cboTipoConexao.SelectedIndex == 1)
            {
                if (!ValidarCamposSoftcomShop())
                    return;

                SalvarConfiguracoes(); // Chama sua rotina interna que já salva o Softcom
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private async void btnTestarConexao_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposSoftcomShop())
                return;

            // Salvar antes de testar
            SalvarConfiguracoes();

            try
            {
                btnTestarConexao.Enabled = false;
                Cursor = Cursors.WaitCursor;

                _service = new SoftcomShopService(_config.SoftcomShop);
                bool sucesso = await _service.TestarConexaoAsync();

                if (sucesso)
                {
                    MessageBox.Show("Conexão estabelecida com sucesso!",
                        "Teste de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Falha ao conectar. Verifique as credenciais.",
                        "Teste de Conexão", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao testar conexão: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnTestarConexao.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private async void btnCadastrarDispositivo_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposSoftcomShop())
                return;

            var result = MessageBox.Show(
                $"Deseja cadastrar este dispositivo no SoftcomShop?\n\nDispositivo: {txtDeviceName.Text}",
                "Confirmar Cadastro",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            // Salvar antes de cadastrar
            SalvarConfiguracoes();

            try
            {
                btnCadastrarDispositivo.Enabled = false;
                Cursor = Cursors.WaitCursor;

                _service = new SoftcomShopService(_config.SoftcomShop);
                string clientSecret = await _service.CadastrarDispositivoAsync();

                if (!string.IsNullOrEmpty(clientSecret))
                {
                    // Atualizar Client Secret
                    txtClientSecret.Text = clientSecret;
                    _config.SoftcomShop.ClientSecret = clientSecret;
                    _config.Salvar();

                    MessageBox.Show("Dispositivo cadastrado com sucesso!\n\nClient Secret foi gerado e salvo automaticamente.",
                        "Cadastro Concluído", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Falha ao cadastrar dispositivo.\n\nVerifique se os dados estão corretos.",
                        "Erro no Cadastro", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao cadastrar dispositivo: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnCadastrarDispositivo.Enabled = true;
                Cursor = Cursors.Default;
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Validações

        private bool ValidarCamposSoftcomShop()
        {
            if (string.IsNullOrWhiteSpace(txtBaseURL.Text))
            {
                MessageBox.Show("Informe a URL Base da API!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBaseURL.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtClientId.Text))
            {
                MessageBox.Show("Informe o Client ID!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtClientId.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmpresaName.Text))
            {
                MessageBox.Show("Informe o Nome da Empresa!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpresaName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmpresaCNPJ.Text))
            {
                MessageBox.Show("Informe o CNPJ da Empresa!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpresaCNPJ.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDeviceName.Text))
            {
                MessageBox.Show("Informe o Nome do Dispositivo!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtDeviceName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtDeviceId.Text))
            {
                MessageBox.Show("Gere um Device ID!", "Validação", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnGerarDeviceId.Focus();
                return false;
            }

            return true;
        }

        #endregion

        #region Cleanup

        /// <summary>
        /// Limpa recursos quando o formulário é fechado
        /// </summary>
        private void FormConfiguracao_FormClosing(object sender, FormClosingEventArgs e)
        {
            RemoverConfigFormSql();
        }

        #endregion
    }
}