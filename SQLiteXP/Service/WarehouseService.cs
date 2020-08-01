using Newtonsoft.Json;
using SQLiteXP.Database;
using SQLiteXP.Models;
using SQLiteXP.Web;
using System;
using System.Collections.Generic;
using System.Net;

namespace SQLiteXP.Service
{
    public class WarehouseService
    {
        public static void CreateDbIfNotExists()
        {
            SQLiteDataAccess.CreateDbIfNotExists();
        }

        private static void RecreateDb()
        {
            SQLiteDataAccess.RecreateDb();
        }

        public static List<Products> GetProducts()
        {
            return SQLiteDataAccess.GetAll<Products>();
        }

        public static List<Products> GetFilteredProducts(string column, string criteria)
        {
            return SQLiteDataAccess.GetFilteredProducts(column, criteria);
        }

        public static Users GetLoggedInUser()
        {
            return SQLiteDataAccess.GetLoggedInUser();
        }

        public static bool SyncData(string userName, string userPass)
        {
            try
            {
                var products = GetProductsFromAPI(userName, userPass);
                var docs = GetDocTypesFromAPI(userName, userPass);
                RecreateDb();
                SQLiteDataAccess.SaveProducts(products);
                SQLiteDataAccess.SaveDocTypes(docs);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static List<string> GetDocTypesNames()
        {
            List<DocTypes> docTypes = SQLiteDataAccess.GetAll<DocTypes>();
            return docTypes.ConvertAll(d => $"{d.acDocType} {d.acName}");
        }

        public static Users LoginUser(string userEmail, string userPass)
        {
            try
            {
                Users user = null;
                if (JSONParser.LoginUser(userEmail, userPass))
                {
                    user = new Users()
                    {
                        loggedIn = 1,
                        user = userEmail,
                        pass = userPass
                    };
                    SQLiteDataAccess.SaveUserAsLoggedIn(user);
                }
                return user;
            }
            catch (JsonReaderException)
            {
                throw new Exception("Pogresno korisnicko ime ili lozinka");
            }
            catch (WebException)
            {
                throw new Exception("Neuspesno povezivanje na server");
            }
        }

        private static List<Products> GetProductsFromAPI(string userEmail, string userPass)
        {
            return JSONParser.GetProducts(userEmail, userPass);
        }

        private static List<DocTypes> GetDocTypesFromAPI(string userEmail, string userPass)
        {
            return JSONParser.GetDocTypes(userEmail, userPass);
        }


        public static void Logout()
        {
            SQLiteDataAccess.LogoutUser();
        }

    }
}
