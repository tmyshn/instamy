using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using Newtonsoft.Json;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using System.Threading;
namespace instamy
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        CookieCollection cookies = new CookieCollection();
        bool first = false;
        bool takipcifirst = true;
        string kullanci = "kaplumbabatv";
        string parola = "256254ty33*";
        string id = "";
        string pattern = "\"([0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]?)\"";
        //  string pattern_count = "\"count\": ?([0-9]*?,)";

        string idname = "\"id\": \"[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]?\", \"username\": \".*?\"";
        string pattern_count = "\"count\": ?[0-9]*";
        string pattern_cursor = "\"end_cursor\":(.*?})";
        Dictionary<string, string> takipciler = new Dictionary<string, string>();
        Dictionary<string, string> takipciler2 = new Dictionary<string, string>();

        int count = 0;
        string cursor = "";
        //string pattern = "^(\"id\":)(.)";
      





        private void Form1_Load(object sender, EventArgs e)
        {

        }



        private void button1_Click(object sender, EventArgs e)
        {
            //CookieContainer cookies = new CookieContainer();
            // CookieCollection cookies = new CookieCollection();
            Uri uri = new Uri("https://www.instagram.com/accounts/login/");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/accounts/login/");

            request.Method = WebRequestMethods.Http.Get;
            // -------- Cookie İşlemleri------------
            request.CookieContainer = new CookieContainer();
            request.CookieContainer.Add(cookies);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            cookies = response.Cookies;

            string csrftoken = cookies["csrftoken"].Value;
            string rur = cookies["rur"].ToString();
            string mid = cookies["mid"].ToString();


            var responseString2 = new StreamReader(response.GetResponseStream()).ReadToEnd();
            //  MessageBox.Show(responseString2);
            // var s= cookies.GetCookies(new Uri("https://www.instagram.com/accounts/login/"))["csrftoken"].Value;

            /* foreach (Cookie item in cookies)
              {
                  MessageBox.Show(item.Name+item.Value);
              }*/

            // MessageBox.Show(csrftoken);


            //------------- 2. Aşama

            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/accounts/login/ajax/");
            request2.Method = WebRequestMethods.Http.Post;

            // -------- Cookie İşlemleri------------
            request2.CookieContainer = new CookieContainer();
            request2.CookieContainer.Add(cookies);

            //   string postData = String.Format("email={0}&pass={1}", "value1", "value2");
            var postData = "username=" + kullanci;
            postData += "&password=" + parola;
            var data = Encoding.ASCII.GetBytes(postData);
            request2.ContentLength = data.Length;
            request2.Accept = "*/*";
            request2.ContentType = "application/x-www-form-urlencoded";
            request2.KeepAlive = true;
            request2.Referer = "https://www.instagram.com/accounts/login/";

            request2.KeepAlive = true;
            request2.Headers["origin"] = "https://www.instagram.com";
            //  request2.Headers["X-CSRFToken"] = request2.CookieContainer.GetCookies(new Uri("https://www.instagram.com/accounts/login/"))["csrftoken"].Value;
            request2.Headers["x-csrftoken"] = csrftoken;
            request2.Headers["x-instagram-ajax"] = "1";
            request2.Headers["x-requested-with"] = "XMLHttpRequest";

            request2.KeepAlive = true;

            request2.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.133 Safari/537.36";
            request2.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request2.Headers.Add("Accept-Encoding", "gzip, deflate, br");


            using (var stream = request2.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Close();
            }

            var response2 = (HttpWebResponse)request2.GetResponse();
            cookies = response2.Cookies;

            // var responseString = new StreamReader(response2.GetResponseStream()).ReadToEnd();

            StreamReader responseString = new StreamReader(response2.GetResponseStream());




            // JsonConvert.DeserializeObject<LoginResult>(responseString);
            // Clipboard.SetText(responseString);





            //------------- 3. Aşama ----------------------------------------------------------------

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

            // string pattern = @"""id"":(.){12}";
            HttpWebRequest request3 = (HttpWebRequest)WebRequest.Create("https://www.instagram.com");

            request3.Method = WebRequestMethods.Http.Get;
            // -------- Cookie İşlemleri------------
            request3.CookieContainer = new CookieContainer();
            request3.CookieContainer.Add(cookies);

            request3.Referer = "https://www.instagram.com/accounts/login/";
            request3.Headers["x-csrftoken"] = csrftoken;
            request3.Headers["X-Instagram-AJAX"] = "1";
            request3.Headers["X-Requested-With"] = "XMLHttpRequest";

            HttpWebResponse response3 = (HttpWebResponse)request3.GetResponse();

            cookies = response3.Cookies;


            StreamReader responseString3 = new StreamReader(response3.GetResponseStream());
            doc.Load(responseString3);
            var oku = responseString3.ReadToEnd();
            //JsonConvert.DeserializeObject<LoginResult>(oku);
            // MessageBox.Show(doc.DocumentNode.OuterHtml);


            //  Clipboard.SetText(doc.DocumentNode.OuterHtml);


            //  https://www.instagram.com/graphql/query/?query_id=17866917712078875&fetch_media_item_count=1 // Anasayfa yüklenenler
            // https://www.instagram.com/graphql/query/?query_id=17874545323001329&id=5399607028&first=10 // Takip kontrol

            idcek(doc.DocumentNode.OuterHtml, pattern);

            MessageBox.Show(id);

            takipliste("https://www.instagram.com/graphql/query/?query_id=17874545323001329&id=" +id+ "&first=5000&after=" + cursor);
            //first = false;

            
            foreach (var item in takipciler)
            {

                var key =item.Key.Replace('\"', ' ');
                MessageBox.Show(item.Value);
                takipliste("https://www.instagram.com/graphql/query/?query_id=17874545323001329&id="+key+"&first=5000&after=" + cursor);
                Thread.Sleep(500);

              /*  ListViewItem lvi = new ListViewItem();
                lvi.Text = item.Key;
                lvi.SubItems.Add(item.Value);
                listView1.Items.Add(lvi);
                */
            }

           
          

        }
            
        /*  public string idcek(string uname, CookieCollection cc)
          {
              HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://www.instagram.com/" + uname);

              request.Method = WebRequestMethods.Http.Get;
              // -------- Cookie İşlemleri------------
              request.CookieContainer = new CookieContainer();
              request.CookieContainer.Add(cc);

              HttpWebResponse response = (HttpWebResponse)request.GetResponse();
              StreamReader rs = new StreamReader(response.GetResponseStream());
              MessageBox.Show(rs.ReadToEnd().ToString());
              cc = response.Cookies;

              return "";
          }*/


        private static HttpWebRequest CreateRequest(Uri uri, CookieContainer cookies = null)
        {
            var request = WebRequest.Create(uri) as HttpWebRequest;
            request.ProtocolVersion = HttpVersion.Version11;
            request.Timeout = 10000;
            request.Host = uri.Host;
            request.Accept = "*/*";
            request.KeepAlive = true;

            request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:46.0) Gecko/20100101 Firefox/46.0";
            request.Headers.Add("Accept-Language", "en-US,en;q=0.5");
            request.Headers.Add("Accept-Encoding", "gzip, deflate, br");
            if (cookies != null) request.CookieContainer = cookies;
            return request;
        }

        public static HttpWebRequest Post(string url, CookieContainer cookies = null)
        {
            var request = CreateRequest(new Uri(url), cookies);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "application/x-www-form-urlencoded";
            return request;
        }

        public static HttpWebRequest Get(string url, CookieContainer cookies = null)
        {
            var request = CreateRequest(new Uri(url), cookies);
            request.Method = WebRequestMethods.Http.Get;
            return request;
        }


        public class LoginResult
        {
            public string status { get; set; }
            public bool reactivated { get; set; }
            public bool authenticated { get; set; }
            public string user { get; set; }
            public string text { get; set; }
            public string id { get; set; }
        }


        public void tidnamecek(string kaynak, string patternidname)
        {
            var v = Regex.Matches(kaynak, patternidname);
            var takipkontrol = Regex.IsMatch(kaynak,kullanci);
            foreach (Match item in v)
            {
                var tmp=item.Value.Replace("\"id\":", " ");
                var tmp2 = tmp.Replace("\"username\":", " ");
                //  var tmp3 = tmp2.Replace(',',' ');
                //  var tmp4 = Regex.Match(tmp3, @"\d+");
                var tmp3 = tmp2.Split(',');

                if (takipcifirst)
               {
                    takipciler.Add(tmp3[0], tmp3[1]);
               }



                //  else
                //  {
                //       takipciler2.Add(tmp3[0], tmp3[1]);

                //  } 
 }
            MessageBox.Show(takipkontrol.ToString());

        }


        public void idcek(string kaynak, string pattern)
        {
            Match v = Regex.Match(kaynak, pattern);

            id = v.Value.Trim('"');


        }

        public void cokluidcek(string kaynak, string pcount, string pcursor)
        {
            if (first == false)
            {
                Match v = Regex.Match(kaynak, pcount);
                count = int.Parse(Regex.Match(v.Value, @"\d+").Value);

            }
            Match v2 = Regex.Match(kaynak, pcursor);

            string temp = Regex.Match(v2.Value, ":.*?}").Value;
          
            cursor= temizle(temp);
            MessageBox.Show(cursor.ToString());
            first = true;


        }
        public string temizle(string v)
        {
           var temp= v.Trim(':', '}');
           var temp2=temp.Replace("\"", String.Empty).Trim();
            return temp2;

        }
        public void takipliste(string url)
        {
            double i = 0;
            do
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url+cursor);

                request.Method = WebRequestMethods.Http.Get;
                // -------- Cookie İşlemleri------------
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Thread.Sleep(1000);
                StreamReader rs = new StreamReader(response.GetResponseStream());
                string veri = rs.ReadToEnd().ToString();
                Thread.Sleep(2000);
                //   MessageBox.Show(veri);
                //   Clipboard.SetText(veri);

                cokluidcek(veri, pattern_count, pattern_cursor);

                i++;

                tidnamecek(veri, idname);
            }

            while (i<=count/5000);

            cursor = "";
            takipcifirst = false;
           
            MessageBox.Show(takipciler2.ContainsValue("tmyshn").ToString());
            takipciler2.Clear();

            // MessageBox.Show(rs.ReadToEnd().ToString());

        }

      
    }
}
