using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Windows.Forms;
using BarcodeStandard;
using SkiaSharp;
using EtiquetaFORNew.Data;

namespace EtiquetaFORNew
{
    public class FormPreview : Form
    {
        private TemplateEtiqueta template;
        private ConfiguracaoEtiqueta configuracao;
        private PictureBox pbPreview;
        private const float PIXELS_POR_MM = 3.0f;
        private string nomePapel;
        private float larguraPapelMM;
        private float alturaPapelMM;

        // ⭐ NOVO: Checkbox para mostrar com dados exemplo
        private CheckBox chkMostrarDados;
        private bool mostrarComDados = true;

        // ⭐ NOVO: Produto exemplo para preview
        private Produto produtoExemplo;

        public FormPreview(TemplateEtiqueta template, ConfiguracaoEtiqueta configuracao,
                          string nomePapel, float larguraPapelMM, float alturaPapelMM)
        {
            this.template = template;
            this.configuracao = configuracao;
            this.nomePapel = nomePapel;
            this.larguraPapelMM = larguraPapelMM;
            this.alturaPapelMM = alturaPapelMM;

            // ⭐ NOVO: Buscar produto real do banco ou criar exemplo
            BuscarOuCriarProdutoExemplo();

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = string.Format("Preview de Impressão ({0} elementos - {1}x{2} - Papel: {3} [{4:0}x{5:0}mm])",
                template.Elementos.Count, configuracao.NumColunas, configuracao.NumLinhas,
                nomePapel, larguraPapelMM, alturaPapelMM);
            this.Size = new Size(1100, 850);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(70, 70, 70);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;
            this.MinimizeBox = false;

            // Painel superior
            Panel panelTop = new Panel();
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 60;
            panelTop.BackColor = Color.FromArgb(52, 73, 94);
            this.Controls.Add(panelTop);

            Label lblTitulo = new Label();
            lblTitulo.Text = "PREVIEW DE IMPRESSÃO";
            lblTitulo.Location = new Point(20, 15);
            lblTitulo.Size = new Size(400, 30);
            lblTitulo.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitulo.ForeColor = Color.White;
            panelTop.Controls.Add(lblTitulo);

            // ⭐ NOVO: Checkbox para mostrar com dados exemplo
            chkMostrarDados = new CheckBox();
            chkMostrarDados.Text = "📄 Mostrar com dados exemplo";
            chkMostrarDados.Location = new Point(450, 18);
            chkMostrarDados.Size = new Size(250, 25);
            chkMostrarDados.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            chkMostrarDados.ForeColor = Color.White;
            chkMostrarDados.Checked = true;
            chkMostrarDados.CheckedChanged += (s, e) =>
            {
                mostrarComDados = chkMostrarDados.Checked;
                pbPreview.Invalidate();  // Redesenhar
            };
            panelTop.Controls.Add(chkMostrarDados);

            // Botão Imprimir
            Button btnImprimir = new Button();
            btnImprimir.Text = "🖨 Imprimir";
            btnImprimir.Location = new Point(850, 15);
            btnImprimir.Size = new Size(120, 30);
            btnImprimir.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnImprimir.BackColor = Color.FromArgb(46, 204, 113);
            btnImprimir.ForeColor = Color.White;
            btnImprimir.FlatStyle = FlatStyle.Flat;
            btnImprimir.Cursor = Cursors.Hand;
            btnImprimir.FlatAppearance.BorderSize = 0;
            btnImprimir.Click += BtnImprimir_Click;
            panelTop.Controls.Add(btnImprimir);

            // Botão Fechar
            Button btnFechar = new Button();
            btnFechar.Text = "✕ Fechar";
            btnFechar.Location = new Point(980, 15);
            btnFechar.Size = new Size(100, 30);
            btnFechar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnFechar.BackColor = Color.FromArgb(231, 76, 60);
            btnFechar.ForeColor = Color.White;
            btnFechar.FlatStyle = FlatStyle.Flat;
            btnFechar.Cursor = Cursors.Hand;
            btnFechar.FlatAppearance.BorderSize = 0;
            btnFechar.Click += delegate { this.Close(); };
            panelTop.Controls.Add(btnFechar);

            // Painel de scroll
            Panel panelScroll = new Panel();
            panelScroll.Dock = DockStyle.Fill;
            panelScroll.AutoScroll = true;
            panelScroll.BackColor = Color.FromArgb(70, 70, 70);
            panelScroll.Padding = new Padding(50, 80, 50, 50);
            this.Controls.Add(panelScroll);

            // PictureBox para desenhar
            pbPreview = new PictureBox();
            pbPreview.Location = new Point(50, 80);
            pbPreview.BackColor = Color.White;
            pbPreview.BorderStyle = BorderStyle.FixedSingle;
            pbPreview.Paint += PbPreview_Paint;
            panelScroll.Controls.Add(pbPreview);

            CalcularTamanhoPreview();
        }

