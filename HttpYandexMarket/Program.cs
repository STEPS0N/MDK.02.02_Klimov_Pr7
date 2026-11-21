using System.Diagnostics;
using System.Net;
using System.Text;

namespace HttpYandexMarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Cookie token = SingIn("user", "user");
            string html = GetContent(token);
            Console.WriteLine(html);
            Console.WriteLine(token.Value);
            Console.Read();
        }

        public static Cookie SingIn(string Login, string Password)
        {
            string url = "http://localhost/ajax/login.php";
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
            //Console.WriteLine(responseFromServer);

            Cookie token = cookies.GetCookies(new Uri(url))["token"];

            return token;
        }

        public static string GetContent(Cookie Token)
        {
            string url = "http://localhost/main.php";
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(Token);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
            
            return responseFromServer;
        }
    }
}
