using HtmlAgilityPack;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.IO;

namespace HttpYandexMarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TextWriterTraceListener fileListener = new TextWriterTraceListener("debug_trace.log");
            Trace.Listeners.Add(fileListener);
            Trace.AutoFlush = true;

            Cookie token = SingIn("user", "user");
            string html = GetContent(token);
            ParsingHtml(html);
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

        public static void ParsingHtml(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;
            IEnumerable DivsNews = Document.Descendants(0).Where(n => n.HasClass("news"));

            foreach (HtmlNode DivNews in DivsNews)
            {
                var src = DivNews.ChildNodes[1].GetAttributeValue("src", "none");
                var name = DivNews.ChildNodes[3].InnerText;
                var description = DivNews.ChildNodes[5].InnerText;

                Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description + "\n");

                Debug.WriteLine($"[{DateTime.Now}] Новость: {name} | Картинка: {src}");
            }
        }
    }
}
