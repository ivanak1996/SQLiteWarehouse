using SQLiteXP.Service;
using SQLiteXP.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace SQLiteXP
{
    public partial class ProductListForm : Form
    {
        private List<Products> products = new List<Products>();
        private Dictionary<string, string> comboBoxOptions = new Dictionary<string, string>();

        public ProductListForm()
        {
            InitializeComponent();
            products = WarehouseService.GetProducts();
            PopulateTable();
            comboBoxOptions.Add("Name", "acName");
            comboBoxOptions.Add("Barcode", "acBarCode");

            foreach (var key in comboBoxOptions.Keys)
            {
                comboBox_searchColumn.Items.Add(key);
            }

            comboBox_searchColumn.SelectedIndex = 0;
        }

        private void PopulateTable()
        {
            DataTable productsTable = new DataTable("Products");

            DataColumn c0 = new DataColumn("ID");
            DataColumn c1 = new DataColumn("Name");
            DataColumn c2 = new DataColumn("Price");
            DataColumn c3 = new DataColumn("Retail Price");
            DataColumn c4 = new DataColumn("Barcode");

            productsTable.Columns.Add(c0);
            productsTable.Columns.Add(c1);
            productsTable.Columns.Add(c2);
            productsTable.Columns.Add(c3);
            productsTable.Columns.Add(c4);

            foreach(var product in products)
            {
                var row = productsTable.NewRow();
                row["ID"] = product.id;
                row["Name"] = product.acName;
                row["Price"] = product.anSalePrice;
                row["Retail Price"] = product.anRtPrice;
                row["Barcode"] = product.acBarCode;
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
