namespace EtiquetaFORNew
{
    partial class telaTecnico
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(telaTecnico));
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.panelPrincipal = new System.Windows.Forms.Panel();
            this.panelConteudo = new System.Windows.Forms.Panel();
            this.groupBoxDeteccao = new System.Windows.Forms.GroupBox();
            this.listViewDispositivos = new System.Windows.Forms.ListView();
            this.btnInstalarDriver = new System.Windows.Forms.Button();
            this.btnProcurar = new System.Windows.Forms.Button();
            this.groupBoxManual = new System.Windows.Forms.GroupBox();
            this.lblSelecione = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnDownloadDriver = new System.Windows.Forms.Button();
            this.panelModo = new System.Windows.Forms.Panel();
            this.lblModo = new System.Windows.Forms.Label();
            this.panelTitulo = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.panelPrincipal.SuspendLayout();
            this.panelConteudo.SuspendLayout();
            this.groupBoxDeteccao.SuspendLayout();
            this.groupBoxManual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panelModo.SuspendLayout();
            this.panelTitulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.checkBox1.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.checkBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.checkBox1.FlatAppearance.BorderSize = 2;
            this.checkBox1.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.checkBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.checkBox1.Location = new System.Drawing.Point(252, 15);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(200, 45);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "🔍 Detecção Automática";
            this.checkBox1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox1.UseVisualStyleBackColor = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.Appearance = System.Windows.Forms.Appearance.Button;
            this.checkBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(240)))), ((int)(((byte)(241)))));
            this.checkBox2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkBox2.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.checkBox2.FlatAppearance.BorderSize = 2;
            this.checkBox2.FlatAppearance.CheckedBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.checkBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox2.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.checkBox2.Location = new System.Drawing.Point(472, 15);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(200, 45);
            this.checkBox2.TabIndex = 2;
            this.checkBox2.Text = "📋 Instalação Manual";
            this.checkBox2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBox2.UseVisualStyleBackColor = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // panelPrincipal
            // 
            this.panelPrincipal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.panelPrincipal.Controls.Add(this.panelConteudo);
            this.panelPrincipal.Controls.Add(this.panelModo);
            this.panelPrincipal.Controls.Add(this.panelTitulo);
            this.panelPrincipal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPrincipal.Location = new System.Drawing.Point(0, 0);
            this.panelPrincipal.Name = "panelPrincipal";
            this.panelPrincipal.Padding = new System.Windows.Forms.Padding(20);
            this.panelPrincipal.Size = new System.Drawing.Size(900, 600);
            this.panelPrincipal.TabIndex = 0;
            // 
            // panelConteudo
            // 
            this.panelConteudo.BackColor = System.Drawing.Color.White;
            this.panelConteudo.Controls.Add(this.groupBoxDeteccao);
            this.panelConteudo.Controls.Add(this.groupBoxManual);
            this.panelConteudo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelConteudo.Location = new System.Drawing.Point(20, 160);
            this.panelConteudo.Name = "panelConteudo";
            this.panelConteudo.Padding = new System.Windows.Forms.Padding(20);
            this.panelConteudo.Size = new System.Drawing.Size(860, 420);
            this.panelConteudo.TabIndex = 2;
            // 
            // groupBoxDeteccao
            // 
            this.groupBoxDeteccao.Controls.Add(this.listViewDispositivos);
            this.groupBoxDeteccao.Controls.Add(this.btnInstalarDriver);
            this.groupBoxDeteccao.Controls.Add(this.btnProcurar);
            this.groupBoxDeteccao.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxDeteccao.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxDeteccao.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.groupBoxDeteccao.Location = new System.Drawing.Point(20, 20);
            this.groupBoxDeteccao.Name = "groupBoxDeteccao";
            this.groupBoxDeteccao.Padding = new System.Windows.Forms.Padding(15);
            this.groupBoxDeteccao.Size = new System.Drawing.Size(820, 380);
            this.groupBoxDeteccao.TabIndex = 0;
            this.groupBoxDeteccao.TabStop = false;
            this.groupBoxDeteccao.Text = "Impressoras USB Detectadas";
            this.groupBoxDeteccao.Visible = false;
            // 
            // listViewDispositivos
            // 
            this.listViewDispositivos.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewDispositivos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewDispositivos.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listViewDispositivos.FullRowSelect = true;
            this.listViewDispositivos.GridLines = true;
            this.listViewDispositivos.HideSelection = false;
            this.listViewDispositivos.Location = new System.Drawing.Point(15, 35);
            this.listViewDispositivos.Name = "listViewDispositivos";
            this.listViewDispositivos.Size = new System.Drawing.Size(790, 270);
            this.listViewDispositivos.TabIndex = 0;
            this.listViewDispositivos.UseCompatibleStateImageBehavior = false;
            this.listViewDispositivos.View = System.Windows.Forms.View.Details;
            // 
            // btnInstalarDriver
            // 
            this.btnInstalarDriver.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnInstalarDriver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(143)))), ((int)(((byte)(0)))));
            this.btnInstalarDriver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInstalarDriver.FlatAppearance.BorderSize = 0;
            this.btnInstalarDriver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInstalarDriver.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInstalarDriver.ForeColor = System.Drawing.Color.Black;
            this.btnInstalarDriver.Location = new System.Drawing.Point(210, 313);
            this.btnInstalarDriver.Name = "btnInstalarDriver";
            this.btnInstalarDriver.Size = new System.Drawing.Size(180, 57);
            this.btnInstalarDriver.TabIndex = 2;
            this.btnInstalarDriver.Text = "⚡ Instalar Driver";
            this.btnInstalarDriver.UseVisualStyleBackColor = false;
            this.btnInstalarDriver.Click += new System.EventHandler(this.btnInstalarDriver_Click);
            // 
            // btnProcurar
            // 
            this.btnProcurar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnProcurar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnProcurar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnProcurar.FlatAppearance.BorderSize = 0;
            this.btnProcurar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProcurar.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnProcurar.ForeColor = System.Drawing.Color.Black;
            this.btnProcurar.Location = new System.Drawing.Point(15, 313);
            this.btnProcurar.Name = "btnProcurar";
            this.btnProcurar.Size = new System.Drawing.Size(180, 57);
            this.btnProcurar.TabIndex = 1;
            this.btnProcurar.Text = "🔍 Procurar Impressoras";
            this.btnProcurar.UseVisualStyleBackColor = false;
            this.btnProcurar.Click += new System.EventHandler(this.btnProcurar_Click);
            // 
            // groupBoxManual
            // 
            this.groupBoxManual.Controls.Add(this.lblSelecione);
            this.groupBoxManual.Controls.Add(this.comboBox1);
            this.groupBoxManual.Controls.Add(this.pictureBox1);
            this.groupBoxManual.Controls.Add(this.btnDownloadDriver);
            this.groupBoxManual.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxManual.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBoxManual.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.groupBoxManual.Location = new System.Drawing.Point(20, 20);
            this.groupBoxManual.Name = "groupBoxManual";
            this.groupBoxManual.Padding = new System.Windows.Forms.Padding(15);
            this.groupBoxManual.Size = new System.Drawing.Size(820, 380);
            this.groupBoxManual.TabIndex = 1;
            this.groupBoxManual.TabStop = false;
            this.groupBoxManual.Text = "Selecione o Modelo da Impressora";
            this.groupBoxManual.Visible = false;
            // 
            // lblSelecione
            // 
            this.lblSelecione.AutoSize = true;
            this.lblSelecione.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelecione.Location = new System.Drawing.Point(15, 40);
            this.lblSelecione.Name = "lblSelecione";
            this.lblSelecione.Size = new System.Drawing.Size(205, 17);
            this.lblSelecione.TabIndex = 0;
            this.lblSelecione.Text = "Escolha o modelo da impressora:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(15, 65);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(400, 25);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(15, 110);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(400, 200);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btnDownloadDriver
            // 
            this.btnDownloadDriver.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnDownloadDriver.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDownloadDriver.FlatAppearance.BorderSize = 0;
            this.btnDownloadDriver.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDownloadDriver.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDownloadDriver.ForeColor = System.Drawing.Color.White;
            this.btnDownloadDriver.Location = new System.Drawing.Point(15, 325);
            this.btnDownloadDriver.Name = "btnDownloadDriver";
            this.btnDownloadDriver.Size = new System.Drawing.Size(200, 40);
            this.btnDownloadDriver.TabIndex = 3;
            this.btnDownloadDriver.Text = "📥 Baixar e Instalar Driver";
            this.btnDownloadDriver.UseVisualStyleBackColor = false;
            this.btnDownloadDriver.Click += new System.EventHandler(this.button1_Click);
            // 
            // panelModo
            // 
            this.panelModo.BackColor = System.Drawing.Color.White;
            this.panelModo.Controls.Add(this.lblModo);
            this.panelModo.Controls.Add(this.checkBox1);
            this.panelModo.Controls.Add(this.checkBox2);
            this.panelModo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelModo.Location = new System.Drawing.Point(20, 80);
            this.panelModo.Name = "panelModo";
            this.panelModo.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.panelModo.Size = new System.Drawing.Size(860, 80);
            this.panelModo.TabIndex = 1;
            // 
            // lblModo
            // 
            this.lblModo.AutoSize = true;
            this.lblModo.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(73)))), ((int)(((byte)(94)))));
            this.lblModo.Location = new System.Drawing.Point(20, 15);
            this.lblModo.Name = "lblModo";
            this.lblModo.Size = new System.Drawing.Size(226, 19);
            this.lblModo.TabIndex = 0;
            this.lblModo.Text = "Selecione o modo de instalação:";
            // 
            // panelTitulo
            // 
            this.panelTitulo.BackColor = System.Drawing.Color.DarkGray;
            this.panelTitulo.Controls.Add(this.pictureBox2);
            this.panelTitulo.Controls.Add(this.lblTitulo);
            this.panelTitulo.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTitulo.Location = new System.Drawing.Point(20, 20);
            this.panelTitulo.Name = "panelTitulo";
            this.panelTitulo.Size = new System.Drawing.Size(860, 60);
            this.panelTitulo.TabIndex = 0;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::EtiquetaFORNew.Properties.Resources.icone_novo_2025_PNG;
            this.pictureBox2.Location = new System.Drawing.Point(3, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(67, 54);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // lblTitulo
            // 
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.Black;
            this.lblTitulo.Location = new System.Drawing.Point(63, 15);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new System.Drawing.Size(254, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Instalação de Drivers";
            // 
            // telaTecnico
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.panelPrincipal);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(900, 600);
            this.Name = "telaTecnico";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.panelPrincipal.ResumeLayout(false);
            this.panelConteudo.ResumeLayout(false);
            this.groupBoxDeteccao.ResumeLayout(false);
            this.groupBoxManual.ResumeLayout(false);
            this.groupBoxManual.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panelModo.ResumeLayout(false);
            this.panelModo.PerformLayout();
            this.panelTitulo.ResumeLayout(false);
            this.panelTitulo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelPrincipal;
        private System.Windows.Forms.Panel panelTitulo;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel panelModo;
        private System.Windows.Forms.Label lblModo;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Panel panelConteudo;
        private System.Windows.Forms.GroupBox groupBoxDeteccao;
        private System.Windows.Forms.ListView listViewDispositivos;
        private System.Windows.Forms.Button btnProcurar;
        private System.Windows.Forms.Button btnInstalarDriver;
        private System.Windows.Forms.GroupBox groupBoxManual;
        private System.Windows.Forms.Label lblSelecione;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnDownloadDriver;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}