        // ⭐ NOVO: Buscar produto real do banco ou criar dados exemplo
        private void BuscarOuCriarProdutoExemplo()
        {
            try
            {
                // Tentar buscar primeiro produto do banco local
                DataTable dt = BuscarPrimeiroProdutoDoBanco();

                if (dt != null && dt.Rows.Count > 0)
                {
                    // Converter DataRow para Produto
                    produtoExemplo = ConverterDataRowParaProduto(dt.Rows[0]);
                }
                else
                {
                    // Se não houver produtos, criar dados exemplo
                    CriarProdutoExemploFicticio();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar produto exemplo: {ex.Message}");
                // Em caso de erro, usar dados fictícios
                CriarProdutoExemploFicticio();
            }
        }

        // ⭐ NOVO: Buscar primeiro produto do banco local
        private DataTable BuscarPrimeiroProdutoDoBanco()
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection($"Data Source={System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LocalData.db")};Version=3;"))
                {
                    conn.Open();

                    string query = @"
                        SELECT *
                        FROM Mercadorias
                        ORDER BY CodigoMercadoria
                        LIMIT 1
                    ";

                    using (var cmd = new System.Data.SQLite.SQLiteCommand(query, conn))
                    {
                        DataTable dt = new DataTable();
                        using (var adapter = new System.Data.SQLite.SQLiteDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                            return dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar produto: {ex.Message}");
                return null;
            }
        }

        // ⭐ NOVO: Converter DataRow para Produto
        private Produto ConverterDataRowParaProduto(DataRow row)
        {
            return new Produto
            {
                Nome = row["Mercadoria"]?.ToString() ?? "",
                Codigo = row["CodigoMercadoria"]?.ToString() ?? "",
                CodFabricante = row["CodFabricante"]?.ToString() ?? "",
                CodBarras = row["CodBarras"]?.ToString() ?? "",
                CodBarras_Grade = row.Table.Columns.Contains("CodBarras_Grade") ? row["CodBarras_Grade"]?.ToString() ?? "" : "",
                Preco = row["PrecoVenda"] != DBNull.Value ? Convert.ToDecimal(row["PrecoVenda"]) : 0m,
                PrecoVenda = row["PrecoVenda"] != DBNull.Value ? Convert.ToDecimal(row["PrecoVenda"]) : 0m,
                VendaA = row["VendaA"] != DBNull.Value ? Convert.ToDecimal(row["VendaA"]) : 0m,
                VendaB = row["VendaB"] != DBNull.Value ? Convert.ToDecimal(row["VendaB"]) : 0m,
                VendaC = row["VendaC"] != DBNull.Value ? Convert.ToDecimal(row["VendaC"]) : 0m,
                VendaD = row["VendaD"] != DBNull.Value ? Convert.ToDecimal(row["VendaD"]) : 0m,
                VendaE = row["VendaE"] != DBNull.Value ? Convert.ToDecimal(row["VendaE"]) : 0m,
                Quantidade = 1,
                Fornecedor = row.Table.Columns.Contains("Fornecedor") ? row["Fornecedor"]?.ToString() ?? "" : "",
                Fabricante = row.Table.Columns.Contains("Fabricante") ? row["Fabricante"]?.ToString() ?? "" : "",
                Grupo = row.Table.Columns.Contains("Grupo") ? row["Grupo"]?.ToString() ?? "" : "",
                Prateleira = row.Table.Columns.Contains("Prateleira") ? row["Prateleira"]?.ToString() ?? "" : "",
                Garantia = row.Table.Columns.Contains("Garantia") ? row["Garantia"]?.ToString() ?? "" : "",
                Tam = row.Table.Columns.Contains("Tam") ? row["Tam"]?.ToString() ?? "" : "",
                Cores = row.Table.Columns.Contains("Cores") ? row["Cores"]?.ToString() ?? "" : "",
                PrecoOriginal = row.Table.Columns.Contains("PrecoOriginal") && row["PrecoOriginal"] != DBNull.Value ? Convert.ToDecimal(row["PrecoOriginal"]) : (decimal?)null,
                PrecoPromocional = row.Table.Columns.Contains("PrecoPromocional") && row["PrecoPromocional"] != DBNull.Value ? Convert.ToDecimal(row["PrecoPromocional"]) : (decimal?)null
            };
        }

        // Criar produto com dados fictícios (fallback)
        private void CriarProdutoExemploFicticio()
        {
            produtoExemplo = new Produto
            {
                Nome = "Camisa Polo Masculina",
                Codigo = "12345",
                CodFabricante = "REF-2024",
                CodBarras = "7891234567890",
                CodBarras_Grade = "7891234567890001",
                Preco = 89.90m,
                PrecoVenda = 89.90m,
                VendaA = 79.90m,
                VendaB = 84.90m,
                VendaC = 89.90m,
                VendaD = 94.90m,
                VendaE = 99.90m,
                Quantidade = 1,
                Fornecedor = "Confecções XYZ Ltda",
                Fabricante = "Têxtil ABC",
                Grupo = "Vestuário Masculino",
                Prateleira = "A-15",
                Garantia = "90 dias",
                Tam = "M",
                Cores = "Azul Marinho",
                PrecoOriginal = 129.90m,
                PrecoPromocional = 89.90m
            };
        }

        private void CalcularTamanhoPreview()
        {
            int larguraPixels = (int)(larguraPapelMM * PIXELS_POR_MM);
            int alturaPixels = (int)(alturaPapelMM * PIXELS_POR_MM);
            pbPreview.Size = new Size(larguraPixels, alturaPixels);
        }

        private void PbPreview_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.Clear(Color.White);

            DesenharMargens(g);

            for (int linha = 0; linha < configuracao.NumLinhas; linha++)
            {
                for (int coluna = 0; coluna < configuracao.NumColunas; coluna++)
                {
                    DesenharEtiqueta(g, linha, coluna);
                }
            }
        }

