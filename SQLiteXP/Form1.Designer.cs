using System;

namespace SQLiteXP
{
    partial class Form1
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
            this.tableLayoutPanel_main = new System.Windows.Forms.TableLayoutPanel();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.podesavanjaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.proizvodiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kupciToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sinhronizacijaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.logoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dokumentiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.izvestajiToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.zalihaToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.prometToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel_login = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.button_login = new System.Windows.Forms.Button();
            this.textBox_username = new System.Windows.Forms.TextBox();
            this.textBox_pwd = new System.Windows.Forms.TextBox();
            this.tabControl_documents = new SQLiteXP.Misc.TabControlEx();
            this.tableLayoutPanel_main.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.tableLayoutPanel_login.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel_main
            // 
            this.tableLayoutPanel_main.ColumnCount = 1;
            this.tableLayoutPanel_main.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.Controls.Add(this.menuStrip2, 0, 0);
            this.tableLayoutPanel_main.Controls.Add(this.tabControl_documents, 0, 1);
            this.tableLayoutPanel_main.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_main.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_main.Name = "tableLayoutPanel_main";
            this.tableLayoutPanel_main.RowCount = 2;
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel_main.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel_main.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel_main.TabIndex = 1;
            // 
            // menuStrip2
            // 
            this.menuStrip2.Font = new System.Drawing.Font("Segoe UI", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.podesavanjaToolStripMenuItem,
            this.dokumentiToolStripMenuItem,
            this.izvestajiToolStripMenuItem1});
            this.menuStrip2.Location = new System.Drawing.Point(0, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Size = new System.Drawing.Size(800, 33);
            this.menuStrip2.TabIndex = 0;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // podesavanjaToolStripMenuItem
            // 
            this.podesavanjaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.proizvodiToolStripMenuItem,
            this.kupciToolStripMenuItem,
            this.sinhronizacijaToolStripMenuItem,
            this.logoutToolStripMenuItem});
            this.podesavanjaToolStripMenuItem.Name = "podesavanjaToolStripMenuItem";
            this.podesavanjaToolStripMenuItem.Size = new System.Drawing.Size(131, 29);
            this.podesavanjaToolStripMenuItem.Text = "Podesavanja";
            // 
            // proizvodiToolStripMenuItem
            // 
            this.proizvodiToolStripMenuItem.Name = "proizvodiToolStripMenuItem";
            this.proizvodiToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.proizvodiToolStripMenuItem.Text = "Proizvodi";
            this.proizvodiToolStripMenuItem.Click += new System.EventHandler(this.proizvodiToolStripMenuItem_Click);
            // 
            // kupciToolStripMenuItem
            // 
            this.kupciToolStripMenuItem.Name = "kupciToolStripMenuItem";
            this.kupciToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.kupciToolStripMenuItem.Text = "Kupci";
            // 
            // sinhronizacijaToolStripMenuItem
            // 
            this.sinhronizacijaToolStripMenuItem.Name = "sinhronizacijaToolStripMenuItem";
            this.sinhronizacijaToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.sinhronizacijaToolStripMenuItem.Text = "Sinhronizacija";
            this.sinhronizacijaToolStripMenuItem.Click += new System.EventHandler(this.sinhronizacijaToolStripMenuItem_Click);
            // 
            // logoutToolStripMenuItem
            // 
            this.logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            this.logoutToolStripMenuItem.Size = new System.Drawing.Size(217, 30);
            this.logoutToolStripMenuItem.Text = "Logout";
            this.logoutToolStripMenuItem.Click += new System.EventHandler(this.logoutToolStripMenuItem_Click);
            // 
            // dokumentiToolStripMenuItem
            // 
            this.dokumentiToolStripMenuItem.Name = "dokumentiToolStripMenuItem";
            this.dokumentiToolStripMenuItem.Size = new System.Drawing.Size(118, 29);
            this.dokumentiToolStripMenuItem.Text = "Dokumenti";
            // 
            // izvestajiToolStripMenuItem1
            // 
            this.izvestajiToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zalihaToolStripMenuItem1,
            this.prometToolStripMenuItem1});
            this.izvestajiToolStripMenuItem1.Name = "izvestajiToolStripMenuItem1";
            this.izvestajiToolStripMenuItem1.Size = new System.Drawing.Size(93, 29);
            this.izvestajiToolStripMenuItem1.Text = "Izvestaji";
            // 
            // zalihaToolStripMenuItem1
            // 
            this.zalihaToolStripMenuItem1.Name = "zalihaToolStripMenuItem1";
            this.zalihaToolStripMenuItem1.Size = new System.Drawing.Size(159, 30);
            this.zalihaToolStripMenuItem1.Text = "Zaliha";
            // 
            // prometToolStripMenuItem1
            // 
            this.prometToolStripMenuItem1.Name = "prometToolStripMenuItem1";
            this.prometToolStripMenuItem1.Size = new System.Drawing.Size(159, 30);
            this.prometToolStripMenuItem1.Text = "Promet";
            // 
            // tableLayoutPanel_login
            // 
            this.tableLayoutPanel_login.ColumnCount = 3;
            this.tableLayoutPanel_login.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.12851F));
            this.tableLayoutPanel_login.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.74297F));
            this.tableLayoutPanel_login.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.12851F));
            this.tableLayoutPanel_login.Controls.Add(this.tableLayoutPanel1, 1, 1);
            this.tableLayoutPanel_login.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel_login.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel_login.Name = "tableLayoutPanel_login";
            this.tableLayoutPanel_login.RowCount = 3;
            this.tableLayoutPanel_login.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.48102F));
            this.tableLayoutPanel_login.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 43.03797F));
            this.tableLayoutPanel_login.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 28.48101F));
            this.tableLayoutPanel_login.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel_login.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.button_login, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.textBox_username, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.textBox_pwd, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(260, 131);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(279, 187);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // button_login
            // 
            this.button_login.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.button_login.AutoSize = true;
            this.button_login.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button_login.Location = new System.Drawing.Point(68, 83);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(143, 35);
            this.button_login.TabIndex = 2;
            this.button_login.Text = "login";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // textBox_username
            // 
            this.textBox_username.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox_username.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_username.Location = new System.Drawing.Point(15, 6);
            this.textBox_username.Name = "textBox_username";
            this.textBox_username.Size = new System.Drawing.Size(249, 28);
            this.textBox_username.TabIndex = 1;
            this.textBox_username.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_pwd
            // 
            this.textBox_pwd.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.textBox_pwd.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_pwd.Location = new System.Drawing.Point(16, 46);
            this.textBox_pwd.Name = "textBox_pwd";
            this.textBox_pwd.Size = new System.Drawing.Size(246, 28);
            this.textBox_pwd.TabIndex = 0;
            this.textBox_pwd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBox_pwd.UseSystemPasswordChar = true;
            // 
            // tabControl_documents
            // 
            this.tabControl_documents.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl_documents.DrawMode = System.Windows.Forms.TabDrawMode.OwnerDrawFixed;
            this.tabControl_documents.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl_documents.Location = new System.Drawing.Point(3, 53);
            this.tabControl_documents.Name = "tabControl_documents";
            this.tabControl_documents.Padding = new System.Drawing.Point(10, 5);
            this.tabControl_documents.SelectedIndex = 0;
            this.tabControl_documents.Size = new System.Drawing.Size(794, 394);
            this.tabControl_documents.TabIndex = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel_main);
            this.Controls.Add(this.tableLayoutPanel_login);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tableLayoutPanel_main.ResumeLayout(false);
            this.tableLayoutPanel_main.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.tableLayoutPanel_login.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void proizvodiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new ProductListForm();
            form.Show();
        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_main;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel_login;
        private System.Windows.Forms.TextBox textBox_pwd;
        private System.Windows.Forms.TextBox textBox_username;
        private System.Windows.Forms.Button button_login;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem podesavanjaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem proizvodiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kupciToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sinhronizacijaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem logoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dokumentiToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem izvestajiToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem zalihaToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem prometToolStripMenuItem1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Misc.TabControlEx tabControl_documents;
    }
}

