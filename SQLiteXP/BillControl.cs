using SQLiteXP.Models.Billing;
using SQLiteXP.Service;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System;
using SQLiteXP.Models;

namespace SQLiteXP
{
    public partial class BillControl : UserControl
    {
        private string docType;
        private List<Products> products = new List<Products>();
        private List<Bill> bills = new List<Bill>();
        private List<Buyers> buyers = new List<Buyers>();
        private Bill currentBill;
        private int currentBillIndex;
        private DataTable billItemsTable;
        private Dictionary<string, string> comboBoxOptions = new Dictionary<string, string>();
        private string lastComboBoxInput = string.Empty;
        private TextBox lastFocusedTextBox;

        private void SelectBill(int index)
        {
            currentBill = bills[index];
            currentBillIndex = index;
            currentBill.LoadItems();
            button_previousBill.Enabled = index != 0;
            button_nextBill.Enabled = index != bills.Count - 1;
            label_billNumber.Text = currentBill.GetBillNumber();
            PopulateBillItemsTable();
            PopulateTotalPrices();
        }

        private void PopulateTotalPrices()
        {
            textBox_cek.Text = currentBill.cek.ToString("0.00");
            textBox_kartica.Text = currentBill.kartica.ToString("0.00");
            textBox_gotovina.Text = currentBill.gotovina.ToString("0.00");
            textBox_virman.Text = currentBill.virman.ToString("0.00");
            textBox_povracaj.Text = currentBill.povracaj.ToString("0.00");
            textBox_uplata.Text = currentBill.uplaceno.ToString("0.00");
        }

        public BillControl(string docType, List<Products> products, List<Buyers> buyers)
        {
            this.docType = docType;
            this.products = products;
            InitializeComponent();
            label_billNumber.Text = docType;
            button_previousBill.Enabled = false;
            button_nextBill.Enabled = false;
            bills = WarehouseService.GetAllBillsWithDocType(docType);
            InitBillItemsTable();
            if (bills.Count > 0)
            {
                SelectBill(bills.Count - 1);
            }
            quantity_textBox1.KeyUp += AddToBill_KeyUp;
            sifraProizvoda_textBox.KeyUp += AddToBill_KeyUp;

            comboBoxOptions.Add("Sifra", "sifra");
            comboBoxOptions.Add("Naziv", "naziv");
            comboBoxOptions.Add("Barkod", "barkod");

            uplata_textBox1.KeyUp += UplataKeyUp;

            this.buyers = buyers;
            foreach (var buyer in buyers)
            {
                comboBox_kupci.Items.Add(buyer.sifra);
            }
            comboBox_kupci.SelectedIndexChanged += ComboBoxKupci_SelectedIndexChanged;
            
            uplata_textBox1.LostFocus += new EventHandler(dataGridView1_LostFocus);
            sifraProizvoda_textBox.LostFocus += new EventHandler(dataGridView1_LostFocus);
            quantity_textBox1.LostFocus += new EventHandler(dataGridView1_LostFocus);
            textBox_cek.LostFocus += new EventHandler(dataGridView1_LostFocus);
            textBox_gotovina.LostFocus += new EventHandler(dataGridView1_LostFocus);
            textBox_kartica.LostFocus += new EventHandler(dataGridView1_LostFocus);
            textBox_virman.LostFocus += new EventHandler(dataGridView1_LostFocus);

            // focus za nacine placanja
            textBox_cek.Click += KeyEvent;
            textBox_gotovina.Click += KeyEvent;
            textBox_kartica.Click += KeyEvent;
            textBox_virman.Click += KeyEvent;

            textBox_cek.KeyPress += TextBoxPrices_KeyPress;
            textBox_gotovina.KeyPress += TextBoxPrices_KeyPress;
            textBox_kartica.KeyPress += TextBoxPrices_KeyPress;
            textBox_virman.KeyPress += TextBoxPrices_KeyPress;
        }

        private void TextBoxPrices_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            CalculatePrices(currentBill.TotalPrice());

        }

