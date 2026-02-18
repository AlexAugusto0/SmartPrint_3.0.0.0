using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public partial class telaTecnico : Form
    {
        private List<ImpressoraInfo> impressoras = new List<ImpressoraInfo>();
        private DriverInstaller driverInstaller;
        private Timer timerAtualizacao;

        public telaTecnico()
        {
            InitializeComponent();

            // Inicializa o instalador de drivers
            driverInstaller = new DriverInstaller(this);

            CarregarImpressoras();
            VersaoHelper.DefinirTituloComVersao(this, "Instalação de Drivers");
            InicializarListView();

            // Timer para atualizar lista periodicamente
            InicializarTimer();

            // Aplica efeitos hover nos botões
            AplicarEfeitosHover();
        }

        private void InicializarTimer()
        {
            // Timer para atualizar a lista a cada 3 segundos (quando visível)
            timerAtualizacao = new Timer();
            timerAtualizacao.Interval = 3000; // 3 segundos
            timerAtualizacao.Tick += TimerAtualizacao_Tick;
        }

        private void TimerAtualizacao_Tick(object sender, EventArgs e)
        {
            // Só atualiza se a lista estiver visível e tiver itens
            if (groupBoxDeteccao.Visible && listViewDispositivos.Items.Count > 0)
            {
                AtualizarListaDispositivos();
            }
        }

        private void AplicarEfeitosHover()
        {
            // Efeito hover para checkBox1 (Detecção Automática)
            checkBox1.MouseEnter += (s, e) =>
            {
                if (!checkBox1.Checked)
                    checkBox1.BackColor = Color.FromArgb(189, 224, 254);
            };
            checkBox1.MouseLeave += (s, e) =>
            {
                if (!checkBox1.Checked)
                    checkBox1.BackColor = Color.FromArgb(236, 240, 241);
            };

            // Efeito hover para checkBox2 (Instalação Manual)
            checkBox2.MouseEnter += (s, e) =>
            {
                if (!checkBox2.Checked)
                    checkBox2.BackColor = Color.FromArgb(162, 238, 195);
            };
            checkBox2.MouseLeave += (s, e) =>
            {
                if (!checkBox2.Checked)
                    checkBox2.BackColor = Color.FromArgb(236, 240, 241);
            };

            // Efeito hover para botões
            //AplicarHoverBotao(btnProcurar, Color.FromArgb(52, 152, 219), Color.FromArgb(41, 128, 185));
            //AplicarHoverBotao(btnInstalarDriver, Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96));
            //AplicarHoverBotao(btnDownloadDriver, Color.FromArgb(46, 204, 113), Color.FromArgb(39, 174, 96));
        }

        private void AplicarHoverBotao(Button btn, Color corNormal, Color corHover)
        {
            btn.MouseEnter += (s, e) => { btn.BackColor = corHover; };
            btn.MouseLeave += (s, e) => { btn.BackColor = corNormal; };
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Checked = false;

                // Mostra modo de detecção automática
                groupBoxDeteccao.Visible = true;
                groupBoxManual.Visible = false;

                // Atualiza cor do botão selecionado
                checkBox1.ForeColor = Color.White;
                checkBox2.ForeColor = Color.FromArgb(52, 73, 94);

                // Inicia timer de atualização
                timerAtualizacao.Start();
            }
            else
            {
                groupBoxDeteccao.Visible = false;
                checkBox1.ForeColor = Color.FromArgb(52, 73, 94);

                // Para timer
                timerAtualizacao.Stop();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Checked = false;

                // Mostra modo manual
                groupBoxManual.Visible = true;
                groupBoxDeteccao.Visible = false;

                // Atualiza cor do botão selecionado
                checkBox2.ForeColor = Color.White;
                checkBox1.ForeColor = Color.FromArgb(52, 73, 94);

                // Para timer
                timerAtualizacao.Stop();
            }
            else
            {
                groupBoxManual.Visible = false;
                checkBox2.ForeColor = Color.FromArgb(52, 73, 94);
            }
        }

        private void CarregarImpressoras()
        {
            try
            {
                impressoras = ImpressoraManager.CarregarImpressoras();

                if (impressoras == null || impressoras.Count == 0)
                {
                    MessageBox.Show(
                        "Nenhuma impressora foi carregada. Verifique o arquivo impressoras.json",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    impressoras = new List<ImpressoraInfo>();
                    return;
                }

                comboBox1.Items.Clear();
                foreach (var imp in impressoras)
                    comboBox1.Items.Add(imp.Nome);

                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;

                if (comboBox1.Items.Count > 0)
                    comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar impressoras: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null) return;

            string selecionada = comboBox1.SelectedItem.ToString();
            var info = impressoras.Find(i => i.Nome == selecionada);

            if (info != null)
            {
                try
                {
                    if (pictureBox1.Image != null)
                    {
                        var imagemAnterior = pictureBox1.Image;
                        pictureBox1.Image = null;
                        imagemAnterior.Dispose();
                    }

                    pictureBox1.Image = info.ObterImagem();

                    if (pictureBox1.Image == null)
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Erro ao carregar imagem: {ex.Message}",
                        "Aviso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }

                btnDownloadDriver.Tag = info;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (btnDownloadDriver.Tag is ImpressoraInfo impressora)
            {
                driverInstaller.BaixarEInstalarDriver(impressora);
            }
            else
            {
                MessageBox.Show(
                    "Selecione uma impressora primeiro.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        private void InicializarListView()
        {
            listViewDispositivos.View = View.Details;
            listViewDispositivos.FullRowSelect = true;
            listViewDispositivos.GridLines = true;
            listViewDispositivos.Columns.Clear();
            listViewDispositivos.Columns.Add("Nome", 280);
            listViewDispositivos.Columns.Add("Fabricante", 120);
            listViewDispositivos.Columns.Add("Status do Driver", 140);
            listViewDispositivos.Columns.Add("Device ID", 240);
        }

        /// <summary>
        /// Verifica o status detalhado do driver de um dispositivo
        /// </summary>
        private string VerificarStatusDriver(ManagementObject device)
        {
            try
            {
                string deviceId = device["DeviceID"]?.ToString() ?? "";
                string nome = device["Name"]?.ToString() ?? "";

                // Verifica se a impressora está instalada no Windows
                bool impressoraInstalada = VerificarImpressoraInstalada(nome, deviceId);

                // ConfigManagerErrorCode: 0 = OK, outros valores = problema
                object errorCodeObj = device["ConfigManagerErrorCode"];
                int? errorCode = null;

                if (errorCodeObj != null)
                {
                    if (errorCodeObj is int)
                        errorCode = (int)errorCodeObj;
                    else if (errorCodeObj is uint)
                        errorCode = Convert.ToInt32((uint)errorCodeObj);
                }

                // Status do dispositivo
                string status = device["Status"]?.ToString();

                // Nome do driver
                string driverName = device["Service"]?.ToString();

                // Verifica se tem driver instalado
                bool temDriverName = !string.IsNullOrEmpty(driverName);

                // Se a impressora está instalada no Windows, considera instalado
                if (impressoraInstalada)
                {
                    return "✓ Instalado";
                }

                // Análise detalhada
                if (errorCode.HasValue && errorCode.Value == 0 && status == "OK" && temDriverName)
                {
                    return "✓ Instalado";
                }
                else if (errorCode.HasValue && errorCode.Value == 28) // Driver não instalado
                {
                    return "✗ Sem driver";
                }
                else if (errorCode.HasValue && errorCode.Value == 1) // Configuração incorreta
                {
                    return "⚠ Configuração incorreta";
                }
                else if (errorCode.HasValue && errorCode.Value == 10) // Dispositivo não iniciado
                {
                    return "⚠ Não iniciado";
                }
                else if (errorCode.HasValue && errorCode.Value == 22) // Desabilitado
                {
                    return "⊘ Desabilitado";
                }
                else if (errorCode.HasValue && errorCode.Value != 0)
                {
                    return $"✗ Erro ({errorCode.Value})";
                }
                else if (!temDriverName)
                {
                    return "✗ Driver ausente";
                }
                else if (!errorCode.HasValue)
                {
                    // Se errorCode é null, verifica outros indicadores
                    if (temDriverName && status == "OK")
                        return "✓ Instalado";
                    else if (!temDriverName)
                        return "✗ Sem driver";
                    else
                        return "⚠ Status desconhecido";
                }
                else
                {
                    return "⚠ Status desconhecido";
                }
            }
            catch
            {
                return "? Indeterminado";
            }
        }

        /// <summary>
        /// Verifica se a impressora está instalada consultando Win32_Printer
        /// </summary>
        private bool VerificarImpressoraInstalada(string nomeDispositivo, string deviceId)
        {
            try
            {
                // Remove "(U)" e outros sufixos do nome para comparação
                string nomeParaComparar = nomeDispositivo.Replace("(U)", "").Trim();

                // Consulta impressoras instaladas
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(
                    "SELECT * FROM Win32_Printer");

                foreach (ManagementObject printer in searcher.Get())
                {
                    string printerName = printer["Name"]?.ToString() ?? "";
                    string portName = printer["PortName"]?.ToString() ?? "";

                    // Verifica se o nome corresponde
                    if (printerName.IndexOf(nomeParaComparar, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }

                    // Verifica se a porta USB corresponde ao device ID
                    if (portName.StartsWith("USB", StringComparison.OrdinalIgnoreCase) &&
                        deviceId.IndexOf(portName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Obtém o fabricante do dispositivo
        /// </summary>
        private string ObterFabricante(ManagementObject device)
        {
            try
            {
                string manufacturer = device["Manufacturer"]?.ToString();

                if (!string.IsNullOrEmpty(manufacturer))
                {
                    // Remove "(Standard printer)" e textos comuns
                    manufacturer = manufacturer.Replace("(Standard printer)", "").Trim();
                    return manufacturer;
                }

                // Tenta extrair do nome do dispositivo
                string nome = device["Name"]?.ToString() ?? "";
                string[] marcas = { "Elgin", "Zebra", "Argox", "Epson", "HP", "Canon",
                                   "Brother", "Samsung", "Xerox", "Lexmark", "Dell",
                                   "Kyocera", "Ricoh", "Toshiba", "C3Tech", "Tanca",
                                   "Tomate", "SNBC", "Bematech", "Knup", "Coibel" };

                foreach (var marca in marcas)
                {
                    if (nome.IndexOf(marca, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        return marca;
                    }
                }

                return "Desconhecido";
            }
            catch
            {
                return "Desconhecido";
            }
        }

        /// <summary>
        /// Obtém cor baseada no status
        /// </summary>
        private Color ObterCorStatus(string status)
        {
            if (status.StartsWith("✓"))
                return Color.FromArgb(39, 174, 96);  // Verde - OK
            else if (status.StartsWith("✗"))
                return Color.FromArgb(231, 76, 60);  // Vermelho - Erro
            else if (status.StartsWith("⚠"))
                return Color.FromArgb(243, 156, 18); // Laranja - Aviso
            else if (status.StartsWith("⊘"))
                return Color.FromArgb(149, 165, 166); // Cinza - Desabilitado
            else
                return Color.FromArgb(52, 73, 94);   // Azul escuro - Desconhecido
        }

        private void BuscarDispositivosDeImpressoras()
        {
            try
            {
                listViewDispositivos.Items.Clear();

                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%USB%'");

                int encontrados = 0;
                foreach (ManagementObject obj in searcher.Get())
                {
                    string nome = obj["Name"]?.ToString() ?? "Desconhecido";
                    string deviceId = obj["DeviceID"]?.ToString() ?? "-";

                    // Filtra apenas impressoras USB
                    if (deviceId.StartsWith("USBPRINT", StringComparison.OrdinalIgnoreCase))
                    {
                        encontrados++;

                        string fabricante = ObterFabricante(obj);
                        string statusDriver = VerificarStatusDriver(obj);

                        var item = new ListViewItem(new[] { nome, fabricante, statusDriver, deviceId });
                        item.Tag = new { DeviceId = deviceId, Device = obj };

                        // Define cor baseada no status
                        Color corStatus = ObterCorStatus(statusDriver);
                        item.ForeColor = corStatus;

                        // Negrito se tiver problema
                        if (statusDriver.StartsWith("✗") || statusDriver.StartsWith("⚠"))
                        {
                            item.Font = new Font(listViewDispositivos.Font, FontStyle.Bold);
                        }

                        listViewDispositivos.Items.Add(item);
                    }
                }

                if (encontrados == 0)
                {
                    MessageBox.Show(
                        "Nenhuma impressora USB detectada.\n\n" +
                        "Verifique se:\n" +
                        "• A impressora está ligada\n" +
                        "• O cabo USB está conectado\n" +
                        "• O Windows detectou o dispositivo",
                        "Nenhuma Impressora Encontrada",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    string mensagem = $"{encontrados} impressora(s) USB encontrada(s).";

                    // Conta quantas estão OK vs com problema
                    int comProblema = listViewDispositivos.Items.Cast<ListViewItem>()
                        .Count(i => i.SubItems[2].Text.StartsWith("✗") || i.SubItems[2].Text.StartsWith("⚠"));

                    if (comProblema > 0)
                    {
                        mensagem += $"\n\n{comProblema} com problema de driver.";
                    }

                    MessageBox.Show(mensagem, "Busca Concluída",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao buscar impressoras: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Atualiza lista sem mostrar mensagens (para timer)
        /// </summary>
        private void AtualizarListaDispositivos()
        {
            try
            {
                // Salva item selecionado
                string deviceIdSelecionado = null;
                if (listViewDispositivos.SelectedItems.Count > 0)
                {
                    var tag = listViewDispositivos.SelectedItems[0].Tag;
                    if (tag != null)
                    {
                        var anonimo = tag as dynamic;
                        deviceIdSelecionado = anonimo?.DeviceId;
                    }
                }

                listViewDispositivos.Items.Clear();

                var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE DeviceID LIKE '%USB%'");

                foreach (ManagementObject obj in searcher.Get())
                {
                    string nome = obj["Name"]?.ToString() ?? "Desconhecido";
                    string deviceId = obj["DeviceID"]?.ToString() ?? "-";

                    if (deviceId.StartsWith("USBPRINT", StringComparison.OrdinalIgnoreCase))
                    {
                        string fabricante = ObterFabricante(obj);
                        string statusDriver = VerificarStatusDriver(obj);

                        var item = new ListViewItem(new[] { nome, fabricante, statusDriver, deviceId });
                        item.Tag = new { DeviceId = deviceId, Device = obj };

                        Color corStatus = ObterCorStatus(statusDriver);
                        item.ForeColor = corStatus;

                        if (statusDriver.StartsWith("✗") || statusDriver.StartsWith("⚠"))
                        {
                            item.Font = new Font(listViewDispositivos.Font, FontStyle.Bold);
                        }

                        listViewDispositivos.Items.Add(item);

                        // Reseleciona item se era o selecionado antes
                        if (deviceId == deviceIdSelecionado)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }
            catch
            {
                // Ignora erros na atualização automática
            }
        }

        private void btnProcurar_Click(object sender, EventArgs e)
        {
            listViewDispositivos.Items.Clear();
            BuscarDispositivosDeImpressoras();
        }

        private void btnInstalarDriver_Click(object sender, EventArgs e)
        {
            if (listViewDispositivos.SelectedItems.Count == 0)
            {
                MessageBox.Show(
                    "Selecione um dispositivo na lista para instalar o driver.",
                    "Nenhum Dispositivo Selecionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            string nomeDispositivo = listViewDispositivos.SelectedItems[0].SubItems[0].Text;
            string statusAtual = listViewDispositivos.SelectedItems[0].SubItems[2].Text;

            // Avisa se já está instalado
            if (statusAtual.StartsWith("✓"))
            {
                DialogResult continuar = MessageBox.Show(
                    $"O driver desta impressora já está instalado.\n\n" +
                    $"Dispositivo: {nomeDispositivo}\n" +
                    $"Status: {statusAtual}\n\n" +
                    $"Deseja reinstalar mesmo assim?",
                    "Driver Já Instalado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (continuar != DialogResult.Yes)
                    return;
            }

            var impressoraEncontrada = TentarIdentificarImpressora(nomeDispositivo);

            if (impressoraEncontrada != null)
            {
                DialogResult resultado = MessageBox.Show(
                    $"Dispositivo: {nomeDispositivo}\n\n" +
                    $"Impressora identificada: {impressoraEncontrada.Nome}\n\n" +
                    $"Deseja baixar e instalar o driver?",
                    "Driver Identificado",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (resultado == DialogResult.Yes)
                {
                    driverInstaller.BaixarEInstalarDriver(impressoraEncontrada);

                    // Aguarda 2 segundos e atualiza lista
                    Timer timerAtualizarAposInstalar = new Timer();
                    timerAtualizarAposInstalar.Interval = 2000;
                    timerAtualizarAposInstalar.Tick += (s, ev) =>
                    {
                        AtualizarListaDispositivos();
                        timerAtualizarAposInstalar.Stop();
                        timerAtualizarAposInstalar.Dispose();
                    };
                    timerAtualizarAposInstalar.Start();
                }
            }
            else
            {
                MostrarSelecaoManualDriver(nomeDispositivo);
            }
        }

        private ImpressoraInfo TentarIdentificarImpressora(string nomeDispositivo)
        {
            string nomeNormalizado = nomeDispositivo.ToLower().Replace(" ", "");

            foreach (var impressora in impressoras)
            {
                string nomeImpressoraNormalizado = impressora.Nome.ToLower().Replace(" ", "");

                if (nomeNormalizado.Contains(nomeImpressoraNormalizado) ||
                    nomeImpressoraNormalizado.Contains(nomeNormalizado))
                {
                    return impressora;
                }

                string[] partesDispositivo = nomeDispositivo.ToLower().Split(' ');
                string[] partesImpressora = impressora.Nome.ToLower().Split(' ');

                int correspondencias = partesDispositivo.Count(pd =>
                    partesImpressora.Any(pi => pi.Contains(pd) || pd.Contains(pi)));

                if (correspondencias >= 2)
                {
                    return impressora;
                }
            }

            return null;
        }

        private void MostrarSelecaoManualDriver(string nomeDispositivo)
        {
            using (FormSelecaoDriver formSelecao = new FormSelecaoDriver(impressoras, nomeDispositivo))
            {
                if (formSelecao.ShowDialog(this) == DialogResult.OK)
                {
                    var impressoraSelecionada = formSelecao.ImpressoraSelecionada;
                    if (impressoraSelecionada != null)
                    {
                        driverInstaller.BaixarEInstalarDriver(impressoraSelecionada);

                        // Aguarda e atualiza lista
                        Timer timerAtualizarAposInstalar = new Timer();
                        timerAtualizarAposInstalar.Interval = 2000;
                        timerAtualizarAposInstalar.Tick += (s, ev) =>
                        {
                            AtualizarListaDispositivos();
                            timerAtualizarAposInstalar.Stop();
                            timerAtualizarAposInstalar.Dispose();
                        };
                        timerAtualizarAposInstalar.Start();
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Para timer
            if (timerAtualizacao != null)
            {
                timerAtualizacao.Stop();
                timerAtualizacao.Dispose();
            }

            // Libera imagem
            if (pictureBox1.Image != null)
            {
                pictureBox1.Image.Dispose();
                pictureBox1.Image = null;
            }

            base.OnFormClosing(e);
        }
    }

    /// <summary>
    /// Formulário para seleção manual do driver
    /// </summary>
    public class FormSelecaoDriver : Form
    {
        private ComboBox comboImpressoras;
        private Button btnOK;
        private Button btnCancelar;
        private Label lblInfo;
        private PictureBox picturePreview;

        public ImpressoraInfo ImpressoraSelecionada { get; private set; }

        public FormSelecaoDriver(List<ImpressoraInfo> impressoras, string nomeDispositivo)
        {
            InitializeComponent(nomeDispositivo);
            CarregarImpressoras(impressoras);
        }

        private void InitializeComponent(string nomeDispositivo)
        {
            this.Text = "Selecionar Driver Manualmente";
            this.Size = new Size(550, 420);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(240, 240, 240);

            Label lblTitulo = new Label
            {
                Text = "Seleção Manual de Driver",
                Location = new Point(20, 20),
                Size = new Size(500, 25),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            lblInfo = new Label
            {
                Text = $"Dispositivo detectado:\n{nomeDispositivo}\n\nSelecione o modelo correto da impressora:",
                Location = new Point(20, 55),
                Size = new Size(500, 60),
                Font = new Font("Segoe UI", 9.5F),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            Label lblModelo = new Label
            {
                Text = "Modelo da Impressora:",
                Location = new Point(20, 125),
                Size = new Size(200, 20),
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(52, 73, 94)
            };

            comboImpressoras = new ComboBox
            {
                Location = new Point(20, 150),
                Size = new Size(500, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F)
            };
            comboImpressoras.SelectedIndexChanged += ComboImpressoras_SelectedIndexChanged;

            picturePreview = new PictureBox
            {
                Location = new Point(20, 190),
                Size = new Size(500, 150),
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White
            };

            btnOK = new Button
            {
                Text = "Baixar e Instalar",
                Location = new Point(310, 355),
                Size = new Size(130, 35),
                DialogResult = DialogResult.OK,
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;
            btnOK.MouseEnter += (s, e) => btnOK.BackColor = Color.FromArgb(39, 174, 96);
            btnOK.MouseLeave += (s, e) => btnOK.BackColor = Color.FromArgb(46, 204, 113);

            btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(450, 355),
                Size = new Size(80, 35),
                DialogResult = DialogResult.Cancel,
                BackColor = Color.FromArgb(149, 165, 166),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.FlatAppearance.BorderSize = 0;
            btnCancelar.MouseEnter += (s, e) => btnCancelar.BackColor = Color.FromArgb(127, 140, 141);
            btnCancelar.MouseLeave += (s, e) => btnCancelar.BackColor = Color.FromArgb(149, 165, 166);

            this.Controls.AddRange(new Control[] {
                lblTitulo, lblInfo, lblModelo, comboImpressoras, picturePreview, btnOK, btnCancelar
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancelar;
        }

        private void CarregarImpressoras(List<ImpressoraInfo> impressoras)
        {
            comboImpressoras.Items.Clear();
            foreach (var imp in impressoras)
            {
                comboImpressoras.Items.Add(imp);
            }
            comboImpressoras.DisplayMember = "Nome";

            if (comboImpressoras.Items.Count > 0)
                comboImpressoras.SelectedIndex = 0;
        }

        private void ComboImpressoras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboImpressoras.SelectedItem is ImpressoraInfo impressora)
            {
                if (picturePreview.Image != null)
                {
                    picturePreview.Image.Dispose();
                }
                picturePreview.Image = impressora.ObterImagem();
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            ImpressoraSelecionada = comboImpressoras.SelectedItem as ImpressoraInfo;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (picturePreview.Image != null)
            {
                picturePreview.Image.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}