using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EtiquetaFORNew.Forms
{
    /// <summary>
    /// FormDesign modernizado com configuração de página integrada e funcionalidade completa de elementos
    /// </summary>
    public partial class FormDesignNovo : Form
    {
        #region Campos Privados

        private TemplateEtiqueta template;
        private ConfiguracaoEtiqueta configuracao;
        private string nomeTemplateAtual;

        // Controles do canvas
        private Panel panelCanvas;
        private PictureBox pbCanvas;

        // Controles do painel de configuração
        private Panel panelConfiguracao;
        private Button btnToggleConfig;  // Botão para mostrar/ocultar painel de configurações
        private NumericUpDown numLargura;
        private NumericUpDown numAltura;
        private ComboBox cmbImpressora;
        private ComboBox cmbPapel;
        private NumericUpDown numColunas;
        private NumericUpDown numLinhas;
        private NumericUpDown numEspacamentoColunas;
        private NumericUpDown numEspacamentoLinhas;
        private NumericUpDown numMargemSuperior;
        private NumericUpDown numMargemInferior;
        private NumericUpDown numMargemEsquerda;
        private NumericUpDown numMargemDireita;
        private CheckBox chkPadraoDesativar;
        private Panel panelPropriedades;
        private Button btnAlinharEsquerda;
        private Button btnAlinharCentro;
        private Button btnAlinharDireita;
        private NumericUpDown numTamanhoFonte;
        private CheckBox chkNegrito;
        private CheckBox chkItalico;
        private Button btnCor;
        private Label lblPropriedadesElemento;
        private ComboBox cmbFonte;  // ✅ NOVO: ComboBox de seleção de fonte


        // Toolbox de elementos
        private Panel panelToolbox;

        // Controles de elementos e seleção
        private ElementoEtiqueta elementoSelecionado;
        private bool arrastando = false;
        private bool redimensionando = false;
        private Point pontoInicialMouse;
        private Rectangle boundsIniciais;
        private Point deltaArrasto;  // Delta do movimento (em pixels) para aplicar no final
        private int handleSelecionado = -1;

        private List<ElementoEtiqueta> elementosSelecionados = new List<ElementoEtiqueta>();
        private bool selecionandoComRetangulo = false;
        private Point pontoInicialSelecao;
        private Rectangle retanguloSelecao;

        // Constantes
        private const float MM_PARA_PIXEL = 3.78f;
        private float zoom = 1.0f;

        #endregion

        #region Construtor e Inicialização

        public FormDesignNovo(TemplateEtiqueta templateInicial, string nomeTemplate = null)
        {
            InitializeComponent();

            this.template = templateInicial ?? new TemplateEtiqueta();
            this.nomeTemplateAtual = nomeTemplate;

            // Carrega ou cria configuração
            if (!string.IsNullOrEmpty(nomeTemplate))
            {
                configuracao = ConfiguracaoManager.CarregarConfiguracao(nomeTemplate);
            }

            if (configuracao == null)
            {
                configuracao = new ConfiguracaoEtiqueta
                {
                    NomeEtiqueta = nomeTemplate ?? "Novo Template",
                    LarguraEtiqueta = template.Largura > 0 ? template.Largura : 100,
                    AlturaEtiqueta = template.Altura > 0 ? template.Altura : 30,
                    ImpressoraPadrao = "BTP-L42(D)",
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

            // Sincroniza template com config
            template.Largura = configuracao.LarguraEtiqueta;
            template.Altura = configuracao.AlturaEtiqueta;

            ConfigurarFormulario();
        }

        private void FormDesignNovo_Load(object sender, EventArgs e)
        {
            VersaoHelper.DefinirTituloComVersao(this, "Designer de Etiquetas");
            CriarInterface();
            CarregarDadosNaInterface();

            // Posicionar o botão de toggle após a interface estar criada
            if (btnToggleConfig != null && panelConfiguracao != null)
            {
                btnToggleConfig.Location = new Point(
                    this.ClientSize.Width - panelConfiguracao.Width - btnToggleConfig.Width,
                    (this.ClientSize.Height - btnToggleConfig.Height) / 2
                );
            }
        }

        private void ConfigurarFormulario()
        {
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 700);
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.KeyPreview = true;
            this.KeyDown += FormDesignNovo_KeyDown;
        }

        #endregion

        #region Criação de Interface

        private void CriarInterface()
        {
            // ==================== BARRA SUPERIOR ====================
            Panel panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(94, 97, 99),
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(panelTop);

            // Logo e título
            Label lblEmoji = new Label
            {
                Text = "🎨",
                Location = new Point(20, 15),
                Size = new Size(30, 30), // Pequeno, apenas para o emoji
                Font = new Font("Segoe UI", 14, FontStyle.Bold), // Sem sublinhado
                ForeColor = Color.FromArgb(231, 129, 39)
            };
            panelTop.Controls.Add(lblEmoji);

            Label lblTitulo = new Label
            {
                Text = "DESIGNER DE ETIQUETAS",
                Location = new Point(50, 15),
                Size = new Size(270, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold | FontStyle.Underline),
                ForeColor = Color.FromArgb(231, 129, 39)
            };
            panelTop.Controls.Add(lblTitulo);

            // ==================== BOTÕES DE AÇÃO ====================
            // Painel para os botões (ancorado à direita)
            Panel panelBotoes = new Panel
            {
                Dock = DockStyle.Right,
                Width = 450,  // Aumentado para caber mais um botão
                Height = 60,
                BackColor = Color.Transparent
            };
            panelTop.Controls.Add(panelBotoes);

            // Botão Fechar (mais à direita)
            Button btnFechar = new Button
            {
                Text = "✕ Fechar",
                Location = new Point(340, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnFechar.FlatAppearance.BorderSize = 1;
            btnFechar.FlatAppearance.BorderColor = Color.Black;
            btnFechar.Click += BtnFechar_Click;
            panelBotoes.Controls.Add(btnFechar);

            // Botão Preview
            Button btnPreview = new Button
            {
                Text = "👁 Preview",
                Location = new Point(230, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(155, 89, 182),  // Roxo
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPreview.FlatAppearance.BorderSize = 1;
            btnPreview.FlatAppearance.BorderColor = Color.Black;
            btnPreview.Click += BtnPreview_Click;
            panelBotoes.Controls.Add(btnPreview);

            // Botão Novo (meio)
            Button btnNovo = new Button
            {
                Text = "📄 Novo",
                Location = new Point(120, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnNovo.FlatAppearance.BorderSize = 1;
            btnNovo.FlatAppearance.BorderColor = Color.Black;
            btnNovo.Click += BtnNovo_Click;
            panelBotoes.Controls.Add(btnNovo);

            // Botão Salvar (mais à esquerda do painel)
            Button btnSalvar = new Button
            {
                Text = "💾 Salvar",
                Location = new Point(10, 15),
                Size = new Size(100, 30),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSalvar.FlatAppearance.BorderSize = 1;
            btnSalvar.FlatAppearance.BorderColor = Color.Black;
            btnSalvar.Click += BtnSalvar_Click;
            panelBotoes.Controls.Add(btnSalvar);

            // ==================== PAINEL LATERAL DIREITO - CONFIGURAÇÃO ====================
            panelConfiguracao = new Panel
            {
                Dock = DockStyle.Right,
                Width = 350,
                BackColor = Color.White,
                Padding = new Padding(10),
                AutoScroll = true
            };
            this.Controls.Add(panelConfiguracao);

            CriarPainelConfiguracao();

            // Botão para ocultar/mostrar painel de configurações (flutuante)
            btnToggleConfig = new Button
            {
                Text = "▶",  // Seta para DIREITA quando visível (clicar para ocultar)
                Size = new Size(30, 80),
                BackColor = Color.FromArgb(46, 204, 113),  // VERDE quando visível
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnToggleConfig.FlatAppearance.BorderSize = 0;

            this.Controls.Add(btnToggleConfig);
            btnToggleConfig.BringToFront();

            // Reposicionar ao redimensionar a janela
            this.Resize += (s, e) =>
            {
                if (btnToggleConfig == null) return;

                if (panelConfiguracao.Width > 0)
                {
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - panelConfiguracao.Width - btnToggleConfig.Width,
                        (this.ClientSize.Height - btnToggleConfig.Height) / 2
                    );
                }
                else
                {
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - btnToggleConfig.Width - 5,
                        (this.ClientSize.Height - btnToggleConfig.Height) / 2
                    );
                }
            };

            btnToggleConfig.Click += (s, e) =>
            {
                if (panelConfiguracao.Width > 0)
                {
                    // Ocultar - botão fica AZUL com seta para ESQUERDA (mostrar)
                    panelConfiguracao.Width = 0;
                    btnToggleConfig.Text = "◀";  // Seta para ESQUERDA
                    btnToggleConfig.BackColor = Color.FromArgb(52, 152, 219);  // AZUL
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - btnToggleConfig.Width - 5,
                        btnToggleConfig.Location.Y
                    );
                }
                else
                {
                    // Mostrar - botão fica VERDE com seta para DIREITA (ocultar)
                    panelConfiguracao.Width = 350;
                    btnToggleConfig.Text = "▶";  // Seta para DIREITA
                    btnToggleConfig.BackColor = Color.FromArgb(46, 204, 113);  // VERDE
                    btnToggleConfig.Location = new Point(
                        this.ClientSize.Width - panelConfiguracao.Width - btnToggleConfig.Width,
                        btnToggleConfig.Location.Y
                    );
                }
            };

            // ==================== PAINEL LATERAL ESQUERDO - TOOLBOX ====================
            panelToolbox = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(236, 240, 241),
                Padding = new Padding(10),
                AutoScroll = true,                      // ✅ ADICIONAR
                AutoScrollMinSize = new Size(0, 800)
            };
            this.Controls.Add(panelToolbox);

            CriarToolbox();

            // ==================== CANVAS CENTRAL ====================
            panelCanvas = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(189, 195, 199),
                AutoScroll = true
            };
            panelCanvas.Resize += (s, e) => AtualizarTamanhoCanvas();
            this.Controls.Add(panelCanvas);

            pbCanvas = new PictureBox
            {
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(50, 50),
                Size = new Size(400, 300)  // Tamanho inicial, será atualizado
            };
            pbCanvas.Paint += PbCanvas_Paint;
            pbCanvas.MouseDown += PbCanvas_MouseDown;
            pbCanvas.MouseMove += PbCanvas_MouseMove;
            pbCanvas.MouseUp += PbCanvas_MouseUp;
            pbCanvas.MouseWheel += PbCanvas_MouseWheel;

            panelCanvas.Controls.Add(pbCanvas);

            AtualizarTamanhoCanvas();
        }

        #endregion

        #region Painel de Configuração

        private void CriarPainelConfiguracao()
        {
            int yPos = 10;

            // Título
            Label lblTituloConfig = new Label
            {
                Text = "⚙ CONFIGURAÇÕES DA PÁGINA",
                Location = new Point(10, yPos),
                Size = new Size(320, 30),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelConfiguracao.Controls.Add(lblTituloConfig);
            yPos += 40;

            // Linha separadora
            Panel linha1 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha1);
            yPos += 15;

            // DIMENSÕES DA ETIQUETA
            Label lblDimensoes = CriarLabelSecao("📐 Dimensões da Etiqueta", yPos);
            panelConfiguracao.Controls.Add(lblDimensoes);
            yPos += 25;

            yPos = CriarCampoNumerico("Largura (mm):", out numLargura, yPos, 1, 500, (decimal)configuracao.LarguraEtiqueta);
            numLargura.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Altura (mm):", out numAltura, yPos, 1, 500, (decimal)configuracao.AlturaEtiqueta);
            numAltura.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos += 10;

            Panel linha2 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha2);
            yPos += 15;

            // IMPRESSORA E PAPEL
            Label lblImpressao = CriarLabelSecao("🖨️ Impressão", yPos);
            panelConfiguracao.Controls.Add(lblImpressao);
            yPos += 25;

            Label lblImpressora = new Label
            {
                Text = "Impressora:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelConfiguracao.Controls.Add(lblImpressora);

            cmbImpressora = new ComboBox
            {
                Location = new Point(120, yPos - 2),
                Size = new Size(210, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbImpressora.SelectedIndexChanged += CmbImpressora_SelectedIndexChanged;
            panelConfiguracao.Controls.Add(cmbImpressora);
            yPos += 30;

            Label lblPapel = new Label
            {
                Text = "Papel:",
                Location = new Point(15, yPos),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelConfiguracao.Controls.Add(lblPapel);

            cmbPapel = new ComboBox
            {
                Location = new Point(120, yPos - 2),
                Size = new Size(210, 23),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbPapel.SelectedIndexChanged += CmbPapel_SelectedIndexChanged;
            panelConfiguracao.Controls.Add(cmbPapel);
            yPos += 35;

            Panel linha3 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha3);
            yPos += 15;

            // LAYOUT
            Label lblLayout = CriarLabelSecao("📊 Layout da Página", yPos);
            panelConfiguracao.Controls.Add(lblLayout);
            yPos += 25;

            Label lblColunas = new Label
            {
                Text = "Colunas:",
                Location = new Point(15, yPos),
                Size = new Size(60, 20)
            };
            panelConfiguracao.Controls.Add(lblColunas);

            numColunas = new NumericUpDown
            {
                Location = new Point(80, yPos - 2),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 10,
                Value = configuracao.NumColunas
            };
            numColunas.ValueChanged += (s, e) => AtualizarConfiguracao();
            panelConfiguracao.Controls.Add(numColunas);

            Label lblLinhas = new Label
            {
                Text = "Linhas:",
                Location = new Point(175, yPos),
                Size = new Size(50, 20)
            };
            panelConfiguracao.Controls.Add(lblLinhas);

            numLinhas = new NumericUpDown
            {
                Location = new Point(230, yPos - 2),
                Size = new Size(60, 23),
                Minimum = 1,
                Maximum = 20,
                Value = configuracao.NumLinhas
            };
            numLinhas.ValueChanged += (s, e) => AtualizarConfiguracao();
            panelConfiguracao.Controls.Add(numLinhas);
            yPos += 30;

            Panel linha4 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha4);
            yPos += 15;

            // ESPAÇAMENTOS
            Label lblEspacamento = CriarLabelSecao("↔️ Espaçamentos", yPos);
            panelConfiguracao.Controls.Add(lblEspacamento);
            yPos += 25;

            yPos = CriarCampoNumerico("Entre Colunas (mm):", out numEspacamentoColunas, yPos, 0, 50,
                (decimal)configuracao.EspacamentoColunas, 0.1m);
            numEspacamentoColunas.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Entre Linhas (mm):", out numEspacamentoLinhas, yPos, 0, 50,
                (decimal)configuracao.EspacamentoLinhas, 0.1m);
            numEspacamentoLinhas.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos += 10;

            Panel linha5 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha5);
            yPos += 15;

            // MARGENS
            Label lblMargens = CriarLabelSecao("📏 Margens da Página", yPos);
            panelConfiguracao.Controls.Add(lblMargens);
            yPos += 25;

            //chkPadraoDesativar = new CheckBox
            //{
            //    Text = "Ir no Painel de Controle, clicar no item 'Propriedades do servidor de impressão'",
            //    Location = new Point(15, yPos),
            //    Size = new Size(300, 40),
            //    Font = new Font("Segoe UI", 8),
            //    ForeColor = Color.FromArgb(127, 140, 141),
            //    Checked = configuracao.MargemSuperior == 0
            //};
            //chkPadraoDesativar.CheckedChanged += ChkPadraoDesativar_CheckedChanged;
            //panelConfiguracao.Controls.Add(chkPadraoDesativar);
            //yPos += 45;

            yPos = CriarCampoNumerico("Superior (mm):", out numMargemSuperior, yPos, 0, 50,
                (decimal)configuracao.MargemSuperior, 0.1m);
            numMargemSuperior.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Inferior (mm):", out numMargemInferior, yPos, 0, 50,
                (decimal)configuracao.MargemInferior, 0.1m);
            numMargemInferior.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Esquerda (mm):", out numMargemEsquerda, yPos, 0, 50,
                (decimal)configuracao.MargemEsquerda, 0.1m);
            numMargemEsquerda.ValueChanged += (s, e) => AtualizarConfiguracao();

            yPos = CriarCampoNumerico("Direita (mm):", out numMargemDireita, yPos, 0, 50,
                (decimal)configuracao.MargemDireita, 0.1m);
            numMargemDireita.ValueChanged += (s, e) => AtualizarConfiguracao();

            Panel linha6 = new Panel
            {
                Location = new Point(10, yPos),
                Size = new Size(320, 2),
                BackColor = Color.FromArgb(230, 126, 34)
            };
            panelConfiguracao.Controls.Add(linha6);
            yPos += 15;
            //AtualizarEstadoMargens();
        }

        private Label CriarLabelSecao(string texto, int yPos)
        {
            return new Label
            {
                Text = texto,
                Location = new Point(10, yPos),
                Size = new Size(320, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
        }

        private int CriarCampoNumerico(string label, out NumericUpDown control, int yPos,
            decimal min, decimal max, decimal valor, decimal increment = 1)
        {
            Label lbl = new Label
            {
                Text = label,
                Location = new Point(15, yPos),
                Size = new Size(140, 20),
                Font = new Font("Segoe UI", 9)
            };
            panelConfiguracao.Controls.Add(lbl);

            control = new NumericUpDown
            {
                Location = new Point(160, yPos - 2),
                Size = new Size(100, 23),
                Minimum = min,
                Maximum = max,
                Value = valor,
                DecimalPlaces = increment < 1 ? 2 : 0,
                Increment = increment
            };
            panelConfiguracao.Controls.Add(control);

            Label lblUnidade = new Label
            {
                Text = "mm",
                Location = new Point(270, yPos),
                Size = new Size(30, 20),
                Font = new Font("Segoe UI", 8),
                ForeColor = Color.Gray
            };
            panelConfiguracao.Controls.Add(lblUnidade);

            return yPos + 30;
        }

        #endregion

        #region Toolbox

        private void CriarToolbox()
        {
            Label lblTitulo = new Label
            {
                Text = "🧰 ELEMENTOS",
                Location = new Point(10, 10),
                Size = new Size(180, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelToolbox.Controls.Add(lblTitulo);

            int yPos = 45;

            // Label Campos
            Label lblCampos = new Label
            {
                Text = "Campos Dinâmicos:",
                Location = new Point(10, yPos),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelToolbox.Controls.Add(lblCampos);
            yPos += 25;

            // ComboBox de Campos
            ComboBox cmbCampos = new ComboBox
            {
                Location = new Point(10, yPos),
                Size = new Size(180, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8)
            };
            cmbCampos.Items.AddRange(new object[] {
                "Mercadoria",
                "CodigoMercadoria",
                "CodFabricante",
                "CodBarras",
                "PrecoVenda",
                "VendaA",
                "VendaB",
                "VendaC",
                "VendaD",
                "VendaE",
                "Fornecedor",
                "Fabricante",
                "Grupo",
                "Prateleira",
                "Garantia",
                "Tam",
                "Cores",
                "CodBarras_Grade"
            });
            cmbCampos.SelectedIndexChanged += (s, e) => {
                if (cmbCampos.SelectedItem != null)
                {
                    AdicionarCampo(cmbCampos.SelectedItem.ToString());
                    cmbCampos.SelectedIndex = -1;
                }
            };
            panelToolbox.Controls.Add(cmbCampos);
            yPos += 35;

            // Label Códigos de Barras
            Label lblCodigoBarras = new Label
            {
                Text = "Códigos de Barras:",
                Location = new Point(10, yPos),
                Size = new Size(180, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelToolbox.Controls.Add(lblCodigoBarras);
            yPos += 25;

            // ComboBox de Códigos de Barras
            ComboBox cmbCodigoBarras = new ComboBox
            {
                Location = new Point(10, yPos),
                Size = new Size(180, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 8)
            };
            cmbCodigoBarras.Items.AddRange(new object[] {
                "CodigoMercadoria",
                "CodFabricante",
                "CodBarras",
                "CodBarras_Grade"
            });
            cmbCodigoBarras.SelectedIndexChanged += (s, e) => {
                if (cmbCodigoBarras.SelectedItem != null)
                {
                    AdicionarCodigoBarras(cmbCodigoBarras.SelectedItem.ToString());
                    cmbCodigoBarras.SelectedIndex = -1;
                }
            };
            panelToolbox.Controls.Add(cmbCodigoBarras);
            yPos += 35;

            // Botão Texto
            Button btnTexto = CriarBotaoElemento("📝 Texto", yPos, () => AdicionarElemento(TipoElemento.Texto));
            yPos += 40;

            // Botão Imagem
            Button btnImagem = CriarBotaoElemento("🖼️ Imagem", yPos, () => AdicionarImagem());
            yPos += 40;

            // Botão Remover
            Button btnRemover = CriarBotaoElemento("🗑️ Remover", yPos, () => RemoverElementoSelecionado());
            btnRemover.BackColor = Color.FromArgb(231, 76, 60);
            CriarPainelPropriedades();
        }
        private void CriarPainelPropriedades()
        {
            panelPropriedades = new Panel
            {
                Location = new Point(10, 400),
                Size = new Size(180, 350),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false  // Invisível até selecionar elemento
            };
            panelToolbox.Controls.Add(panelPropriedades);

            int yPos = 10;

            // Título
            lblPropriedadesElemento = new Label
            {
                Text = "⚙ PROPRIEDADES",
                Location = new Point(10, yPos),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };
            panelPropriedades.Controls.Add(lblPropriedadesElemento);
            yPos += 35;

            Label lblConteudo = new Label
            {
                Name = "lblConteudoTexto",
                Text = "Conteúdo:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray,
                Visible = false  // Inicialmente invisível
            };
            panelPropriedades.Controls.Add(lblConteudo);
            yPos += 25;

            TextBox txtConteudo = new TextBox
            {
                Name = "txtConteudoElemento",
                Location = new Point(10, yPos),
                Size = new Size(160, 25),
                Font = new Font("Segoe UI", 9),
                Visible = false  // Inicialmente invisível
            };
            txtConteudo.TextChanged += (s, e) =>
            {
                if (elementoSelecionado != null && elementoSelecionado.Tipo == TipoElemento.Texto)
                {
                    elementoSelecionado.Conteudo = txtConteudo.Text;
                    pbCanvas.Invalidate();
                }
            };
            panelPropriedades.Controls.Add(txtConteudo);
            yPos += 35;

            // Label Alinhamento
            Label lblAlinhamento = new Label
            {
                Text = "Alinhamento:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblAlinhamento);
            yPos += 25;

            // Botões de alinhamento
            btnAlinharEsquerda = new Button
            {
                Text = "⬅️",
                Location = new Point(10, yPos),
                Size = new Size(50, 35),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(236, 240, 241),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAlinharEsquerda.FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199);
            btnAlinharEsquerda.Click += (s, e) => AlterarAlinhamento(StringAlignment.Near);
            panelPropriedades.Controls.Add(btnAlinharEsquerda);

            btnAlinharCentro = new Button
            {
                Text = "↔️",
                Location = new Point(65, yPos),
                Size = new Size(50, 35),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(236, 240, 241),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAlinharCentro.FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199);
            btnAlinharCentro.Click += (s, e) => AlterarAlinhamento(StringAlignment.Center);
            panelPropriedades.Controls.Add(btnAlinharCentro);

            btnAlinharDireita = new Button
            {
                Text = "➡️",
                Location = new Point(120, yPos),
                Size = new Size(50, 35),
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(236, 240, 241),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAlinharDireita.FlatAppearance.BorderColor = Color.FromArgb(189, 195, 199);
            btnAlinharDireita.Click += (s, e) => AlterarAlinhamento(StringAlignment.Far);
            panelPropriedades.Controls.Add(btnAlinharDireita);
            yPos += 45;

            // ✅ NOVO: Seleção de Fonte
            Label lblFamilia = new Label
            {
                Text = "Família da Fonte:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblFamilia);
            yPos += 25;

            cmbFonte = new ComboBox
            {
                Location = new Point(10, yPos),
                Size = new Size(160, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };

            // Adiciona as fontes disponíveis
            foreach (var fontFamily in FontFamily.Families)
            {
                cmbFonte.Items.Add(fontFamily.Name);
            }
            cmbFonte.SelectedIndexChanged += CmbFonte_SelectedIndexChanged;
            panelPropriedades.Controls.Add(cmbFonte);
            yPos += 35;

            // Label Fonte
            Label lblFonte = new Label
            {
                Text = "Tamanho da Fonte:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblFonte);
            yPos += 25;

            numTamanhoFonte = new NumericUpDown
            {
                Location = new Point(10, yPos),
                Size = new Size(70, 23),
                Minimum = 6,
                Maximum = 72,
                Value = 10
            };
            numTamanhoFonte.ValueChanged += (s, e) => AlterarTamanhoFonte();
            panelPropriedades.Controls.Add(numTamanhoFonte);
            yPos += 35;

            // Estilo
            Label lblEstilo = new Label
            {
                Text = "Estilo:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblEstilo);
            yPos += 25;

            chkNegrito = new CheckBox
            {
                Text = "Negrito",
                Location = new Point(10, yPos),
                Size = new Size(80, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };
            chkNegrito.CheckedChanged += (s, e) => AlterarEstiloFonte();
            panelPropriedades.Controls.Add(chkNegrito);

            chkItalico = new CheckBox
            {
                Text = "Itálico",
                Location = new Point(95, yPos),
                Size = new Size(75, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Italic)
            };
            chkItalico.CheckedChanged += (s, e) => AlterarEstiloFonte();
            panelPropriedades.Controls.Add(chkItalico);
            yPos += 35;

            // Cor
            Label lblCor = new Label
            {
                Text = "Cor do Texto:",
                Location = new Point(10, yPos),
                Size = new Size(160, 20),
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = Color.Gray
            };
            panelPropriedades.Controls.Add(lblCor);
            yPos += 25;

            btnCor = new Button
            {
                Text = "Escolher Cor",
                Location = new Point(10, yPos),
                Size = new Size(160, 30),
                BackColor = Color.Black,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCor.Click += BtnCor_Click;
            panelPropriedades.Controls.Add(btnCor);
        }

        private Button CriarBotaoElemento(string texto, int yPos, Action onClick)
        {
            Button btn = new Button
            {
                Text = texto,
                Location = new Point(10, yPos),
                Size = new Size(180, 35),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(255, 143, 0),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            panelToolbox.Controls.Add(btn);
            return btn;
        }

        #endregion

        #region Adicionar Elementos

        private void AdicionarElemento(TipoElemento tipo)
        {
            var elemento = new ElementoEtiqueta
            {
                Tipo = tipo,
                NomeFonte = "Arial",
                Fonte = new Font("Arial", 10),
                Cor = Color.Black
            };

            if (tipo == TipoElemento.Texto)
            {
                elemento.Conteudo = "Texto";

                using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
                {
                    SizeF tamanhoTexto = g.MeasureString(elemento.Conteudo, elemento.Fonte);
                    int largura = Math.Min((int)(tamanhoTexto.Width / MM_PARA_PIXEL) + 2, (int)template.Largura - 2);
                    int altura = Math.Min((int)(tamanhoTexto.Height / MM_PARA_PIXEL) + 2, (int)template.Altura - 2);
                    elemento.Bounds = new Rectangle(1, 1, Math.Max(3, largura), Math.Max(2, altura));
                }
            }

            template.Elementos.Add(elemento);
            elementoSelecionado = elemento;
            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
        }

        private void AdicionarCampo(string campo)
        {
            var elemento = new ElementoEtiqueta
            {
                Tipo = TipoElemento.Campo,
                Conteudo = campo,
                Fonte = new Font("Arial", 8),
                Cor = Color.Black
            };

            string textoExemplo = "[" + campo + "]";
            using (Graphics g = Graphics.FromImage(new Bitmap(1, 1)))
            {
                SizeF tamanhoTexto = g.MeasureString(textoExemplo, elemento.Fonte);
                int largura = Math.Min((int)(tamanhoTexto.Width / MM_PARA_PIXEL) + 2, (int)template.Largura - 2);
                int altura = Math.Min((int)(tamanhoTexto.Height / MM_PARA_PIXEL) + 2, (int)template.Altura - 2);
                elemento.Bounds = new Rectangle(1, 1, Math.Max(3, largura), Math.Max(2, altura));
            }

            template.Elementos.Add(elemento);
            elementoSelecionado = elemento;
            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
        }

        private void AdicionarCodigoBarras(string campoCodigo)
        {
            var elemento = new ElementoEtiqueta
            {
                Tipo = TipoElemento.CodigoBarras,
                Conteudo = campoCodigo,
                Fonte = new Font("Arial", 8),
                Cor = Color.Black
            };

            int largura = Math.Max(10, Math.Min((int)(template.Largura * 0.8f), (int)template.Largura - 2));
            int altura = Math.Max(5, Math.Min((int)(template.Altura * 0.4f), (int)template.Altura - 2));

            elemento.Bounds = new Rectangle(1, 1, largura, altura);
            template.Elementos.Add(elemento);
            elementoSelecionado = elemento;
            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
        }

        private void AdicionarImagem()
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Imagens|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var elemento = new ElementoEtiqueta
                    {
                        Tipo = TipoElemento.Imagem,
                        Imagem = Image.FromFile(ofd.FileName),
                        Bounds = new Rectangle(1, 1, 20, 20)
                    };

                    template.Elementos.Add(elemento);
                    elementoSelecionado = elemento;
                    AtualizarPainelPropriedades();
                    pbCanvas.Invalidate();
                }
            }
        }

        private void RemoverElementoSelecionado()
        {
            if (elementoSelecionado != null)
            {
                var resultado = MessageBox.Show(
                    "Deseja remover o elemento selecionado?",
                    "Confirmar Remoção",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    template.Elementos.Remove(elementoSelecionado);
                    elementoSelecionado = null;
                    AtualizarPainelPropriedades();
                    pbCanvas.Invalidate();
                }
            }
            else
            {
                MessageBox.Show("Nenhum elemento selecionado!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void AtualizarPainelPropriedades()
        {
            if (elementoSelecionado == null)
            {
                panelPropriedades.Visible = false;
                return;
            }

            panelPropriedades.Visible = true;

            // Atualiza valores baseado no elemento selecionado
            if (elementoSelecionado.Fonte != null)
            {
                numTamanhoFonte.Value = (decimal)elementoSelecionado.Fonte.Size;
                chkNegrito.Checked = elementoSelecionado.Fonte.Bold;
                chkItalico.Checked = elementoSelecionado.Fonte.Italic;

                // ✅ NOVO: Atualiza ComboBox de fonte
                string nomeFonte = elementoSelecionado.NomeFonte ?? elementoSelecionado.Fonte.FontFamily.Name;
                if (cmbFonte.Items.Contains(nomeFonte))
                {
                    cmbFonte.SelectedItem = nomeFonte;
                }
                else
                {
                    cmbFonte.SelectedIndex = -1;
                }
            }

            btnCor.BackColor = elementoSelecionado.Cor;
            btnCor.ForeColor = elementoSelecionado.Cor.GetBrightness() > 0.5 ? Color.Black : Color.White;

            // Atualiza visual dos botões de alinhamento
            AtualizarBotoesAlinhamento();
            panelToolbox.ScrollControlIntoView(panelPropriedades);
            var txtConteudo = panelPropriedades.Controls.Find("txtConteudoElemento", false).FirstOrDefault() as TextBox;
            var lblConteudo = panelPropriedades.Controls.Find("lblConteudoTexto", false).FirstOrDefault() as Label;

            if (elementoSelecionado.Tipo == TipoElemento.Texto)
            {
                // Mostra e preenche o campo de texto
                if (txtConteudo != null)
                {
                    txtConteudo.Visible = true;
                    txtConteudo.Text = elementoSelecionado.Conteudo ?? "Texto";
                }
                if (lblConteudo != null)
                {
                    lblConteudo.Visible = true;
                }
            }
            else
            {
                // Esconde o campo de texto para outros tipos
                if (txtConteudo != null)
                {
                    txtConteudo.Visible = false;
                }
                if (lblConteudo != null)
                {
                    lblConteudo.Visible = false;
                }
            }
        }

        private void AtualizarBotoesAlinhamento()
        {
            if (elementoSelecionado == null) return;

            // Reseta cores
            btnAlinharEsquerda.BackColor = Color.FromArgb(236, 240, 241);
            btnAlinharCentro.BackColor = Color.FromArgb(236, 240, 241);
            btnAlinharDireita.BackColor = Color.FromArgb(236, 240, 241);

            // Destaca o alinhamento atual
            StringAlignment alinhamento = elementoSelecionado.Alinhamento;

            if (alinhamento == StringAlignment.Near)
                btnAlinharEsquerda.BackColor = Color.FromArgb(52, 152, 219);
            else if (alinhamento == StringAlignment.Center)
                btnAlinharCentro.BackColor = Color.FromArgb(52, 152, 219);
            else if (alinhamento == StringAlignment.Far)
                btnAlinharDireita.BackColor = Color.FromArgb(52, 152, 219);
        }

        private void AlterarAlinhamento(StringAlignment novoAlinhamento)
        {
            if (elementoSelecionado == null) return;

            elementoSelecionado.Alinhamento = novoAlinhamento;
            AtualizarBotoesAlinhamento();
            pbCanvas.Invalidate();
        }

        private void CmbFonte_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (elementoSelecionado == null || elementoSelecionado.Fonte == null) return;
            if (cmbFonte.SelectedItem == null) return;

            string nomeFonte = cmbFonte.SelectedItem.ToString();
            float tamanho = elementoSelecionado.Fonte.Size;
            FontStyle estilo = elementoSelecionado.Fonte.Style;

            try
            {
                elementoSelecionado.NomeFonte = nomeFonte;
                elementoSelecionado.Fonte = new Font(nomeFonte, tamanho, estilo);
                pbCanvas.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao aplicar fonte: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AlterarTamanhoFonte()
        {
            if (elementoSelecionado == null || elementoSelecionado.Fonte == null) return;

            float novoTamanho = (float)numTamanhoFonte.Value;
            FontStyle estilo = elementoSelecionado.Fonte.Style;

            string nomeFonte = elementoSelecionado.NomeFonte ?? elementoSelecionado.Fonte.FontFamily.Name;
            elementoSelecionado.Fonte = new Font(nomeFonte, novoTamanho, estilo);
            pbCanvas.Invalidate();
        }

        private void AlterarEstiloFonte()
        {
            if (elementoSelecionado == null || elementoSelecionado.Fonte == null) return;

            FontStyle estilo = FontStyle.Regular;

            if (chkNegrito.Checked)
                estilo |= FontStyle.Bold;

            if (chkItalico.Checked)
                estilo |= FontStyle.Italic;

            string nomeFonte = elementoSelecionado.NomeFonte ?? elementoSelecionado.Fonte.FontFamily.Name;
            elementoSelecionado.Fonte = new Font(
                nomeFonte,
                elementoSelecionado.Fonte.Size,
                estilo);

            // Salvar as flags no elemento
            elementoSelecionado.Negrito = chkNegrito.Checked;
            elementoSelecionado.Italico = chkItalico.Checked;

            pbCanvas.Invalidate();
        }

        private void BtnCor_Click(object sender, EventArgs e)
        {
            if (elementoSelecionado == null) return;

            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = elementoSelecionado.Cor;

                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    elementoSelecionado.Cor = colorDialog.Color;
                    btnCor.BackColor = colorDialog.Color;
                    btnCor.ForeColor = colorDialog.Color.GetBrightness() > 0.5 ? Color.Black : Color.White;
                    pbCanvas.Invalidate();
                }
            }
        }

        #endregion

        #region Desenhar Canvas

        private void PbCanvas_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.White);

            float escala = zoom * MM_PARA_PIXEL;

            // Desenha fundo da etiqueta
            RectangleF rectEtiqueta = new RectangleF(
                25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom
            );

            g.FillRectangle(Brushes.White, rectEtiqueta);
            g.DrawRectangle(new Pen(Color.FromArgb(230, 126, 34), 2), Rectangle.Round(rectEtiqueta));

            // Desenha grid de colunas/linhas se > 1
            if (configuracao.NumColunas > 1 || configuracao.NumLinhas > 1)
            {
                DesenharGrid(g, rectEtiqueta);
            }

            // Desenha elementos do template
            DesenharElementos(g, rectEtiqueta);

            // NOVO: Desenhar retângulo de seleção múltipla
            if (selecionandoComRetangulo)
            {
                using (Pen penSelecao = new Pen(Color.DodgerBlue, 1))
                using (SolidBrush brushSelecao = new SolidBrush(Color.FromArgb(30, Color.DodgerBlue)))
                {
                    penSelecao.DashStyle = DashStyle.Dot;
                    g.FillRectangle(brushSelecao, retanguloSelecao);
                    g.DrawRectangle(penSelecao, retanguloSelecao);
                }
            }
        }

        private void DesenharGrid(Graphics g, RectangleF rect)
        {
            using (Pen penGrid = new Pen(Color.FromArgb(100, 189, 195, 199), 1))
            {
                penGrid.DashStyle = DashStyle.Dash;

                // Grid vertical (colunas)
                if (configuracao.NumColunas > 1)
                {
                    float larguraColuna = configuracao.LarguraEtiqueta / configuracao.NumColunas;
                    for (int i = 1; i < configuracao.NumColunas; i++)
                    {
                        float x = rect.X + (larguraColuna * i * MM_PARA_PIXEL);
                        g.DrawLine(penGrid, x, rect.Y, x, rect.Bottom);
                    }
                }

                // Grid horizontal (linhas)
                if (configuracao.NumLinhas > 1)
                {
                    float alturaLinha = configuracao.AlturaEtiqueta / configuracao.NumLinhas;
                    for (int i = 1; i < configuracao.NumLinhas; i++)
                    {
                        float y = rect.Y + (alturaLinha * i * MM_PARA_PIXEL);
                        g.DrawLine(penGrid, rect.X, y, rect.Right, y);
                    }
                }
            }
        }

        private void DesenharElementos(Graphics g, RectangleF rectEtiqueta)
        {
            if (template.Elementos.Count == 0)
            {
                string texto = "Adicione elementos usando a toolbox ←";
                using (Font fonteComZoom = new Font(this.Font.FontFamily, this.Font.Size * zoom, this.Font.Style))
                {
                    SizeF tamanho = g.MeasureString(texto, fonteComZoom);
                    g.DrawString(texto, fonteComZoom, Brushes.Gray,
                        rectEtiqueta.X + (rectEtiqueta.Width - tamanho.Width) / 2,
                        rectEtiqueta.Y + (rectEtiqueta.Height - tamanho.Height) / 2);
                }
                return;
            }

            foreach (var elem in template.Elementos)
            {
                DesenharElemento(g, elem, rectEtiqueta, null);

                bool estaSelecionado = (elem == elementoSelecionado) || elementosSelecionados.Contains(elem);

                if (estaSelecionado)
                {
                    Rectangle bounds = ConverterParaPixels(elem.Bounds, rectEtiqueta);

                    // Aplicar delta de arrasto se estiver arrastando
                    if (arrastando)
                    {
                        bounds.X += deltaArrasto.X;
                        bounds.Y += deltaArrasto.Y;
                    }

                    using (Pen penSelecao = new Pen(Color.Blue, 2))
                    {
                        penSelecao.DashStyle = DashStyle.Dash;
                        g.DrawRectangle(penSelecao, bounds);
                    }

                    // Desenhar handles apenas se for seleção única
                    if (elementoSelecionado == elem && elementosSelecionados.Count == 0)
                    {
                        DesenharHandles(g, bounds);
                    }
                }
            }
        }

        private void DesenharElemento(Graphics g, ElementoEtiqueta elem, RectangleF rectEtiqueta, Produto produto)
        {
            // Calcular bounds considerando o delta de arrasto se este elemento está sendo arrastado
            Rectangle bounds = ConverterParaPixels(elem.Bounds, rectEtiqueta);
            if (elem == elementoSelecionado && arrastando && deltaArrasto != Point.Empty)
            {
                bounds.X += deltaArrasto.X;
                bounds.Y += deltaArrasto.Y;
            }

            switch (elem.Tipo)
            {
                case TipoElemento.Texto:
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    using (Font fonteComZoom = new Font(elem.Fonte.FontFamily, elem.Fonte.Size * zoom, elem.Fonte.Style))
                    {
                        StringFormat sf = new StringFormat
                        {
                            Alignment = elem.Alinhamento,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,  // ← ADICIONAR
                            FormatFlags = StringFormatFlags.LineLimit     // ← ADICIONAR
                        };
                        g.DrawString(elem.Conteudo ?? "Texto", fonteComZoom, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.Campo:
                    string valor = ObterValorCampo(elem.Conteudo, produto);
                    using (SolidBrush brush = new SolidBrush(elem.Cor))
                    using (Font fonteComZoom = new Font(elem.Fonte.FontFamily, elem.Fonte.Size * zoom, elem.Fonte.Style))
                    {
                        StringFormat sf = new StringFormat
                        {
                            Alignment = elem.Alinhamento,
                            LineAlignment = StringAlignment.Center,
                            Trimming = StringTrimming.EllipsisCharacter,  // ← ADICIONAR
                            FormatFlags = StringFormatFlags.LineLimit     // ← ADICIONAR
                        };
                        g.DrawString(valor, fonteComZoom, brush, bounds, sf);
                    }
                    break;

                case TipoElemento.CodigoBarras:
                    string codigoBarras = ObterValorCampo(elem.Conteudo, produto);
                    DesenharCodigoBarras(g, codigoBarras, bounds);
                    break;

                case TipoElemento.Imagem:
                    if (elem.Imagem != null)
                    {
                        g.DrawImage(elem.Imagem, bounds);
                    }
                    else
                    {
                        g.FillRectangle(Brushes.LightGray, bounds);
                        using (Font fonteComZoom = new Font("Arial", 8 * zoom, FontStyle.Regular))
                        {
                            g.DrawString("Imagem", fonteComZoom, Brushes.Black, bounds);
                        }
                    }
                    break;
            }

            g.DrawRectangle(Pens.LightGray, bounds);
        }

        #endregion

        #region Métodos Auxiliares de Desenho

        private Rectangle ConverterParaPixels(Rectangle boundsEmMM, RectangleF rectEtiqueta)
        {
            return new Rectangle(
                (int)(rectEtiqueta.X + boundsEmMM.X * MM_PARA_PIXEL * zoom),
                (int)(rectEtiqueta.Y + boundsEmMM.Y * MM_PARA_PIXEL * zoom),
                (int)(boundsEmMM.Width * MM_PARA_PIXEL * zoom),
                (int)(boundsEmMM.Height * MM_PARA_PIXEL * zoom)
            );
        }

        private Rectangle ConverterParaMM(Rectangle boundsEmPixels, RectangleF rectEtiqueta)
        {
            const float pixelParaMM = 1f / MM_PARA_PIXEL;

            return new Rectangle(
                (int)((boundsEmPixels.X - rectEtiqueta.X) * pixelParaMM / zoom),
                (int)((boundsEmPixels.Y - rectEtiqueta.Y) * pixelParaMM / zoom),
                (int)(boundsEmPixels.Width * pixelParaMM / zoom),
                (int)(boundsEmPixels.Height * pixelParaMM / zoom)
            );
        }

        private string ObterValorCampo(string campo, Produto produto)
        {
            if (produto == null)
            {
                return $"[{campo}]";
            }

            switch (campo)
            {
                case "Mercadoria": return produto.Nome ?? "";
                case "CodigoMercadoria": return produto.Codigo ?? "";
                case "CodFabricante": return produto.CodFabricante ?? "";
                case "CodBarras": return produto.CodBarras ?? "";
                case "PrecoVenda": return produto.Preco.ToString("C2");
                case "VendaA": return produto.Preco.ToString("C2");
                case "VendaB": return produto.Preco.ToString("C2");
                case "VendaC": return produto.Preco.ToString("C2");
                case "VendaD": return produto.Preco.ToString("C2");
                case "VendaE": return produto.Preco.ToString("C2");
                case "Fornecedor": return produto.Nome ?? "";
                case "Fabricante": return produto.Nome ?? "";
                case "Grupo": return "";
                case "Prateleira": return "";
                case "Garantia": return "";
                case "Tam": return "";
                case "Cores": return "";
                case "CodBarras_Grade": return produto.CodBarras_Grade ?? "";
                default: return "";
            }
        }

        private void DesenharCodigoBarras(Graphics g, string codigo, Rectangle bounds)
        {
            string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), c => char.IsDigit(c)));
            if (string.IsNullOrEmpty(codigoLimpo)) codigoLimpo = "0000000000";
            if (codigoLimpo.Length < 8) codigoLimpo = codigoLimpo.PadLeft(8, '0');

            float larguraBarra = (float)bounds.Width / (codigoLimpo.Length * 2);
            float alturaBarras = bounds.Height;// * 0.7f;

            for (int i = 0; i < codigoLimpo.Length; i++)
            {
                int digito = int.Parse(codigoLimpo[i].ToString());
                float larguraAtual = (digito % 2 == 0) ? larguraBarra : larguraBarra * 1.5f;

                float x = bounds.X + (i * larguraBarra * 2);

                using (SolidBrush brush = new SolidBrush(Color.Black))
                {
                    g.FillRectangle(brush, x, bounds.Y, larguraAtual, alturaBarras);
                }
            }

            //using (Font fonte = new Font("Arial", 7 * zoom))  // Aplicar zoom na fonte
            //using (SolidBrush brush = new SolidBrush(Color.Black))
            //{
            //    StringFormat sf = new StringFormat
            //    {
            //        Alignment = StringAlignment.Center,
            //        LineAlignment = StringAlignment.Far
            //    };
            //    g.DrawString(codigoLimpo, fonte, brush, bounds, sf);
            //}
        }

        private void DesenharHandles(Graphics g, Rectangle bounds)
        {
            int handleSize = 8;
            using (SolidBrush brush = new SolidBrush(Color.White))
            using (Pen pen = new Pen(Color.Blue, 1))
            {
                Point[] handles = new Point[]
                {
                    new Point(bounds.Left, bounds.Top),
                    new Point(bounds.Right, bounds.Top),
                    new Point(bounds.Right, bounds.Bottom),
                    new Point(bounds.Left, bounds.Bottom),
                    new Point(bounds.Left + bounds.Width / 2, bounds.Top),
                    new Point(bounds.Right, bounds.Top + bounds.Height / 2),
                    new Point(bounds.Left + bounds.Width / 2, bounds.Bottom),
                    new Point(bounds.Left, bounds.Top + bounds.Height / 2)
                };

                foreach (Point handle in handles)
                {
                    Rectangle handleRect = new Rectangle(
                        handle.X - handleSize / 2,
                        handle.Y - handleSize / 2,
                        handleSize,
                        handleSize
                    );
                    g.FillRectangle(brush, handleRect);
                    g.DrawRectangle(pen, handleRect);
                }
            }
        }

        private int ObterHandleClicado(Point mouse, Rectangle bounds)
        {
            // Mantém handleSize fixo em pixels, independente do zoom
            int handleSize = 8;
            int tolerance = 4;

            Point[] handles = new Point[]
            {
                new Point(bounds.Left, bounds.Top),
                new Point(bounds.Right, bounds.Top),
                new Point(bounds.Right, bounds.Bottom),
                new Point(bounds.Left, bounds.Bottom),
                new Point(bounds.Left + bounds.Width / 2, bounds.Top),
                new Point(bounds.Right, bounds.Top + bounds.Height / 2),
                new Point(bounds.Left + bounds.Width / 2, bounds.Bottom),
                new Point(bounds.Left, bounds.Top + bounds.Height / 2)
            };

            for (int i = 0; i < handles.Length; i++)
            {
                Rectangle handleRect = new Rectangle(
                    handles[i].X - handleSize / 2 - tolerance,
                    handles[i].Y - handleSize / 2 - tolerance,
                    handleSize + tolerance * 2,
                    handleSize + tolerance * 2
                );

                if (handleRect.Contains(mouse))
                {
                    return i;
                }
            }

            return -1;
        }

        #endregion

        #region Eventos do Mouse

        private void PbCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;

            RectangleF rectEtiqueta = new RectangleF(25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

            // Verificar se clicou em um elemento já selecionado
            if (elementoSelecionado != null)
            {
                Rectangle bounds = ConverterParaPixels(elementoSelecionado.Bounds, rectEtiqueta);
                handleSelecionado = ObterHandleClicado(e.Location, bounds);

                if (handleSelecionado >= 0)
                {
                    redimensionando = true;
                    pontoInicialMouse = e.Location;
                    boundsIniciais = bounds;
                    return;
                }

                if (bounds.Contains(e.Location))
                {
                    arrastando = true;
                    pontoInicialMouse = e.Location;
                    boundsIniciais = bounds;
                    return;
                }
            }

            // Verificar se clicou em algum dos elementos da seleção múltipla
            foreach (var elem in elementosSelecionados)
            {
                Rectangle bounds = ConverterParaPixels(elem.Bounds, rectEtiqueta);
                if (bounds.Contains(e.Location))
                {
                    arrastando = true;
                    pontoInicialMouse = e.Location;
                    return;
                }
            }

            // Procurar elemento clicado
            ElementoEtiqueta elementoClicado = null;
            for (int i = template.Elementos.Count - 1; i >= 0; i--)
            {
                Rectangle bounds = ConverterParaPixels(template.Elementos[i].Bounds, rectEtiqueta);
                if (bounds.Contains(e.Location))
                {
                    elementoClicado = template.Elementos[i];
                    break;
                }
            }

            if (elementoClicado != null)
            {
                // CTRL pressionado = adicionar/remover da seleção múltipla
                if (ModifierKeys == Keys.Control)
                {
                    if (elementosSelecionados.Contains(elementoClicado))
                    {
                        elementosSelecionados.Remove(elementoClicado);
                    }
                    else
                    {
                        elementosSelecionados.Add(elementoClicado);
                        elementoSelecionado = null; // Limpar seleção única
                    }
                }
                else
                {
                    // Clique normal = selecionar apenas este elemento
                    elementoSelecionado = elementoClicado;
                    elementosSelecionados.Clear();
                    pontoInicialMouse = e.Location;

                    Rectangle bounds = ConverterParaPixels(elementoClicado.Bounds, rectEtiqueta);
                    boundsIniciais = bounds;
                    handleSelecionado = ObterHandleClicado(e.Location, bounds);

                    if (handleSelecionado >= 0)
                    {
                        redimensionando = true;
                    }
                    else
                    {
                        arrastando = true;
                    }
                }

                AtualizarPainelPropriedades();
                pbCanvas.Invalidate();
                return;
            }

            // Não clicou em nenhum elemento = iniciar seleção por retângulo
            elementoSelecionado = null;
            elementosSelecionados.Clear();
            selecionandoComRetangulo = true;
            pontoInicialSelecao = e.Location;
            retanguloSelecao = new Rectangle(e.Location, Size.Empty);

            AtualizarPainelPropriedades();
            pbCanvas.Invalidate();
        }

        private void PbCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            RectangleF rectEtiqueta = new RectangleF(25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

            // Selecionando com retângulo
            if (selecionandoComRetangulo)
            {
                int x = Math.Min(pontoInicialSelecao.X, e.X);
                int y = Math.Min(pontoInicialSelecao.Y, e.Y);
                int width = Math.Abs(e.X - pontoInicialSelecao.X);
                int height = Math.Abs(e.Y - pontoInicialSelecao.Y);

                retanguloSelecao = new Rectangle(x, y, width, height);
                pbCanvas.Invalidate();
                return;
            }

            // Redimensionando elemento único
            if (redimensionando && elementoSelecionado != null)
            {
                int deltaX = e.X - pontoInicialMouse.X;
                int deltaY = e.Y - pontoInicialMouse.Y;

                Rectangle newBounds = boundsIniciais;

                switch (handleSelecionado)
                {
                    case 0:  // Canto superior esquerdo
                        newBounds = new Rectangle(
                            boundsIniciais.X + deltaX,
                            boundsIniciais.Y + deltaY,
                            boundsIniciais.Width - deltaX,
                            boundsIniciais.Height - deltaY);
                        break;

                    case 1:  // Canto superior direito
                        newBounds = new Rectangle(
                            boundsIniciais.X,
                            boundsIniciais.Y + deltaY,
                            boundsIniciais.Width + deltaX,
                            boundsIniciais.Height - deltaY);
                        break;

                    case 2:  // Canto inferior direito
                        newBounds = new Rectangle(
                            boundsIniciais.X,
                            boundsIniciais.Y,
                            boundsIniciais.Width + deltaX,
                            boundsIniciais.Height + deltaY);
                        break;

                    case 3:  // Canto inferior esquerdo
                        newBounds = new Rectangle(
                            boundsIniciais.X + deltaX,
                            boundsIniciais.Y,
                            boundsIniciais.Width - deltaX,
                            boundsIniciais.Height + deltaY);
                        break;

                    case 4:  // Lado superior (centro)
                        newBounds = new Rectangle(
                            boundsIniciais.X,
                            boundsIniciais.Y + deltaY,
                            boundsIniciais.Width,
                            boundsIniciais.Height - deltaY);
                        break;

                    case 5:  // Lado direito (centro)
                        newBounds = new Rectangle(
                            boundsIniciais.X,
                            boundsIniciais.Y,
                            boundsIniciais.Width + deltaX,
                            boundsIniciais.Height);
                        break;

                    case 6:  // Lado inferior (centro)
                        newBounds = new Rectangle(
                            boundsIniciais.X,
                            boundsIniciais.Y,
                            boundsIniciais.Width,
                            boundsIniciais.Height + deltaY);
                        break;

                    case 7:  // Lado esquerdo (centro)
                        newBounds = new Rectangle(
                            boundsIniciais.X + deltaX,
                            boundsIniciais.Y,
                            boundsIniciais.Width - deltaX,
                            boundsIniciais.Height);
                        break;
                }

                if (newBounds.Width >= 10 && newBounds.Height >= 5)
                {
                    elementoSelecionado.Bounds = ConverterParaMM(newBounds, rectEtiqueta);
                    pbCanvas.Invalidate();
                }
            }
            // Arrastando elemento(s)
            else if (arrastando)
            {
                deltaArrasto = new Point(
                    e.X - pontoInicialMouse.X,
                    e.Y - pontoInicialMouse.Y
                );
                pbCanvas.Invalidate();
            }
            // Atualizar cursor
            else
            {
                if (elementoSelecionado != null)
                {
                    Rectangle bounds = ConverterParaPixels(elementoSelecionado.Bounds, rectEtiqueta);
                    int handle = ObterHandleClicado(e.Location, bounds);

                    if (handle >= 0)
                    {
                        switch (handle)
                        {
                            case 0:
                            case 2:
                                pbCanvas.Cursor = Cursors.SizeNWSE;
                                break;
                            case 1:
                            case 3:
                                pbCanvas.Cursor = Cursors.SizeNESW;
                                break;
                            case 4:
                            case 6:
                                pbCanvas.Cursor = Cursors.SizeNS;
                                break;
                            case 5:
                            case 7:
                                pbCanvas.Cursor = Cursors.SizeWE;
                                break;
                            default:
                                pbCanvas.Cursor = Cursors.SizeAll;
                                break;
                        }
                    }
                    else if (bounds.Contains(e.Location))
                        pbCanvas.Cursor = Cursors.SizeAll;
                    else
                        pbCanvas.Cursor = Cursors.Default;
                }
            }
        }

        private void PbCanvas_MouseUp(object sender, MouseEventArgs e)
        {
            RectangleF rectEtiqueta = new RectangleF(25, 25,
                configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom,
                configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

            // Finalizar seleção por retângulo
            if (selecionandoComRetangulo)
            {
                selecionandoComRetangulo = false;

                // Selecionar todos os elementos dentro do retângulo
                elementosSelecionados.Clear();
                foreach (var elemento in template.Elementos)
                {
                    Rectangle bounds = ConverterParaPixels(elemento.Bounds, rectEtiqueta);
                    if (retanguloSelecao.IntersectsWith(bounds))
                    {
                        elementosSelecionados.Add(elemento);
                    }
                }

                pbCanvas.Invalidate();
                return;
            }

            // Finalizar arrasto
            if (arrastando && deltaArrasto != Point.Empty)
            {
                float deltaXMM = deltaArrasto.X / (MM_PARA_PIXEL * zoom);
                float deltaYMM = deltaArrasto.Y / (MM_PARA_PIXEL * zoom);

                // Arrastar elemento único
                if (elementoSelecionado != null)
                {
                    elementoSelecionado.Bounds = new Rectangle(
                        (int)(elementoSelecionado.Bounds.X + deltaXMM),
                        (int)(elementoSelecionado.Bounds.Y + deltaYMM),
                        elementoSelecionado.Bounds.Width,
                        elementoSelecionado.Bounds.Height
                    );
                }

                // Arrastar múltiplos elementos
                foreach (var elemento in elementosSelecionados)
                {
                    elemento.Bounds = new Rectangle(
                        (int)(elemento.Bounds.X + deltaXMM),
                        (int)(elemento.Bounds.Y + deltaYMM),
                        elemento.Bounds.Width,
                        elemento.Bounds.Height
                    );
                }

                deltaArrasto = Point.Empty;
                pbCanvas.Invalidate(); // Forçar redesenho imediato
            }

            arrastando = false;
            redimensionando = false;
            handleSelecionado = -1;
            pbCanvas.Cursor = Cursors.Default;
            pbCanvas.Invalidate(); // Garantir redesenho final
        }

        private void PbCanvas_MouseWheel(object sender, MouseEventArgs e)
        {
            // Zoom apenas com CTRL pressionado
            if (ModifierKeys == Keys.Control)
            {
                // Aumentar ou diminuir o zoom com base na direção do scroll
                if (e.Delta > 0)
                {
                    // Scroll para cima = aumentar zoom
                    zoom += 0.1f;
                    if (zoom > 3.0f) zoom = 3.0f; // Limitar zoom máximo
                }
                else
                {
                    // Scroll para baixo = diminuir zoom
                    zoom -= 0.1f;
                    if (zoom < 0.3f) zoom = 0.3f; // Limitar zoom mínimo
                }

                AtualizarTamanhoCanvas();
                pbCanvas.Invalidate();

                // Evitar que o scroll padrão também aconteça
                ((HandledMouseEventArgs)e).Handled = true;
            }
        }

        #endregion

        #region Eventos de Configuração

        private void CarregarDadosNaInterface()
        {
            cmbImpressora.Items.Clear();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                cmbImpressora.Items.Add(printer);
            }

            if (!string.IsNullOrEmpty(configuracao.ImpressoraPadrao) &&
                cmbImpressora.Items.Contains(configuracao.ImpressoraPadrao))
            {
                cmbImpressora.SelectedItem = configuracao.ImpressoraPadrao;
            }
            else if (cmbImpressora.Items.Count > 0)
            {
                cmbImpressora.SelectedIndex = 0;
            }

            CarregarPapeisDaImpressora();
        }

        private void CmbImpressora_SelectedIndexChanged(object sender, EventArgs e)
        {
            CarregarPapeisDaImpressora();
            AtualizarConfiguracao();
        }

        private void CmbPapel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //CarregarPapeisDaImpressora();
            AtualizarConfiguracao();
        }

        private void CarregarPapeisDaImpressora()
        {
            cmbPapel.Items.Clear();

            if (cmbImpressora.SelectedItem == null) return;

            try
            {
                var printerSettings = new System.Drawing.Printing.PrinterSettings
                {
                    PrinterName = cmbImpressora.SelectedItem.ToString()
                };

                foreach (System.Drawing.Printing.PaperSize papel in printerSettings.PaperSizes)
                {
                    cmbPapel.Items.Add(papel.PaperName);
                }

                if (!string.IsNullOrEmpty(configuracao.PapelPadrao) &&
                    cmbPapel.Items.Cast<object>().Any(x => x.ToString() == configuracao.PapelPadrao))
                {
                    cmbPapel.SelectedItem = configuracao.PapelPadrao;
                }
                else if (cmbPapel.Items.Count > 0)
                {
                    cmbPapel.SelectedIndex = 0;
                }
            }
            catch
            {
                cmbPapel.Items.Add("(Erro ao carregar papéis)");
                cmbPapel.SelectedIndex = 0;
            }
        }

        private void ChkPadraoDesativar_CheckedChanged(object sender, EventArgs e)
        {
            AtualizarEstadoMargens();
        }

        private void AtualizarEstadoMargens()
        {
            bool desabilitado = chkPadraoDesativar.Checked;

            numMargemSuperior.Enabled = !desabilitado;
            numMargemInferior.Enabled = !desabilitado;
            numMargemEsquerda.Enabled = !desabilitado;
            numMargemDireita.Enabled = !desabilitado;

            if (desabilitado)
            {
                numMargemSuperior.Value = 0;
                numMargemInferior.Value = 0;
                numMargemEsquerda.Value = 0;
                numMargemDireita.Value = 0;
            }
        }

        private void AtualizarConfiguracao()
        {
            configuracao.LarguraEtiqueta = (float)numLargura.Value;
            configuracao.AlturaEtiqueta = (float)numAltura.Value;
            configuracao.ImpressoraPadrao = cmbImpressora.SelectedItem?.ToString() ?? "";
            configuracao.PapelPadrao = cmbPapel.SelectedItem?.ToString() ?? "";
            configuracao.NumColunas = (int)numColunas.Value;
            configuracao.NumLinhas = (int)numLinhas.Value;
            configuracao.EspacamentoColunas = (float)numEspacamentoColunas.Value;
            configuracao.EspacamentoLinhas = (float)numEspacamentoLinhas.Value;
            configuracao.MargemSuperior = (float)numMargemSuperior.Value;
            configuracao.MargemInferior = (float)numMargemInferior.Value;
            configuracao.MargemEsquerda = (float)numMargemEsquerda.Value;
            configuracao.MargemDireita = (float)numMargemDireita.Value;

            template.Largura = configuracao.LarguraEtiqueta;
            template.Altura = configuracao.AlturaEtiqueta;

            AtualizarTamanhoCanvas();
            pbCanvas?.Invalidate();
        }

        private void AtualizarTamanhoCanvas()
        {

            // PROTEÇÃO: Verifica se pbCanvas foi inicializado
            if (pbCanvas == null || panelCanvas == null)
                return;

            int larguraPixels = (int)(configuracao.LarguraEtiqueta * MM_PARA_PIXEL * zoom);
            int alturaPixels = (int)(configuracao.AlturaEtiqueta * MM_PARA_PIXEL * zoom);

            // Adicionar margem ao redor do canvas (50 pixels no total)
            pbCanvas.Size = new Size(larguraPixels + 50, alturaPixels + 50);

            // Centralizar apenas se o canvas for menor que o painel
            // Se for maior, posicionar em (0,0) para permitir scroll
            int posX, posY;

            if (pbCanvas.Width < panelCanvas.Width)
            {
                posX = (panelCanvas.Width - pbCanvas.Width) / 2;
            }
            else
            {
                posX = 0;
            }

            if (pbCanvas.Height < panelCanvas.Height)
            {
                posY = (panelCanvas.Height - pbCanvas.Height) / 2;
            }
            else
            {
                posY = 0;
            }

            pbCanvas.Location = new Point(posX, posY);
        }

        #endregion

        #region Eventos de Teclado

        private void FormDesignNovo_KeyDown(object sender, KeyEventArgs e)
        {
            // Trabalhar com seleção múltipla ou única
            List<ElementoEtiqueta> elementosParaMover = new List<ElementoEtiqueta>();

            if (elementosSelecionados.Count > 0)
            {
                elementosParaMover.AddRange(elementosSelecionados);
            }
            else if (elementoSelecionado != null)
            {
                elementosParaMover.Add(elementoSelecionado);
            }

            if (elementosParaMover.Count == 0) return;

            bool houveAlteracao = false;
            int passo = 1;

            // Verifica qual tecla foi pressionada
            switch (e.KeyCode)
            {
                case Keys.Left:
                    foreach (var elem in elementosParaMover)
                    {
                        var novaPosicao = elem.Bounds;
                        novaPosicao.X -= passo;
                        elem.Bounds = novaPosicao;
                    }
                    houveAlteracao = true;
                    break;

                case Keys.Right:
                    foreach (var elem in elementosParaMover)
                    {
                        var novaPosicao = elem.Bounds;
                        novaPosicao.X += passo;
                        elem.Bounds = novaPosicao;
                    }
                    houveAlteracao = true;
                    break;

                case Keys.Up:
                    foreach (var elem in elementosParaMover)
                    {
                        var novaPosicao = elem.Bounds;
                        novaPosicao.Y -= passo;
                        elem.Bounds = novaPosicao;
                    }
                    houveAlteracao = true;
                    break;

                case Keys.Down:
                    foreach (var elem in elementosParaMover)
                    {
                        var novaPosicao = elem.Bounds;
                        novaPosicao.Y += passo;
                        elem.Bounds = novaPosicao;
                    }
                    houveAlteracao = true;
                    break;

                case Keys.Delete:
                    // Remover todos os elementos selecionados
                    foreach (var elem in elementosParaMover)
                    {
                        template.Elementos.Remove(elem);
                    }
                    elementoSelecionado = null;
                    elementosSelecionados.Clear();
                    AtualizarPainelPropriedades();
                    pbCanvas.Invalidate();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    return;
            }

            // Se moveu, atualiza
            if (houveAlteracao)
            {
                pbCanvas.Invalidate();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        #endregion

        #region Botões de Ação

        private void BtnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(nomeTemplateAtual))
            {
                using (var formNome = new FormNomeTemplate())
                {
                    if (formNome.ShowDialog() == DialogResult.OK)
                    {
                        nomeTemplateAtual = formNome.NomeTemplate;
                        configuracao.NomeEtiqueta = nomeTemplateAtual;
                    }
                    else
                    {
                        return;
                    }
                }
            }

            if (TemplateManager.SalvarTemplate(template, nomeTemplateAtual))
            {
                ConfiguracaoManager.SalvarConfiguracao(nomeTemplateAtual, configuracao);

                MessageBox.Show(
                    $"Template '{nomeTemplateAtual}' salvo com sucesso!\n\n" +
                    $"✅ Template: {configuracao.LarguraEtiqueta:F1} x {configuracao.AlturaEtiqueta:F1} mm\n" +
                    $"✅ Elementos: {template.Elementos.Count}\n" +
                    $"✅ Layout: {configuracao.NumColunas} col x {configuracao.NumLinhas} lin\n" +
                    $"✅ Configuração vinculada",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Erro ao salvar template!", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNovo_Click(object sender, EventArgs e)
        {
            var resultado = MessageBox.Show(
                "Deseja criar um novo template?\n\nAs alterações não salvas serão perdidas.",
                "Novo Template",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                template = new TemplateEtiqueta { Largura = 100, Altura = 30 };
                configuracao = new ConfiguracaoEtiqueta
                {
                    LarguraEtiqueta = 100,
                    AlturaEtiqueta = 30,
                    NumColunas = 1,
                    NumLinhas = 1
                };
                nomeTemplateAtual = null;
                elementoSelecionado = null;

                CarregarDadosNaInterface();
                AtualizarConfiguracao();
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            // Obter dimensões do papel selecionado
            float larguraPapel = 210; // A4 padrão
            float alturaPapel = 297;
            string nomePapel = cmbPapel.SelectedItem?.ToString() ?? "A4";

            // Tentar obter as dimensões reais do papel da impressora
            if (cmbImpressora.SelectedItem != null)
            {
                try
                {
                    PrinterSettings printerSettings = new PrinterSettings();
                    printerSettings.PrinterName = cmbImpressora.SelectedItem.ToString();

                    foreach (PaperSize paperSize in printerSettings.PaperSizes)
                    {
                        if (paperSize.PaperName == nomePapel)
                        {
                            // Converter de centésimos de polegada para MM
                            larguraPapel = (paperSize.Width / 100f) * 25.4f;
                            alturaPapel = (paperSize.Height / 100f) * 25.4f;
                            break;
                        }
                    }
                }
                catch
                {
                    // Se falhar, usar dimensões padrão baseadas no nome
                }
            }

            // Criar e mostrar formulário de preview
            FormPreview formPreview = new FormPreview(template, configuracao, nomePapel, larguraPapel, alturaPapel);
            formPreview.ShowDialog();
        }

        private void BtnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Propriedades Públicas

        public TemplateEtiqueta ObterTemplate()
        {
            return template;
        }

        public ConfiguracaoEtiqueta ObterConfiguracao()
        {
            return configuracao;
        }

        #endregion
    }
}