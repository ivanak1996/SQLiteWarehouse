using SQLiteXP.Service;
using SQLiteXP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using System.Linq;
using SQLiteXP.Models.Billing;
using System.Drawing;

namespace SQLiteXP
{
    public partial class ProductListForm : Form
    {
        private List<Products> products = new List<Products>();
        private DataTable productsTable;
        private Dictionary<string, string> comboBoxOptions = new Dictionary<string, string>();

        public ProductListForm()
        {
            InitializeComponent();
            products = WarehouseService.GetProducts();
            InitDataTable();

            comboBoxOptions.Add("Sifra", "sifra");
            comboBoxOptions.Add("Naziv", "naziv");
            comboBoxOptions.Add("Barkod", "barkod");

            foreach (var key in comboBoxOptions.Keys)
            {
                comboBox_searchColumn.Items.Add(key);
            }

            comboBox_searchColumn.SelectedIndex = 0;
        }
        
        private void PopulateDataTable()
        {
            productsTable.Rows.Clear();
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
        }              

        private void InitDataTable()
        {
            productsTable = new DataTable("Products");

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

            PopulateDataTable();
            
            dataGridView_products.DataSource = productsTable;
            dataGridView_products.ReadOnly = true;
            dataGridView_products.CellDoubleClick += DataGridView_CellDoubleClick;
            dataGridView_products.DefaultCellStyle.Font = new Font("Arial", 18.5F, GraphicsUnit.Pixel);
        }

        public string GetSelectedProduct_Sifra()
        {
            return dataGridView_products.SelectedCells.Count > 0
                ? dataGridView_products.Rows[dataGridView_products.SelectedCells[0].RowIndex].Cells["Sifra"].Value.ToString()
                : string.Empty;
        }

        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DialogResult = DialogResult.OK;            
        }

        private void UpdateResults(List<Products> updatedProducts)
        {
            products = updatedProducts;
            //InitDataTable();
            PopulateDataTable();
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
