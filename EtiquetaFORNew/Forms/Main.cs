using EtiquetaFORNew;
using EtiquetaFORNew.Data;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace EtiquetaFORNew
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            //testerepositorio
            senhaBox.UseSystemPasswordChar = true; // Oculta caracteres
            senhaBox.KeyDown += senhaBox_KeyDown;  // Detecta F11
            this.Text = AppInfo.GetTituloAplicacao();
            this.KeyPreview = true; // faz o formulário "enxergar" as teclas antes dos campos
            // Conecta o evento KeyDown
            this.KeyDown += Main_KeyDown;

            LoadUsuarios();
        }

        private void Main_Load(object sender, EventArgs e)
        {
            usuarioBox.Focus();
            ArredondarPainel(panel1, 30);
            ArredondarPainel(panel2, 30);
            panel1.Resize += (s, ev) => ArredondarPainel(panel1, 30);

            // Aplicar versão usando helper
            VersaoHelper.AplicarVersaoLabel(Versao);
            VersaoHelper.DefinirTituloComVersao(this);
        }


        private void senhaBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.F11)
            //{
            //    e.SuppressKeyPress = true; // evita beep do F11

            //    if (senhaBox.Text == "suporte@softcom")
            //    {
            //        telaTecnico tela = new telaTecnico();
            //        tela.ShowDialog();
            //        senhaBox.Clear();
            //    }
            //    else
            //    {
            //        MessageBox.Show(
            //            "Ops! A senha digitada não confere. Verifique e tente novamente, por favor.",
            //            "Senha incorreta",
            //            MessageBoxButtons.OK,
            //            MessageBoxIcon.Warning
            //        );

            //        senhaBox.Clear();
            //        senhaBox.Focus();
            //    }
            //}
        }

        private async void btnLogar_Click(object sender, EventArgs e)
        {
            string senha = senhaBox.Text.Trim();

            if (usuarioBox.SelectedItem == null || string.IsNullOrEmpty(senha))
            {
                MessageBox.Show("Por favor, preencha o usuário e a senha.",
                                "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string codigoSelecionado = ((ComboItem)usuarioBox.SelectedItem).Value;

            try
            {
                // Caminho da configuração
                string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");

                if (!File.Exists(caminhoArquivo))
                {
                    MessageBox.Show("⚠️ Configuração de banco não encontrada. Configure primeiro nas Configurações.",
                                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string json = File.ReadAllText(caminhoArquivo);
                var config = JsonConvert.DeserializeObject<ConfiguracaoBD>(json);

                // Monta connection string
                string servidorCompleto = string.IsNullOrEmpty(config.Porta)
                    ? config.Servidor
                    : $"{config.Servidor},{config.Porta}";

                string connectionString =
                    $"Server={servidorCompleto};Database={config.Banco};User Id={config.Usuario};Password={config.Senha};";

                // Verificação de login
                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();

                    string query = @"SELECT [Nome] 
                                     FROM [Cadastro De Vendedores] 
                                     WHERE [Código do Vendedor] = @codigo AND [Senha] = @Senha";

                    using (SqlCommand cmd = new SqlCommand(query, conexao))
                    {
                        cmd.Parameters.Add("@Codigo", SqlDbType.NVarChar, 50).Value = codigoSelecionado.Trim();
                        cmd.Parameters.Add("@Senha", SqlDbType.NVarChar, 50).Value = senha.Trim();

                        string nomeVendedor = cmd.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(nomeVendedor))
                        {
                            //MessageBox.Show($"✅ Bem-vindo, {nomeVendedor}!", "Login realizado", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            // Chamar GetSetRegistroJsonAsync se a loja estiver configurada
                            await ChamarRegistroLojaAsync(config.Loja, connectionString);

                            // Abre a próxima tela e esconde a principal
                            FormPrincipal Entrada = new FormPrincipal();
                            Entrada.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("❌ Usuário ou senha incorretos.\nVerifique e tente novamente.",
                                            "Falha no login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao tentar logar:\n{ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async System.Threading.Tasks.Task ChamarRegistroLojaAsync(string loja, string connectionString)
        {
            if (string.IsNullOrEmpty(loja))
                return; // Loja não configurada, não faz nada

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();

                    string query = @"
                        SELECT Fantasia, CGC, CodigoSuporte
                        FROM Integrar_Lojas 
                        WHERE Loja = @Loja";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Loja", loja);

                        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                string fantasia = reader.IsDBNull(0) ? "" : reader.GetString(0);
                                string cgc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                string codigoSuporte = reader.IsDBNull(2) ? "" : reader.GetString(2);

                                // Chamar a função GetSetRegistroJsonAsync
                                if (!string.IsNullOrEmpty(codigoSuporte) && !string.IsNullOrEmpty(cgc) && !string.IsNullOrEmpty(fantasia))
                                {
                                    string resultado = await DatabaseConfig.GetSetRegistroJsonAsync(codigoSuporte, cgc, fantasia);
                                    System.Diagnostics.Debug.WriteLine($"Resultado do registro ao logar: {resultado}");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Não mostrar erro ao usuário, apenas logar
                System.Diagnostics.Debug.WriteLine($"Erro ao chamar registro da loja: {ex.Message}");
            }
        }

        private void LoadUsuarios()
        {
            try
            {
                string caminhoArquivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
                if (!File.Exists(caminhoArquivo)) return;

                string json = File.ReadAllText(caminhoArquivo);
                var config = JsonConvert.DeserializeObject<ConfiguracaoBD>(json);

                string servidorCompleto = string.IsNullOrEmpty(config.Porta) ? config.Servidor : $"{config.Servidor},{config.Porta}";
                string connectionString = $"Server={servidorCompleto};Database={config.Banco};User Id={config.Usuario};Password={config.Senha};";

                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();

                    string query = "SELECT [Código do Vendedor], [Nome] FROM [Cadastro De Vendedores] ORDER BY [Nome]";

                    using (SqlCommand cmd = new SqlCommand(query, conexao))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        usuarioBox.Items.Clear();
                        while (reader.Read())
                        {
                            usuarioBox.Items.Add(new ComboItem
                            {
                                Text = reader["Nome"].ToString(),
                                Value = reader["Código do Vendedor"].ToString()
                            });
                        }
                    }
                }

                usuarioBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                usuarioBox.AutoCompleteSource = AutoCompleteSource.ListItems;
                usuarioBox.DisplayMember = "Text";
                usuarioBox.ValueMember = "Value";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar usuários:\n{ex.Message}");
            }
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // evita beep
                this.SelectNextControl(this.ActiveControl, true, true, true, true); // simula o Tab
            }
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        public static class AppInfo
        {
            public static string GetTituloAplicacao()
            {
                string nome = "SmartPrint";//Assembly.GetExecutingAssembly().GetName().Name;
                return $"{nome} - v1.0";
            }
        }

        public class ComboItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
            public override string ToString() => Text;
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            //configuracoes tela = new configuracoes();
            ConfigForm tela = new ConfigForm();
            tela.ShowDialog();
        }
        private void ArredondarPainel(Panel panel, int raio)
        {
            // Impede erro se o painel ainda não tiver tamanho
            if (panel.Width < 1 || panel.Height < 1)
                return;

            // Cria o caminho com cantos arredondados
            using (GraphicsPath path = new GraphicsPath())
            {
                int diametro = raio * 2;
                Rectangle rect = new Rectangle(0, 0, diametro, diametro);

                path.AddArc(rect, 180, 90);
                rect.X = panel.Width - diametro;
                path.AddArc(rect, 270, 90);
                rect.Y = panel.Height - diametro;
                path.AddArc(rect, 0, 90);
                rect.X = 0;
                path.AddArc(rect, 90, 90);
                path.CloseFigure();

                panel.Region = new Region(path);
            }
        }
        private void btnConfig_Click(object sender, EventArgs e)
        {
            using (var configForm = new ConfigForm())
            {
                // Abre como modal e espera
                if (configForm.ShowDialog() == DialogResult.OK)
                {
                    // 🔁 Recarrega os dados ou atualiza conexão
                    RecarregarConfiguracao();
                }
            }
        }
        public void RecarregarConfiguracao()
        {
            try
            {
                var config = DatabaseConfig.LoadConfiguration();
                if (config == null)
                {
                    MessageBox.Show("Nenhuma configuração encontrada.");
                    return;
                }

                // Atualiza labels informativos (se existirem)
                //lblServidorAtual.Text = config.Servidor;
                //lblBancoAtual.Text = config.Banco;

                // Atualiza campos de login
                //if (!string.IsNullOrEmpty(config.Usuario))
                //    usuarioBox.Text = config.Usuario;

                //if (!string.IsNullOrEmpty(config.Senha))
                //    senhaBox.Text = config.Senha;

                // Reabre conexão
                InicializarConexao();

                MessageBox.Show("Configurações e conexão atualizadas com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao recarregar configuração: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InicializarConexao()
        {
            try
            {
                var config = DatabaseConfig.LoadConfiguration();

                if (config == null)
                {
                    MessageBox.Show("Configuração do banco não encontrada. Configure o sistema primeiro.",
                        "Atenção", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string servidorCompleto = string.IsNullOrEmpty(config.Porta)
                    ? config.Servidor
                    : $"{config.Servidor},{config.Porta}";

                string connectionString =
                    $"Server={servidorCompleto};Database={config.Banco};User Id={config.Usuario};Password={config.Senha};TrustServerCertificate=True;";

                using (SqlConnection conexao = new SqlConnection(connectionString))
                {
                    conexao.Open();
                    // Teste de conexão bem-sucedido
                    conexao.Close();
                }

                // Após conexão bem-sucedida, recarrega usuários na ComboBox
                LoadUsuarios();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao tentar conectar com o banco de dados:\n{ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            telaTecnico tela = new telaTecnico();
            tela.ShowDialog();
        }
    }

    // Classe para mapear configuração do JSON
    public class ConfiguracaoBD
    {
        public string Servidor { get; set; }
        public string Porta { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Banco { get; set; }
        public string Loja { get; set; }
    }



}