        private void DesenharMargens(Graphics g)
        {
            Pen penMargem = new Pen(Color.Red, 1);
            penMargem.DashStyle = DashStyle.Dot;

            if (configuracao.MargemSuperior > 0)
            {
                float y = configuracao.MargemSuperior * PIXELS_POR_MM;
                g.DrawLine(penMargem, 0, y, pbPreview.Width, y);
            }

            if (configuracao.MargemInferior > 0)
            {
                float y = (alturaPapelMM - configuracao.MargemInferior) * PIXELS_POR_MM;
                g.DrawLine(penMargem, 0, y, pbPreview.Width, y);
            }

            if (configuracao.MargemEsquerda > 0)
            {
                float x = configuracao.MargemEsquerda * PIXELS_POR_MM;
                g.DrawLine(penMargem, x, 0, x, pbPreview.Height);
            }

            if (configuracao.MargemDireita > 0)
            {
                float x = (larguraPapelMM - configuracao.MargemDireita) * PIXELS_POR_MM;
                g.DrawLine(penMargem, x, 0, x, pbPreview.Height);
            }

            penMargem.Dispose();
        }

        private void DesenharEtiqueta(Graphics g, int linha, int coluna)
        {
            float xMM = configuracao.MargemEsquerda +
                       (coluna * (configuracao.LarguraEtiqueta + configuracao.EspacamentoColunas));
            float yMM = configuracao.MargemSuperior +
                       (linha * (configuracao.AlturaEtiqueta + configuracao.EspacamentoLinhas));

            float x = xMM * PIXELS_POR_MM;
            float y = yMM * PIXELS_POR_MM;
            float largura = configuracao.LarguraEtiqueta * PIXELS_POR_MM;
            float altura = configuracao.AlturaEtiqueta * PIXELS_POR_MM;

            RectangleF rectEtiqueta = new RectangleF(x, y, largura, altura);

            Pen penBorda = new Pen(Color.FromArgb(100, 100, 100), 2);
            penBorda.DashStyle = DashStyle.Dash;
            g.DrawRectangle(penBorda, x, y, largura, altura);
            penBorda.Dispose();

            // ⭐ CORRIGIDO: Renderizar elementos se checkbox marcado
            if (mostrarComDados && template.Elementos.Count > 0)
            {
                foreach (var elem in template.Elementos)
                {
                    DesenharElemento(g, elem, rectEtiqueta, produtoExemplo);
                }
            }
            else
            {
                Font fonte = new Font("Segoe UI", 10, FontStyle.Bold);
                SolidBrush brush = new SolidBrush(Color.FromArgb(180, 180, 180));
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                string texto = string.Format("ETIQUETA\n{0}x{1} mm",
                    configuracao.LarguraEtiqueta, configuracao.AlturaEtiqueta);
                g.DrawString(texto, fonte, brush, rectEtiqueta, sf);

                fonte.Dispose();
                brush.Dispose();
                sf.Dispose();
            }
        }

