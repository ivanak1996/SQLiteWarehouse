using SQLiteXP.Service;
using SQLiteXP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using SQLiteXP.Models.Billing;

namespace SQLiteXP
{
    public partial class ProductListForm : Form
    {
        private List<Products> products = new List<Products>();
        private Bill currentBill;

        private Dictionary<string, string> comboBoxOptions = new Dictionary<string, string>();

        public ProductListForm()
        {
            InitializeComponent();
            currentBill = WarehouseService.GetOpenBill();
            products = WarehouseService.GetProducts();
            quantity_textBox1.KeyUp += QuantityKeyUp;
            PopulateTable();
            PopulateBillItemsTable();
            comboBoxOptions.Add("Sifra", "sifra");
            comboBoxOptions.Add("Naziv", "naziv");
            comboBoxOptions.Add("Barkod", "barkod");

            foreach (var key in comboBoxOptions.Keys)
            {
                comboBox_searchColumn.Items.Add(key);
            }

            comboBox_searchColumn.SelectedIndex = 0;
        }

        private void QuantityKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (dataGridView_products.SelectedRows.Count > 0)
                {
                    var selectedRow = dataGridView_products.SelectedRows[0];
                    Products selectedProduct = products.FirstOrDefault(p => p.ident == int.Parse(selectedRow.Cells["ID"].Value.ToString()));

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
                            productBarkod = selectedProduct.barkod
                        });

                        PopulateBillItemsTable();
                    }
                }
                e.Handled = true;
            }
        }

        private void PopulateBillItemsTable()
        {
            DataTable billItemsTable = new DataTable("BillItems");

            DataColumn c0 = new DataColumn("ID");
            DataColumn c1 = new DataColumn("Sifra");
            DataColumn c2 = new DataColumn("Naziv");
            DataColumn c3 = new DataColumn("JM");
            DataColumn c4 = new DataColumn("Barkod");
            DataColumn c5 = new DataColumn("Cena");
            DataColumn c6 = new DataColumn("Popust");
            DataColumn c7 = new DataColumn("Kolicina");
            DataColumn c8 = new DataColumn("UkupnaCena");

            billItemsTable.Columns.Add(c0);
            billItemsTable.Columns.Add(c1);
            billItemsTable.Columns.Add(c2);
            billItemsTable.Columns.Add(c3);
            billItemsTable.Columns.Add(c4);
            billItemsTable.Columns.Add(c5);
            billItemsTable.Columns.Add(c6);
            billItemsTable.Columns.Add(c7);
            billItemsTable.Columns.Add(c8);

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
                row["Kolicina"] = item.Quantity.ToString("0.00");
                row["UkupnaCena"] = (itemTotalPrice).ToString("0.00");

                billItemsTable.Rows.Add(row);

                totalPrice += itemTotalPrice;
            }
            billitems_dataGridView1.DataSource = billItemsTable;
            billitems_dataGridView1.ReadOnly = true;
            billitems_dataGridView1.KeyDown += dataGridView1_PreviewKeyDown;

            ukupno_label2.Text = totalPrice.ToString("0.00");
        }

        private void dataGridView1_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Control && billitems_dataGridView1.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Brisanje stavke racuna?", "Confirmation", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    // confirmed, need to delete
                    var selectedRow = billitems_dataGridView1.SelectedRows[0];
                    BillItem selectedItem = currentBill.Items.FirstOrDefault(i=>i.productIdent == int.Parse(selectedRow.Cells["ID"].Value.ToString()));
                    
                    if (selectedItem != null)
                    {
                        currentBill.RemoveItem(selectedItem);
                        PopulateBillItemsTable();
                    }
                }
                e.Handled = true;
            }
        }

        private void PopulateTable()
        {
            DataTable productsTable = new DataTable("Products");

            DataColumn c0 = new DataColumn("ID");
            DataColumn c1 = new DataColumn("Sifra");
            DataColumn c2 = new DataColumn("Naziv");
            DataColumn c3 = new DataColumn("Grupa");
            DataColumn c8 = new DataColumn("JM");
            DataColumn c9 = new DataColumn("Barkod");
            DataColumn c4 = new DataColumn("PDV");
            DataColumn c5 = new DataColumn("Cena");
            DataColumn c6 = new DataColumn("Popust");
            DataColumn c7 = new DataColumn("Opis");            

            productsTable.Columns.Add(c0);
            productsTable.Columns.Add(c1);
            productsTable.Columns.Add(c2);
            productsTable.Columns.Add(c3);
            productsTable.Columns.Add(c8);
            productsTable.Columns.Add(c9);
            productsTable.Columns.Add(c4);
            productsTable.Columns.Add(c5);
            productsTable.Columns.Add(c6);
            productsTable.Columns.Add(c7);

            foreach (var product in products)
            {
                var row = productsTable.NewRow();
                row["ID"] = product.ident;
                row["Sifra"] = product.sifra;
                row["Naziv"] = product.naziv;
                row["Grupa"] = product.grupa;
                row["JM"] = product.jm;
                row["Barkod"] = product.barkod;
                row["PDV"] = product.pdv;
                row["Cena"] = product.cena;
                row["Popust"] = product.popust;
                row["Opis"] = product.opis;
                productsTable.Rows.Add(row);
            }
            dataGridView_products.DataSource = productsTable;
            dataGridView_products.ReadOnly = true;
        }

        private void UpdateResults(List<Products> updatedProducts)
        {
            products = updatedProducts;
            PopulateTable();
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            var column = comboBoxOptions[comboBox_searchColumn.SelectedItem as string];
            var criteria = textBox_searchCriteria.Text;

            List<Products> filteredProducts = WarehouseService.GetFilteredProducts(column, criteria);
            UpdateResults(filteredProducts);
        }

    }
}
