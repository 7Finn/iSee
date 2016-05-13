using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Newtonsoft.Json;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;
using Windows.UI.Popups;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using SQLitePCL;

namespace iSee.Models
{
    public class Movie
    {
        public string title;
        public string tag;
        public string act;
        public string year;
        public string url;
        public int row;
        public static SQLiteConnection conn;
        public Movie()
        {
            this.title = this.tag = this.act = this.year = this.url = null;
        }
        public string get_title()
        {
            return this.title;
        }
        public Movie(string title, string tag, string act, string year, string url)
        {
            this.title = title;
            this.tag = tag;
            this.act = act;
            this.year = year;
            this.url = url;
            this.row = 1;
            conn = new SQLiteConnection("iSee.db");
            string sql = @"CREATE TABLE IF NOT EXISTS
                            movie (Title VARCHAR( 140 ),
                            Tag VARCHAR( 140 ),
                            Act VARCHAR( 140 ),
                            Year VARCHAR(140),
                            URL VARCHAR(140),
                            Row int);";
            using (var statement = conn.Prepare(sql))
            {
                statement.Step();
            }
        }
        public void save()
        {
            string sql = @"INSERT INTO movie VALUES (?, ?, ?, ?, ?, ?)";
            using (var statement = conn.Prepare(sql))
            {
                statement.Bind(1, this.title);
                statement.Bind(2, this.tag);
                statement.Bind(3, this.act);
                statement.Bind(4, this.year);
                statement.Bind(5, this.url);
                statement.Bind(6, this.row);
                statement.Step();
            }
        }

        public void remove()
        {
            Debug.WriteLine("Delete");
            string sql = @"DELETE FROM movie WHERE title = ?";
            using (var statement = conn.Prepare(sql))
            {
                statement.Bind(1, this.title);
                statement.Step();
            }
        }

        public void update()
        {
            string sql = @"UPDATE movie SET Row = ? WHERE Title = ?";
            using (var statement = conn.Prepare(sql))
            {
                statement.Bind(1, 2);
                statement.Bind(2, this.title);
                statement.Step();
            }
        }

        /*public async Task query(string movie)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var headers = httpClient.DefaultRequestHeaders;
                headers.Add("apikey", "ea6c6be7c1fc529b040b019f1149c10a");
                string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                if (!headers.UserAgent.TryParseAdd(header))
                {
                    throw new Exception("Invalid header value: " + header);
                }
                Debug.WriteLine("1");
                string getMovie = "http://op.juhe.cn/onebox/movie/video?key=ea6c6be7c1fc529b040b019f1149c10a&q=" + movie;
                Debug.WriteLine("2");
                HttpResponseMessage response = await httpClient.GetAsync(getMovie);
                Debug.WriteLine("3");
                response.EnsureSuccessStatusCode();
                Byte[] getByte = await response.Content.ReadAsByteArrayAsync();
                Debug.WriteLine("4");
                Encoding code = Encoding.GetEncoding("UTF-8");
                string Result = code.GetString(getByte);
                JsonTextReader json = new JsonTextReader(new StringReader(Result));
                Debug.WriteLine(json.Value);
                Debug.WriteLine("5");
                while (json.Read())
                {
                    if (json.Value != null)
                    {
                        if (json.Value.Equals("title"))
                        {
                            json.Read();
                            this.title = json.Value.ToString();
                        }
                        if (json.Value.Equals("tag"))
                        {
                            json.Read();
                            this.tag = json.Value.ToString();
                        }
                        if (json.Value.Equals("act"))
                        {
                            json.Read();
                            this.act = json.Value.ToString();
                        }
                        if (json.Value.Equals("year"))
                        {
                            json.Read();
                            this.year = json.Value.ToString();
                        }
                        if (json.Value.Equals("cover"))
                        {
                            json.Read();
                            this.url = json.Value.ToString();
                            break;
                        }
                    }
                }
            }
            catch (HttpRequestException ex1)
            {
                Debug.Write(ex1.ToString());
            }
            catch (Exception ex2)
            {
                Debug.Write(ex2.ToString());
            }
        }*/
    }
}