        // ⭐ CORRIGIDO: Desenhar elemento com escala correta
        private void DesenharElemento(Graphics g, ElementoEtiqueta elem, RectangleF rectEtiqueta, Produto produto)
        {
            // Calcular escala correta (MM para pixels)
            float escala = PIXELS_POR_MM;

            RectangleF bounds = new RectangleF(
                rectEtiqueta.X + (elem.Bounds.X * escala),
                rectEtiqueta.Y + (elem.Bounds.Y * escala),
                elem.Bounds.Width * escala,
                elem.Bounds.Height * escala
            );

            GraphicsState state = null;
            if (elem.Rotacao != 0)
            {
                state = g.Save();
                PointF centro = new PointF(
                    bounds.X + bounds.Width / 2f,
                    bounds.Y + bounds.Height / 2f
                );
                g.TranslateTransform(centro.X, centro.Y);
                g.RotateTransform(elem.Rotacao);
                g.TranslateTransform(-centro.X, -centro.Y);
            }

            // Desenhar cor de fundo
            if (elem.CorFundo.HasValue && elem.CorFundo.Value != Color.Transparent)
            {
                using (SolidBrush fundoBrush = new SolidBrush(elem.CorFundo.Value))
                {
                    g.FillRectangle(fundoBrush, bounds);
                }
            }

            // Desenhar conteúdo
            float tamanhoFonte = elem.Fonte.Size * escala / PIXELS_POR_MM;
            using (Font fonte = new Font(elem.Fonte.FontFamily, tamanhoFonte, elem.Fonte.Style))
            using (SolidBrush brush = new SolidBrush(elem.Cor))
            {
                StringFormat sf = new StringFormat
                {
                    Alignment = elem.Alinhamento,
                    LineAlignment = StringAlignment.Center,
                    Trimming = StringTrimming.EllipsisCharacter,
                    FormatFlags = StringFormatFlags.LineLimit
                };

                switch (elem.Tipo)
                {
                    case TipoElemento.Texto:
                        g.DrawString(elem.Conteudo ?? "Texto", fonte, brush, bounds, sf);
                        break;

                    case TipoElemento.Campo:
                        string valor = ObterValorCampo(elem.Conteudo, produto);
                        g.DrawString(valor, fonte, brush, bounds, sf);
                        break;

                    case TipoElemento.CodigoBarras:
                        string codigoBarras = ObterCodigoBarras(elem.Conteudo, produto);
                        DesenharCodigoBarras(g, codigoBarras, bounds);
                        break;

                    case TipoElemento.Imagem:
                        if (elem.Imagem != null)
                            g.DrawImage(elem.Imagem, bounds);
                        break;
                }
            }

            if (state != null)
            {
                g.Restore(state);
            }
        }

