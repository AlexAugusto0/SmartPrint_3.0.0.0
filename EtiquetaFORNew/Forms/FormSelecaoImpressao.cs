using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace EtiquetaFORNew.Forms
{
    public partial class FormSelecaoImpressao : Form
    {
        public string TemplateSelecionado { get; private set; }
        public ConfiguracaoEtiqueta ConfiguracaoSelecionada { get; private set; }

        private PictureBox pbPreview;
        private ComboBox cmbTemplate;
        private ComboBox cmbImpressora;
        private ComboBox cmbPapel;
        private Button btnConfigurar;
        private Button btnConfirmar;
        private Button btnCancelar;
        private Label lblInfo;

        public FormSelecaoImpressao()
        {
            InitializeComponent();
            VersaoHelper.DefinirTituloComVersao(this, "Seleção de Impressão");
            this.Text = "Configuração de Impressão";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            CriarControles();
            CarregarDados();

            // NOVO: Carrega automaticamente o template padrão
            CarregarTemplatePadraoAutomaticamente();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.Name = "FormSelecaoImpressao";
            this.ResumeLayout(false);
        }

        private void CriarControles()
        {
            // Panel principal com borda
            Panel panelPrincipal = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(460, 340),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelPrincipal);

            // Logo/Título
            Label lblTitulo = new Label
            {
                Text = "SOFTSHOP - Configurações das Etiquetas",
                Location = new Point(15, 15),
                Size = new Size(430, 25),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(230, 126, 34),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelPrincipal.Controls.Add(lblTitulo);

            // Linha separadora 1
            Panel linha1 = new Panel
            {
                Location = new Point(15, 45),
                Size = new Size(430, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelPrincipal.Controls.Add(linha1);

            // ==================== SEÇÃO 1: Seleção de Template ====================
            Label lblSecao1 = new Label
            {
                Text = "1 Selecione uma Etiqueta:",
                Location = new Point(15, 55),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelPrincipal.Controls.Add(lblSecao1);

            Label lblTemplate = new Label
            {
                Text = "Etiqueta:",
                Location = new Point(30, 85),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelPrincipal.Controls.Add(lblTemplate);

            cmbTemplate = new ComboBox
            {
                Location = new Point(135, 83),
                Size = new Size(300, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(255, 255, 204) // Amarelo claro
            };
            cmbTemplate.SelectedIndexChanged += CmbTemplate_SelectedIndexChanged;
            panelPrincipal.Controls.Add(cmbTemplate);

            // ==================== SEÇÃO 2: Dados da Etiqueta ====================
            Label lblSecao2 = new Label
            {
                Text = "2 Dados da Etiqueta:",
                Location = new Point(15, 120),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelPrincipal.Controls.Add(lblSecao2);

            // Impressora
            Label lblImpressora = new Label
            {
                Text = "Impressora Padrão:",
                Location = new Point(30, 150),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelPrincipal.Controls.Add(lblImpressora);

            cmbImpressora = new ComboBox
            {
                Location = new Point(155, 148),
                Size = new Size(280, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            panelPrincipal.Controls.Add(cmbImpressora);

            // Papel
            Label lblPapel = new Label
            {
                Text = "Papel Padrão:",
                Location = new Point(30, 180),
                Size = new Size(120, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelPrincipal.Controls.Add(lblPapel);

            cmbPapel = new ComboBox
            {
                Location = new Point(155, 178),
                Size = new Size(280, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            panelPrincipal.Controls.Add(cmbPapel);

            // Preview
            pbPreview = new PictureBox
            {
                Location = new Point(30, 215),
                Size = new Size(120, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            panelPrincipal.Controls.Add(pbPreview);

            // Info de dimensões
            lblInfo = new Label
            {
                Location = new Point(160, 215),
                Size = new Size(275, 80),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.FromArgb(127, 140, 141),
                TextAlign = ContentAlignment.TopLeft
            };
            panelPrincipal.Controls.Add(lblInfo);

            // ==================== BOTÕES ====================
            btnConfirmar = new Button
            {
                Text = "✓ Confirmar",
                Location = new Point(200, 305),
                Size = new Size(120, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnConfirmar.FlatAppearance.BorderSize = 0;
            btnConfirmar.Click += BtnConfirmar_Click;
            panelPrincipal.Controls.Add(btnConfirmar);

            btnCancelar = new Button
            {
                Text = "✗ Fechar",
                Location = new Point(330, 305),
                Size = new Size(105, 30),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.Click += BtnCancelar_Click;
            panelPrincipal.Controls.Add(btnCancelar);
        }

        private void CarregarDados()
        {
            // Carrega templates disponíveis
            var templates = TemplateManager.ListarTemplates()
                .Where(t => t != "_ultimo_template")
                .ToList();

            cmbTemplate.Items.Clear();

            // Ordena para colocar o template padrão primeiro
            string templatePadrao = TemplatePadraoManager.ObterTemplatePadrao();

            var templatesOrdenados = templates.OrderBy(t =>
                t.Equals(templatePadrao, StringComparison.OrdinalIgnoreCase) ? 0 : 1
            ).ThenBy(t => t).ToList();

            foreach (var template in templatesOrdenados)
            {
                // Adiciona estrela ao template padrão
                string item = TemplatePadraoManager.EhTemplatePadrao(template)
                    ? $"⭐ {template}"
                    : template;
                cmbTemplate.Items.Add(item);
            }

            // Carrega impressoras disponíveis
            cmbImpressora.Items.Clear();
            foreach (string impressora in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbImpressora.Items.Add(impressora);
            }

            if (cmbImpressora.Items.Count > 0)
            {
                cmbImpressora.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// NOVO: Carrega automaticamente o template padrão ao abrir o form
        /// </summary>
        private void CarregarTemplatePadraoAutomaticamente()
        {
            string templatePadrao = TemplatePadraoManager.ObterTemplatePadrao();

            if (!string.IsNullOrEmpty(templatePadrao) && cmbTemplate.Items.Count > 0)
            {
                // Procura o template padrão no combobox
                for (int i = 0; i < cmbTemplate.Items.Count; i++)
                {
                    string item = cmbTemplate.Items[i].ToString();
                    // Remove a estrela se existir para comparar
                    string nomeTemplate = item.StartsWith("⭐ ") ? item.Substring(2) : item;

                    if (nomeTemplate.Equals(templatePadrao, StringComparison.OrdinalIgnoreCase))
                    {
                        cmbTemplate.SelectedIndex = i;
                        return;
                    }
                }
            }

            // Se não encontrou template padrão, seleciona o primeiro
            if (cmbTemplate.Items.Count > 0)
            {
                cmbTemplate.SelectedIndex = 0;
            }
        }

        private void CmbTemplate_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTemplate.SelectedItem == null)
                return;

            // Remove a estrela se existir
            string itemSelecionado = cmbTemplate.SelectedItem.ToString();
            string nomeTemplate = itemSelecionado.StartsWith("⭐ ")
                ? itemSelecionado.Substring(2)
                : itemSelecionado;

            TemplateSelecionado = nomeTemplate;

            // Carrega configuração vinculada ao template
            ConfiguracaoSelecionada = ConfiguracaoManager.CarregarConfiguracao(nomeTemplate);

            if (ConfiguracaoSelecionada != null)
            {
                // Atualiza interface com dados da configuração
                AtualizarInterfaceComConfiguracao();
                GerarPreview();
            }
            else
            {
                // Configuração não existe - usar padrão
                ConfiguracaoSelecionada = CriarConfiguracaoPadrao(nomeTemplate);
                lblInfo.Text = "⚠ Configuração não encontrada.\nClique em 'Configurar Papel' para criar.";
            }
        }

        private void AtualizarInterfaceComConfiguracao()
        {
            if (ConfiguracaoSelecionada == null) return;

            // Atualiza impressora
            if (!string.IsNullOrEmpty(ConfiguracaoSelecionada.ImpressoraPadrao) &&
                cmbImpressora.Items.Contains(ConfiguracaoSelecionada.ImpressoraPadrao))
            {
                cmbImpressora.SelectedItem = ConfiguracaoSelecionada.ImpressoraPadrao;
            }

            // Carrega papéis da impressora selecionada
            CarregarPapeisDaImpressora();

            // Seleciona papel configurado
            if (!string.IsNullOrEmpty(ConfiguracaoSelecionada.PapelPadrao) &&
                cmbPapel.Items.Cast<object>().Any(x => x.ToString() == ConfiguracaoSelecionada.PapelPadrao))
            {
                cmbPapel.SelectedItem = ConfiguracaoSelecionada.PapelPadrao;
            }

            // Atualiza label de informações
            lblInfo.Text = $"📐 Dimensões:\n" +
                          $"  Largura: {ConfiguracaoSelecionada.LarguraEtiqueta:F1} mm\n" +
                          $"  Altura: {ConfiguracaoSelecionada.AlturaEtiqueta:F1} mm\n" +
                          $"\n📊 Layout:\n" +
                          $"  Colunas: {ConfiguracaoSelecionada.NumColunas}\n" +
                          $"  Linhas: {ConfiguracaoSelecionada.NumLinhas}";
        }

        private void CarregarPapeisDaImpressora()
        {
            cmbPapel.Items.Clear();

            if (cmbImpressora.SelectedItem == null) return;

            string impressora = cmbImpressora.SelectedItem.ToString();

            try
            {
                var printerSettings = new System.Drawing.Printing.PrinterSettings
                {
                    PrinterName = impressora
                };

                foreach (System.Drawing.Printing.PaperSize papel in printerSettings.PaperSizes)
                {
                    cmbPapel.Items.Add(papel.PaperName);
                }

                if (cmbPapel.Items.Count > 0)
                    cmbPapel.SelectedIndex = 0;
            }
            catch
            {
                cmbPapel.Items.Add("(Erro ao carregar papéis)");
                cmbPapel.SelectedIndex = 0;
            }
        }

        private void GerarPreview()
        {
            if (ConfiguracaoSelecionada == null) return;

            try
            {
                Bitmap bmp = new Bitmap(pbPreview.Width, pbPreview.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.SmoothingMode = SmoothingMode.AntiAlias;

                    // Calcula escala para caber no preview
                    float escalaW = pbPreview.Width / (float)ConfiguracaoSelecionada.LarguraEtiqueta;
                    float escalaH = pbPreview.Height / (float)ConfiguracaoSelecionada.AlturaEtiqueta;
                    float escala = Math.Min(escalaW, escalaH) * 0.8f;

                    float largura = ConfiguracaoSelecionada.LarguraEtiqueta * escala;
                    float altura = ConfiguracaoSelecionada.AlturaEtiqueta * escala;

                    float x = (pbPreview.Width - largura) / 2;
                    float y = (pbPreview.Height - altura) / 2;

                    // Desenha etiqueta
                    g.FillRectangle(Brushes.White, x, y, largura, altura);
                    g.DrawRectangle(new Pen(Color.FromArgb(230, 126, 34), 2), x, y, largura, altura);

                    // Desenha "linhas" internas simulando conteúdo
                    using (Pen penLinha = new Pen(Color.LightGray, 1))
                    {
                        float espacamento = altura / 5;
                        for (int i = 1; i < 5; i++)
                        {
                            g.DrawLine(penLinha, x + 5, y + (espacamento * i), x + largura - 5, y + (espacamento * i));
                        }
                    }
                }

                pbPreview.Image = bmp;
            }
            catch
            {
                // Ignora erros de preview
            }
        }

        private ConfiguracaoEtiqueta CriarConfiguracaoPadrao(string nomeTemplate)
        {
            return new ConfiguracaoEtiqueta
            {
                NomeEtiqueta = nomeTemplate,
                ImpressoraPadrao = cmbImpressora.SelectedItem?.ToString() ?? "",
                PapelPadrao = "",
                LarguraEtiqueta = 100,
                AlturaEtiqueta = 30,
                NumColunas = 1,
                NumLinhas = 1,
                EspacamentoColunas = 0,
                EspacamentoLinhas = 0,
                MargemSuperior = 0,
                MargemInferior = 0,
                MargemEsquerda = 0,
                MargemDireita = 0
            };
        }

        private void BtnConfigurar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TemplateSelecionado))
            {
                MessageBox.Show("Selecione um template primeiro!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Abre FormConfigEtiqueta para editar
            using (var formConfig = new FormConfigEtiqueta(ConfiguracaoSelecionada))
            {
                if (formConfig.ShowDialog() == DialogResult.OK)
                {
                    ConfiguracaoSelecionada = formConfig.Configuracao;

                    // Salva configuração vinculada ao template
                    ConfiguracaoManager.SalvarConfiguracao(TemplateSelecionado, ConfiguracaoSelecionada);

                    // Atualiza interface
                    AtualizarInterfaceComConfiguracao();
                    GerarPreview();

                    MessageBox.Show("Configuração salva com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnConfirmar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TemplateSelecionado))
            {
                MessageBox.Show("Selecione um template!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (ConfiguracaoSelecionada == null)
            {
                MessageBox.Show("Configure o papel primeiro!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Atualiza configuração com seleções atuais
            if (cmbImpressora.SelectedItem != null)
                ConfiguracaoSelecionada.ImpressoraPadrao = cmbImpressora.SelectedItem.ToString();

            if (cmbPapel.SelectedItem != null)
                ConfiguracaoSelecionada.PapelPadrao = cmbPapel.SelectedItem.ToString();

            // Salva como último usado
            ConfiguracaoManager.SalvarUltimoTemplateUsado(TemplateSelecionado);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}