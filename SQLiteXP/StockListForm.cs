using SQLiteXP.Models;
using SQLiteXP.Service;
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
    public partial class StockListForm : Form
    {
        private List<StockWithProductInfo> stockList = new List<StockWithProductInfo>();
        public StockListForm()
        {
            InitializeComponent();
            stockList = WarehouseService.GetStockWithProductInfo();
            this.PopulateTable();
        }

        private void PopulateTable()
        {
            DataTable productsTable = new DataTable("Products");

            DataColumn c0 = new DataColumn("ID");
            DataColumn c1 = new DataColumn("Sifra skladista");
            DataColumn c2 = new DataColumn("Sifra proizvoda");
            DataColumn c3 = new DataColumn("Zaliha");
            DataColumn c4 = new DataColumn("Naziv");
            DataColumn c5 = new DataColumn("JM");

            productsTable.Columns.Add(c0);
            productsTable.Columns.Add(c1);
            productsTable.Columns.Add(c2);
            productsTable.Columns.Add(c3);
            productsTable.Columns.Add(c4);
            productsTable.Columns.Add(c5);

            foreach (var stock in stockList)
            {
                var row = productsTable.NewRow();
                row["ID"] = stock.anRow;
                row["Sifra skladista"] = stock.sifraSkladista;
                row["Sifra proizvoda"] = stock.sifraProizvoda;
                row["Zaliha"] = stock.zaliha;
                row["Naziv"] = stock.naziv;
                row["JM"] = stock.jm;
                productsTable.Rows.Add(row);
            }
            dataGridView_stockWithProductInfo.DataSource = productsTable;
            dataGridView_stockWithProductInfo.ReadOnly = true;
        }
    }
}
