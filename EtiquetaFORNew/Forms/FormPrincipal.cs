using EtiquetaFORNew;

using EtiquetaFORNew.Data;

using EtiquetaFORNew.Forms;

using System;

using System.Collections.Generic;

using System.Data;

using System.Drawing;

using System.Drawing.Drawing2D;

using System.IO;

using System.Linq;

using System.Windows.Forms;

using System.Xml.Serialization;





namespace EtiquetaFORNew

{

    public partial class FormPrincipal : Form

    {

        private List<Produto> produtos = new List<Produto>();

        private TemplateEtiqueta template;



        // ⭐ NOVO: Configuração de etiqueta atual

        private ConfiguracaoEtiqueta configuracaoAtual;

        // ⭐ Flag para controlar carregamento único de mercadorias
        private bool mercadoriasCarregadas = false;
        public bool PesquisaCodigo = false;



        // ⭐ NOVO: Campos transferidos de FormBuscaMercadoria

        private Timer timerBusca;

        private DataTable mercadorias;



        // ⭐ NOVO: Armazena dados completos do último produto buscado

        private DataRow produtoAtualCompleto = null;

        // ⭐ NOVO CONFECÇÃO: Módulo da aplicação e flag de confecção
        public string moduloApp = "";
        public bool isConfeccao = false;

        // ========================================
        // 🔹 CAMPOS PARA IMPORTAÇÃO EXTERNA
        // ========================================
        private DadosImportacao _dadosImportacao = null;
        private bool _modoImportacao = false;



        private static readonly string CAMINHO_CONFIGURACOES =

    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),

