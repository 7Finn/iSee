using iSee.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        ViewModels.MovieViewModel WantToSeeViewModel = App.WantToSeeViewModel;
        ViewModels.MovieViewModel AlreadySeenViewModel = App.AlreadySeenViewModel;

        private Movie movie;
        public string title, tag, act, year, url;

        public MovieDetail()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            title = (string)e.Parameter;
            GetDetail();
        }

        private async void GetDetail()
        {
            HttpClient httpClient = new HttpClient();
            var headers = httpClient.DefaultRequestHeaders;
            headers.Add("apikey", "ea6c6be7c1fc529b040b019f1149c10a");

            string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            //string getMovie = "http://op.juhe.cn/onebox/movie/video?key=ea6c6be7c1fc529b040b019f1149c10a&q=" + movieTitle;
            string getMovie = "https://api.douban.com/v2/movie/search?q=" + title;

            HttpResponseMessage response = await httpClient.GetAsync(getMovie);
            response.EnsureSuccessStatusCode();
            Byte[] getByte = await response.Content.ReadAsByteArrayAsync();
            Encoding code = System.Text.Encoding.GetEncoding("UTF-8");
            string Result = code.GetString(getByte);
            JsonTextReader json = new JsonTextReader(new StringReader(Result));

            
            //只检索第一个结果(Genres只能出现一次)
            bool theFirst = true;
            while (json.Read())
            {
                if (json.Value != null)
                {
                    if (json.Value.Equals("genres"))
                    {
                        if (theFirst == false) break;
                        theFirst = false;
                        json.Read();
                        json.Read();
                        while (json.Value != null)
                        {
                            tag += json.Value.ToString() + " ";
                            json.Read();
                        }
                        Debug.WriteLine(json.Value);
                        json.Read();
                        //tag = json.Value.ToString();
                    }
                    if (json.Value.Equals("directors"))
                    {
                        while(json.Read())
                        {
                            if (json.Value != null)
                            {
                                if (json.Value.Equals("name"))
                                {
                                    json.Read();
                                    act += json.Value.ToString() + " ";
                                }
                                if (json.Value.Equals("year")) break;
                            }
                        }
                    }
                    if (json.Value.Equals("year"))
                    {
                        json.Read();
                        year = json.Value.ToString();
                    }
                    if (json.Value.Equals("images"))
                    {
                        while (json.Read())
                        {
                            if (json.Value != null && json.Value.Equals("large"))
                            {
                                json.Read();
                                url = json.Value.ToString();
                                break;
                            }
                        }
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
            MovieDir.Text = act;
            MovieYear.Text = year;
            MovieTag.Text = tag;

        }

        private void AddWantToSeeAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //加载进WantToSee
            movie.save();
            WantToSeeViewModel.AddMovie(movie);
            //Debug.WriteLine(((AppBarButton)sender).Tag.ToString());
        }

        private void AlreadySeenAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            //加载进AlreadySeen
            movie.save();
            AlreadySeenViewModel.AddMovie(movie);
            AlreadySeenViewModel.UpdateMovie(movie.get_title());
            //Debug.WriteLine(((AppBarButton)sender).Tag.ToString());
        }
    }
}
