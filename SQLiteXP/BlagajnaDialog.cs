using SQLiteXP.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SQLiteXP
{
    public partial class BlagajnaDialog : Form
    {
        public BlagajnaDialog(BlagajnaDetailsModel blagajna)
        {
            InitializeComponent();

            label_cek.Text = $"Cek: {blagajna.cek} ({blagajna.cekNefiskalizovan})";
            label_gotovina.Text = $"Gotovina: {blagajna.gotovina} ({blagajna.gotovinaNefiskalizovan})";
            label_kartica.Text = $"Kartica: {blagajna.kartica} ({blagajna.karticaNefiskalizovan})";
            label_virman.Text = $"Virman: {blagajna.virman} ({blagajna.virmanNefiskalizovan})";
            label_promet.Text = $"Promet: {blagajna.uplaceno} ({blagajna.uplacenoNefiskalizovan})";
        }
    }
}