        void dataGridView1_LostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox)
            {
                lastFocusedTextBox = (TextBox)sender;
            }
        }

        private void ComboBoxKupci_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.OnBuyerSelected(comboBox_kupci.SelectedIndex);
        }

        private void OnBuyerSelected(int index)
        {
            textBox_kupacAdresa.Text = buyers[index].adresa;
            textBox_kupacGrad.Text = buyers[index].grad;
        }

        private void UplataKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                float billTotalPrice = currentBill.TotalPrice();
                if (float.TryParse(uplata_textBox1.Text, out float toPay) && toPay - billTotalPrice >= 0)
                {
                    //povracaj_label.Text = (toPay - billTotalPrice).ToString("0.00");
                }
            }
        }

        private void AddToBillOnEnter()
        {
            if(quantity_textBox1.Text == string.Empty)
            {
                quantity_textBox1.Text = "1";
            }

            string selectedProductSifra = sifraProizvoda_textBox.Text;

            Products selectedProduct = products.FirstOrDefault(products => products.sifra == selectedProductSifra);
            if (selectedProduct != null)
            {
                float quantityToAdd = 1; //default
                if (float.TryParse(quantity_textBox1.Text, out quantityToAdd))
                {
                    currentBill.AddItem(new BillItem()
                    {
                        billId = currentBill.Id,
                        Quantity = quantityToAdd,
                        productIdent = selectedProduct.ident,
                        productCena = selectedProduct.cena,
                        productSifra = selectedProduct.sifra,
                        productNaziv = selectedProduct.naziv,
                        productJM = selectedProduct.jm,
                        productPdv = selectedProduct.pdv,
                        productBarkod = selectedProduct.barkod
                    });

                    PopulateBillItemsTable();
                }
            }
        }

        private void AddToBill_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && currentBill!= null)
            {
                AddToBillOnEnter();                    
                e.Handled = true;
            }
        }

        private void InitBillItemsTable()
        {
            billItemsTable = new DataTable("BillItems");

            DataColumn c0 = new DataColumn("ID");
            DataColumn c1 = new DataColumn("Sifra");
            DataColumn c2 = new DataColumn("Naziv");
            DataColumn c3 = new DataColumn("JM");
            DataColumn c4 = new DataColumn("Barkod");
            DataColumn c5 = new DataColumn("Cena");
            DataColumn c6 = new DataColumn("PDV");
            DataColumn c7 = new DataColumn("Popust");
            DataColumn c8 = new DataColumn("Kolicina");
            DataColumn c9 = new DataColumn("UkupnaCena");

            billItemsTable.Columns.Add(c0);
            billItemsTable.Columns.Add(c1);
            billItemsTable.Columns.Add(c2);
            billItemsTable.Columns.Add(c3);
            billItemsTable.Columns.Add(c4);
            billItemsTable.Columns.Add(c5);
            billItemsTable.Columns.Add(c6);
            billItemsTable.Columns.Add(c7);
            billItemsTable.Columns.Add(c8);
            billItemsTable.Columns.Add(c9);

            if (currentBill != null)
            {
                PopulateBillItemsTable();
            }

            dataGridView_billItems.DataSource = billItemsTable;
            dataGridView_billItems.ReadOnly = true;
            dataGridView_billItems.KeyDown += dataGridView1_PreviewKeyDown;
            dataGridView_billItems.RowHeaderMouseDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView_billItems.DefaultCellStyle.Font = new Font("Arial", 18.5F, GraphicsUnit.Pixel);
        }

        private void dataGridView1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                if (e.Modifiers == Keys.Control && dataGridView_billItems.SelectedRows.Count > 0
                    && MessageBox.Show("Brisanje stavke racuna?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // confirmed, need to delete
                    var selectedRow = dataGridView_billItems.SelectedRows[0];
                    BillItem selectedItem = currentBill.Items.FirstOrDefault(i => i.productIdent == int.Parse(selectedRow.Cells["ID"].Value.ToString()));

                    if (selectedItem != null)
                    {
                        currentBill.RemoveItem(selectedItem);
                        PopulateBillItemsTable();
                    }
                }
                e.Handled = true;
            }
        }

        private void DataGridView1_CellDoubleClick(Object sender, DataGridViewCellMouseEventArgs e)
        {
            ShowMyDialogBox();
        }

        private void ShowMyDialogBox()
        {
            var selectedRow = dataGridView_billItems.SelectedRows[0];
            if (int.TryParse(selectedRow.Cells["ID"].Value?.ToString(), out int selectedId))
            {
                BillItem selectedItem = currentBill.Items.FirstOrDefault(i => i.productIdent == selectedId);
                EditBillItemDialog testDialog = new EditBillItemDialog(selectedItem.Quantity, selectedItem.productPopust, selectedItem.productCena);

                // Show testDialog as a modal dialog and determine if DialogResult = OK.
                if (testDialog.ShowDialog(this) == DialogResult.OK)
                {
                    selectedItem.Quantity = testDialog.Quantity;
                    selectedItem.productPopust = testDialog.Rabat;
                    selectedItem.productCena = testDialog.Cena;
                    currentBill.UpdateItem(selectedItem);
                    PopulateBillItemsTable();
                }
                testDialog.Dispose();
            }
        }

        private void PopulateBillItemsTable()
        {
            billItemsTable.Rows.Clear();

            float totalPrice = 0;

            foreach (var item in currentBill.Items)
            {
                float itemTotalPrice = item.Quantity * item.productCena - item.productPopust;

                var row = billItemsTable.NewRow();
                row["ID"] = item.productIdent;
                row["Sifra"] = item.productSifra;
                row["Naziv"] = item.productNaziv;
                row["JM"] = item.productJM;
                row["Barkod"] = item.productBarkod;
                row["Cena"] = item.productCena.ToString("0.00");
                row["PDV"] = item.productPdv.ToString("0.00");
                row["Popust"] = item.productPopust.ToString("0.00");
                row["Kolicina"] = item.Quantity.ToString("0.00");
                row["UkupnaCena"] = (itemTotalPrice).ToString("0.00");

                billItemsTable.Rows.Add(row);

                totalPrice += itemTotalPrice;
            }
            uplata_textBox1.Text = totalPrice.ToString("0.00");
        }

        private void button_previousBill_Click_1(object sender, EventArgs e)
        {
            SelectBill(currentBillIndex - 1);
        }

        private void button_nextBill_Click_1(object sender, EventArgs e)
        {
            SelectBill(currentBillIndex + 1);
        }

        private void button_uplata_Click_1(object sender, EventArgs e)
        {
            
            TotalPricesDialog dialog = new TotalPricesDialog(currentBill);

            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                currentBill.Save();
            }
            else
            {
                //this.txtResult.Text = "Cancelled";
            }
            dialog.Dispose();
        }

        private void CalculatePricesIfBilling()
        {
            if (lastFocusedTextBox == textBox_cek
                    || lastFocusedTextBox == textBox_kartica
                    || lastFocusedTextBox == textBox_virman
                    || lastFocusedTextBox == textBox_gotovina)
            {
                CalculatePrices(currentBill.TotalPrice());
            }
        }

        private void KeyboardNumericClickHandler(int number)
        {
            if (lastFocusedTextBox != null)
            {
                lastFocusedTextBox.Text += $"{number}";
                CalculatePricesIfBilling();
            }
        }

        private void button_0_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(0);
        }

        private void button_point_Click(object sender, EventArgs e)
        {
            if (lastFocusedTextBox != null && float.TryParse(lastFocusedTextBox.Text, out float value))
            {
                string[] parts = lastFocusedTextBox.Text.Split('.');
                if (parts.Length == 2) return;
                lastFocusedTextBox.Text += ".";
            }
        }

        private void button_1_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(1);
        }

        private void button_2_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(2);
        }

        private void button_3_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(3);
        }

        private void button_4_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(4);
        }

        private void button_5_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(5);
        }

        private void button_6_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(6);
        }

        private void button_7_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(7);
        }

        private void button_8_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(8);
        }

        private void button_9_Click(object sender, EventArgs e)
        {
            KeyboardNumericClickHandler(9);
        }

        private void button_backspace_Click(object sender, EventArgs e)
        {
            if (lastFocusedTextBox != null && float.TryParse(lastFocusedTextBox.Text, out float value))
            {
                lastFocusedTextBox.Text = lastFocusedTextBox.Text.Remove(lastFocusedTextBox.Text.Length - 1);
                CalculatePricesIfBilling();
            }
        }

        private void button_enter_Click(object sender, EventArgs e)
        {
            if (lastFocusedTextBox == sifraProizvoda_textBox || lastFocusedTextBox == quantity_textBox1)
            {
                AddToBillOnEnter();
            }
        }

        private void searchDialogOpen(object sender, EventArgs e)
        {
            ProductListForm productsDialog = new ProductListForm();

            if (productsDialog.ShowDialog(this) == DialogResult.OK)
            {
                sifraProizvoda_textBox.Text = productsDialog.GetSelectedProduct_Sifra();
                this.lastFocusedTextBox = sifraProizvoda_textBox;
            }
            else
            {
                //this.txtResult.Text = "Cancelled";
            }
            productsDialog.Dispose();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            quantity_textBox1.Focus();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            BuyersListForm buyersDialog = new BuyersListForm();

            if (buyersDialog.ShowDialog(this) == DialogResult.OK)
            {
                comboBox_kupci.SelectedIndex = comboBox_kupci.FindStringExact(buyersDialog.GetSelectedBuyer());
            }
            buyersDialog.Dispose();
        }

        private bool CalculatePrices(float iznos)
        {
            if (float.TryParse(textBox_cek.Text, out float cek)
                && float.TryParse(textBox_virman.Text, out float virman)
                && float.TryParse(textBox_kartica.Text, out float kartica)
                && float.TryParse(textBox_gotovina.Text, out float gotovina))
            {
                currentBill.cek = cek;
                currentBill.virman = virman;
                currentBill.kartica = kartica;
                currentBill.gotovina = gotovina;

                currentBill.uplaceno = cek + virman + kartica + gotovina;
                currentBill.povracaj = currentBill.uplaceno - iznos;

                textBox_povracaj.Text = currentBill.povracaj.ToString("0.00");
                textBox_uplata.Text = currentBill.uplaceno.ToString("0.00");
                button_fiskalizacija.Enabled = currentBill.povracaj >= 0;
                return true;
            }
            else
            {
                button_fiskalizacija.Enabled = false;
                return false;
            }
        }

        private void KeyEvent(object sender, EventArgs e) //Keyup Event 
        {
            float iznos = currentBill.TotalPrice();
            float toAdd = iznos - currentBill.uplaceno;
            if (toAdd > 0 && sender is TextBox)
            {
                switch (((TextBox)sender).Name)
                {
                    case "textBox_gotovina":
                        currentBill.gotovina += toAdd;
                        textBox_gotovina.Text = currentBill.gotovina.ToString("0.00");
                        break;
                    case "textBox_kartica":
                        currentBill.kartica += toAdd;
                        textBox_kartica.Text = currentBill.kartica.ToString("0.00");
                        break;
                    case "textBox_virman":
                        currentBill.virman += toAdd;
                        textBox_virman.Text = currentBill.virman.ToString("0.00");
                        break;
                    case "textBox_cek":
                        currentBill.cek += toAdd;
                        textBox_cek.Text = currentBill.cek.ToString("0.00");
                        break;
                    default:
                        return;
                }
                CalculatePrices(iznos);
            }
        }
        private void button_newBill_Click(object sender, EventArgs e)
        {
            Bill bill = WarehouseService.CreateNewBill(DateTime.Now.Year % 100, docType);
            bills.Add(bill);
            SelectBill(bills.Count - 1);
        }

        private void button_fiskalizacija_Click(object sender, EventArgs e)
        {
            // TODO
        }
        
    }
}
