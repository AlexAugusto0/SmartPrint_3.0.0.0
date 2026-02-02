namespace EtiquetaFORNew
{
    partial class FormConfiguracao
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.cboTipoConexao = new System.Windows.Forms.ComboBox();
            this.panelSqlServer = new System.Windows.Forms.Panel();
            this.panelSoftcomShop = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtUrlDispositivo = new System.Windows.Forms.TextBox();
            this.labelUrlDispositivo = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtBaseURL = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtClientId = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtClientSecret = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEmpresaName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtEmpresaCNPJ = new System.Windows.Forms.MaskedTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtDeviceId = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.lblCaracteres = new System.Windows.Forms.Label();
            this.btnTestarConexao = new System.Windows.Forms.Button();
            this.btnSalvar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.panelSoftcomShop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Tipo de Conexão:";
            // 
            // cboTipoConexao
            // 
            this.cboTipoConexao.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTipoConexao.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.cboTipoConexao.FormattingEnabled = true;
            this.cboTipoConexao.Location = new System.Drawing.Point(134, 12);
            this.cboTipoConexao.Name = "cboTipoConexao";
            this.cboTipoConexao.Size = new System.Drawing.Size(200, 23);
            this.cboTipoConexao.TabIndex = 1;
            this.cboTipoConexao.SelectedIndexChanged += new System.EventHandler(this.cboTipoConexao_SelectedIndexChanged);
            // 
            // panelSqlServer
            // 
            this.panelSqlServer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSqlServer.Location = new System.Drawing.Point(12, 50);
            this.panelSqlServer.Name = "panelSqlServer";
            this.panelSqlServer.Size = new System.Drawing.Size(660, 450);
            this.panelSqlServer.TabIndex = 2;
            this.panelSqlServer.Visible = false;
            // 
            // panelSoftcomShop
            // 
            this.panelSoftcomShop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelSoftcomShop.Controls.Add(this.groupBox1);
            this.panelSoftcomShop.Location = new System.Drawing.Point(12, 50);
            this.panelSoftcomShop.Name = "panelSoftcomShop";
            this.panelSoftcomShop.Size = new System.Drawing.Size(660, 460);
            this.panelSoftcomShop.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(658, 458);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Configurações SoftcomShop";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labelUrlDispositivo);
            this.groupBox2.Controls.Add(this.txtUrlDispositivo);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox2.ForeColor = System.Drawing.Color.Blue;
            this.groupBox2.Location = new System.Drawing.Point(15, 25);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(625, 85);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "1 - Cole a URL do dispositivo cadastrado no Softcomshop ou informe os dados.";
            // 
            // labelUrlDispositivo
            // 
            this.labelUrlDispositivo.AutoSize = true;
            this.labelUrlDispositivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.labelUrlDispositivo.ForeColor = System.Drawing.Color.Black;
            this.labelUrlDispositivo.Location = new System.Drawing.Point(10, 20);
            this.labelUrlDispositivo.Name = "labelUrlDispositivo";
            this.labelUrlDispositivo.Size = new System.Drawing.Size(278, 13);
            this.labelUrlDispositivo.TabIndex = 0;
            this.labelUrlDispositivo.Text = "Cole aqui a URL (preenche os campos automaticamente):";
            // 
            // txtUrlDispositivo
            // 
            this.txtUrlDispositivo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtUrlDispositivo.Location = new System.Drawing.Point(13, 38);
            this.txtUrlDispositivo.Multiline = true;
            this.txtUrlDispositivo.Name = "txtUrlDispositivo";
            this.txtUrlDispositivo.Size = new System.Drawing.Size(600, 35);
            this.txtUrlDispositivo.TabIndex = 1;
            this.txtUrlDispositivo.Leave += new System.EventHandler(this.txtUrlDispositivo_Leave);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtDeviceName);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtEmpresaCNPJ);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.txtEmpresaName);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtClientSecret);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtClientId);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.txtBaseURL);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.groupBox3.Location = new System.Drawing.Point(15, 120);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(625, 220);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Base URL";
            // 
            // txtBaseURL
            // 
            this.txtBaseURL.Location = new System.Drawing.Point(13, 30);
            this.txtBaseURL.Name = "txtBaseURL";
            this.txtBaseURL.ReadOnly = true;
            this.txtBaseURL.Size = new System.Drawing.Size(290, 20);
            this.txtBaseURL.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(320, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "Empresa Name";
            // 
            // txtEmpresaName
            // 
            this.txtEmpresaName.Location = new System.Drawing.Point(323, 30);
            this.txtEmpresaName.Name = "txtEmpresaName";
            this.txtEmpresaName.ReadOnly = true;
            this.txtEmpresaName.Size = new System.Drawing.Size(290, 20);
            this.txtEmpresaName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Client Id";
            // 
            // txtClientId
            // 
            this.txtClientId.Location = new System.Drawing.Point(13, 75);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.ReadOnly = true;
            this.txtClientId.Size = new System.Drawing.Size(290, 20);
            this.txtClientId.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(320, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Empresa CNPJ";
            // 
            // txtEmpresaCNPJ
            // 
            this.txtEmpresaCNPJ.Location = new System.Drawing.Point(323, 75);
            this.txtEmpresaCNPJ.Mask = "00.000.000/0000-00";
            this.txtEmpresaCNPJ.Name = "txtEmpresaCNPJ";
            this.txtEmpresaCNPJ.ReadOnly = true;
            this.txtEmpresaCNPJ.Size = new System.Drawing.Size(290, 20);
            this.txtEmpresaCNPJ.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(10, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Device Name";
            // 
            // txtDeviceName
            // 
            this.txtDeviceName.Location = new System.Drawing.Point(13, 120);
            this.txtDeviceName.Name = "txtDeviceName";
            this.txtDeviceName.ReadOnly = true;
            this.txtDeviceName.Size = new System.Drawing.Size(600, 20);
            this.txtDeviceName.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Client Secret";
            // 
            // txtClientSecret
            // 
            this.txtClientSecret.Location = new System.Drawing.Point(13, 165);
            this.txtClientSecret.Name = "txtClientSecret";
            this.txtClientSecret.ReadOnly = true;
            this.txtClientSecret.Size = new System.Drawing.Size(600, 20);
            this.txtClientSecret.TabIndex = 11;
            this.txtClientSecret.UseSystemPasswordChar = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnTestarConexao);
            this.groupBox4.Controls.Add(this.lblCaracteres);
            this.groupBox4.Controls.Add(this.txtDeviceId);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.groupBox4.ForeColor = System.Drawing.Color.Blue;
            this.groupBox4.Location = new System.Drawing.Point(15, 350);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(625, 95);
            this.groupBox4.TabIndex = 2;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "2- Informe uma chave de no mínimo 16 caracteres e clique em conectar.";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(10, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Device Id";
            // 
            // txtDeviceId
            // 
            this.txtDeviceId.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.txtDeviceId.Location = new System.Drawing.Point(13, 43);
            this.txtDeviceId.MaxLength = 50;
            this.txtDeviceId.Name = "txtDeviceId";
            this.txtDeviceId.Size = new System.Drawing.Size(300, 20);
            this.txtDeviceId.TabIndex = 1;
            this.txtDeviceId.TextChanged += new System.EventHandler(this.txtDeviceId_TextChanged);
            // 
            // lblCaracteres
            // 
            this.lblCaracteres.AutoSize = true;
            this.lblCaracteres.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F);
            this.lblCaracteres.ForeColor = System.Drawing.Color.Gray;
            this.lblCaracteres.Location = new System.Drawing.Point(11, 67);
            this.lblCaracteres.Name = "lblCaracteres";
            this.lblCaracteres.Size = new System.Drawing.Size(61, 13);
            this.lblCaracteres.TabIndex = 2;
            this.lblCaracteres.Text = "0 caracteres";
            // 
            // btnTestarConexao
            // 
            this.btnTestarConexao.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnTestarConexao.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestarConexao.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnTestarConexao.ForeColor = System.Drawing.Color.White;
            this.btnTestarConexao.Location = new System.Drawing.Point(330, 35);
            this.btnTestarConexao.Name = "btnTestarConexao";
            this.btnTestarConexao.Size = new System.Drawing.Size(130, 35);
            this.btnTestarConexao.TabIndex = 3;
            this.btnTestarConexao.Text = "🔌 Conectar";
            this.btnTestarConexao.UseVisualStyleBackColor = false;
            this.btnTestarConexao.Click += new System.EventHandler(this.btnTestarConexao_Click);
            // 
            // btnSalvar
            // 
            this.btnSalvar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            this.btnSalvar.Location = new System.Drawing.Point(497, 525);
            this.btnSalvar.Name = "btnSalvar";
            this.btnSalvar.Size = new System.Drawing.Size(85, 30);
            this.btnSalvar.TabIndex = 4;
            this.btnSalvar.Text = "Salvar";
            this.btnSalvar.UseVisualStyleBackColor = true;
            this.btnSalvar.Click += new System.EventHandler(this.btnSalvar_Click);
            // 
            // btnCancelar
            // 
            this.btnCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F);
            this.btnCancelar.Location = new System.Drawing.Point(588, 525);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(85, 30);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // FormConfiguracao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 571);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnSalvar);
            this.Controls.Add(this.panelSoftcomShop);
            this.Controls.Add(this.panelSqlServer);
            this.Controls.Add(this.cboTipoConexao);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormConfiguracao";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configurações do Sistema";
            this.Load += new System.EventHandler(this.FormConfiguracao_Load);
            this.panelSoftcomShop.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboTipoConexao;
        private System.Windows.Forms.Panel panelSqlServer;
        private System.Windows.Forms.Panel panelSoftcomShop;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtUrlDispositivo;
        private System.Windows.Forms.Label labelUrlDispositivo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtBaseURL;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtClientId;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtClientSecret;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEmpresaName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.MaskedTextBox txtEmpresaCNPJ;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtDeviceId;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblCaracteres;
        private System.Windows.Forms.Button btnTestarConexao;
        private System.Windows.Forms.Button btnSalvar;
        private System.Windows.Forms.Button btnCancelar;
    }
}