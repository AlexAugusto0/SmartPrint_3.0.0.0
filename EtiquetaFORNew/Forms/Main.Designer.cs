namespace EtiquetaFORNew
{
    partial class Main
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.usuarioBox = new System.Windows.Forms.ComboBox();
            this.senhaBox = new System.Windows.Forms.TextBox();
            this.Usuario = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnLogar = new System.Windows.Forms.Button();
            this.btnFechar = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.Versao = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.btnConfig = new System.Windows.Forms.Button();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // usuarioBox
            // 
            this.usuarioBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.usuarioBox.FormattingEnabled = true;
            this.usuarioBox.Location = new System.Drawing.Point(62, 78);
            this.usuarioBox.Name = "usuarioBox";
            this.usuarioBox.Size = new System.Drawing.Size(187, 21);
            this.usuarioBox.TabIndex = 0;
            // 
            // senhaBox
            // 
            this.senhaBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.senhaBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.senhaBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.senhaBox.Location = new System.Drawing.Point(62, 117);
            this.senhaBox.Name = "senhaBox";
            this.senhaBox.Size = new System.Drawing.Size(187, 20);
            this.senhaBox.TabIndex = 1;
            // 
            // Usuario
            // 
            this.Usuario.AutoSize = true;
            this.Usuario.ForeColor = System.Drawing.SystemColors.Desktop;
            this.Usuario.Location = new System.Drawing.Point(10, 81);
            this.Usuario.Name = "Usuario";
            this.Usuario.Size = new System.Drawing.Size(46, 13);
            this.Usuario.TabIndex = 6;
            this.Usuario.Text = "Usuário:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(10, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Senha:";
            // 
            // btnLogar
            // 
            this.btnLogar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogar.Location = new System.Drawing.Point(170, 161);
            this.btnLogar.Name = "btnLogar";
            this.btnLogar.Size = new System.Drawing.Size(79, 21);
            this.btnLogar.TabIndex = 2;
            this.btnLogar.Text = "Logar";
            this.btnLogar.UseVisualStyleBackColor = true;
            this.btnLogar.Click += new System.EventHandler(this.btnLogar_Click);
            // 
            // btnFechar
            // 
            this.btnFechar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFechar.Location = new System.Drawing.Point(62, 162);
            this.btnFechar.Name = "btnFechar";
            this.btnFechar.Size = new System.Drawing.Size(82, 20);
            this.btnFechar.TabIndex = 3;
            this.btnFechar.Text = "Fechar";
            this.btnFechar.UseVisualStyleBackColor = true;
            this.btnFechar.Click += new System.EventHandler(this.btnFechar_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Location = new System.Drawing.Point(12, 35);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(333, 373);
            this.panel1.TabIndex = 11;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.Versao);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnFechar);
            this.panel2.Controls.Add(this.btnLogar);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.Usuario);
            this.panel2.Controls.Add(this.senhaBox);
            this.panel2.Controls.Add(this.usuarioBox);
            this.panel2.Location = new System.Drawing.Point(20, 115);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(292, 244);
            this.panel2.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(85, 222);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Banco (SQL SERVER)";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(68, 195);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 13);
            this.label4.TabIndex = 14;
            this.label4.Text = "SmartPrint    |";
            // 
            // Versao
            // 
            this.Versao.AutoSize = true;
            this.Versao.Location = new System.Drawing.Point(140, 195);
            this.Versao.Name = "Versao";
            this.Versao.Size = new System.Drawing.Size(58, 13);
            this.Versao.TabIndex = 13;
            this.Versao.Text = "Versão 1.0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Olá, Seja Bem Vindo!";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.Transparent;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.button1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.Image = global::EtiquetaFORNew.Properties.Resources.pngegg30x30;
            this.button1.Location = new System.Drawing.Point(650, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(45, 34);
            this.button1.TabIndex = 14;
            this.toolTip1.SetToolTip(this.button1, "Instalação de Drivers");
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.pictureBox5_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.BackColor = System.Drawing.Color.Transparent;
            this.btnConfig.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.btnConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnConfig.FlatAppearance.BorderSize = 0;
            this.btnConfig.Image = global::EtiquetaFORNew.Properties.Resources.Engrenagem20x20;
            this.btnConfig.Location = new System.Drawing.Point(651, 12);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(45, 34);
            this.btnConfig.TabIndex = 13;
            this.toolTip1.SetToolTip(this.btnConfig, "Configurações");
            this.btnConfig.UseVisualStyleBackColor = false;
            this.btnConfig.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // pictureBox5
            // 
            this.pictureBox5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox5.Image = global::EtiquetaFORNew.Properties.Resources.pngegg;
            this.pictureBox5.Location = new System.Drawing.Point(652, 133);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(44, 35);
            this.pictureBox5.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox5.TabIndex = 12;
            this.pictureBox5.TabStop = false;
            this.pictureBox5.Visible = false;
            this.pictureBox5.Click += new System.EventHandler(this.pictureBox5_Click);
            // 
            // pictureBox4
            // 
            this.pictureBox4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox4.Image = global::EtiquetaFORNew.Properties.Resources.Engrenagem;
            this.pictureBox4.Location = new System.Drawing.Point(652, 100);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(44, 27);
            this.pictureBox4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox4.TabIndex = 3;
            this.pictureBox4.TabStop = false;
            this.pictureBox4.Visible = false;
            this.pictureBox4.Click += new System.EventHandler(this.pictureBox4_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::EtiquetaFORNew.Properties.Resources.icone_novo_2025_PNG;
            this.pictureBox1.Location = new System.Drawing.Point(16, 14);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(58, 78);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::EtiquetaFORNew.Properties.Resources.SoftcomTecnologia2;
            this.pictureBox2.Location = new System.Drawing.Point(33, 14);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(304, 78);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::EtiquetaFORNew.Properties.Resources.LogoLogin;
            this.pictureBox3.Location = new System.Drawing.Point(272, -99);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(492, 580);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(235)))), ((int)(((byte)(255)))));
            this.ClientSize = new System.Drawing.Size(707, 432);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnConfig);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pictureBox3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Main_Load);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.ComboBox usuarioBox;
        private System.Windows.Forms.TextBox senhaBox;
        private System.Windows.Forms.Label Usuario;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnLogar;
        private System.Windows.Forms.Button btnFechar;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Label Versao;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.PictureBox pictureBox5;
        private System.Windows.Forms.Button btnConfig;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

