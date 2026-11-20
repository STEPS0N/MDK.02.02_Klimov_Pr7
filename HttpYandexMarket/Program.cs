using System.Diagnostics;
using System.Net;
using System.Text;

namespace HttpYandexMarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            SingIn("user", "user");
            Console.Read();
        }

        public static CookieContainer SingIn(string Login, string Password)
        {
            string url = "http://10.111.20.114/ajax/login.php";
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            CookieContainer cookies = new CookieContainer();
            request.CookieContainer = cookies;
            string postData = $"login={Login}&password={Password}";
            byte[] Data = Encoding.ASCII.GetBytes(postData);
            request.ContentLength = Data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(Data, 0, Data.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.WriteLine($"Статуc выполнения: {response.StatusCode}");
            string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
            Console.WriteLine(responseFromServer);

            return cookies;
        }
    }
}
