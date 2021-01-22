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
    public partial class EditBillItemDialog : Form
    {
        public float Quantity { get; private set; }
        public float Rabat { get; private set; }
        public float Cena { get; private set; }

        public EditBillItemDialog(float quantity, float rabat, float cena)
        {
            InitializeComponent();

            Quantity = quantity;
            Cena = cena;
            Rabat = rabat;

            textBox_cena.Text = cena.ToString("0.00");
            textBox_rabat.Text = rabat.ToString("0.00");
            textBox_kolicina.Text = quantity.ToString("0.00");
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if(float.TryParse(textBox_cena.Text, out float cena) 
                && float.TryParse(textBox_rabat.Text, out float rabat) 
                && float.TryParse(textBox_kolicina.Text, out float quantity))
            {
                Quantity = quantity;
                Cena = cena;
                Rabat = rabat;
                DialogResult = DialogResult.OK;
            }
        }
    }
}
