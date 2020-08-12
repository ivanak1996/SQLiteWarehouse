using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using Dapper;
using SQLiteXP.Models;

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
                    acIdent varchar(16), acName VARCHAR(250),acUM VARCHAR(3),anVat FLOAT,anSalePrice FLOAT,
                    anRtPrice FLOAT, anPluCode INT, acBarCode VARCHAR(100))";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS Customers";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Customers (id INTEGER PRIMARY KEY NOT NULL,
                    acSubject varchar(30), acName2 VARCHAR(250),acAddress VARCHAR(250),acPost VARCHAR(10),
                    acCity varchar(250), acCode varchar(20), acRegNo  varchar(20), anRebate decimal(19,6),
                    anDaysForPayment INT)";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = "DROP TABLE IF EXISTS Stock";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Stock (id INTEGER PRIMARY KEY NOT NULL,
                    acWarehouse varchar(30), acIdent VARCHAR(16),anStock decimal(19, 6))";
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