        "EtiquetaFornew", "configuracoes.xml");



        private static readonly string CAMINHO_MODELOS_PAPEL =

    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),

        "EtiquetaFornew", "modelos_papel.xml");



        public FormPrincipal()

        {

            InitializeComponent();


            TemplatesPreDefinidos.InstalarSeNecessario();

            template = new TemplateEtiqueta();

            CarregarUltimoTemplate();

            this.DoubleBuffered = true;

            this.Load += FormPrincipal_Load;
            this.FormClosing += FormPrincipal_FormClosing;

            ConfigurarBuscaMercadoria();

            cmbBuscaNome.KeyDown += ComboBoxBusca_KeyDown;

            cmbBuscaReferencia.KeyDown += ComboBoxBusca_KeyDown;

            cmbBuscaCodigo.KeyDown += ComboBoxBusca_KeyDown;

            CarregarConfiguracoesPapel();

            configuracaoAtual = CarregarConfiguracaoAtual();

            CarregarModelosPapel();

            dgvProdutos.CellEndEdit += dgvProdutos_CellEndEdit;

            if (cmbTamanho != null) cmbTamanho.SelectedIndexChanged += CmbTamanho_SelectedIndexChanged;
            if (cmbCor != null) cmbCor.SelectedIndexChanged += CmbCor_SelectedIndexChanged;

        }



        private void FormPrincipal_Load(object sender, EventArgs e)
        {
            // ========================================
            // 🔹 VALIDAR MÓDULO DA APLICAÇÃO
            // ========================================
            try
            {
                var config = Data.DatabaseConfig.LoadConfiguration();
                moduloApp = config.ModuloApp ?? "";
                isConfeccao = moduloApp.Equals("CONFECCAO", StringComparison.OrdinalIgnoreCase);

                // Configura visibilidade dos controles de confecção

                ConfigurarControlesConfeccao();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar configuração do módulo:\n{ex.Message}",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }

            // ========================================
            // 🔹 INICIALIZAR BANCO LOCAL SQLITE

            // ========================================

            try

            {

                LocalDatabaseManager.InicializarBanco();



                // Verificar se precisa sincronizar (mais de 24h desde última sync)

                if (LocalDatabaseManager.PrecisaSincronizar())

                {

                    var result = MessageBox.Show(

                        "Detectamos que faz mais de 24 horas desde a última sincronização.\n\n" +

                        "Deseja sincronizar as mercadorias do SQL Server agora?",

                        "Sincronização Recomendada",

                        MessageBoxButtons.YesNo,

                        MessageBoxIcon.Question);



                    if (result == DialogResult.Yes)

                    {

                        SincronizarMercadorias();

                    }

                }

            }

            catch (Exception ex)

            {

                MessageBox.Show(

                    $"Erro ao inicializar banco local:\n{ex.Message}\n\n" +

                    "O sistema continuará funcionando, mas você precisará adicionar produtos manualmente.",

                    "Aviso",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Warning);

              
            }



            // ========================================

            // 🔹 CARREGAR CONFIGURAÇÃO DE IMPRESSÃO

            // ========================================

            CarregarConfiguracaoImpressao();

            AtualizarListaConfiguracoes();

            // ⭐ OTIMIZAÇÃO: Carregar apenas se não foi carregado por SincronizarMercadorias
            if (!mercadoriasCarregadas)
            {
                CarregarTodasMercadorias();
            }



            // ========================================

            // 🔹 ARREDONDAR BOTÕES

            // ========================================

            ArredondarBotao(btnDesigner, 12);

            ArredondarBotao(btnImprimir, 12);

            ArredondarBotao(BtnAdicionar2, 12);

            if (_modoImportacao && _dadosImportacao != null)
            {
                ProcessarImportacaoExterna();
            }

        }



        // ========================================

        // ⭐ NOVO: GERENCIAMENTO DE CONFIGURAÇÕES

        // ========================================



        /// <summary>

        /// Carrega a configuração de impressão ao iniciar

        /// </summary>

        private void CarregarConfiguracaoImpressao()

        {

            configuracaoAtual = GerenciadorConfiguracoesEtiqueta.CarregarConfiguracaoPadrao();



            if (configuracaoAtual == null)

            {

                // Se não houver configuração, cria uma padrão baseada no template

                configuracaoAtual = new ConfiguracaoEtiqueta

                {

                    NomeEtiqueta = "Etiqueta Padrão",

                    ImpressoraPadrao = "BTP-L42(D)",

                    PapelPadrao = "Tamanho do papel-SoftcomGondBar",

                    LarguraEtiqueta = template.Largura,

                    AlturaEtiqueta = template.Altura,

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


        }



        /// <summary>

        /// Atualiza a lista de configurações no ComboBox

        /// </summary>

        private void AtualizarListaConfiguracoes()

        {



            // Adiciona configurações salvas

            List<ConfiguracaoPapel> papeisSalvos = GerenciadorConfiguracoesEtiqueta.CarregarTodasConfiguracoes();



            foreach (var papel in papeisSalvos)

            {

                var config = GerenciadorConfiguracoesEtiqueta.ConverterPapelParaConfig(

                    papel,

                    configuracaoAtual.ImpressoraPadrao

                );


            }

            // Atualiza o status



        }





        // ========================================

        // 🔹 SINCRONIZAR MERCADORIAS DO SQL SERVER

        // ========================================

        public void SincronizarMercadorias() // MUDADO PARA PUBLIC

        {

            try

            {

                Cursor = Cursors.WaitCursor;



                // Sincronizar todas as mercadorias (pode adicionar filtro se necessário)

                int total = LocalDatabaseManager.SincronizarMercadorias(); // Atualiza o banco local



                // ⭐ CHAVE DA SOLUÇÃO: RECARREGA O DATATABLE 'mercadorias' E ATUALIZA OS COMBOBOXES


                // ⭐ OTIMIZAÇÃO: Reseta flag para forçar recarregamento após sincronização
                mercadoriasCarregadas = false;

                CarregarTodasMercadorias();



                Cursor = Cursors.Default;



                MessageBox.Show(

                    $"Sincronização concluída com sucesso!\n\n" +

                    $"Total de mercadorias importadas: {total:N0}",

                    "Sucesso",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Information);

            }

            catch (Exception ex)

            {

                Cursor = Cursors.Default;

                MessageBox.Show(

                    $"Erro ao sincronizar:\n{ex.Message}",

                    "Erro",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);

            }

        }



        private void CarregarUltimoTemplate()

        {

            var ultimoTemplate = TemplateManager.CarregarUltimoTemplate();

            if (ultimoTemplate != null)

            {

                template = ultimoTemplate;

            }

        }


        private void btnDesigner_Click(object sender, EventArgs e)
        {


            TemplateEtiqueta templateParaAbrir = null;
            string nomeTemplate = null;


            using (var formLista = new FormListaTemplates())
            {
                if (formLista.ShowDialog() == DialogResult.OK)
                {
                    nomeTemplate = formLista.TemplateSelecionado;
                    templateParaAbrir = TemplateManager.CarregarTemplate(nomeTemplate);

                    if (templateParaAbrir == null)
                    {
                        MessageBox.Show($"Erro ao carregar template '{nomeTemplate}'!",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    return;
                }
            }


            // 1. Abre o Designer NOVO com template e nome
            if (templateParaAbrir != null && !string.IsNullOrEmpty(nomeTemplate))
            {
                using (var formDesigner = new FormDesignNovo(templateParaAbrir, nomeTemplate))
                {
                    if (formDesigner.ShowDialog() == DialogResult.OK)
                    {
                        MessageBox.Show(
                            $"Template '{nomeTemplate}' salvo com sucesso!",
                            "Sucesso",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        // Atualiza lista de templates
                        //CarregarTemplatesDisponiveis();
                    }
                }
            }
        }



        // ========================================

        // ⭐ MODIFICADO: IMPRIMIR COM CONFIGURAÇÃO

        // ========================================

        //private void btnImprimir_Click(object sender, EventArgs e)

        //{

        //    var produtosSelecionados = ObterProdutosSelecionados();

        //    if (produtosSelecionados.Count == 0)

        //    {

        //        MessageBox.Show("Selecione pelo menos um produto!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //        return;

        //    }



        //    if (template.Elementos.Count == 0)

        //    {

        //        MessageBox.Show("Configure o template primeiro usando o Designer!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //        return;

        //    }



        //    // ⭐ VERIFICA SE HÁ CONFIGURAÇÃO

        //    if (configuracaoAtual == null)

        //    {

        //        var resultado = MessageBox.Show(

        //            "Nenhuma configuração de impressão foi definida.\n\n" +

        //            "Deseja configurar agora?",

        //            "Configuração Necessária",

        //            MessageBoxButtons.YesNo,

        //            MessageBoxIcon.Question);



        //        if (resultado == DialogResult.Yes)

        //        {

        //            btnConfigPapel_Click(sender, e);

        //            return;

        //        }

        //        else

        //        {

        //            return;

        //        }



        //    }



        //    //// ⭐ PASSA A CONFIGURAÇÃO PARA O FORM DE IMPRESSÃO

        //    var formImpressao = new FormImpressao(produtosSelecionados, template, configuracaoAtual);

        //    formImpressao.ShowDialog();

        //}



        private void btnImprimir_Click(object sender, EventArgs e)
        {
            // 1. OBTÉM OS PRODUTOS SELECIONADOS
            var produtosSelecionados = ObterProdutosSelecionados();
            if (produtosSelecionados.Count == 0)
            {
                MessageBox.Show("Selecione pelo menos um produto!", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. ABRE O DIÁLOGO DE SELEÇÃO DE IMPRESSÃO
            using (var formSelecao = new FormSelecaoImpressao())
            {
                if (formSelecao.ShowDialog() == DialogResult.OK)
                {
                    string nomeTemplateSelecionado = formSelecao.TemplateSelecionado;
                    ConfiguracaoEtiqueta configSelecionada = formSelecao.ConfiguracaoSelecionada;

                    // 3. CARREGA O TEMPLATE SELECIONADO
                    TemplateEtiqueta templateAtual = TemplateManager.CarregarTemplate(nomeTemplateSelecionado);

                    if (templateAtual == null)
                    {
                        MessageBox.Show($"Falha ao carregar o template: {nomeTemplateSelecionado}",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 4. VALIDA TEMPLATE
                    if (templateAtual.Elementos.Count == 0)
                    {
                        MessageBox.Show("O template selecionado não possui elementos configurados!\n\n" +
                                       "Configure-o primeiro usando o Designer.",
                                       "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 5. VALIDA CONFIGURAÇÃO
                    if (configSelecionada == null)
                    {
                        MessageBox.Show("Erro ao carregar configuração de impressão!",
                            "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 6. ATUALIZA DIMENSÕES DO TEMPLATE COM A CONFIGURAÇÃO
                    templateAtual.Largura = configSelecionada.LarguraEtiqueta;
                    templateAtual.Altura = configSelecionada.AlturaEtiqueta;

                    // 7. ATUALIZA CONFIGURAÇÃO ATUAL DO FORM
                    configuracaoAtual = configSelecionada;
                    template = templateAtual;

                    // 8. SALVA COMO CONFIGURAÇÃO PADRÃO
                    GerenciadorConfiguracoesEtiqueta.SalvarConfiguracaoPadrao(configSelecionada);

                    // 9. ABRE O FORM DE IMPRESSÃO
                    using (var formImpressao = new FormImpressao(produtosSelecionados, templateAtual, configSelecionada))
                    {
                        formImpressao.ShowDialog();
                    }
                }
            }
        }







        private void dgvProdutos_CellContentClick(object sender, DataGridViewCellEventArgs e)

        {

            if (e.RowIndex >= 0 && dgvProdutos.Columns[e.ColumnIndex].Name == "colRemover")

            {

                if (MessageBox.Show("Deseja remover este produto?", "Confirmar",

                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)

                {

                    produtos.RemoveAt(e.RowIndex);

                    dgvProdutos.Rows.RemoveAt(e.RowIndex);

                }

            }

        }

        private void dgvProdutos_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // 1. Verificar se a edição ocorreu na coluna de Quantidade
            if (dgvProdutos.Columns[e.ColumnIndex].Name == "Qtde") // Assumindo que o nome da sua coluna é "colQuantidade"
            {
                // 2. Tentar obter o novo valor da célula
                object cellValue = dgvProdutos.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                if (cellValue != null && int.TryParse(cellValue.ToString(), out int novaQuantidade))
                {
                    // 3. Validar a nova quantidade
                    if (novaQuantidade > 0)
                    {
                        // 4. Atualizar a lista subjacente
                        if (e.RowIndex < produtos.Count)
                        {
                            produtos[e.RowIndex].Quantidade = novaQuantidade;

                            // 5. (OPCIONAL) Recalcular totais e atualizar a tela, se necessário.
                            // Ex: AtualizarTotalDaLista(); 
                        }
                    }
                    else
                    {
                        // Se a quantidade for zero ou negativa, você pode removê-lo ou reverter
                        // Neste exemplo, vamos reverter para o valor anterior e remover se for zero:
                        if (novaQuantidade <= 0)
                        {
                            produtos.RemoveAt(e.RowIndex);
                            dgvProdutos.Rows.RemoveAt(e.RowIndex);
                            MessageBox.Show("Produto removido, pois a quantidade foi definida para zero.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    // Reverte para o valor antigo se a entrada não for numérica
                    MessageBox.Show("Por favor, insira um número inteiro válido para a quantidade.", "Erro de Entrada", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    // Você pode forçar a célula a reverter o valor aqui se necessário.
                }
            }
        }



        private List<Produto> ObterProdutosSelecionados()

        {

            var selecionados = new List<Produto>();



            for (int i = 0; i < dgvProdutos.Rows.Count; i++)

            {

                if (Convert.ToBoolean(dgvProdutos.Rows[i].Cells["colSelecionar"].Value))

                {

                    selecionados.Add(produtos[i]);

                }

            }



            return selecionados;

        }



        protected override void OnPaintBackground(PaintEventArgs e)

        {

            base.OnPaintBackground(e);



            using (LinearGradientBrush brush = new LinearGradientBrush(

                this.ClientRectangle,

                Color.White,

                Color.White,

                LinearGradientMode.Vertical))

            {

                ColorBlend blend = new ColorBlend();

                blend.Positions = new float[] { 0.0f, 0.85f, 1.0f };

                blend.Colors = new Color[] {

                    Color.FromArgb(240, 235, 255),

                    Color.FromArgb(240, 235, 255),

                    Color.FromArgb(255, 255, 200, 50)

                };



                brush.InterpolationColors = blend;

                e.Graphics.FillRectangle(brush, this.ClientRectangle);

            }

        }



        public static void ArredondarBotao(Button botao, int raio)

        {

            GraphicsPath path = new GraphicsPath();

            Rectangle rect = botao.ClientRectangle;



            int d = raio * 2;



            path.AddArc(rect.X, rect.Y, d, d, 180, 90);

            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);

            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);

            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);

            path.CloseFigure();



            botao.Region = new Region(path);

        }



        private void btnCarregarTemplate_Click(object sender, EventArgs e)

        {

            var formLista = new FormListaTemplates();

            if (formLista.ShowDialog() == DialogResult.OK)

            {

                string nomeTemplate = formLista.TemplateSelecionado;



                var templateCarregado = TemplateManager.CarregarTemplate(nomeTemplate);

                if (templateCarregado != null)

                {

                    template = templateCarregado;

                    MessageBox.Show($"Template '{nomeTemplate}' carregado com sucesso!",

                        "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }

            }

        }



        // ========================================

        // ⭐ MODIFICADO: CONFIGURAR PAPEL

        // ========================================

        private void btnConfigPapel_Click(object sender, EventArgs e)

        {

            ConfiguracaoPapel papelParaAbrir = null;



            // 1. Abre o Menu de Configuração (NOVO ou CARREGAR)

            using (var formMenu = new FormMenuConfiguracao())

            {

                var escolha = formMenu.ShowDialog(this);



                if (escolha == DialogResult.Cancel)

                    return;



                if (escolha == DialogResult.Yes) // NOVO

                {

                    // Cria nova configuração baseada na atual ou padrão

                    var configBase = configuracaoAtual ?? new ConfiguracaoEtiqueta

                    {

                        NomeEtiqueta = "Nova Configuração",

                        ImpressoraPadrao = "BTP-L42(D)",

                        LarguraEtiqueta = 100,

                        AlturaEtiqueta = 30,

                        NumColunas = 1,

                        NumLinhas = 1

                    };



                    papelParaAbrir = GerenciadorConfiguracoesEtiqueta.ConverterConfigParaPapel(configBase);

                    papelParaAbrir.NomePapel = "Nova Configuração";

                }

                else if (escolha == DialogResult.No) // CARREGAR

                {

                    using (var formListaConfig = new FormListaConfiguracoes())

                    {

                        if (formListaConfig.ShowDialog(this) == DialogResult.OK)

                        {

                            string nomeConfig = formListaConfig.ConfiguracaoSelecionada;

                            // Certifique-se de que CarregarConfiguracao retorna ConfiguracaoPapel ou trate o retorno.

                            papelParaAbrir = GerenciadorConfiguracoesEtiqueta.CarregarConfiguracao(nomeConfig);

                        }

                        else

                        {

                            return;

                        }

                    }

                }

            }



            // ⭐ PASSO 2 (CORREÇÃO): ABRIR FormConfigEtiqueta SE UMA CONFIGURAÇÃO FOI SELECIONADA/CRIADA

            if (papelParaAbrir != null)

            {

                // Cria a Configuração Etiqueta para edição (FormConfigEtiqueta trabalha com ConfiguracaoEtiqueta)

                // OBS: Você pode precisar de uma função para converter ConfiguracaoPapel de volta para ConfiguracaoEtiqueta

                // ou adaptar FormConfigEtiqueta para receber ConfiguracaoPapel e carregar seus campos.



                // Assumindo que você tem uma função para carregar ConfigEtiqueta baseada em ConfigPapel

                // Usarei a configuração atual como base para a impressora.

                ConfiguracaoEtiqueta configParaEditar = GerenciadorConfiguracoesEtiqueta.ConverterPapelParaConfig(

                    papelParaAbrir, configuracaoAtual?.ImpressoraPadrao ?? "BTP-L42(D)");



                using (var formConfig = new FormConfigEtiqueta(configParaEditar))

                {

                    if (formConfig.ShowDialog() == DialogResult.OK)

                    {

                        // Configuração foi salva (verifiquei que formConfig.ShowDialog() == DialogResult.OK 

                        // após o salvamento em FormConfigEtiqueta)



                        configuracaoAtual = formConfig.Configuracao;



                        // Atualiza o template com as novas dimensões

                        template.Largura = configuracaoAtual.LarguraEtiqueta;

                        template.Altura = configuracaoAtual.AlturaEtiqueta;



                        // Salva como configuração padrão (última usada)

                        GerenciadorConfiguracoesEtiqueta.SalvarConfiguracaoPadrao(configuracaoAtual);


                        // Tenta selecionar a configuração que acabou de ser salva/aplicada no ComboBox

                        if (!string.IsNullOrEmpty(configuracaoAtual.PapelPadrao))

                        {

                            // Se o seu método SelecionarConfiguracaoNaLista existir, use-o

                            // Exemplo: SelecionarConfiguracaoNaLista(configuracaoAtual.PapelPadrao); 

                            // Se não, AtualizarListaConfiguracoesAposSalvar já deve ter selecionado a padrão.

                        }



                        MessageBox.Show($"✅ Configuração de etiqueta aplicada com sucesso!\n\n" +

                            $"📏 Dimensões: {configuracaoAtual.LarguraEtiqueta} x {configuracaoAtual.AlturaEtiqueta} mm\n" +

                            $"📐 Layout: {configuracaoAtual.NumColunas} coluna(s) x {configuracaoAtual.NumLinhas} linha(s)\n" +

                            $"🖨️ Impressora: {configuracaoAtual.ImpressoraPadrao}",

                            "Configuração Aplicada", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    }

                }

            }

        }



        // ========================================

        // ⭐ CLASSE AUXILIAR PARA ITENS DO COMBOBOX

        // ========================================

        private class ConfiguracaoItem

        {

            public string Nome { get; set; }

            public ConfiguracaoEtiqueta Configuracao { get; set; }

            public bool IsPadrao { get; set; }



            public override string ToString()

            {

                return Nome;

            }

        }

        private void ConfigurarBuscaMercadoria()

        {

            // 1. Configurar Timer para delay na busca

            timerBusca = new Timer();

            timerBusca.Interval = 300; // 300ms de delay

            timerBusca.Tick += TimerBusca_Tick;



            // 2. Configurar ComboBoxes

            // Assumindo que os ComboBoxes se chamam: cmbBuscaNome, cmbBuscaReferencia, cmbBuscaCodigo



            // Configuração comum para todos os ComboBoxes (AutoCompleteSource deve ser CustomSource)

            Action<ComboBox> setupComboBox = (cmb) =>

            {

                if (cmb != null)

                {

                    cmb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;

                    cmb.AutoCompleteSource = AutoCompleteSource.CustomSource;

                    cmb.DropDownStyle = ComboBoxStyle.DropDown;

                    cmb.TextUpdate += cmbBusca_TextUpdate;

                }

            };



            setupComboBox(cmbBuscaNome);

            setupComboBox(cmbBuscaReferencia);

            setupComboBox(cmbBuscaCodigo);



            // Adicionar handlers de seleção

            if (cmbBuscaNome != null) cmbBuscaNome.SelectedIndexChanged += cmbBuscaNome_SelectedIndexChanged;

            if (cmbBuscaReferencia != null) cmbBuscaReferencia.SelectedIndexChanged += cmbBuscaReferencia_SelectedIndexChanged;

            if (cmbBuscaCodigo != null) cmbBuscaCodigo.SelectedIndexChanged += cmbBuscaCodigo_SelectedIndexChanged;

        }

        private void CarregarTodasMercadorias()
        {
            // ⭐ OTIMIZAÇÃO: Evita carregamento duplicado
            if (mercadoriasCarregadas)
            {
                return; // Já foi carregado, não precisa recarregar
            }

            try
            {
                // ⭐ Carrega TODOS os produtos (ou aumenta muito o limite)
                mercadorias = LocalDatabaseManager.BuscarMercadorias("", limite: 100000);

                // Listas para AutoComplete E para Items
                AutoCompleteStringCollection acscNome = new AutoCompleteStringCollection();
                AutoCompleteStringCollection acscReferencia = new AutoCompleteStringCollection();
                AutoCompleteStringCollection acscCodigo = new AutoCompleteStringCollection();

                List<string> listaNome = new List<string>();
                List<string> listaReferencia = new List<string>();
                List<string> listaCodigo = new List<string>();

                foreach (DataRow row in mercadorias.Rows)
                {
                    string nome = row["Mercadoria"]?.ToString();
                    string referencia = row["CodFabricante"]?.ToString();
                    string codigo;
                    if (isConfeccao)
                    {
                        codigo = row["CodBarras_Grade"]?.ToString();
                    }
                    else
                    {
                        codigo = row["CodigoMercadoria"]?.ToString();
                    }


                    if (!string.IsNullOrEmpty(nome))
                    {
                        acscNome.Add(nome);
                        listaNome.Add(nome);
                    }
                    if (!string.IsNullOrEmpty(referencia))
                    {
                        acscReferencia.Add(referencia);
                        listaReferencia.Add(referencia);
                    }
                    if (!string.IsNullOrEmpty(codigo))
                    {
                        acscCodigo.Add(codigo);
                        listaCodigo.Add(codigo);
                    }
                }

                // Configura AutoComplete
                if (cmbBuscaNome != null) cmbBuscaNome.AutoCompleteCustomSource = acscNome;
                if (cmbBuscaReferencia != null) cmbBuscaReferencia.AutoCompleteCustomSource = acscReferencia;
                if (cmbBuscaCodigo != null) cmbBuscaCodigo.AutoCompleteCustomSource = acscCodigo;

                // ⭐ AGORA SIM: Popular os Items para o dropdown funcionar
                if (cmbBuscaNome != null)
                {
                    cmbBuscaNome.Items.Clear();
                    cmbBuscaNome.Items.AddRange(listaNome.Distinct().OrderBy(s => s).ToArray());
                }
                if (cmbBuscaReferencia != null)
                {
                    cmbBuscaReferencia.Items.Clear();
                    cmbBuscaReferencia.Items.AddRange(listaReferencia.Distinct().OrderBy(s => s).ToArray());
                }
                if (cmbBuscaCodigo != null)
                {
                    cmbBuscaCodigo.Items.Clear();
                    cmbBuscaCodigo.Items.AddRange(listaCodigo.Distinct().OrderBy(s => s).ToArray());
                }


                // ⭐ Marca como carregado com sucesso
                mercadoriasCarregadas = true;
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar lista de mercadorias: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cmbBusca_TextUpdate(object sender, EventArgs e)

        {

            // Inicia/Reinicia o timer a cada tecla digitada

            timerBusca.Stop();

            timerBusca.Start();

        }

        private void TimerBusca_Tick(object sender, EventArgs e)

        {

            // O timer serve apenas para dar tempo do AutoComplete agir.

            timerBusca.Stop();

        }

        private void cmbBuscaNome_SelectedIndexChanged(object sender, EventArgs e)

        {

            if (cmbBuscaNome.SelectedIndex != -1)
                PesquisaCodigo = false;
            {

                string termoSelecionado = cmbBuscaNome.SelectedItem.ToString();

                // ⭐ PASSANDO O COMBOBOX DE ORIGEM

                AdicionarProdutoSelecionado(termoSelecionado, "Mercadoria", cmbBuscaNome);

            }

        }

        private void cmbBuscaReferencia_SelectedIndexChanged(object sender, EventArgs e)

        {

            if (cmbBuscaReferencia.SelectedIndex != -1)
                PesquisaCodigo = false;
            {

                string termoSelecionado = cmbBuscaReferencia.SelectedItem.ToString();

                // ⭐ PASSANDO O COMBOBOX DE ORIGEM

                AdicionarProdutoSelecionado(termoSelecionado, "CodFabricante", cmbBuscaReferencia);

            }

        }

        private void cmbBuscaCodigo_SelectedIndexChanged(object sender, EventArgs e)

        {

            if (cmbBuscaCodigo.SelectedIndex != -1)
                PesquisaCodigo = true;
            {

                string termoSelecionado = cmbBuscaCodigo.SelectedItem.ToString();

                // ⭐ PASSANDO O COMBOBOX DE ORIGEM
                if (isConfeccao)
                {
                    AdicionarProdutoSelecionado(termoSelecionado, "CodBarras_Grade", cmbBuscaCodigo);
                }
                else
                {
                    AdicionarProdutoSelecionado(termoSelecionado, "CodigoMercadoria", cmbBuscaCodigo);
                }



            }

        }

        // Em FormPrincipal.cs



        private void AdicionarProdutoSelecionado(string termo, string nomeCampo, ComboBox cmbOrigem)
        {
            if (string.IsNullOrEmpty(termo)) return;

            // Remove os eventos para evitar recursão
            RemoverEventosSelecao();

            try
            {
                string termoFiltrado = termo.Replace("'", "''");
                LocalDatabaseManager.isConfeccao = isConfeccao;


                // ⭐ CORREÇÃO 1: Busca primeiro na memória (rápido)
                DataRow[] resultados = mercadorias.Select($"{nomeCampo} = '{termoFiltrado}'");
                DataRow row = null;

                if (resultados.Length > 0)
                {
                    row = resultados[0];
                }
                else
                {
                    // ⭐ CORREÇÃO 2: Busca DIRETO NO BANCO usando método existente
                    try
                    {
                        // Usa o método existente BuscarMercadorias(termo, nomeCampo)
                        // que já faz busca LIKE no campo específico
                        DataTable resultadoBanco = LocalDatabaseManager.BuscarMercadorias(termo, nomeCampo, limite: 10);

                        if (resultadoBanco != null && resultadoBanco.Rows.Count > 0)
                        {
                            // Tenta busca exata primeiro (LIKE pode retornar múltiplos)
                            DataRow[] resultadosExatos = resultadoBanco.Select($"{nomeCampo} = '{termoFiltrado}'");

                            if (resultadosExatos.Length > 0)
                            {
                                row = resultadosExatos[0]; // Busca exata prioritária
                            }
                            else
                            {
                                row = resultadoBanco.Rows[0]; // Se não houver exata, pega primeira
                            }
                        }
                    }
                    catch (Exception exBanco)
                    {
                        // Log do erro para debug
                        System.Diagnostics.Debug.WriteLine($"Erro na busca no banco: {exBanco.Message}");
                    }
                }

                if (row != null)
                {
                    // Armazena o DataRow completo para uso no btnAdicionar
                    produtoAtualCompleto = row;

                    // Obter todos os campos da tabela Mercadorias
                    string codigo = row["CodigoMercadoria"]?.ToString();
                    string nome = row["Mercadoria"]?.ToString();
                    string referencia = row["CodFabricante"]?.ToString();
                    string codBarras = row["CodBarras"]?.ToString();
                    string codBarras_grade = row["CodBarras_Grade"]?.ToString();
                    decimal preco = row["PrecoVenda"] != DBNull.Value ? Convert.ToDecimal(row["PrecoVenda"]) : 0m;

                    // Campos de preços alternativos
                    decimal vendaA = row["VendaA"] != DBNull.Value ? Convert.ToDecimal(row["VendaA"]) : 0m;
                    decimal vendaB = row["VendaB"] != DBNull.Value ? Convert.ToDecimal(row["VendaB"]) : 0m;
                    decimal vendaC = row["VendaC"] != DBNull.Value ? Convert.ToDecimal(row["VendaC"]) : 0m;
                    decimal vendaD = row["VendaD"] != DBNull.Value ? Convert.ToDecimal(row["VendaD"]) : 0m;
                    decimal vendaE = row["VendaE"] != DBNull.Value ? Convert.ToDecimal(row["VendaE"]) : 0m;

                    // Campos de informação
                    string fornecedor = row["Fornecedor"]?.ToString();
                    string fabricante = row["Fabricante"]?.ToString();
                    string grupo = row["Grupo"]?.ToString();
                    string prateleira = row["Prateleira"]?.ToString();
                    string garantia = row["Garantia"]?.ToString();
                    string tam = row["Tam"]?.ToString();
                    string cores = row["Cores"]?.ToString();


                    // Sincronizar os ComboBoxes
                    cmbBuscaNome.Text = nome;
                    cmbBuscaReferencia.Text = referencia;

                    if (isConfeccao)
                    {
                        cmbBuscaCodigo.Text = codBarras_grade;
                        cmbTamanho.Text = tam;
                        cmbCor.Text = cores;

                    }
                    else
                    {
                        cmbBuscaCodigo.Text = codigo;
                    }


                    // Preencher campos de cadastro
                    txtNome.Text = nome;
                    txtCodigo.Text = codigo;
                    txtPreco.Text = preco.ToString("F2");

                    numQtd.Value = 1;

                    // ⭐ CONFECÇÃO: Carregar tamanhos e cores do produto
                    if (isConfeccao && !string.IsNullOrEmpty(codigo))
                    {
                        if (PesquisaCodigo != true)
                        {
                            CarregarTamanhosECores(codigo);

                        }
                        ;


                    }
                }
                else
                {
                    MessageBox.Show($"Nenhum produto encontrado com o valor '{termo}' no campo '{nomeCampo}'.",
                        "Busca Vazia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao processar o produto: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Adiciona os eventos de volta
                AdicionarEventosSelecao();
            }
        }

        private void AdicionarProdutoNaLista(Produto produto)

        {

            // Implementação Placeholder: Substitua pela sua lógica real de adição ao DataGridView.

            // O ideal é adicionar à lista 'produtos' e redefinir o DataSource do dgvProdutos.



            // 1. Adicionar à lista interna

            produtos.Add(produto);



            // 2. Atualizar o DataGridView (assumindo que o controle se chama dgvProdutos)

            // Se você usar BindingSource, a atualização é automática. Caso contrário:

            dgvProdutos.DataSource = null;

            dgvProdutos.DataSource = produtos;



            // ... (Atualizar resumo/total)

        }

        private void RemoverEventosSelecao()

        {

            if (cmbBuscaNome != null) cmbBuscaNome.SelectedIndexChanged -= cmbBuscaNome_SelectedIndexChanged;

            if (cmbBuscaReferencia != null) cmbBuscaReferencia.SelectedIndexChanged -= cmbBuscaReferencia_SelectedIndexChanged;

            if (cmbBuscaCodigo != null) cmbBuscaCodigo.SelectedIndexChanged -= cmbBuscaCodigo_SelectedIndexChanged;

        }

        private void AdicionarEventosSelecao()

        {

            if (cmbBuscaNome != null) cmbBuscaNome.SelectedIndexChanged += cmbBuscaNome_SelectedIndexChanged;

            if (cmbBuscaReferencia != null) cmbBuscaReferencia.SelectedIndexChanged += cmbBuscaReferencia_SelectedIndexChanged;

            if (cmbBuscaCodigo != null) cmbBuscaCodigo.SelectedIndexChanged += cmbBuscaCodigo_SelectedIndexChanged;

        }



        private void BtnAdicionar2_Click(object sender, EventArgs e)

        {

            AdicionarProdutoPelaBusca();

        }

        private void AdicionarProdutoPelaBusca()

        {

            // Lógica de Validação (Reutilizada da resposta anterior)

            if (string.IsNullOrWhiteSpace(txtNome.Text) || string.IsNullOrWhiteSpace(txtCodigo.Text))

            {

                MessageBox.Show("Nome e Código são obrigatórios!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;

            }



            decimal precoDecimal;

            // O CultureInfo.InvariantCulture e Replace(",", ".") garantem que o preço seja lido corretamente

            if (!decimal.TryParse(txtPreco.Text.Replace(",", "."), System.Globalization.NumberStyles.Any,

                System.Globalization.CultureInfo.InvariantCulture, out precoDecimal))

            {

                MessageBox.Show("Preço inválido!", "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;

            }





            // Criação do objeto Produto
            var produto = new Produto
            {
                Nome = txtNome.Text,
                Codigo = txtCodigo.Text,
                Preco = precoDecimal,
                Quantidade = (int)numQtd.Value
            };

            // ⭐ OTIMIZADO: Usa o DataRow armazenado se disponível (produto foi buscado)
            if (produtoAtualCompleto != null)
            {
                try
                {
                    // Popula todos os campos adicionais do DataRow já carregado
                    produto.CodFabricante = produtoAtualCompleto["CodFabricante"]?.ToString();
                    produto.CodBarras = produtoAtualCompleto["CodBarras"]?.ToString();
                    produto.CodBarras_Grade = produtoAtualCompleto["CodBarras_Grade"]?.ToString();
                    produto.PrecoVenda = produtoAtualCompleto["PrecoVenda"] != DBNull.Value
                        ? Convert.ToDecimal(produtoAtualCompleto["PrecoVenda"])
                        : precoDecimal;
                    produto.VendaA = produtoAtualCompleto["VendaA"] != DBNull.Value
                        ? Convert.ToDecimal(produtoAtualCompleto["VendaA"])
                        : 0m;
                    produto.VendaB = produtoAtualCompleto["VendaB"] != DBNull.Value
                        ? Convert.ToDecimal(produtoAtualCompleto["VendaB"])
                        : 0m;
                    produto.VendaC = produtoAtualCompleto["VendaC"] != DBNull.Value
                        ? Convert.ToDecimal(produtoAtualCompleto["VendaC"])
                        : 0m;
                    produto.VendaD = produtoAtualCompleto["VendaD"] != DBNull.Value
                        ? Convert.ToDecimal(produtoAtualCompleto["VendaD"])
                        : 0m;
                    produto.VendaE = produtoAtualCompleto["VendaE"] != DBNull.Value
                        ? Convert.ToDecimal(produtoAtualCompleto["VendaE"])
                        : 0m;
                    produto.Fornecedor = produtoAtualCompleto["Fornecedor"]?.ToString();
                    produto.Fabricante = produtoAtualCompleto["Fabricante"]?.ToString();
                    produto.Grupo = produtoAtualCompleto["Grupo"]?.ToString();
                    produto.Prateleira = produtoAtualCompleto["Prateleira"]?.ToString();
                    produto.Garantia = produtoAtualCompleto["Garantia"]?.ToString();
                    produto.Tam = produtoAtualCompleto["Tam"]?.ToString();
                    produto.Cores = produtoAtualCompleto["Cores"]?.ToString();

                    // ⭐ CONFECÇÃO: Sobrescreve Tam e Cor com os valores selecionados nas combos
                    if (isConfeccao && cmbTamanho != null && cmbCor != null)
                    {
                        produto.Tam = cmbTamanho.SelectedItem?.ToString() ?? produto.Tam ?? "";
                        produto.Cores = cmbCor.SelectedItem?.ToString() ?? produto.Cores ?? "";
                    }
                }
                catch
                {
                    // Se falhar ao ler campos adicionais, continua com dados básicos
                }
            }
            else if (mercadorias != null)
            {
                // ⭐ FALLBACK: Se não houver DataRow armazenado, tenta buscar (produto digitado manualmente)
                try
                {
                    DataRow[] resultados;
                    string codigoFiltrado = txtCodigo.Text.Replace("'", "''");
                    if (isConfeccao)
                    {
                        resultados = mercadorias.Select($"CodBarras_Grade = '{codigoFiltrado}'");
                    }
                    else
                    {
                        resultados = mercadorias.Select($"CodigoMercadoria = '{codigoFiltrado}'");
                    }


                    if (resultados.Length > 0)
                    {
                        DataRow row = resultados[0];

                        // Popula todos os campos adicionais do banco
                        produto.CodFabricante = row["CodFabricante"]?.ToString();
                        produto.CodBarras = row["CodBarras"]?.ToString();
                        produto.CodBarras_Grade = row["CodBarras_Grade"]?.ToString();
                        produto.PrecoVenda = row["PrecoVenda"] != DBNull.Value
                            ? Convert.ToDecimal(row["PrecoVenda"])
                            : precoDecimal;
                        produto.VendaA = row["VendaA"] != DBNull.Value
                            ? Convert.ToDecimal(row["VendaA"])
                            : 0m;
                        produto.VendaB = row["VendaB"] != DBNull.Value
                            ? Convert.ToDecimal(row["VendaB"])
                            : 0m;
                        produto.VendaC = row["VendaC"] != DBNull.Value
                            ? Convert.ToDecimal(row["VendaC"])
                            : 0m;
                        produto.VendaD = row["VendaD"] != DBNull.Value
                            ? Convert.ToDecimal(row["VendaD"])
                            : 0m;
                        produto.VendaE = row["VendaE"] != DBNull.Value
                            ? Convert.ToDecimal(row["VendaE"])
                            : 0m;
                        produto.Fornecedor = row["Fornecedor"]?.ToString();
                        produto.Fabricante = row["Fabricante"]?.ToString();
                        produto.Grupo = row["Grupo"]?.ToString();
                        produto.Prateleira = row["Prateleira"]?.ToString();
                        produto.Garantia = row["Garantia"]?.ToString();
                        produto.Tam = row["Tam"]?.ToString();
                        produto.Cores = row["Cores"]?.ToString();

                        // ⭐ CONFECÇÃO: Sobrescreve Tam e Cor com os valores selecionados nas combos
                        if (isConfeccao && cmbTamanho != null && cmbCor != null)
                        {
                            produto.Tam = cmbTamanho.SelectedItem?.ToString() ?? produto.Tam ?? "";
                            produto.Cores = cmbCor.SelectedItem?.ToString() ?? produto.Cores ?? "";
                        }
                    }
                }
                catch
                {
                    // Se falhar a busca, usa apenas os dados básicos já preenchidos
                }
            }

            // Adiciona o produto à lista e ao DataGridView
            produtos.Add(produto);

            // ⭐ CONFECÇÃO: Inclui colunas Tam e Cor se o módulo for CONFECÇÃO
            if (isConfeccao && dgvProdutos.Columns.Contains("colTam") && dgvProdutos.Columns.Contains("colCor"))
            {
                dgvProdutos.Rows.Add(false, produto.Nome, produto.CodBarras_Grade, produto.Preco.ToString("C2"),
                    produto.Quantidade, produto.Tam ?? "", produto.Cores ?? "");
            }
            else
            {
                dgvProdutos.Rows.Add(false, produto.Nome, produto.Codigo, produto.Preco.ToString("C2"), produto.Quantidade);
            }

            // ⭐ Limpar DataRow armazenado após adicionar
            produtoAtualCompleto = null;


            // Limpeza dos campos de cadastro manual

            txtNome.Clear();

            txtCodigo.Clear();

            txtPreco.Clear();


            numQtd.Value = 1;



            // Limpeza das ComboBoxes de busca (⭐ Essencial para que a busca funcione para o próximo item)

            if (cmbBuscaNome != null) cmbBuscaNome.Text = "";

            if (cmbBuscaReferencia != null) cmbBuscaReferencia.Text = "";

            if (cmbBuscaCodigo != null) cmbBuscaCodigo.Text = "";

            // ⭐ CONFECÇÃO: Limpar combos de Tam e Cor
            if (isConfeccao)
            {
                if (cmbTamanho != null) cmbTamanho.Text = "";
                if (cmbCor != null) cmbCor.Text = "";
            }



            // Foco para o próximo item

            cmbBuscaNome.Focus(); // ou o campo que você deseja que comece a próxima busca

        }

        private void ComboBoxBusca_KeyDown(object sender, KeyEventArgs e)

        {

            ComboBox cmb = (ComboBox)sender;



            if (e.KeyCode == Keys.Enter)

            {

                // 1. Bloqueia a propagação imediata do Enter

                e.Handled = true;

                e.SuppressKeyPress = true;



                string nomeCampo = GetNomeCampoBusca(cmb);

                if (nomeCampo == null) return;



                // Pega o texto atual (parcial ou completo) digitado pelo usuário.

                string termoDigitado = cmb.Text.Trim();

                string termoCompleto = termoDigitado; // Valor padrão para o caso de falha na busca



                if (string.IsNullOrWhiteSpace(termoDigitado)) return;



                // 2. FORÇA A FINALIZAÇÃO DO AUTOCOMPLETE (Ainda importante para atualizar índices)

                if (cmb.DroppedDown)

                {

                    cmb.DroppedDown = false;

                    Application.DoEvents(); // Força o processamento de eventos pendentes

                }



                // 3. TENTA PEGAR O NOME COMPLETO PELO SelectedItem

                if (cmb.SelectedIndex >= 0 && cmb.SelectedItem != null)

                {

                    // Tenta pegar a string completa do item que foi selecionado

                    termoCompleto = cmb.GetItemText(cmb.SelectedItem);

                }

                else

                {

                    // 4. BUSCA MANUALMENTE O NOME COMPLETO NA LISTA (A CHAVE DA CORREÇÃO)

                    // Itera sobre todos os itens e procura por um que comece com o que o usuário digitou.

                    foreach (object item in cmb.Items)

                    {

                        string itemText = cmb.GetItemText(item);



                        // Compara se o item completo da lista começa com o texto digitado

                        if (itemText.StartsWith(termoDigitado, StringComparison.OrdinalIgnoreCase))

                        {

                            // Encontramos o termo completo e correto (Ex: "Fone de Ouvido GameNote (s/fio)")

                            termoCompleto = itemText;

                            break;

                        }

                    }

                }



                // 5. ATUALIZA O TEXTO VISUAL DO COMBOBOX PARA O NOME COMPLETO

                // Isso resolve o problema de visualização truncada (opcional, mas recomendado).

                cmb.Text = termoCompleto;



                // 6. EXECUTA A LÓGICA DE SELEÇÃO com o termo garantido

                AdicionarProdutoSelecionado(termoCompleto, nomeCampo, cmb);



                // Move o foco para a quantidade ou próximo campo

                numQtd.Focus();

                numQtd.Select(0, numQtd.Text.Length);

            }

        }

        private string GetNomeCampoBusca(ComboBox cmb)

        {

            if (cmb == cmbBuscaNome) return "Mercadoria";

            if (cmb == cmbBuscaReferencia) return "CodFabricante";

            if (isConfeccao)
            {
                if (cmb == cmbBuscaCodigo) return "CodBarras_Grade";
            }
            else
            {
                if (cmb == cmbBuscaCodigo) return "CodigoMercadoria";
            }


            return null;

        }



        private void pictureBox2_Click(object sender, EventArgs e)

        {

            try

            {

                if (MessageBox.Show(

                    "Deseja sincronizar as mercadorias do SQL Server?\n\n" +

                    "Isso pode levar alguns minutos dependendo da quantidade de registros.",

                    "Confirmar Sincronização",

                    MessageBoxButtons.YesNo,

                    MessageBoxIcon.Question) != DialogResult.Yes)

                {

                    return;

                }



                Cursor = Cursors.WaitCursor;

                pictureBox2.Enabled = false;

                //pictureBox2.Text = "Sincronizando...";



                // 1. SINCRONIZAR O BANCO DE DADOS LOCAL

                int total = LocalDatabaseManager.SincronizarMercadorias();



                // ⭐ 2. RECARREGAR AS MERCADORIAS NA MEMÓRIA E ATUALIZAR OS COMBOBOXES


                // ⭐ OTIMIZAÇÃO: Reseta flag para forçar recarregamento após sincronização manual
                mercadoriasCarregadas = false;

                CarregarTodasMercadorias();



                // Se a linha a seguir for para atualizar status no rodapé, mantenha-a.

                //CarregarEstatisticas();



                Cursor = Cursors.Default;

                pictureBox2.Enabled = true;

                //pictureBox2.Text = "🔄 Sincronizar";



                MessageBox.Show(

                    $"Sincronização concluída com sucesso!\n\n" +

                    $"Total de mercadorias importadas: {total:N0}",

                    "Sucesso",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Information);





            }

            catch (Exception ex)

            {

                Cursor = Cursors.Default;

                pictureBox2.Enabled = true;

                //btnSincronizar.Text = "🔄 Sincronizar";



                MessageBox.Show(

                    $"Erro ao sincronizar:\n\n{ex.Message}",

                    "Erro",

                    MessageBoxButtons.OK,

                    MessageBoxIcon.Error);

            }

        }



        private void CarregarConfiguracoesPapel()

        {

            // 1. Usa o Gerenciador para listar os nomes

            List<string> nomesConfig = GerenciadorConfiguracoesEtiqueta.ListarNomesConfiguracoes();


            // 2. Tenta selecionar a última configuração salva como padrão




        }



        /// <summary>

        /// Procura e seleciona um nome de configuração no ComboBox.

        /// </summary>





        /// <summary>

        /// Carrega o objeto de configuração completo quando o usuário seleciona um item no ComboBox.

        /// </summary>



        private ConfiguracaoEtiqueta CarregarConfiguracaoAtual()

        {

            if (File.Exists(CAMINHO_CONFIGURACOES))

            {

                try

                {

                    XmlSerializer serializer = new XmlSerializer(typeof(ConfiguracaoEtiqueta));

                    using (StreamReader reader = new StreamReader(CAMINHO_CONFIGURACOES))

                    {

                        return (ConfiguracaoEtiqueta)serializer.Deserialize(reader);

                    }

                }

                catch (Exception ex)

                {

                    MessageBox.Show($"Erro ao carregar configuração salva: {ex.Message}",

                                    "Erro de Leitura", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }

            }

            // Retorna uma configuração padrão (assumindo que ConfiguracaoEtiqueta tem um construtor sem argumentos)

            return new ConfiguracaoEtiqueta();

        }



        private List<ConfiguracaoPapel> CarregarModelosPapel()

        {

            if (!File.Exists(CAMINHO_MODELOS_PAPEL))

            {

                // Se o arquivo não existe, é normal retornar vazio.

                return new List<ConfiguracaoPapel>();

            }



            // ⭐ NOVO: Verificação de arquivo vazio

            FileInfo info = new FileInfo(CAMINHO_MODELOS_PAPEL);

            if (info.Length == 0)

            {

                // Se o arquivo estiver vazio (0 bytes), a desserialização falhará.

                // Isso pode indicar que o salvamento falhou ou o arquivo foi corrompido.

                return new List<ConfiguracaoPapel>();

            }



            try

            {

                XmlSerializer serializer = new XmlSerializer(typeof(List<ConfiguracaoPapel>));

                using (StreamReader reader = new StreamReader(CAMINHO_MODELOS_PAPEL))

                {

                    // Tenta desserializar

                    var modelos = (List<ConfiguracaoPapel>)serializer.Deserialize(reader);

                    return modelos ?? new List<ConfiguracaoPapel>(); // Garante que não retorne null

                }

            }

            catch (Exception ex)

            {

                // Se a leitura falhar, mostre o erro e retorne vazio

                MessageBox.Show($"Erro CRÍTICO ao ler o arquivo de modelos ({CAMINHO_MODELOS_PAPEL}). O arquivo pode estar corrompido ou o formato da classe mudou. Detalhes: {ex.Message}",

                                "Erro de Dados", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return new List<ConfiguracaoPapel>();

            }

        }


        // ========================================
        // ⭐ NOVO: GERENCIAMENTO EM MASSA DE PRODUTOS
        // ========================================

        /// <summary>
        /// Seleciona ou desmarca todos os produtos da lista
        /// </summary>
        private void chkSelecionarTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (dgvProdutos.Rows.Count == 0)
            {
                chkSelecionarTodos.Checked = false;
                return;
            }

            // Suspende o layout para melhorar performance
            dgvProdutos.SuspendLayout();

            try
            {
                foreach (DataGridViewRow row in dgvProdutos.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        row.Cells["colSelecionar"].Value = chkSelecionarTodos.Checked;
                    }
                }
            }
            finally
            {
                dgvProdutos.ResumeLayout();
            }
        }

        /// <summary>
        /// Remove todos os produtos da lista
        /// </summary>
        private void btnLimparTodos_Click(object sender, EventArgs e)
        {
            if (dgvProdutos.Rows.Count == 0)
            {
                MessageBox.Show(
                    "Não há produtos na lista para remover.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Deseja realmente remover TODOS os {dgvProdutos.Rows.Count} produtos da lista?\n\n" +
                "Esta ação não pode ser desfeita.",
                "Confirmar Remoção",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);  // Botão "Não" é o padrão

            if (result == DialogResult.Yes)
            {
                // Limpa a lista de produtos e o DataGridView
                produtos.Clear();
                dgvProdutos.Rows.Clear();

                // Desmarca o checkbox de selecionar todos
                chkSelecionarTodos.Checked = false;

                MessageBox.Show(
                    "Todos os produtos foram removidos da lista.",
                    "Sucesso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }


        // ========================================
        // 🔹 CONFIGURAÇÃO MÓDULO CONFECÇÃO
        // ========================================

        /// <summary>
        /// Configura a visibilidade dos controles específicos do módulo CONFECÇÃO
        /// </summary>
        private void ConfigurarControlesConfeccao()
        {
            // Mostra/oculta os controles de Tamanho e Cor baseado no módulo
            if (isConfeccao)
            {
                txtNome.Size = new System.Drawing.Size(220, 23);
                cmbBuscaNome.Size = new System.Drawing.Size(500, 23);
                colNome.Width = 500;
            }

            if (cmbTamanho != null) cmbTamanho.Visible = isConfeccao;
            if (lblTamanho != null) lblTamanho.Visible = isConfeccao;
            if (cmbCor != null) cmbCor.Visible = isConfeccao;
            if (lblCor != null) lblCor.Visible = isConfeccao;

            // Configura colunas do DataGridView
            if (dgvProdutos.Columns.Contains("colTam"))
                dgvProdutos.Columns["colTam"].Visible = isConfeccao;
            if (dgvProdutos.Columns.Contains("colCor"))
                dgvProdutos.Columns["colCor"].Visible = isConfeccao;
        }

        /// <summary>
        /// Carrega os tamanhos e cores disponíveis para um produto específico
        /// </summary>
        private void CarregarTamanhosECores(string codigoMercadoria)
        {
            if (!isConfeccao || string.IsNullOrEmpty(codigoMercadoria))
                return;

            // Proteção: Se controles não existem, retorna silenciosamente
            if (cmbTamanho == null || cmbCor == null)
                return;

            try
            {
                cmbTamanho.Items.Clear();
                cmbCor.Items.Clear();

                // Converter código para int
                if (!int.TryParse(codigoMercadoria, out int codigo))
                    return;

                // ⭐ NOVO: Buscar TODOS os tamanhos e cores daquele produto
                // Este método retorna listas distintas de TODOS os registros
                var (tamanhos, cores) = LocalDatabaseManager.BuscarTamanhosECoresPorCodigo(codigo);

                // Adicionar tamanhos encontrados
                foreach (var tamanho in tamanhos)
                {
                    if (!string.IsNullOrEmpty(tamanho))
                        cmbTamanho.Items.Add(tamanho);
                }

                // Adicionar cores encontradas
                foreach (var cor in cores)
                {
                    if (!string.IsNullOrEmpty(cor))
                        cmbCor.Items.Add(cor);
                }

                // Seleciona primeiro item se disponível
                if (cmbTamanho.Items.Count > 0)
                    cmbTamanho.SelectedIndex = 0;
                if (cmbCor.Items.Count > 0)
                    cmbCor.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar tamanhos e cores: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void FormPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Finalizar completamente a aplicação
            Application.Exit();

            // Garantir que o processo seja encerrado
            Environment.Exit(0);
        }
        private void groupProduto_Enter(object sender, EventArgs e)
        {

        }

        private void lblQtd_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ⭐ CONFECÇÃO: Atualiza o código de barras quando o tamanho é alterado
        /// </summary>
        private void CmbTamanho_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarCodigoBarrasPorTamanhoECor();
        }

        /// <summary>
        /// ⭐ CONFECÇÃO: Atualiza o código de barras quando a cor é alterada
        /// </summary>
        private void CmbCor_SelectedIndexChanged(object sender, EventArgs e)
        {
            AtualizarCodigoBarrasPorTamanhoECor();
        }

        /// <summary>
        /// ⭐ CONFECÇÃO: Busca e atualiza o código de barras baseado em Código + Tamanho + Cor
        /// </summary>
        private void AtualizarCodigoBarrasPorTamanhoECor()
        {
            // Só executa no modo confecção
            if (!isConfeccao) return;

            // Verifica se temos todos os dados necessários
            if (string.IsNullOrEmpty(txtCodigo.Text)) return;
            if (cmbTamanho == null || cmbCor == null) return;
            if (cmbTamanho.SelectedItem == null || cmbCor.SelectedItem == null) return;

            try
            {
                string codigo = txtCodigo.Text;
                string tamanho = cmbTamanho.SelectedItem.ToString();
                string cor = cmbCor.SelectedItem.ToString();

                // Busca o código de barras no banco local baseado em Código + Tam + Cor
                string codBarrasEncontrado = LocalDatabaseManager.BuscarCodigoBarrasPorCodTamCor(
                    codigo, tamanho, cor);

                if (!string.IsNullOrEmpty(codBarrasEncontrado))
                {
                    // Remove o evento temporariamente para evitar loop
                    if (cmbBuscaCodigo != null)
                        cmbBuscaCodigo.SelectedIndexChanged -= cmbBuscaCodigo_SelectedIndexChanged;

                    // Atualiza o campo de código de barras
                    if (cmbBuscaCodigo != null)
                        cmbBuscaCodigo.Text = codBarrasEncontrado;

                    // Restaura o evento
                    if (cmbBuscaCodigo != null)
                        cmbBuscaCodigo.SelectedIndexChanged += cmbBuscaCodigo_SelectedIndexChanged;

                    // Atualiza também o DataRow completo para quando adicionar o produto
                    AtualizarProdutoAtualCompleto(codigo, tamanho, cor);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar código de barras: {ex.Message}");
            }
        }

        /// <summary>
        /// ⭐ CONFECÇÃO: Atualiza o produtoAtualCompleto com os dados corretos de Tam e Cor
        /// </summary>
        private void AtualizarProdutoAtualCompleto(string codigo, string tamanho, string cor)
        {
            try
            {
                // Busca o registro completo no banco com Código + Tam + Cor
                DataTable resultado = LocalDatabaseManager.BuscarMercadoriaPorCodTamCor(codigo, tamanho, cor);

                if (resultado != null && resultado.Rows.Count > 0)
                {
                    produtoAtualCompleto = resultado.Rows[0];
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao atualizar produto completo: {ex.Message}");
            }
        }

        private void cmbTamanho_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ⭐ CARREGAMENTO: Abre o formulário de filtros para carregar produtos em massa
        /// </summary>
        private void btnCarregar_Click(object sender, EventArgs e)
        {
            try
            {
                using (FormFiltrosCarregamento formFiltros = new FormFiltrosCarregamento())
                {
                    if (formFiltros.ShowDialog() == DialogResult.OK)
                    {
                        // Obter os filtros selecionados
                        string grupo = formFiltros.GrupoSelecionado;
                        string fabricante = formFiltros.FabricanteSelecionado;
                        string fornecedor = formFiltros.FornecedorSelecionado;

                        // Buscar mercadorias com os filtros
                        DataTable mercadoriasFiltradas = LocalDatabaseManager.BuscarMercadoriasPorFiltros(
                            grupo, fabricante, fornecedor,isConfeccao);

                        if (mercadoriasFiltradas != null && mercadoriasFiltradas.Rows.Count > 0)
                        {
                            // Confirmar com o usuário
                            string mensagem = $"Foram encontrados {mercadoriasFiltradas.Rows.Count} produtos.\n\n" +
                                            $"Deseja adicionar todos ao painel de impressão?";

                            if (MessageBox.Show(mensagem, "Confirmar Carregamento",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                // Adicionar cada produto ao painel
                                int adicionados = 0;
                                foreach (DataRow row in mercadoriasFiltradas.Rows)
                                {
                                    try
                                    {
                                        AdicionarProdutoAoPanel(row);
                                        adicionados++;
                                    }
                                    catch (Exception exRow)
                                    {
                                        System.Diagnostics.Debug.WriteLine(
                                            $"Erro ao adicionar produto {row["Mercadoria"]}: {exRow.Message}");
                                    }
                                }

                                MessageBox.Show($"{adicionados} produtos foram adicionados ao painel!",
                                    "Carregamento Concluído",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Nenhum produto encontrado com os filtros selecionados.",
                                "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        /// <summary>
        /// Adiciona produto completo ao grid com dados do banco
        /// </summary>
        private void AdicionarProdutoAoGrid(Produto produto, ItemImportacao itemImportado)
        {
            // Adicionar à lista interna
            produtos.Add(produto);

            // Adicionar ao DataGridView
            if (isConfeccao && dgvProdutos.Columns.Contains("colTam") && dgvProdutos.Columns.Contains("colCor"))
            {
                // Modo confecção: inclui tamanho e cor
                dgvProdutos.Rows.Add(
                    itemImportado.Gerar,           // colSelecionar
                    produto.Nome,                   // colNome
                    produto.Codigo,                 // colCodigo
                    produto.Preco.ToString("C2"),  // colPreco
                    itemImportado.Quantidade,      // colQuantidade (Qtde)
                    itemImportado.Tamanho ?? "",   // colTam
                    itemImportado.Cor ?? ""        // colCor
                );
            }
            else
            {
                // Modo normal: sem tamanho e cor
                dgvProdutos.Rows.Add(
                    itemImportado.Gerar,           // colSelecionar
                    produto.Nome,                   // colNome
                    produto.Codigo,                 // colCodigo
                    produto.Preco.ToString("C2"),  // colPreco'
                    itemImportado.Quantidade       // colQuantidade (Qtde)
                );
            }
        }

        /// <summary>
        /// Adiciona produto básico ao grid quando não foi encontrado no banco
        /// </summary>
        private void AdicionarProdutoBasicoAoGrid(ItemImportacao item)
        {
            // Criar produto básico
            var produto = new Produto
            {
                Nome = item.Mercadoria,
                Codigo = item.Codigo,
                Preco = item.Preco ?? 0,
                Quantidade = item.Quantidade
            };

            // Adicionar à lista interna
            produtos.Add(produto);

            // Adicionar ao DataGridView
            if (isConfeccao && dgvProdutos.Columns.Contains("colTam") && dgvProdutos.Columns.Contains("colCor"))
            {
                // Modo confecção: inclui tamanho e cor
                dgvProdutos.Rows.Add(
                    item.Gerar,                    // colSelecionar
                    item.Mercadoria,               // colNome
                    item.Codigo ?? "",             // colCodigo
                    (item.Preco ?? 0).ToString("C2"), // colPreco
                    item.Quantidade,               // colQuantidade (Qtde)
                    item.Tamanho ?? "",           // colTam
                    item.Cor ?? ""                // colCor
                );
            }
            else
            {
                // Modo normal: sem tamanho e cor
                dgvProdutos.Rows.Add(
                    item.Gerar,                    // colSelecionar
                    item.Mercadoria,               // colNome
                    item.Codigo ?? "",             // colCodigo
                    (item.Preco ?? 0).ToString("C2"), // colPreco
                    item.Quantidade                // colQuantidade (Qtde)
                );
            }
        }
        private void ProcessarImportacaoExterna()
        {
            if (!_modoImportacao || _dadosImportacao == null || _dadosImportacao.Itens.Count == 0)
                return;

            try
            {
                this.Text = $"SmartPrint - Importação de {_dadosImportacao.FonteImportacao}";

                // ========================================
                // 🔹 POPULAR GRID COM DADOS IMPORTADOS
                // ========================================
                dgvProdutos.Rows.Clear();

                int importadosComSucesso = 0;
                int importadosComFalha = 0;
                List<string> erros = new List<string>();

                foreach (var itemImportado in _dadosImportacao.Itens)
                {
                    try
                    {
                        // Buscar produto completo no banco de dados
                        Produto produtoCompleto = BuscarProdutoParaImportacao(itemImportado);

                        if (produtoCompleto != null)
                        {
                            // Adicionar ao grid
                            AdicionarProdutoAoGrid(produtoCompleto, itemImportado);
                            importadosComSucesso++;
                        }
                        else
                        {
                            // Produto não encontrado - adicionar com dados básicos
                            AdicionarProdutoBasicoAoGrid(itemImportado);
                            importadosComSucesso++;
                        }
                    }
                    catch (Exception ex)
                    {
                        importadosComFalha++;
                        erros.Add($"Item {itemImportado.Codigo}/{itemImportado.Referencia}: {ex.Message}");
                    }
                }

                // ========================================
                // 🔹 FEEDBACK PARA O USUÁRIO
                // ========================================
                //string mensagem = $"✅ Importação concluída!\n\n" +
                //                $"• Itens importados: {importadosComSucesso}\n";

                //if (importadosComFalha > 0)
                //{
                //    mensagem += $"• Itens com erro: {importadosComFalha}\n\n";
                //    if (erros.Count <= 5)
                //    {
                //        mensagem += "Erros:\n" + string.Join("\n", erros);
                //    }
                //    else
                //    {
                //        mensagem += "Erros:\n" + string.Join("\n", erros.Take(5)) + $"\n... e mais {erros.Count - 5} erros";
                //    }
                //}

                //mensagem += $"\n\nFonte: {_dadosImportacao.FonteImportacao}";

                //MessageBox.Show(mensagem, "Importação de Dados",
                //    MessageBoxButtons.OK,
                //    importadosComFalha > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

                // Focar no grid
                if (dgvProdutos.Rows.Count > 0)
                {
                    dgvProdutos.Focus();
                    dgvProdutos.CurrentCell = dgvProdutos.Rows[0].Cells[0];
                }

                // ========================================
                // 🔹 APLICAR CONFIGURAÇÕES DA IMPORTAÇÃO
                // ========================================
                if (_dadosImportacao.Configuracao != null)
                {
                    // Auto-imprimir se solicitado
                    if (_dadosImportacao.Configuracao.AutoImprimir)
                    {
                        // Aguardar um pouco para o usuário ver o grid
                        var timer = new System.Windows.Forms.Timer();
                        timer.Interval = 1000;
                        timer.Tick += (s, e) =>
                        {
                            timer.Stop();
                            timer.Dispose();
                            btnImprimir_Click(this, EventArgs.Empty);
                        };
                        timer.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao processar importação:\n\n{ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private Produto BuscarProdutoParaImportacao(ItemImportacao item)
        {
            try
            {
                // Tentar buscar por código primeiro
                if (!string.IsNullOrEmpty(item.Codigo))
                {
                    var porCodigo = mercadorias?.AsEnumerable()
                        .FirstOrDefault(r => r["Codigo"]?.ToString() == item.Codigo);

                    if (porCodigo != null)
                        return ConverterDataRowParaProduto(porCodigo);
                }

                // Se não encontrou, tentar por referência
                if (!string.IsNullOrEmpty(item.Referencia))
                {
                    var porReferencia = mercadorias?.AsEnumerable()
                        .FirstOrDefault(r => r["Referencia"]?.ToString() == item.Referencia);

                    if (porReferencia != null)
                        return ConverterDataRowParaProduto(porReferencia);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }
        private Produto ConverterDataRowParaProduto(DataRow row)
        {
            try
            {
                var produto = new Produto
                {
                    // ⭐ NOMES CORRETOS DAS COLUNAS DA SUA TABELA:
                    Codigo = row["CodigoMercadoria"]?.ToString() ?? "",      // Era "Codigo"
                    Nome = row["Mercadoria"]?.ToString() ?? "",               // Era "Descricao"
                    CodFabricante = row["CodFabricante"]?.ToString() ?? "",  // Era "Referencia"
                    CodBarras = row["CodBarras"]?.ToString() ?? "",           // Era "CodBarra"
                    Quantidade = 1
                };

                // ⭐ PREÇO: Tentar PrecoVenda primeiro, depois VendaD como fallback
                if (row.Table.Columns.Contains("PrecoVenda") && row["PrecoVenda"] != DBNull.Value)
                {
                    produto.Preco = Convert.ToDecimal(row["PrecoVenda"]);
                    produto.PrecoVenda = produto.Preco;
                }
                else if (row.Table.Columns.Contains("VendaD") && row["VendaD"] != DBNull.Value)
                {
                    produto.Preco = Convert.ToDecimal(row["VendaD"]);
                    produto.VendaD = produto.Preco;
                }

                // ⭐ PREÇOS ALTERNATIVOS (com verificação de existência da coluna)
                if (row.Table.Columns.Contains("VendaA") && row["VendaA"] != DBNull.Value)
                    produto.VendaA = Convert.ToDecimal(row["VendaA"]);

                if (row.Table.Columns.Contains("VendaB") && row["VendaB"] != DBNull.Value)
                    produto.VendaB = Convert.ToDecimal(row["VendaB"]);

                if (row.Table.Columns.Contains("VendaC") && row["VendaC"] != DBNull.Value)
                    produto.VendaC = Convert.ToDecimal(row["VendaC"]);

                if (row.Table.Columns.Contains("VendaD") && row["VendaD"] != DBNull.Value)
                    produto.VendaD = Convert.ToDecimal(row["VendaD"]);

                if (row.Table.Columns.Contains("VendaE") && row["VendaE"] != DBNull.Value)
                    produto.VendaE = Convert.ToDecimal(row["VendaE"]);

                // ⭐ CAMPOS ADICIONAIS (com verificação de existência)
                if (row.Table.Columns.Contains("Fornecedor"))
                    produto.Fornecedor = row["Fornecedor"]?.ToString() ?? "";

                if (row.Table.Columns.Contains("Fabricante"))
                    produto.Fabricante = row["Fabricante"]?.ToString() ?? "";

                if (row.Table.Columns.Contains("Grupo"))
                    produto.Grupo = row["Grupo"]?.ToString() ?? "";

                if (row.Table.Columns.Contains("Prateleira"))
                    produto.Prateleira = row["Prateleira"]?.ToString() ?? "";

                if (row.Table.Columns.Contains("Garantia"))
                    produto.Garantia = row["Garantia"]?.ToString() ?? "";

                // ⭐ CAMPOS DE CONFECÇÃO (se aplicável)
                if (isConfeccao)
                {
                    if (row.Table.Columns.Contains("Tam"))
                        produto.Tam = row["Tam"]?.ToString() ?? "";

                    if (row.Table.Columns.Contains("Cores"))
                        produto.Cores = row["Cores"]?.ToString() ?? "";

                    if (row.Table.Columns.Contains("CodBarras_Grade"))
                        produto.CodBarras_Grade = row["CodBarras_Grade"]?.ToString() ?? "";
                }

                return produto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao converter produto: {ex.Message}");
            }
        }


        // ✅ MÉTODO CORRIGIDO - SUBSTITUIR NO FormPrincipal.cs A PARTIR DA LINHA 2487

        /// <summary>
        /// ⭐ CARREGAMENTO: Adiciona um produto ao painel a partir de um DataRow
        /// REFATORADO para seguir EXATAMENTE o padrão do AdicionarProdutoPelaBusca
        /// </summary>
        private void AdicionarProdutoAoPanel(DataRow row)
        {
            // ========================================
            // ⭐ ETAPA 1: CRIAR PRODUTO COM DADOS BÁSICOS (igual lançamento manual linhas 1571-1577)
            // ========================================
            string codigo = row["CodigoMercadoria"]?.ToString();
            string nome = row["Mercadoria"]?.ToString();
            decimal precoVenda = row["PrecoVenda"] != DBNull.Value ? Convert.ToDecimal(row["PrecoVenda"]) : 0m;

            var produto = new Produto
            {
                Nome = nome,
                Codigo = codigo,
                Preco = precoVenda,
                Quantidade = 1
            };

            // ========================================
            // ⭐ ETAPA 2: POPULAR CAMPOS ADICIONAIS DO DATAROW (igual lançamento manual linhas 1582-1619)
            // ========================================
            try
            {
                // Popula todos os campos adicionais
                produto.CodFabricante = row["CodFabricante"]?.ToString();
                produto.CodBarras = row["CodBarras"]?.ToString();
                produto.CodBarras_Grade = row["CodBarras_Grade"]?.ToString();
                produto.PrecoVenda = row["PrecoVenda"] != DBNull.Value
                    ? Convert.ToDecimal(row["PrecoVenda"])
                    : precoVenda;
                produto.VendaA = row["VendaA"] != DBNull.Value
                    ? Convert.ToDecimal(row["VendaA"])
                    : 0m;
                produto.VendaB = row["VendaB"] != DBNull.Value
                    ? Convert.ToDecimal(row["VendaB"])
                    : 0m;
                produto.VendaC = row["VendaC"] != DBNull.Value
                    ? Convert.ToDecimal(row["VendaC"])
                    : 0m;
                produto.VendaD = row["VendaD"] != DBNull.Value
                    ? Convert.ToDecimal(row["VendaD"])
                    : 0m;
                produto.VendaE = row["VendaE"] != DBNull.Value
                    ? Convert.ToDecimal(row["VendaE"])
                    : 0m;
                produto.Fornecedor = row["Fornecedor"]?.ToString();
                produto.Fabricante = row["Fabricante"]?.ToString();
                produto.Grupo = row["Grupo"]?.ToString();
                produto.Prateleira = row["Prateleira"]?.ToString();
                produto.Garantia = row["Garantia"]?.ToString();
                produto.Tam = row["Tam"]?.ToString();
                produto.Cores = row["Cores"]?.ToString();

                // ⭐ CONFECÇÃO: Sobrescreve Tam e Cor se necessário (igual lançamento manual linhas 1614-1619)
                if (isConfeccao && cmbTamanho != null && cmbCor != null)
                {
                    produto.Tam = cmbTamanho.SelectedItem?.ToString() ?? produto.Tam ?? "";
                    produto.Cores = cmbCor.SelectedItem?.ToString() ?? produto.Cores ?? "";
                }
            }
            catch
            {
                // Se falhar ao ler campos adicionais, continua com dados básicos
            }

            // ========================================
            // ⭐ ETAPA 3: ADICIONAR À LISTA (igual lançamento manual linha 1692)
            // ========================================
            produtos.Add(produto);

            // ========================================
            // ⭐ ETAPA 4: ADICIONAR AO DATAGRIDVIEW (igual lançamento manual linhas 1695-1703)
            // ========================================
            if (isConfeccao && dgvProdutos.Columns.Contains("colTam") && dgvProdutos.Columns.Contains("colCor"))
            {
                dgvProdutos.Rows.Add(false, produto.Nome, produto.CodBarras_Grade,
                    produto.Preco.ToString("C2"), produto.Quantidade,
                    produto.Tam ?? "", produto.Cores ?? "");
            }
            else
            {
                dgvProdutos.Rows.Add(false, produto.Nome, produto.Codigo,
                    produto.Preco.ToString("C2"), produto.Quantidade);
            }



        }
        public FormPrincipal(DadosImportacao dadosImportacao) : this()
        {
            _dadosImportacao = dadosImportacao;
            _modoImportacao = true;

        }


    }


}








       