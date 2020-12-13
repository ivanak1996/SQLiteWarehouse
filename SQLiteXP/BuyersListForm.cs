using SQLiteXP.Models;
using SQLiteXP.Service;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace SQLiteXP
{
    public partial class BuyersListForm : Form
    {
        private List<Buyers> buyers = new List<Buyers>();
        public BuyersListForm()
        {
            InitializeComponent();
            buyers = WarehouseService.GetBuyers();
            PopulateTable();
        }

        private void PopulateTable()
        {
            DataTable buyersTable = new DataTable("Buyers");

            DataColumn c0 = new DataColumn("ID");
            DataColumn c1 = new DataColumn("Sifra");
            DataColumn c2 = new DataColumn("Naziv");
            DataColumn c3 = new DataColumn("Adresa");
            DataColumn c4 = new DataColumn("Posta");
            DataColumn c5 = new DataColumn("Grad");
            DataColumn c6 = new DataColumn("PIB");
            DataColumn c7 = new DataColumn("Maticni broj");

            buyersTable.Columns.Add(c0);
            buyersTable.Columns.Add(c1);
            buyersTable.Columns.Add(c2);
            buyersTable.Columns.Add(c3);
            buyersTable.Columns.Add(c4);
            buyersTable.Columns.Add(c5);
            buyersTable.Columns.Add(c6);
            buyersTable.Columns.Add(c7);

            foreach (var buyer in buyers)
            {
                var row = buyersTable.NewRow();
                row["ID"] = buyer.ident;
                row["Sifra"] = buyer.sifra;
                row["Naziv"] = buyer.naziv;
                row["Adresa"] = buyer.adresa;
                row["Posta"] = buyer.posta;
                row["Grad"] = buyer.grad;
                row["PIB"] = buyer.pib;
                row["Maticni broj"] = buyer.maticniBroj;
                buyersTable.Rows.Add(row);
            }

            dataGridView_buyers.DataSource = buyersTable;
            dataGridView_buyers.ReadOnly = true;
        }

    }
}
