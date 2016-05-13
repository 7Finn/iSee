using iSee.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace iSee
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MovieDetail : Page
    {
        private Movie movie;
        private string movieTitle;

        public MovieDetail()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            movieTitle = (string)e.Parameter;
            GetDetail();
        }

        private async void GetDetail()
        {
            string title, tag, act, year, url;
            title = tag = act = year = "";
            url = "Assets/华尔街之狼.jpg";

            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("apikey", "ea6c6be7c1fc529b040b019f1149c10a");

            string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            string getMovie = "http://op.juhe.cn/onebox/movie/video?key=ea6c6be7c1fc529b040b019f1149c10a&q=" + movieTitle;

            HttpResponseMessage response = await httpClient.GetAsync(getMovie);
            response.EnsureSuccessStatusCode();
            Byte[] getByte = await response.Content.ReadAsByteArrayAsync();
            Encoding code = System.Text.Encoding.GetEncoding("UTF-8");
            string Result = code.GetString(getByte);
            JsonTextReader json = new JsonTextReader(new StringReader(Result));


            while (json.Read())
            {
                if (json.Value != null)
                {
                    if (json.Value.Equals("title"))
                    {
                        json.Read();
                        title = json.Value.ToString();
                    }
                    if (json.Value.Equals("tag"))
                    {
                        json.Read();
                        tag = json.Value.ToString();
                    }
                    if (json.Value.Equals("act"))
                    {
                        json.Read();
                        act = json.Value.ToString();
                    }
                    if (json.Value.Equals("year"))
                    {
                        json.Read();
                        year = json.Value.ToString();
                    }
                    if (json.Value.Equals("cover"))
                    {
                        json.Read();
                        url = json.Value.ToString();
                        break;
                    }
                }
            }
            if (title == "")
            {
                var screen = new MessageDialog("Sorry, Movie doesn't exist!\n").ShowAsync();
                return;
            }

            //新建电影类
            if (SignInContentDialog.current_user == null)
            {
                movie = new Movie("guest", title, tag, act, year, url);
            }
            else
            {
                movie = new Movie(SignInContentDialog.current_user.name, title, tag, act, year, url);
            }

            //赋值给前端
            MovieImage.Source = new BitmapImage(new Uri(url));
            MovieTitle.Text = title;
            MovieYear.Text = year;


        }
    }
}
