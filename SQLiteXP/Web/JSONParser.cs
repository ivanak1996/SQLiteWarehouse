using Newtonsoft.Json.Linq;
using SQLiteXP.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;

namespace SQLiteXP.Web
{
    public static class JSONParser
    {
        static readonly WebClient client = new WebClient()
        {
            Encoding = Encoding.UTF8,

        };
        const string API_URL = "http://89.216.122.162:8080/tkomserver/jsonData/api";

        public static List<Products> GetProducts(string userEmail, string userPass)
        {
            var jResult = GenericJsonResultGetter(userEmail, userPass, "products");
            var jList = jResult["products"];

            List<Products> productList = new List<Products>();
            foreach (var token in jList)
            {
                var ch = JObject.Parse(token.ToString());
                productList.Add(new Products()
                {
                    ident = int.Parse(ch["a"].ToString()),
                    sifra = ch["b"].ToString().Trim(' '),
                    naziv = ch["c"].ToString(),
                    grupa = ch["d"].ToString(),
                    jm = ch["e"].ToString(),
                    barkod = ch["f"].ToString(),
                    pdv = float.Parse(ch["v"].ToString()),
                    cena = float.Parse(ch["p"].ToString()),
                    popust = float.Parse(ch["r"].ToString()),
                    opis = ch["i"].ToString()
                });
            }

            return productList;
        }

        public static List<Buyers> GetBuyers(string userEmail, string userPass)
        {
            var jResult = GenericJsonResultGetter(userEmail, userPass, "buyers");
            var jList = jResult["buyers"];

            List<Buyers> buyersList = new List<Buyers>();
            foreach (var token in jList)
            {
                var ch = JObject.Parse(token.ToString());
                buyersList.Add(new Buyers()
                {
                    ident = int.Parse(ch["a"].ToString()),
                    sifra = ch["b"].ToString(),
                    naziv = ch["c"].ToString(),
                    adresa = ch["d"].ToString(),
                    posta = ch["e"].ToString(),
                    grad = ch["f"].ToString(),
                    pib = ch["v"].ToString(),
                    maticniBroj = ch["p"].ToString()
                });
            }

            return buyersList;
        }

        public static bool LoginUser(string userEmail, string userPass)
        {
            var jResult = GenericJsonResultGetter(userEmail, userPass, "login");
            return jResult["login"] != null ? true : false;

        }

        public static List<DocTypes> GetDocTypes(string userEmail, string userPass)
        {
            var jResult = GenericJsonResultGetter(userEmail, userPass, "doctype");
            var jList = jResult["doctype"];

            List<DocTypes> docTypes = new List<DocTypes>();
            foreach (var token in jList)
            {
                var ch = JObject.Parse(token.ToString());
                docTypes.Add(new DocTypes()
                {
                    acName = ch["acName"].ToString(),
                    anType = ch["anType"].ToString(),
                    acDocType = ch["acDocType"].ToString(),
                });
            }

            return docTypes;
        }

        public static List<Stock> GetStock(string userEmail, string userPass)
        {
            var jResult = GenericJsonResultGetter(userEmail, userPass, "stock");
            var jList = jResult["stock"];

            List<Stock> stock = new List<Stock>();
            foreach (var token in jList)
            {
                var ch = JObject.Parse(token.ToString());
                stock.Add(new Stock()
                {
                    sifraSkladista = ch["a"].ToString(),
                    sifraProizvoda = ch["b"].ToString().Trim(' '),
                    anRow = ch["anRow"].ToString(),
                    zaliha = float.Parse(ch["c"].ToString())
                });
            }

            return stock;
        }

        private static JObject GenericJsonResultGetter(string userEmail, string userPass, string action)
        {
            byte[] response =
            client.UploadValues(API_URL, new NameValueCollection()
            {
                        { "userEmail", $"{userEmail}" },
                        { "userPass", $"{userPass}" },
                        {"action", $"{action}" }
            });

            string result = Encoding.UTF8.GetString(response);
            return JObject.Parse(result);

        }

    }
}
