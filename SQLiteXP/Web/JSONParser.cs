using Newtonsoft.Json.Linq;
using SQLiteXP.Models;
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
                    acName = ch["c"].ToString(),
                    acBarCode = ch["b"].ToString(),
                    acIdent = ch["a"].ToString(),
                    anPluCode = ch["d"].ToString(),
                    acUM = ch["i"].ToString(),
                    anVat = float.Parse(ch["v"].ToString()),
                    anSalePrice = float.Parse(ch["v"].ToString()),
                    anRtPrice = float.Parse(ch["p"].ToString())
                });
            }

            return productList;
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
