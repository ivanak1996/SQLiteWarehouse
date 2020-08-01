﻿using SQLiteXP.Service;
using SQLiteXP.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SQLiteXP
{
    public partial class Form1 : Form
    {
        const string panel_login = "login";
        const string panel_main = "main";
        readonly Dictionary<string, Panel> panels = new Dictionary<string, Panel>();
        string message = "";

        private Users loggedInUser;

        public Form1()
        {
            InitializeComponent();
            panels.Add(panel_login, tableLayoutPanel_login);
            panels.Add(panel_main, tableLayoutPanel_main);

            WarehouseService.CreateDbIfNotExists();

            loggedInUser = WarehouseService.GetLoggedInUser();
            if (loggedInUser != null) 
            {
                FocusMainPanel();
            }
            else
            {
                FocusLoginPanel();
            }
            
        }

        private void button_login_Click(object sender, EventArgs e)
        {
            
            try
            {
                loggedInUser = WarehouseService.LoginUser(textBox_username.Text, textBox_pwd.Text);
            }
            catch(Exception ex)
            {
                loggedInUser = null;
                message = ex.Message;
            }

            if (loggedInUser != null) 
            {
                FocusMainPanel();
            }
            else
            {
                string message = this.message;
                string caption = "Greska";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }        

        private void FocusLoginPanel()
        {
            panels[panel_login].BringToFront();
        }

        private void FocusMainPanel()
        {
            IList<string> docTypes = WarehouseService.GetDocTypesNames();
            dokumentiToolStripMenuItem1.DropDownItems.Clear();
            foreach (var dt in docTypes)
            {
                dokumentiToolStripMenuItem1.DropDownItems.Add(dt);
            }
            panels[panel_main].BringToFront();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO consider deleting all data when user logs out
            WarehouseService.Logout();
            loggedInUser = null;
            panels[panel_login].BringToFront();
        }

        private void sinhronizacijaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(WarehouseService.SyncData(loggedInUser.user, loggedInUser.pass))
            {

            }
            else
            {
                string message = "Neuspesna sinhronizacija";
                string caption = "Greska";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
            }
        }
    }
}
