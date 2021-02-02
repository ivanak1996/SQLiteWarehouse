using SQLiteXP.Models.Billing;
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
    public partial class TotalPricesDialog : Form
    {
        private Bill bill;

        public float iznos { get; private set; }
        public float popust { get; private set; }
        public float uplaceno { get; private set; }
        public float povracaj { get; private set; }
        public float cek { get; private set; }
        public float virman { get; private set; }
        public float kartica { get; private set; }
        public float gotovina { get; private set; }

        public TotalPricesDialog(Bill bill)
        {
            InitializeComponent();

            this.bill = bill;

            this.iznos = bill.RacunBezPopusta();
            this.popust = bill.Popust();
            this.uplaceno = bill.uplaceno;
            this.povracaj = bill.povracaj;
            this.kartica = bill.kartica;
            this.cek = bill.cek;
            this.virman = bill.virman;
            this.gotovina = bill.gotovina;

            textBox_iznos.Text = iznos.ToString("0.00");
            textBox_popust.Text = popust.ToString("0.00");
            textBox_uplaceno.Text = uplaceno.ToString("0.00");
            textBox_povracaj.Text = povracaj.ToString("0.00");
            textBox_kartica.Text = kartica.ToString("0.00");
            textBox_cek.Text = cek.ToString("0.00");
            textBox_virman.Text = virman.ToString("0.00");
            textBox_gotovina.Text = gotovina.ToString("0.00");

            textBox_kartica.LostFocus += textBox_Leave;
            textBox_cek.LostFocus += textBox_Leave;
            textBox_virman.LostFocus += textBox_Leave;
            textBox_gotovina.LostFocus += textBox_Leave;

            this.KeyDown += KeyEvent;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
            CalculatePrices();
        }

        private void KeyEvent(object sender, KeyEventArgs e) //Keyup Event 
        {
            float toAdd = iznos - uplaceno;
            if (toAdd > 0)
            {
                switch (e.KeyCode)
                {
                    case Keys.F2:
                        gotovina += toAdd;
                        textBox_gotovina.Text = gotovina.ToString("0.00");
                        break;
                    case Keys.F3:
                        kartica += toAdd;
                        textBox_kartica.Text = kartica.ToString("0.00");
                        break;
                    case Keys.F4:
                        virman += toAdd;
                        textBox_virman.Text = virman.ToString("0.00");
                        break;
                    case Keys.F5:
                        cek += toAdd;
                        textBox_cek.Text = cek.ToString("0.00");
                        break;
                    default:
                        return;
                }
                CalculatePrices();
            }
        }

        private bool CalculatePrices()
        {
            if(float.TryParse(textBox_cek.Text, out float cek)
                && float.TryParse(textBox_virman.Text, out float virman)
                && float.TryParse(textBox_kartica.Text, out float kartica)
                && float.TryParse(textBox_gotovina.Text, out float gotovina))
            {
                this.cek = cek;
                this.virman = virman;
                this.kartica = kartica;
                this.gotovina = gotovina;

                uplaceno = cek + virman + kartica + gotovina;
                povracaj = uplaceno - iznos;

                textBox_povracaj.Text = povracaj.ToString("0.00");
                textBox_uplaceno.Text = uplaceno.ToString("0.00");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void button_ok_Click(object sender, EventArgs e)
        {
            if (CalculatePrices())
            {
                bill.popust = popust;
                bill.uplaceno = uplaceno;
                bill.povracaj = povracaj;
                bill.kartica = kartica;
                bill.cek = cek;
                bill.virman = virman;
                bill.gotovina = gotovina;
                DialogResult = DialogResult.OK;
            }
        }
    }
}
