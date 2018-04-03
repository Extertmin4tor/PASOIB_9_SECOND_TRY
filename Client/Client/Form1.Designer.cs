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
            this.Hash = new System.Windows.Forms.RadioButton();
            this.Sign = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.CerificatesBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // Send
            // 
            this.Send.Location = new System.Drawing.Point(12, 110);
            this.Send.Name = "Send";
            this.Send.Size = new System.Drawing.Size(75, 23);
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
            // Hash
            // 
            this.Hash.AutoSize = true;
            this.Hash.Location = new System.Drawing.Point(187, 35);
            this.Hash.Name = "Hash";
            this.Hash.Size = new System.Drawing.Size(50, 17);
            this.Hash.TabIndex = 2;
            this.Hash.Text = "Hash";
            this.Hash.UseVisualStyleBackColor = true;
            // 
            // Sign
            // 
            this.Sign.AutoSize = true;
            this.Sign.Location = new System.Drawing.Point(187, 58);
            this.Sign.Name = "Sign";
            this.Sign.Size = new System.Drawing.Size(46, 17);
            this.Sign.TabIndex = 3;
            this.Sign.Text = "Sign";
            this.Sign.UseVisualStyleBackColor = true;
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
            this.CerificatesBox.Location = new System.Drawing.Point(187, 81);
            this.CerificatesBox.Name = "CerificatesBox";
            this.CerificatesBox.Size = new System.Drawing.Size(74, 21);
            this.CerificatesBox.TabIndex = 5;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(266, 147);
            this.Controls.Add(this.CerificatesBox);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.Sign);
            this.Controls.Add(this.Hash);
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
        private System.Windows.Forms.RadioButton Hash;
        private System.Windows.Forms.RadioButton Sign;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox CerificatesBox;
    }
}

