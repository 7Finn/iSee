using iSee.Models;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Newtonsoft.Json;
using SQLitePCL;
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
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace iSee
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class WantToSee : Page
    {
        ViewModels.MovieViewModel ViewModel = App.WantToSeeViewModel;

        public WantToSee()
        {
            this.InitializeComponent();
        }

        public bool ExistMovie(string title, string row)
        {
            var db = App.conn;
            string sql = "SELECT * FROM movie WHERE Title = ? AND User_name = ? AND Row = ?";
            using (var statement = db.Prepare(sql))
            {
                statement.Bind(1, title);
                statement.Bind(3, row);
                if (SignInContentDialog.current_user != null)
                {
                    statement.Bind(2, SignInContentDialog.current_user.name);
                }
                else
                {
                    statement.Bind(2, "guest");
                }
                if (SQLiteResult.ROW == statement.Step())
                {
                    return true;
                }
            }
            return false;
        }

        private async void AddBarButton_Click(object sender, RoutedEventArgs e)
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

            string getMovie = "http://op.juhe.cn/onebox/movie/video?key=ea6c6be7c1fc529b040b019f1149c10a&q=" + Search.Text;
            if (ExistMovie(Search.Text, "1"))
            {
                //var screen = new MessageDialog("Movie exist!\n").ShowAsync();
                return;
            }

            HttpResponseMessage response = await httpClient.GetAsync(getMovie);
            response.EnsureSuccessStatusCode();
            Byte[] getByte = await response.Content.ReadAsByteArrayAsync();
            Encoding code = System.Text.Encoding.GetEncoding("UTF-8");
            string Result = code.GetString(getByte);
            JsonTextReader json = new JsonTextReader(new StringReader(Result));
            Debug.WriteLine(json.Value);
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
            Movie movie;
            if (SignInContentDialog.current_user == null)
            {
                movie = new Movie("guest", title, tag, act, year, url);
            }
            else
            {
                movie = new Movie(SignInContentDialog.current_user.name, title, tag, act, year, url);
            }

            movie.save();
            ViewModel.AddMovie(movie);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Movie)(e.ClickedItem);

        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton apt = sender as AppBarButton;
            string title = apt.Tag.ToString();
            if (!ExistMovie(apt.Tag.ToString(), "2"))
            {
                App.AlreadySeenViewModel.AddMovie(ViewModel.SearchMovie(apt.Tag.ToString()));
                ViewModel.UpdateMovie(apt.Tag.ToString());
            }
            else
            {
                for (int i = 0; i < ViewModel.AllItems.Count; i++)
                {
                    if (ViewModel.AllItems[i].get_title() == title)
                    {
                        ViewModel.AllItems[i].remove();
                    }
                }
            }
            ViewModel.RemoveMovie(title);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton apt = sender as AppBarButton;
            string title = apt.Tag.ToString();
            for (int i = 0; i < ViewModel.AllItems.Count; i++)
            {
                if (ViewModel.AllItems[i].get_title() == title)
                {
                    ViewModel.AllItems[i].remove();
                    ViewModel.RemoveMovie(title);
                    return;
                }
            }
        }

    }

    public class CoverWidthConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double height = (double)value;
            Debug.WriteLine(height);
            return height * 5 / 7;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }

}