        private string ObterValorCampo(string campo, Produto produto)
        {
            if (produto == null) return $"[{campo}]";

            switch (campo)
            {
                case "Nome":
                case "Mercadoria":
                    return produto.Nome ?? "";
                case "Codigo":
                case "CodigoMercadoria":
                    return produto.Codigo ?? "";
                case "Preco":
                    return produto.Preco.ToString("C2");
                case "Quantidade":
                    return produto.Quantidade.ToString();
                case "CodFabricante":
                    return produto.CodFabricante ?? "";
                case "CodBarras":
                    return produto.CodBarras ?? "";
                case "PrecoVenda":
                    return produto.PrecoVenda > 0 ? produto.PrecoVenda.ToString("C2") : produto.Preco.ToString("C2");
                case "VendaA":
                    return produto.VendaA > 0 ? produto.VendaA.ToString("C2") : "-";
                case "VendaB":
                    return produto.VendaB > 0 ? produto.VendaB.ToString("C2") : "-";
                case "VendaC":
                    return produto.VendaC > 0 ? produto.VendaC.ToString("C2") : "-";
                case "VendaD":
                    return produto.VendaD > 0 ? produto.VendaD.ToString("C2") : "-";
                case "VendaE":
                    return produto.VendaE > 0 ? produto.VendaE.ToString("C2") : "-";
                case "Fornecedor":
                    return produto.Fornecedor ?? "";
                case "Fabricante":
                    return produto.Fabricante ?? "";
                case "Grupo":
                    return produto.Grupo ?? "";
                case "Prateleira":
                    return produto.Prateleira ?? "";
                case "Garantia":
                    return produto.Garantia ?? "";
                case "Tam":
                    return produto.Tam ?? "";
                case "Cores":
                    return produto.Cores ?? "";
                case "CodBarras_Grade":
                    return produto.CodBarras_Grade ?? "";
                case "PrecoOriginal":
                    return produto.PrecoOriginal.HasValue ? produto.PrecoOriginal.Value.ToString("C2") : "";
                case "PrecoPromocional":
                    return produto.PrecoPromocional.HasValue ? produto.PrecoPromocional.Value.ToString("C2") : "";
                default:
                    return "";
            }
        }

        private string ObterCodigoBarras(string campo, Produto produto)
        {
            if (produto == null) return "123456789012";

            switch (campo)
            {
                case "CodigoMercadoria":
                    return produto.Codigo ?? "123456789012";
                case "CodFabricante":
                    return produto.CodFabricante ?? "123456789012";
                case "CodBarras":
                    return produto.CodBarras ?? "123456789012";
                case "CodBarras_Grade":
                    return produto.CodBarras_Grade ?? "123456789012";
                default:
                    return produto.Codigo ?? "123456789012";
            }
        }

