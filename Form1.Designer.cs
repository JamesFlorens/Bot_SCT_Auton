namespace Test
{
    partial class Form1
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
            listBox1 = new ListBox();
            button1 = new Button();
            button2 = new Button();
            button3 = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            textBoxApiKey = new TextBox();
            groupBox1 = new GroupBox();
            label1 = new Label();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.BackColor = Color.FromArgb(45, 45, 48);
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Font = new Font("Consolas", 9.75F);
            listBox1.ForeColor = Color.FromArgb(0, 192, 0);
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(5, -3);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(584, 450);
            listBox1.TabIndex = 0;
            // 
            // button1
            // 
            button1.BackColor = Color.FromArgb(0, 122, 204);
            button1.FlatAppearance.BorderSize = 0;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            button1.ForeColor = Color.White;
            button1.Location = new Point(595, 12);
            button1.Name = "button1";
            button1.Size = new Size(193, 35);
            button1.TabIndex = 1;
            button1.Text = "▶ ЗАПУСТИТЬ БОТА";
            button1.UseVisualStyleBackColor = false;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.BackColor = Color.FromArgb(200, 30, 30);
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            button2.ForeColor = Color.White;
            button2.Location = new Point(595, 53);
            button2.Name = "button2";
            button2.Size = new Size(193, 35);
            button2.TabIndex = 2;
            button2.Text = "⏹ ОСТАНОВИТЬ";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // button3
            // 
            button3.BackColor = Color.FromArgb(63, 63, 70);
            button3.FlatAppearance.BorderSize = 0;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Font = new Font("Segoe UI", 9F);
            button3.ForeColor = Color.White;
            button3.Location = new Point(595, 105);
            button3.Name = "button3";
            button3.Size = new Size(193, 35);
            button3.TabIndex = 3;
            button3.Text = "📁 ВЫБРАТЬ EXCEL";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.Transparent;
            button4.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 100);
            button4.FlatStyle = FlatStyle.Flat;
            button4.Font = new Font("Segoe UI", 9F);
            button4.ForeColor = Color.FromArgb(200, 200, 200);
            button4.Location = new Point(595, 410);
            button4.Name = "button4";
            button4.Size = new Size(193, 25);
            button4.TabIndex = 4;
            button4.Text = "ВЫХОД";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.BackColor = Color.FromArgb(63, 63, 70);
            button5.FlatAppearance.BorderSize = 0;
            button5.FlatStyle = FlatStyle.Flat;
            button5.Font = new Font("Segoe UI", 9F);
            button5.ForeColor = Color.White;
            button5.Location = new Point(595, 187);
            button5.Name = "button5";
            button5.Size = new Size(193, 35);
            button5.TabIndex = 5;
            button5.Text = "🔄 ОБНОВИТЬ ТОКЕН";
            button5.UseVisualStyleBackColor = false;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.BackColor = Color.FromArgb(63, 63, 70);
            button6.FlatAppearance.BorderSize = 0;
            button6.FlatStyle = FlatStyle.Flat;
            button6.Font = new Font("Segoe UI", 9F);
            button6.ForeColor = Color.White;
            button6.Location = new Point(595, 146);
            button6.Name = "button6";
            button6.Size = new Size(193, 35);
            button6.TabIndex = 6;
            button6.Text = "📝 ПУТЬ ЛОГОВ";
            button6.UseVisualStyleBackColor = false;
            button6.Click += buttonSelectLogPath_Click;
            // 
            // textBoxApiKey
            // 
            textBoxApiKey.BackColor = Color.FromArgb(30, 30, 30);
            textBoxApiKey.BorderStyle = BorderStyle.FixedSingle;
            textBoxApiKey.Font = new Font("Segoe UI", 8F);
            textBoxApiKey.ForeColor = Color.Gray;
            textBoxApiKey.Location = new Point(595, 255);
            textBoxApiKey.Name = "textBoxApiKey";
            textBoxApiKey.PlaceholderText = "Вставьте API токен сюда";
            textBoxApiKey.Size = new Size(193, 22);
            textBoxApiKey.TabIndex = 7;
            textBoxApiKey.Text = "8485314782:AAGSrJ6HZcdKOFPDfg8BXrpUmIkmbBj9qKo";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label1);
            groupBox1.FlatStyle = FlatStyle.Flat;
            groupBox1.ForeColor = Color.FromArgb(0, 122, 204);
            groupBox1.Location = new Point(595, 290);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(193, 60);
            groupBox1.TabIndex = 8;
            groupBox1.TabStop = false;
            groupBox1.Text = "СИСТЕМА";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(15, 25);
            label1.Name = "label1";
            label1.Size = new Size(107, 15);
            label1.TabIndex = 0;
            label1.Text = "БОТ ВЫКЛЮЧЕН";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(28, 28, 28);
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox1);
            Controls.Add(textBoxApiKey);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(listBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "Bot Panel [v2.0]";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ListBox listBox1;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;
        private TextBox textBoxApiKey;
        private GroupBox groupBox1;
        private Label label1;
    }
}