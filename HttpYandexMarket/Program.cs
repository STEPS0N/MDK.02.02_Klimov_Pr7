using HtmlAgilityPack;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace HttpYandexMarket
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TextWriterTraceListener fileListener = new TextWriterTraceListener("debug_trace.log");
            Trace.Listeners.Add(fileListener);
            Trace.AutoFlush = true;

            string html = GetContentNews();
            ParsingHtmlNews(html);

            //Cookie token = SingIn("admin", "admin");
            //AddNews(token);

            //string html = GetContent(token);
            //ParsingHtml(html);
            //Console.WriteLine(html);
            //Console.WriteLine(token.Value);
            Console.Read();
        }

        //public static Cookie SingIn(string Login, string Password)
        //{
        //    string url = "http://localhost/ajax/login.php";
        //    Debug.WriteLine($"Выполняем запрос: {url}");
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Method = "POST";
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    CookieContainer cookies = new CookieContainer();
        //    request.CookieContainer = cookies;
        //    string postData = $"login={Login}&password={Password}";
        //    byte[] Data = Encoding.ASCII.GetBytes(postData);
        //    request.ContentLength = Data.Length;

        //    using (var stream = request.GetRequestStream())
        //    {
        //        stream.Write(Data, 0, Data.Length);
        //    }

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    Debug.WriteLine($"Статуc выполнения: {response.StatusCode}");
        //    string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
        //    //Console.WriteLine(responseFromServer);

        //    Cookie token = cookies.GetCookies(new Uri(url))["token"];

        //    return token;
        //}

        //public static void AddNews(Cookie Token)
        //{
        //    Console.WriteLine("Ссылка на картинку:");
        //    string src = Console.ReadLine();

        //    Console.WriteLine("Название:");
        //    string title = Console.ReadLine();

        //    Console.WriteLine("Описание:");
        //    string description = Console.ReadLine();

        //    string url = "http://localhost/ajax/add.php";
        //    Debug.WriteLine($"Выполняем запрос: {url}");

        //    string postData = "src=" + src + "&" +
        //        "name=" + title + "&" +
        //        "description=" + description;

        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Method = "POST";
        //    request.ContentType = "application/x-www-form-urlencoded";

        //    CookieContainer cookies = new CookieContainer();
        //    cookies.Add(Token);
        //    request.CookieContainer = cookies;

        //    byte[] Data = Encoding.UTF8.GetBytes(postData);
        //    request.ContentLength = Data.Length;

        //    using (var stream = request.GetRequestStream())
        //    {
        //        stream.Write(Data, 0, Data.Length);
        //    }

        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    Debug.WriteLine($"[{DateTime.Now}] Статус добавления: {response.StatusCode}");
        //    Console.WriteLine($"Статус: {response.StatusCode}");
        //}

        //public static string GetContentAviat(Cookie Token)
        //{
        //    string url = "http://localhost/main.php";
        //    Debug.WriteLine($"Выполняем запрос: {url}");
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.CookieContainer = new CookieContainer();
        //    request.CookieContainer.Add(Token);
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
        //    string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //    return responseFromServer;
        //}

        //public static void ParsingHtmlAviat(string htmlCode)
        //{
        //    var html = new HtmlDocument();
        //    html.LoadHtml(htmlCode);
        //    var Document = html.DocumentNode;
        //    IEnumerable DivsNews = Document.Descendants(0).Where(n => n.HasClass("news"));

        //    foreach (HtmlNode DivNews in DivsNews)
        //    {
        //        var src = DivNews.ChildNodes[1].GetAttributeValue("src", "none");
        //        var name = DivNews.ChildNodes[3].InnerText;
        //        var description = DivNews.ChildNodes[5].InnerText;

        //        Console.WriteLine(name + "\n" + "Изображение: " + src + "\n" + "Описание: " + description + "\n");

        //        Debug.WriteLine($"[{DateTime.Now}] Новость: {name} | Картинка: {src}");
        //    }
        //}

        public static string GetContentNews()
        {
            string url = "https://lenta.ru/";
            Debug.WriteLine($"Выполняем запрос: {url}");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Debug.WriteLine($"Статус выполнения: {response.StatusCode}");
            string responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseFromServer;
        }

        public static void ParsingHtmlNews(string htmlCode)
        {
            var html = new HtmlDocument();
            html.LoadHtml(htmlCode);
            var Document = html.DocumentNode;

            var newsCards = Document.SelectNodes("//a[contains(@class, 'card-mini')]");

            int count = 0;
            var seen = new HashSet<string>();

            foreach (var card in newsCards)
            {
                var href = card.GetAttributeValue("href", "");
                if (string.IsNullOrEmpty(href))
                    continue;

                if (href.StartsWith("/"))
                {
                    href = "https://lenta.ru" + href;
                }

                var titleNode = card.SelectSingleNode(".//span[contains(@class, 'card-mini__title')]");
                var title = titleNode.InnerText.Trim();

                var timeNode = card.SelectSingleNode(".//div[contains(@class, 'card-mini__info')]//time");
                var timeText = timeNode.InnerText.Trim();

                Console.WriteLine(title);
                Console.WriteLine("Время: " + timeText);
                Console.WriteLine("Ссылка: " + href);
                Console.WriteLine();

                Debug.WriteLine($"[{DateTime.Now}] Новость: {title} | Время: {timeText} | {href}");
                count++;
            }
        }
    }
}