        private void DesenharCodigoBarras(Graphics g, string codigo, RectangleF bounds)
        {
            string codigoLimpo = new string(Array.FindAll(codigo.ToCharArray(), c => char.IsDigit(c)));

            if (string.IsNullOrEmpty(codigoLimpo))
            {
                g.DrawString("[SEM CÓDIGO]", new Font("Arial", bounds.Height * 0.15f), Brushes.Gray, bounds);
                return;
            }

            try
            {
                Barcode b = new Barcode();

                int larguraPixels = (int)Math.Round(bounds.Width * 2.0f);
                int alturaPixels = (int)Math.Round(bounds.Height * 2.0f);

                if (larguraPixels <= 1 || alturaPixels <= 1)
                {
                    throw new Exception("Dimensões inválidas.");
                }

                b.Width = larguraPixels;
                b.Height = alturaPixels;
                b.IncludeLabel = false;
                b.Alignment = AlignmentPositions.Center;
                b.ForeColor = SKColors.Black;
                b.BackColor = SKColors.White;

                using (SKImage skImage = b.Encode(BarcodeStandard.Type.Code128, codigoLimpo))
                {
                    if (skImage != null)
                    {
                        using (SKData skData = skImage.Encode(SKEncodedImageFormat.Png, 100))
                        {
                            if (skData != null)
                            {
                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                                {
                                    skData.SaveTo(ms);
                                    ms.Seek(0, System.IO.SeekOrigin.Begin);

                                    using (System.Drawing.Image barcodeImage = System.Drawing.Image.FromStream(ms))
                                    {
                                        g.DrawImage(barcodeImage, bounds);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (Font fontErro = new Font("Arial", bounds.Height * 0.10f))
                {
                    StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    g.DrawString($"ERRO: {codigoLimpo}",
                                 fontErro, Brushes.Red, bounds, sf);
                }
            }
        }

        private void BtnImprimir_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDocument printDoc = new PrintDocument();

                if (!string.IsNullOrEmpty(configuracao.ImpressoraPadrao))
                {
                    printDoc.PrinterSettings.PrinterName = configuracao.ImpressoraPadrao;
                }

                bool papelEncontrado = false;
                foreach (PaperSize paperSize in printDoc.PrinterSettings.PaperSizes)
                {
                    if (paperSize.PaperName == nomePapel)
                    {
                        printDoc.DefaultPageSettings.PaperSize = paperSize;
                        papelEncontrado = true;
                        break;
                    }
                }

                if (!papelEncontrado)
                {
                    MessageBox.Show("Papel " + nomePapel + " não encontrado na impressora.\nUsando papel padrão.",
                        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                printDoc.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
                printDoc.PrintPage += PrintDoc_PrintPage;

                PrintDialog printDialog = new PrintDialog();
                printDialog.Document = printDoc;

                if (printDialog.ShowDialog() == DialogResult.OK)
                {
                    printDoc.Print();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao imprimir: " + ex.Message, "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            const float PIXELS_POR_MM_IMPRESSAO = 100f / 25.4f;

            for (int linha = 0; linha < configuracao.NumLinhas; linha++)
            {
                for (int coluna = 0; coluna < configuracao.NumColunas; coluna++)
                {
                    DesenharEtiquetaImpressao(g, linha, coluna, PIXELS_POR_MM_IMPRESSAO);
                }
            }

            e.HasMorePages = false;
        }

        private void DesenharEtiquetaImpressao(Graphics g, int linha, int coluna, float pixelsPorMM)
        {
            float xMM = configuracao.MargemEsquerda +
                       (coluna * (configuracao.LarguraEtiqueta + configuracao.EspacamentoColunas));
            float yMM = configuracao.MargemSuperior +
                       (linha * (configuracao.AlturaEtiqueta + configuracao.EspacamentoLinhas));

            float x = xMM * pixelsPorMM;
            float y = yMM * pixelsPorMM;
            float largura = configuracao.LarguraEtiqueta * pixelsPorMM;
            float altura = configuracao.AlturaEtiqueta * pixelsPorMM;

            RectangleF rectEtiqueta = new RectangleF(x, y, largura, altura);

            Pen penBorda = new Pen(Color.Black, 1);
            g.DrawRectangle(penBorda, x, y, largura, altura);
            penBorda.Dispose();

            Font fonte = new Font("Segoe UI", 10, FontStyle.Bold);
            SolidBrush brush = new SolidBrush(Color.Black);
            StringFormat sf = new StringFormat();
            sf.Alignment = StringAlignment.Center;
            sf.LineAlignment = StringAlignment.Center;

            string texto = string.Format("ETIQUETA\n{0}x{1} mm",
                configuracao.LarguraEtiqueta, configuracao.AlturaEtiqueta);
            g.DrawString(texto, fonte, brush, rectEtiqueta, sf);

            fonte.Dispose();
            brush.Dispose();
            sf.Dispose();
        }
    }
}