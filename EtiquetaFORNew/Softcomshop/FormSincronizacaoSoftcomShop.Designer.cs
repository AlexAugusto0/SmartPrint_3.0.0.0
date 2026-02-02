namespace EtiquetaFORNew
{
    partial class FormSincronizacaoSoftcomShop
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSincronizarProdutos = new System.Windows.Forms.Button();
            this.btnBuscarNotaFiscal = new System.Windows.Forms.Button();
            this.btnBuscarVenda = new System.Windows.Forms.Button();
            this.lblUltimaSinc = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnFechar = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSincronizarProdutos);
            this.groupBox1.Controls.Add(this.btnBuscarNotaFiscal);
            this.groupBox1.Controls.Add(this.btnBuscarVenda);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 50);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(460, 180);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operações Disponíveis";
            // 
            // btnSincronizarProdutos
            // 
            this.btnSincronizarProdutos.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSincronizarProdutos.Location = new System.Drawing.Point(20, 30);
            this.btnSincronizarProdutos.Name = "btnSincronizarProdutos";
            this.btnSincronizarProdutos.Size = new System.Drawing.Size(420, 35);
            this.btnSincronizarProdutos.TabIndex = 0;
            this.btnSincronizarProdutos.Text = "Sincronizar Todos os Produtos";
            this.btnSincronizarProdutos.UseVisualStyleBackColor = true;
            this.btnSincronizarProdutos.Click += new System.EventHandler(this.btnSincronizarProdutos_Click);
            // 
            // btnBuscarNotaFiscal
            // 
            this.btnBuscarNotaFiscal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarNotaFiscal.Location = new System.Drawing.Point(20, 80);
            this.btnBuscarNotaFiscal.Name = "btnBuscarNotaFiscal";
            this.btnBuscarNotaFiscal.Size = new System.Drawing.Size(420, 35);
            this.btnBuscarNotaFiscal.TabIndex = 1;
            this.btnBuscarNotaFiscal.Text = "Buscar por Nota Fiscal";
            this.btnBuscarNotaFiscal.UseVisualStyleBackColor = true;
            this.btnBuscarNotaFiscal.Click += new System.EventHandler(this.btnBuscarNotaFiscal_Click);
            // 
            // btnBuscarVenda
            // 
            this.btnBuscarVenda.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscarVenda.Location = new System.Drawing.Point(20, 130);
            this.btnBuscarVenda.Name = "btnBuscarVenda";
            this.btnBuscarVenda.Size = new System.Drawing.Size(420, 35);
            this.btnBuscarVenda.TabIndex = 2;
            this.btnBuscarVenda.Text = "Buscar por Venda";
            this.btnBuscarVenda.UseVisualStyleBackColor = true;
            this.btnBuscarVenda.Click += new System.EventHandler(this.btnBuscarVenda_Click);
            // 
            // lblUltimaSinc
            // 
            this.lblUltimaSinc.AutoSize = true;
            this.lblUltimaSinc.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUltimaSinc.Location = new System.Drawing.Point(12, 15);
            this.lblUltimaSinc.Name = "lblUltimaSinc";
            this.lblUltimaSinc.Size = new System.Drawing.Size(166, 15);
            this.lblUltimaSinc.TabIndex = 1;
            this.lblUltimaSinc.Text = "Última sincronização: --/--/----";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(12, 250);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(43, 15);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "Pronto";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 270);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(460, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 3;
            this.progressBar.Visible = false;
            // 
            // btnFechar
            // 
            this.btnFechar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFechar.Location = new System.Drawing.Point(382, 305);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(90, 30);
            this.btnFechar.TabIndex = 4;
            this.btnFechar.Text = "Fechar";
            this.btnFechar.UseVisualStyleBackColor = true;
            this.btnFechar.Click += new System.EventHandler(this.btnFechar_Click);
            // 
            // FormSincronizacaoSoftcomShop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 347);
            this.Controls.Add(this.btnFechar);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblUltimaSinc);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormSincronizacaoSoftcomShop";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sincronização SoftcomShop";
            this.Load += new System.EventHandler(this.FormSincronizacaoSoftcomShop_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSincronizarProdutos;
        private System.Windows.Forms.Button btnBuscarNotaFiscal;
        private System.Windows.Forms.Button btnBuscarVenda;
        private System.Windows.Forms.Label lblUltimaSinc;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnFechar;
    }
}
