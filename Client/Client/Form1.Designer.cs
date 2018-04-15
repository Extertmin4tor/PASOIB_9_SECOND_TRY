namespace Client
{
    partial class MainWindow
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.Send = new System.Windows.Forms.Button();
            this.Cipher = new System.Windows.Forms.RadioButton();
            this.Sign = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.CerificatesBox = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.HashBox = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.GetCerts = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Send
            // 
            this.Send.Location = new System.Drawing.Point(106, 179);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(197, 42);
            this.Send.TabIndex = 0;
            this.Send.Text = "Send";
            this.Send.UseVisualStyleBackColor = true;
            this.Send.Click += new System.EventHandler(this.SendData);
            // 
            // Cipher
            // 
            this.Cipher.AutoSize = true;
            this.Cipher.Checked = true;
            this.Cipher.Location = new System.Drawing.Point(187, 12);
            this.Cipher.Name = "Cipher";
            this.Cipher.Size = new System.Drawing.Size(55, 17);
            this.Cipher.TabIndex = 1;
            this.Cipher.TabStop = true;
            this.Cipher.Text = "Cipher";
            this.Cipher.UseVisualStyleBackColor = true;
            this.Cipher.CheckedChanged += new System.EventHandler(this.Cipher_CheckedChanged);
            // 
            // Sign
            // 
            this.Sign.AutoSize = true;
            this.Sign.Location = new System.Drawing.Point(248, 12);
            this.Sign.Name = "Sign";
            this.Sign.Size = new System.Drawing.Size(46, 17);
            this.Sign.TabIndex = 3;
            this.Sign.Text = "Sign";
            this.Sign.UseVisualStyleBackColor = true;
            this.Sign.CheckedChanged += new System.EventHandler(this.SignChecked);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 11);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(169, 93);
            this.textBox1.TabIndex = 4;
            // 
            // CerificatesBox
            // 
            this.CerificatesBox.FormattingEnabled = true;
            this.CerificatesBox.Location = new System.Drawing.Point(186, 35);
            this.CerificatesBox.Name = "CerificatesBox";
            this.CerificatesBox.Size = new System.Drawing.Size(197, 21);
            this.CerificatesBox.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 110);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "Hash";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ComputeHash);
            // 
            // HashBox
            // 
            this.HashBox.Location = new System.Drawing.Point(12, 139);
            this.HashBox.Multiline = true;
            this.HashBox.Name = "HashBox";
            this.HashBox.Size = new System.Drawing.Size(371, 34);
            this.HashBox.TabIndex = 7;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(227, 62);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(157, 20);
            this.textBox2.TabIndex = 8;
            this.textBox2.Text = "127.0.0.1";
            this.textBox2.Click += new System.EventHandler(this.IpClick);
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(227, 88);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(116, 20);
            this.textBox3.TabIndex = 9;
            this.textBox3.Text = "8005";
            this.textBox3.Click += new System.EventHandler(this.PortClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(186, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(17, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "IP";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(186, 94);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(26, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Port";
            // 
            // GetCerts
            // 
            this.GetCerts.Location = new System.Drawing.Point(309, 9);
            this.GetCerts.Name = "GetCerts";
            this.GetCerts.Size = new System.Drawing.Size(75, 23);
            this.GetCerts.TabIndex = 12;
            this.GetCerts.Text = "Get certs";
            this.GetCerts.UseVisualStyleBackColor = true;
            this.GetCerts.Click += new System.EventHandler(this.GetCertsFromServer);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 223);
            this.Controls.Add(this.GetCerts);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.HashBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.CerificatesBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Sign);
            this.Controls.Add(this.Cipher);
            this.Controls.Add(this.Send);
            this.Name = "MainWindow";
            this.Text = "Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Send;
        private System.Windows.Forms.RadioButton Cipher;
        private System.Windows.Forms.RadioButton Sign;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox CerificatesBox;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox HashBox;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button GetCerts;
    }
}

