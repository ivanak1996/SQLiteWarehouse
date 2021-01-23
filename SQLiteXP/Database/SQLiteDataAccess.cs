using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using SQLiteXP.Models;
using SQLiteXP.Models.Billing;

namespace SQLiteXP.Database
{
    class SQLiteDataAccess
    {
        const string dbPath = @".\WarehouseDb.db";

        #region generics
        public static List<T> GetAll<T>()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                var output = cnn.Query<T>($"select * from {typeof(T).Name}", new DynamicParameters());
                return output.ToList();
            }
        }
        private static string GetGenericQueryString<T>(T obj)
        {
            string query = $"insert into {obj.GetType().Name}(";
            string values = $" values(";

            var props = obj.GetType().GetProperties().Where(p => p.Name != "id").ToList();

            for (int i = 0; i < props.Count() - 1; i++)
            {
                var field = props[i];
                query += $"{field.Name},";
                values += $"@{field.Name },";
            }

            query += $"{props.Last().Name})";
            values += $"@{props.Last().Name})";

            return query + values;
        }

        internal static IList<BillItem> GetAllBillItemsForBill(int billId)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                string processQuery = $"select * from BillItem where billId={billId}";
                return cnn.Query<BillItem>(processQuery, new DynamicParameters()).ToList();
            }
        }

        internal static List<Bill> GetAllBillsWithDocType(string docType)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                string processQuery = $"select * from Bill where docType='{docType}'";
                return cnn.Query<Bill>(processQuery, new DynamicParameters()).ToList();
            }
        }

        #endregion

        #region billing

        internal static Bill GetOpenBill()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                string processQuery = $"select * from Bill where status='{Bill.STATUS_OPEN}'";
                var openBill = cnn.Query<Bill>(processQuery, new DynamicParameters()).FirstOrDefault();
                if (openBill != null)
                {
                    processQuery = $"select * from BillItem where billId={openBill.Id}";
                    openBill.Items = cnn.Query<BillItem>(processQuery, new DynamicParameters()).ToList();
                }
                else
                {
                    openBill = new Bill()
                    {
                        Status = Bill.STATUS_OPEN
                    };
                    cnn.Execute("insert into Bill (DateCreated, status) values (@DateCreated, @status)", openBill);
                }
                return openBill;
            }
        }

        internal static void InsertBill(Bill bill)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                cnn.Execute("insert into Bill (DateCreated) values (@DateCreated)", bill);
            }
        }

        internal static void InsertBillItem(BillItem billItem)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                var query = GetGenericQueryString(billItem);
                cnn.Execute(query, billItem);
            }
        }

        internal static void DeleteItem(BillItem item)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                cnn.Execute($"delete from BillItem" +
                    $" where productIdent={item.productIdent}", new DynamicParameters());
            }
        }

        internal static void UpdateItem(BillItem existingItem)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                cnn.Execute($"update BillItem " +
                    $"set quantity={existingItem.Quantity}, productCena={existingItem.productCena}, productPopust={existingItem.productPopust} " +
                    $"where productIdent={existingItem.productIdent}", new DynamicParameters());
            }
        }

        internal static Bill CreateNewBill(int year, string docType)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {                
                string processQuery = $"select max(ordinaryNumber) from Bill where docType='{docType}'";
                int nextOrdinaryId = 1;
                var result = cnn.Query<int?>(processQuery, new DynamicParameters()).FirstOrDefault() ?? 0;
                nextOrdinaryId = 1 + result;
                

                Bill bill = new Bill(year, docType, nextOrdinaryId)
                {
                    Status = Bill.STATUS_OPEN
                };

                processQuery = @"INSERT INTO Bill (DateCreated, status, year, docType, ordinaryNumber) 
                                VALUES (@DateCreated, @status, @year, @docType, @ordinaryNumber)";
                cnn.Execute(processQuery, bill);

                processQuery = $"select Id from Bill where docType='{docType}' and year={bill.year} and ordinaryNumber={nextOrdinaryId}";

                bill.Id = cnn.Query<int>(processQuery, new DynamicParameters()).FirstOrDefault();

                return bill;
            }
        }
        #endregion

        #region users
        public static void SaveUserAsLoggedIn(Users user)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                cnn.Execute("insert into Users (user, pass, loggedIn) values (@user, @pass, @loggedIn)", user);
            }
        }        

        public static Users GetLoggedInUser()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                string processQuery = @"select * from Users where loggedIn=1";
                return cnn.Query<Users>(processQuery, new DynamicParameters()).FirstOrDefault();
            }
        }
        
        public static void LogoutUser()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                string processQuery = @"delete from Users where loggedIn=1";
                cnn.Execute(processQuery, new DynamicParameters());
            }
        }

        #endregion

        #region docTypes
        public static void SaveDocTypes(List<DocTypes> docTypes)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                string processQuery = @"INSERT INTO DocTypes(acDocType, acName, anType) 
                                        VALUES (@acDocType, @acName, @anType)";
                cnn.Execute(processQuery, docTypes);
            }
        }

        #endregion

        #region buyers
        public static void SaveBuyers(List<Buyers> buyers)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                var query = GetGenericQueryString(new Buyers());
                cnn.Execute(query, buyers);
            }
        }

        #endregion

        #region products
        public static List<Products> GetFilteredProducts(string column, string criteria)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                var output = cnn.Query<Products>($"select * from Products where {column} like @crit",
                    new { crit = '%'+criteria+'%' });
                return output.ToList();
            }
        }

        public static void SaveProducts(List<Products> productsList)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                var query = GetGenericQueryString(new Products());
                cnn.Execute(query, productsList);
            }
        }

        public static void SaveStockWithProductInfo(List<Stock> stock, List<Products> products)
        {
            List<StockWithProductInfo> stockWithProducts = (from s in stock
                                     join p in products on s.sifraProizvoda equals p.sifra
                                     select new StockWithProductInfo()
                                     {
                                         sifraProizvoda = s.sifraProizvoda,
                                         sifraSkladista = s.sifraSkladista,
                                         naziv = p.naziv,
                                         jm = p.jm,
                                         anRow = s.anRow
                                     }).ToList();

            using (IDbConnection cnn = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                var query = GetGenericQueryString(new StockWithProductInfo());
                cnn.Execute(query, stockWithProducts);
            }
        }

        #endregion

        #region setup

        public static void CreateDbIfNotExists()
        {
            if (!File.Exists(dbPath))
            {
                CreateWarehouseDb();
            }
        }

        public static void RecreateDb()
        {
            using (var con = new SQLiteConnection(LoadWarehouseConnectionString()))
            {
                con.Open();

                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = "delete from DocTypes";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from Products";
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = "delete from StockWithProductInfo";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateWarehouseDb()
        {
            string cs = LoadWarehouseConnectionString();

            using (var con = new SQLiteConnection(cs))
            {
                con.Open();

                using (var cmd = new SQLiteCommand(con))
                {
                    // TODO: add stavke dokumenta & dokumenti
                    cmd.CommandText = "DROP TABLE IF EXISTS Settings";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Settings(id INTEGER PRIMARY KEY NOT NULL,
                    user TEXT, pass TEXT, acDocType VARCHAR(4), acWarehouse VARCHAR(30))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS Users";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Users(id INTEGER PRIMARY KEY NOT NULL,
                    user TEXT, pass varchar(5), loggedIn INT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS Products";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Products (id INTEGER PRIMARY KEY NOT NULL,
                    ident integer, sifra VARCHAR(16),naziv VARCHAR(250),grupa VARCHAR(250),jm VARCHAR(10),
                    barkod VARCHAR(50), pdv float, cena float, popust float, opis varchar(4000))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS Buyers";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Buyers (id INTEGER PRIMARY KEY NOT NULL,
                    ident integer, sifra VARCHAR(50),naziv VARCHAR(250),adresa VARCHAR(250),
                    posta varchar(10), grad varchar(250), pib varchar(50), maticniBroj varchar(50))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS StockWithProductInfo";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS StockWithProductInfo (id INTEGER PRIMARY KEY NOT NULL,
                    sifraSkladista varchar(30), sifraProizvoda VARCHAR(16), zaliha float, naziv VARCHAR(250),anRow VARCHAR(16),jm VARCHAR(10))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS DocTypes";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS DocTypes(id INTEGER PRIMARY KEY NOT NULL,
                    acDocType varchar(4), acName VARCHAR(250),anType varchar(4))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS Pricebooks";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Pricebooks (id INTEGER PRIMARY KEY NOT NULL,
                    acIdent VARCHAR(16), acSubject varchar(30),anSalePrice FLOAT, anRtPrice FLOAT, anRebate  decimal(19,6), 
                    adDateStart TEXT, adDateEnd TEXT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS DocHeader";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS DocHeader (id INTEGER PRIMARY KEY NOT NULL,acKey varchar(13),
                    acDocType varchar(4), adDate date,acReceiver  varchar(30), acPrsn3 varchar(30),acStatus varchar(1),
                    acPosted varchar(1),acName2 varchar(250),acAddress varchar(250),acPost varchar(250),acCity varchar(250),
                    acCountry varchar(250),anClerk int,anUserIns int,anUserChg int,adTimeIns datetime,adTimeChg datetime)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS DocItems";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS DocItems (id INTEGER PRIMARY KEY NOT NULL, acKey varchar(13),anNo int ,
                    acIdent varchar(16), acName varchar(250),anQty decimal(19,6),anPrice decimal(19,6),
                    anRebate decimal(19,6),anVat decimal(19,6),acVatCode varchar(2),acUM varchar(10))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS Bill";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS Bill (id INTEGER PRIMARY KEY NOT NULL, 
                    dateCreated datetime, status varchar(20), year integer, docType varchar(4), ordinaryNumber integer)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS BillItem";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @" CREATE TABLE IF NOT EXISTS BillItem (id INTEGER PRIMARY KEY NOT NULL, 
                    productIdent int, productCena float, productPopust float,productSifra VARCHAR(16), productNaziv VARCHAR(250), productJM VARCHAR(10), 
                    productBarkod VARCHAR(50), productPdv float , 
                    Quantity float, billId int)";
                    cmd.ExecuteNonQuery();

                }
            }
        }

        private static string LoadWarehouseConnectionString()
        {
            return $"URI=file:{dbPath}";
        }

        #endregion
    }
}
