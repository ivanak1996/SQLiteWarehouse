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
        private Bill currentBill;
        private int currentBillIndex;
        private DataTable billItemsTable;
        private Dictionary<string, string> comboBoxOptions = new Dictionary<string, string>();

        private void SelectBill(int index)
        {
            currentBill = bills[index];
            currentBillIndex = index;
            currentBill.LoadItems();
            button_previousBill.Enabled = index != 0;
            button_nextBill.Enabled = index != bills.Count - 1;
            label_billNumber.Text = currentBill.GetBillNumber();
            PopulateBillItemsTable();
        }

        public BillControl(string docType, List<Products> products)
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
            quantity_textBox1.KeyUp += QuantityKeyUp;           

            comboBoxOptions.Add("Sifra", "sifra");
            comboBoxOptions.Add("Naziv", "naziv");
            comboBoxOptions.Add("Barkod", "barkod");
        }

        private void QuantityKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && currentBill!= null)
            {
                string selectedProductBarcode = textBox_searchCriteria.Text;

                Products selectedProduct = products.FirstOrDefault(products => products.sifra == selectedProductBarcode);
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
                else
                {
                    //this.txtResult.Text = "Cancelled";
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
            ukupno_label2.Text = totalPrice.ToString("0.00");
        }

        private void button_previousBill_Click(object sender, EventArgs e)
        {
            SelectBill(currentBillIndex - 1);
        }

        private void button_nextBill_Click(object sender, EventArgs e)
        {
            SelectBill(currentBillIndex + 1);
        }

        private void button_newBill_Click(object sender, EventArgs e)
        {
            Bill bill = WarehouseService.CreateNewBill(DateTime.Now.Year % 100, docType);
            bills.Add(bill);
            SelectBill(bills.Count - 1);
        }
    }
}
