using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew
{
    /// <summary>
    /// Formulário SIMPLIFICADO de carregamento - COM SUPORTE A PROMOÇÕES
    /// Funciona imediatamente sem precisar alterar o banco
    /// Versão 2.3 - Promoções Ativas
    /// </summary>
    public partial class FormFiltrosCarregamento : Form
    {
        // Controles
        private ComboBox cmbTipo;
        private ComboBox cmbGrupo;
        private ComboBox cmbFabricante;
        private ComboBox cmbFornecedor;
        private ComboBox cmbEmpresa;
        private ComboBox cmbPromocao; // ⭐ NOVO
        private TextBox txtDocumento;
        private DateTimePicker dtpDataInicial;
        private DateTimePicker dtpDataFinal;
        private CheckBox chkUsarFiltroData;
        private Button btnCancelar;
        private Button btnConfirmar;
        private Button btnLimparFiltros;

        private Label lblTipo;
        private Label lblGrupo;
        private Label lblFabricante;
        private Label lblFornecedor;
        private Label lblEmpresa;
        private Label lblPromocao; // ⭐ NOVO
        private Label lblDocumento;
        private Label lblDataInicial;
        private Label lblDataFinal;
        private Label lblTitulo;
        private Panel panelFiltros;

        // Propriedades
        public string TipoSelecionado { get; private set; }
        public string GrupoSelecionado { get; private set; }
        public string FabricanteSelecionado { get; private set; }
        public string FornecedorSelecionado { get; private set; }
        public string EmpresaSelecionada { get; private set; }
        public int? PromocaoSelecionada { get; private set; } // ⭐ NOVO
        public string DocumentoInformado { get; private set; }
        public DateTime? DataInicial { get; private set; }
        public DateTime? DataFinal { get; private set; }
        public bool UsarFiltroData { get; private set; }

        // ⭐ Propriedade vazia para compatibilidade
        public string SubGrupoSelecionado { get; private set; } = "";
        public string ProdutoSelecionado { get; private set; } = "";

        public FormFiltrosCarregamento()
        {
            InitializeComponent();
            ConfigurarFormulario();
            CarregarDados();
            AplicarPermissoesFiltros();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new System.Drawing.Size(500, 540);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "SmartPrint - Carregamento de Produtos";
            this.BackColor = Color.FromArgb(245, 245, 245);

            lblTitulo = new Label
            {
                Text = "🔍 CARREGAR PRODUTOS",
                Location = new Point(0, 0),
                Size = new Size(500, 50),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                BackColor = Color.FromArgb(255, 165, 0),
                ForeColor = Color.White
            };

            panelFiltros = new Panel
            {
                Location = new Point(10, 60),
                Size = new Size(480, 400),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // TIPO
            lblTipo = CriarLabel("Tipo de Carregamento:", 10, 15);
            cmbTipo = CriarComboBox(180, 10);

            // GRUPO
            lblGrupo = CriarLabel("Grupo:", 10, 55);
            cmbGrupo = CriarComboBox(180, 50);

            // FABRICANTE
            lblFabricante = CriarLabel("Fabricante:", 10, 95);
            cmbFabricante = CriarComboBox(180, 90);

            // FORNECEDOR
            lblFornecedor = CriarLabel("Fornecedor:", 10, 135);
            cmbFornecedor = CriarComboBox(180, 130);

            // ⭐ PROMOÇÃO
            lblPromocao = CriarLabel("Promoção:", 10, 175);
            cmbPromocao = CriarComboBox(180, 170);
            cmbPromocao.DisplayMember = "Descricao";
            cmbPromocao.ValueMember = "ID_Promocao";
            lblPromocao.Visible = false;
            cmbPromocao.Visible = false;

            // DOCUMENTO
            lblDocumento = CriarLabel("Documento/NF:", 10, 215);
            txtDocumento = new TextBox
            {
                Location = new Point(180, 210),
                Size = new Size(280, 25),
                Font = new Font("Segoe UI", 9F)
            };
            lblDocumento.Visible = false;
            txtDocumento.Visible = false;

            // FILTRO DE DATA
            chkUsarFiltroData = new CheckBox
            {
                Text = "Filtrar por Data",
                Location = new Point(10, 255),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 9F)
            };
            chkUsarFiltroData.CheckedChanged += ChkUsarFiltroData_CheckedChanged;

            // DATA INICIAL
            lblDataInicial = CriarLabel("Data Inicial:", 10, 295);
            dtpDataInicial = new DateTimePicker
            {
                Location = new Point(180, 290),
                Size = new Size(280, 25),
                Font = new Font("Segoe UI", 9F),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };

            // DATA FINAL
            lblDataFinal = CriarLabel("Data Final:", 10, 335);
            dtpDataFinal = new DateTimePicker
            {
                Location = new Point(180, 330),
                Size = new Size(280, 25),
                Font = new Font("Segoe UI", 9F),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };

            // EMPRESA
            lblEmpresa = CriarLabel("Empresa:", 10, 375);
            cmbEmpresa = CriarComboBox(180, 370);

            panelFiltros.Controls.Add(lblTipo);
            panelFiltros.Controls.Add(cmbTipo);
            panelFiltros.Controls.Add(lblGrupo);
            panelFiltros.Controls.Add(cmbGrupo);
            panelFiltros.Controls.Add(lblFabricante);
            panelFiltros.Controls.Add(cmbFabricante);
            panelFiltros.Controls.Add(lblFornecedor);
            panelFiltros.Controls.Add(cmbFornecedor);
            panelFiltros.Controls.Add(lblPromocao); // ⭐ NOVO
            panelFiltros.Controls.Add(cmbPromocao); // ⭐ NOVO
            panelFiltros.Controls.Add(lblDocumento);
            panelFiltros.Controls.Add(txtDocumento);
            panelFiltros.Controls.Add(chkUsarFiltroData);
            panelFiltros.Controls.Add(lblDataInicial);
            panelFiltros.Controls.Add(dtpDataInicial);
            panelFiltros.Controls.Add(lblDataFinal);
            panelFiltros.Controls.Add(dtpDataFinal);
            panelFiltros.Controls.Add(lblEmpresa);
            panelFiltros.Controls.Add(cmbEmpresa);

            btnLimparFiltros = new Button
            {
                Text = "Limpar",
                Location = new Point(30, 475),
                Size = new Size(100, 45),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLimparFiltros.FlatAppearance.BorderSize = 0;
            btnLimparFiltros.Click += BtnLimparFiltros_Click;

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(150, 475),
                Size = new Size(130, 45),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;

            btnConfirmar = new Button
            {
                Text = "Carregar",
                Location = new Point(300, 475),
                Size = new Size(170, 45),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;

            this.Controls.Add(lblTitulo);
            this.Controls.Add(panelFiltros);
            this.Controls.Add(btnLimparFiltros);
            this.Controls.Add(btnCancelar);
            this.Controls.Add(btnConfirmar);

            this.ResumeLayout(false);
        }

        private Label CriarLabel(string texto, int x, int y)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(x, y),
                Size = new Size(160, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };
        }

        private ComboBox CriarComboBox(int x, int y)
        {
            return new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(280, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9F)
            };
        }

        private void ConfigurarFormulario()
        {
            cmbTipo.Items.Add("FILTROS MANUAIS");
            //cmbTipo.Items.Add("AJUSTES");
            //cmbTipo.Items.Add("BALANÇOS");
            cmbTipo.Items.Add("NOTAS ENTRADA");
            //cmbTipo.Items.Add("PREÇOS ALTERADOS");
            cmbTipo.Items.Add("PROMOÇÕES");
            cmbTipo.SelectedIndex = 0;

            cmbEmpresa.Items.Add("MATRIZ");
            cmbEmpresa.SelectedIndex = 0;

            cmbTipo.SelectedIndexChanged += CmbTipo_SelectedIndexChanged;
        }

        private void CarregarDados()
        {
            try
            {
                CarregarComboDistinto(cmbGrupo, "Grupo");
                CarregarComboDistinto(cmbFabricante, "Fabricante");
                CarregarComboDistinto(cmbFornecedor, "Fornecedor");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CarregarComboDistinto(ComboBox combo, string campo)
        {
            try
            {
                DataTable dt = LocalDatabaseManager.ObterValoresDistintos(campo);

                combo.Items.Clear();
                combo.Items.Add("");

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string valor = row[campo]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(valor))
                        {
                            combo.Items.Add(valor);
                        }
                    }
                }

                combo.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar {campo}: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CmbTipo_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarPermissoesFiltros();
        }

        private void ChkUsarFiltroData_CheckedChanged(object sender, EventArgs e)
        {
            bool usar = chkUsarFiltroData.Checked;
            dtpDataInicial.Enabled = usar;
            dtpDataFinal.Enabled = usar;

            if (usar)
            {
                dtpDataFinal.Value = DateTime.Now;
                dtpDataInicial.Value = DateTime.Now.AddDays(-30);
            }
        }

        private void AplicarPermissoesFiltros()
        {
            string tipoSelecionado = cmbTipo.Text;

            // Reset
            cmbGrupo.Enabled = true;
            cmbFabricante.Enabled = true;
            cmbFornecedor.Enabled = true;
            lblPromocao.Visible = false;
            cmbPromocao.Visible = false;
            lblDocumento.Visible = false;
            txtDocumento.Visible = false;
            chkUsarFiltroData.Visible = true;

            switch (tipoSelecionado)
            {
                case "AJUSTES":
                    cmbGrupo.Enabled = false;
                    cmbFabricante.Enabled = false;
                    cmbFornecedor.Enabled = false;
                    lblDocumento.Text = "Número do Ajuste:";
                    lblDocumento.Visible = true;
                    txtDocumento.Visible = true;
                    break;

                case "BALANÇOS":
                    cmbGrupo.Enabled = false;
                    cmbFabricante.Enabled = false;
                    cmbFornecedor.Enabled = false;
                    lblDocumento.Text = "Número do Balanço:";
                    lblDocumento.Visible = true;
                    txtDocumento.Visible = true;
                    break;

                case "NOTAS ENTRADA":
                    cmbGrupo.Enabled = false;
                    cmbFabricante.Enabled = false;
                    cmbFornecedor.Enabled = false;
                    lblDocumento.Text = "Número da NF:";
                    lblDocumento.Visible = true;
                    txtDocumento.Visible = true;
                    break;

                case "PREÇOS ALTERADOS":
                    cmbGrupo.Enabled = false;
                    cmbFabricante.Enabled = false;
                    cmbFornecedor.Enabled = false;
                    chkUsarFiltroData.Checked = true;
                    break;

                case "PROMOÇÕES":
                    // ⭐ Mostrar combo de promoções e carregar promoções ativas
                    lblPromocao.Visible = true;
                    cmbPromocao.Visible = true;
                    CarregarPromocoesAtivas();

                    chkUsarFiltroData.Visible = false;
                    chkUsarFiltroData.Checked = false;
                    break;
            }
        }

        /// <summary>
        /// ⭐ NOVO: Carrega promoções ativas no ComboBox
        /// </summary>
        private void CarregarPromocoesAtivas()
        {
            try
            {
                Cursor = Cursors.WaitCursor;

                DataTable promocoes = PromocoesManager.BuscarPromocoesAtivas();

                cmbPromocao.DataSource = null;
                cmbPromocao.Items.Clear();

                if (promocoes != null && promocoes.Rows.Count > 0)
                {
                    cmbPromocao.DisplayMember = "Descricao";
                    cmbPromocao.ValueMember = "ID_Promocao";
                    cmbPromocao.DataSource = promocoes;
                }
                else
                {
                    MessageBox.Show(
                        "Não há promoções ativas no momento.",
                        "SmartPrint - Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar promoções:\n{ex.Message}",
                    "SmartPrint - Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void BtnLimparFiltros_Click(object sender, EventArgs e)
        {
            cmbGrupo.SelectedIndex = 0;
            cmbFabricante.SelectedIndex = 0;
            cmbFornecedor.SelectedIndex = 0;
            txtDocumento.Clear();
            chkUsarFiltroData.Checked = false;
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            string tipoSelecionado = cmbTipo.Text;

            if (string.IsNullOrEmpty(tipoSelecionado))
            {
                MessageBox.Show("Selecione um Tipo de Carregamento!",
                    "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbTipo.Focus();
                return;
            }

            switch (tipoSelecionado)
            {
                case "AJUSTES":
                case "BALANÇOS":
                case "NOTAS ENTRADA":
                    if (string.IsNullOrWhiteSpace(txtDocumento.Text))
                    {
                        MessageBox.Show($"Informe o número do documento para {tipoSelecionado}!",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtDocumento.Focus();
                        return;
                    }
                    break;

                case "PREÇOS ALTERADOS":
                    if (!chkUsarFiltroData.Checked)
                    {
                        MessageBox.Show("O filtro de data é obrigatório para PREÇOS ALTERADOS!",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    break;

                case "PROMOÇÕES":
                case "FILTROS MANUAIS":
                    if (tipoSelecionado == "PROMOÇÕES")
                    {
                        // Validar se promoção foi selecionada
                        if (cmbPromocao.SelectedValue == null)
                        {
                            MessageBox.Show("Selecione uma promoção!",
                                "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            cmbPromocao.Focus();
                            return;
                        }
                    }

                    // Para filtros manuais, pelo menos um filtro é obrigatório
                    if (tipoSelecionado == "FILTROS MANUAIS" &&
                        string.IsNullOrEmpty(cmbGrupo.Text) &&
                        string.IsNullOrEmpty(cmbFabricante.Text) &&
                        string.IsNullOrEmpty(cmbFornecedor.Text) &&
                        !chkUsarFiltroData.Checked)
                    {
                        MessageBox.Show("Selecione pelo menos um filtro!",
                            "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    break;
            }

            TipoSelecionado = tipoSelecionado;
            GrupoSelecionado = cmbGrupo.Text;
            FabricanteSelecionado = cmbFabricante.Text;
            FornecedorSelecionado = cmbFornecedor.Text;
            EmpresaSelecionada = cmbEmpresa.Text;
            DocumentoInformado = txtDocumento.Text;
            UsarFiltroData = chkUsarFiltroData.Checked;

            // ⭐ Armazenar ID da promoção se for tipo PROMOÇÕES
            if (tipoSelecionado == "PROMOÇÕES" && cmbPromocao.SelectedValue != null)
            {
                PromocaoSelecionada = Convert.ToInt32(cmbPromocao.SelectedValue);
            }
            else
            {
                PromocaoSelecionada = null;
            }

            if (UsarFiltroData)
            {
                DataInicial = dtpDataInicial.Value.Date;
                DataFinal = dtpDataFinal.Value.Date;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}