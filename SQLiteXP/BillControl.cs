using SQLiteXP.Models.Billing;
using SQLiteXP.Service;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System;
using SQLiteXP.Models;
using System.Text.RegularExpressions;
using System.Globalization;
using SQLiteXP.Database;

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
        private TextBox lastFocusedTextBox;
        private bool billSelectionInProgress = false;

        private const string quantityInSifraTextBox = @"^(\+|\-)[0-9]\d*(\.\d+)?$";
        private const string ERROR_BUYER_WITH_PIB_DOES_NOT_EXIST = "Kupac sa unetim PIB-om ne postoji";
        private const string ERROR_BUYER_WITH_ID_DOES_NOT_EXIST = "Kupac sa unetim maticnim brojem ne postoji";
        private const string ERROR_WRONG_SIFRA_PROIZVODA = "Pogresno uneta sifra proizvoda";
        private const string WARNING = "Obavestenje";
        private const string DELETE_BILL_MESSAGE = "Da li ste sigurni da zelite da obrisete racun?";
        private const string DELETE_BILL_TITLE = "Brisanje racuna";
        private const string DATE_TIME_FORMAT_SPECIFIER_FISKALIZOVAN = "dd.MM.yyyy. hh:mm";
        private const string DATE_TIME_FORMAT_SPECIFIER_NEFISKALIZOVAN = "dd.MM.yyyy.";
        private const string DEFAULT_LANGUAGE_CULTURE = "sr-Latn-RS";
        private const string FISKALIZACIJA_MESSAGE = "Da li zelite da fiskalizujete racun";
        private const string FISKALIZACIJA_TITLE = "Fiskalizacija";

        public static string ConvertDateTimeToDate(DateTime dt, string formatSpecifier, string langCulture)
        {
            CultureInfo culture = new CultureInfo(langCulture);
            return dt.ToString(formatSpecifier, culture);
        }

        private void SelectBill(int index)
        {
            billSelectionInProgress = true;
            currentBill = bills[index];
            currentBillIndex = index;
            currentBill.LoadItems();
            button_previousBill.Enabled = index != 0;
            button_nextBill.Enabled = index != bills.Count - 1;
            label_billNumber.Text = currentBill.GetBillNumber();

            if(currentBill.Status == Bill.STATUS_FISKALIZOVAN)
            {
                label_datum.Text = ConvertDateTimeToDate(currentBill.DateCreated, DATE_TIME_FORMAT_SPECIFIER_FISKALIZOVAN, DEFAULT_LANGUAGE_CULTURE);
                checkBox_fiskalizovan.Checked = true;
            }
            else
            {
                label_datum.Text = ConvertDateTimeToDate(currentBill.DateCreated, DATE_TIME_FORMAT_SPECIFIER_NEFISKALIZOVAN, DEFAULT_LANGUAGE_CULTURE);
                checkBox_fiskalizovan.Checked = false;
            }
            int buyersIndexToSelect = buyers.FindIndex(b => b.sifra == currentBill.sifraKupca);
            if (buyersIndexToSelect != -1)
            {
                comboBox_kupci.SelectedIndex = buyersIndexToSelect;
            }

            PopulateBillItemsTable();
            PopulateTotalPrices();
            billSelectionInProgress = false;
        }

        private void SetupForNoBills()
        {
            currentBill = null;
            currentBillIndex = -1;
            button_previousBill.Enabled = false;
            button_nextBill.Enabled = false;
            label_billNumber.Text = string.Empty;
            checkBox_fiskalizovan.Checked = false;
            label_datum.Text = string.Empty;
            PopulateBillItemsTable();
            PopulateTotalPrices();
        }

        private void PopulateTotalPrices()
        {
            if (currentBill != null)
            {
                textBox_cek.Text = currentBill.cek.ToString("0.00");
                textBox_kartica.Text = currentBill.kartica.ToString("0.00");
                textBox_gotovina.Text = currentBill.gotovina.ToString("0.00");
                textBox_virman.Text = currentBill.virman.ToString("0.00");
                textBox_povracaj.Text = currentBill.povracaj.ToString("0.00");
                textBox_uplata.Text = currentBill.uplaceno.ToString("0.00");
                uplata_textBox1.Text = currentBill.uplaceno.ToString("0.00");
            }
            else
            {
                textBox_cek.Clear();
                textBox_kartica.Clear();
                textBox_gotovina.Clear();
                textBox_virman.Clear();
                textBox_povracaj.Clear();
                textBox_uplata.Clear();
                uplata_textBox1.Clear();
            }            
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

            // enter kada se klikne za sifru proizvoda ili kolicinu da se doda u korpu
            quantity_textBox1.KeyUp += Kolicina_KeyUp;
            sifraProizvoda_textBox.KeyUp += SifraProizvoda_KeyUp;

            // za pretragu po PIB-u ili maticnom broju
            textBox_PIB.KeyUp += PIB_KeyUp;
            textBox_maticniBr.KeyUp += MaticniBroj_KeyUp;

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
            textBox_PIB.LostFocus += new EventHandler(dataGridView1_LostFocus);
            textBox_maticniBr.LostFocus += new EventHandler(dataGridView1_LostFocus);

            // focus za nacine placanja
            textBox_cek.Click += KeyEvent;
            textBox_gotovina.Click += KeyEvent;
            textBox_kartica.Click += KeyEvent;
            textBox_virman.Click += KeyEvent;

            textBox_cek.KeyPress += TextBoxPrices_KeyPress;
            textBox_gotovina.KeyPress += TextBoxPrices_KeyPress;
            textBox_kartica.KeyPress += TextBoxPrices_KeyPress;
            textBox_virman.KeyPress += TextBoxPrices_KeyPress;

            textBox_cek.TextChanged += TextBoxPrices_TextChanged;
            textBox_gotovina.TextChanged += TextBoxPrices_TextChanged;
            textBox_kartica.TextChanged += TextBoxPrices_TextChanged;
            textBox_virman.TextChanged += TextBoxPrices_TextChanged;
        }

        private bool fiskalizovan()
        {
            return currentBill != null && currentBill.Status == Bill.STATUS_FISKALIZOVAN;
        }

        private void TextBoxPrices_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(fiskalizovan()) return;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

        }

        private void TextBoxPrices_TextChanged(object sender, EventArgs e)
        {
            if (fiskalizovan()) return;

            if (currentBill != null && billSelectionInProgress == false)
            {
                CalculatePrices(currentBill.TotalPrice());
            }
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
            textBox_PIB.Text = buyers[index].pib.Trim(' ');
            textBox_maticniBr.Text = buyers[index].maticniBroj.Trim(' ');
            if (currentBill != null)
            {
                currentBill.UpdateBuyer(buyers[index].sifra);
            }
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
            if (fiskalizovan()) return;
            if (currentBill == null) return;
            if (quantity_textBox1.Text == string.Empty)
            {
                if (sifraProizvoda_textBox.Text == string.Empty)
                {
                    // fokus na gotovinu i popuni iznos
                    float iznos = currentBill.TotalPrice();
                    float toAdd = iznos - currentBill.uplaceno;
                    currentBill.gotovina += toAdd;
                    textBox_gotovina.Text = currentBill.gotovina.ToString("0.00");
                    textBox_gotovina.Focus();
                    textBox_gotovina.SelectionStart = 0;
                    textBox_gotovina.SelectionLength = textBox_gotovina.Text.Length;
                    return;
                }
                else
                {
                    quantity_textBox1.Text = "1";
                }
            }           

            string selectedProductSifra = sifraProizvoda_textBox.Text;

            if(selectedProductSifra == string.Empty)
            {
                sifraProizvoda_textBox.Focus();
                return;
            }

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

                    quantity_textBox1.Clear();
                    sifraProizvoda_textBox.Clear();
                    sifraProizvoda_textBox.Focus();
                }                
            }
            else
            {
                showMessageBox(ERROR_WRONG_SIFRA_PROIZVODA, WARNING);
                sifraProizvoda_textBox.Clear();
                sifraProizvoda_textBox.Focus();
                return;
            }
        }

        private void SifraProizvoda_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && currentBill != null)
            {
                if (sifraProizvoda_textBox.Text == string.Empty)
                {
                    quantity_textBox1.Focus();
                }
                else
                {
                    if (Regex.IsMatch(sifraProizvoda_textBox.Text, quantityInSifraTextBox))
                    {
                        string quantity = (sifraProizvoda_textBox.Text[0] == '-') 
                            ? sifraProizvoda_textBox.Text 
                            : sifraProizvoda_textBox.Text.Substring(1);
                        quantity_textBox1.Text = quantity;
                        sifraProizvoda_textBox.Clear();
                        sifraProizvoda_textBox.Focus();
                    }
                    else {
                        AddToBillOnEnter();
                    }
                }
                e.Handled = true;
            }
        }

        private void Kolicina_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && currentBill!= null)
            {
                AddToBillOnEnter();                    
                e.Handled = true;
            }
        }

        private void showMessageBox(string message, string caption)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, caption, buttons);
        }

        private void PIB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && currentBill != null)
            {
                int index = buyers.FindIndex(b => b.pib.Trim(' ') == textBox_PIB.Text);
                if (index != -1)
                {
                    comboBox_kupci.SelectedIndex = index;
                }
                else
                {
                    showMessageBox(ERROR_BUYER_WITH_PIB_DOES_NOT_EXIST, WARNING);
                }
                e.Handled = true;
            }
        }

        private void MaticniBroj_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && currentBill != null)
            {
                int index = buyers.FindIndex(b => b.maticniBroj.Trim(' ') == textBox_maticniBr.Text);
                if (index != -1)
                {
                    comboBox_kupci.SelectedIndex = index;
                }
                else
                {
                    showMessageBox(ERROR_BUYER_WITH_ID_DOES_NOT_EXIST, WARNING);
                }
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
            dataGridView_billItems.KeyDown += dataGridView_Brisanje_KeyDown;
            dataGridView_billItems.RowHeaderMouseDoubleClick += DataGridView1_CellDoubleClick;
            dataGridView_billItems.DefaultCellStyle.Font = new Font("Arial", 18.5F, GraphicsUnit.Pixel);
        }

        private void dataGridView_Brisanje_KeyDown(object sender, KeyEventArgs e)
        {
            if (fiskalizovan()) return;
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
            if (fiskalizovan()) return;
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

            if (currentBill != null)
            {
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
                currentBill.povracaj = currentBill.uplaceno - totalPrice;
                textBox_povracaj.Text = currentBill.povracaj.ToString("0.00");
            }
            uplata_textBox1.Text = totalPrice.ToString("0.00");
        }

        private void button_uplata_Click_1(object sender, EventArgs e)
        {
            if (fiskalizovan()) return;
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

        private void KeyboardNumericClickHandler(int number)
        {
            if (lastFocusedTextBox != null)
            {
                lastFocusedTextBox.Text += $"{number}";
                //CalculatePricesIfBilling();
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
            if (lastFocusedTextBox != null && lastFocusedTextBox.Text.Length > 0)
            {
                if (lastFocusedTextBox.SelectionLength > 0)
                {
                    lastFocusedTextBox.Text 
                        = lastFocusedTextBox.Text.Remove(lastFocusedTextBox.SelectionStart, lastFocusedTextBox.SelectionLength);
                }
                else
                {
                    lastFocusedTextBox.Text = lastFocusedTextBox.Text.Remove(lastFocusedTextBox.Text.Length - 1);
                }               
                
                //CalculatePricesIfBilling();
            }
        }

        private void button_enter_Click(object sender, EventArgs e)
        {
            if (fiskalizovan()) return;
            if (lastFocusedTextBox == sifraProizvoda_textBox)
            {
                if (Regex.IsMatch(sifraProizvoda_textBox.Text, quantityInSifraTextBox))
                {
                    string quantity = (sifraProizvoda_textBox.Text[0] == '-')
                            ? sifraProizvoda_textBox.Text
                            : sifraProizvoda_textBox.Text.Substring(1);
                    quantity_textBox1.Text = quantity;
                    sifraProizvoda_textBox.Clear();
                    sifraProizvoda_textBox.Focus();
                }
                else
                {
                    AddToBillOnEnter();
                }
            } else if (lastFocusedTextBox == quantity_textBox1)
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
                quantity_textBox1.Focus();
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

        private bool CalculatePrices(float iznos)
        {
            if (fiskalizovan()) return false;
            if (!float.TryParse(textBox_cek.Text, out float cek) && !(textBox_cek.Text == string.Empty)
                || !float.TryParse(textBox_virman.Text, out float virman) && !(textBox_virman.Text == string.Empty)
                || !float.TryParse(textBox_kartica.Text, out float kartica) && !(textBox_kartica.Text == string.Empty)
                || !float.TryParse(textBox_gotovina.Text, out float gotovina) && !(textBox_gotovina.Text == string.Empty))
            {
                button_fiskalizacija.Enabled = false;
                return false;
            }

            currentBill.cek = cek;
            currentBill.virman = virman;
            currentBill.kartica = kartica;
            currentBill.gotovina = gotovina;

            currentBill.uplaceno = cek + virman + kartica + gotovina;
            currentBill.povracaj = currentBill.uplaceno - iznos;

            textBox_povracaj.Text = currentBill.povracaj.ToString("0.00");
            textBox_uplata.Text = currentBill.uplaceno.ToString("0.00");
            button_fiskalizacija.Enabled = Math.Round(currentBill.povracaj, 2) >= 0.0;
            currentBill.Save();
            return true;            
        }

        private void KeyEvent(object sender, EventArgs e) //Keyup Event 
        {
            if (currentBill != null)
            {
                if (fiskalizovan()) return;
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
                    TextBox textBox = sender as TextBox;
                    textBox.SelectionStart = 0;
                    textBox.SelectionLength = textBox.Text.Length;
                }
            }
        }
        private void button_newBill_Click(object sender, EventArgs e)
        {
            createNewBill();
        }

        private void createNewBill()
        {
            Bill bill = WarehouseService.CreateNewBill(DateTime.Now.Year % 100, docType);
            bills.Add(bill);
            SelectBill(bills.Count - 1);
        }

        private void button_fiskalizacija_Click(object sender, EventArgs e)
        {
            if (bills.Count == 0) return;
            if (currentBill != null && currentBill.Status != Bill.STATUS_FISKALIZOVAN)
            {
                if(Math.Round(currentBill.povracaj, 2) < 0.0)
                {
                    button_fiskalizacija.Enabled = false;
                    return;
                }

                DialogResult dialogResult = MessageBox.Show(FISKALIZACIJA_MESSAGE, FISKALIZACIJA_TITLE, MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    currentBill.Status = Bill.STATUS_FISKALIZOVAN;
                    currentBill.DateCreated = DateTime.Now;
                    currentBill.Fiskalizuj();
                    createNewBill();
                }
                else if (dialogResult == DialogResult.No)
                {
                    return;
                }
            }
            
        }

        private void button_SearchBuyers_Click(object sender, EventArgs e)
        {
            BuyersListForm buyersDialog = new BuyersListForm();

            if (buyersDialog.ShowDialog(this) == DialogResult.OK)
            {
                comboBox_kupci.SelectedIndex = comboBox_kupci.FindStringExact(buyersDialog.GetSelectedBuyer());
            }
            buyersDialog.Dispose();
        }

        private void button_nextBill_Click(object sender, EventArgs e)
        {
            SelectBill(currentBillIndex + 1);
        }

        private void button_previousBill_Click(object sender, EventArgs e)
        {
            SelectBill(currentBillIndex - 1);
        }

        private void button_deleteBill_Click(object sender, EventArgs e)
        {
            if (bills.Count == 0) return;

            DialogResult dialogResult = MessageBox.Show(DELETE_BILL_MESSAGE, DELETE_BILL_TITLE, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                Bill toDelete = bills[currentBillIndex];

                if (bills.Count == 1)
                {
                    SetupForNoBills();
                }
                else
                {
                    SelectBill(currentBillIndex == 0 ? 1 : currentBillIndex - 1);
                }

                bills.Remove(toDelete);
                WarehouseService.DeleteBill(toDelete);
            }
            else if (dialogResult == DialogResult.No)
            {
                return;
            }                    
        }

        private void KeyboardSymbolHandler(char symbol)
        {
            if (lastFocusedTextBox == quantity_textBox1 
                || lastFocusedTextBox == sifraProizvoda_textBox)
            {
                if (lastFocusedTextBox.Text != string.Empty) return;
                lastFocusedTextBox.Text += symbol;
            }
        }

        private void button_plus_Click(object sender, EventArgs e)
        {
            KeyboardSymbolHandler('+');
        }

        private void button_minus_Click(object sender, EventArgs e)
        {
            KeyboardSymbolHandler('-');
        }

        private void button_blagajna_Click(object sender, EventArgs e)
        {
            BlagajnaDetailsModel blagajna = SQLiteDataAccess.GetBlagajnaFiskalizovaniData();
            BlagajnaDialog testDialog = new BlagajnaDialog(blagajna);

            // Show testDialog as a modal dialog and determine if DialogResult = OK.
            if (testDialog.ShowDialog(this) == DialogResult.OK)
            {
                // TODO implement
            }
            testDialog.Dispose();
        }
    }
}
