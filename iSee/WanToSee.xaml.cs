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
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Notifications;
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UpdateTile();
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

        private void GridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GridView gridView = (GridView)sender;
            Movie movie = (Movie)gridView.SelectedItem;
            if (movie == null) return;
            string movieTitle = movie.get_title();
            Debug.WriteLine(movie.get_title());

            Frame.Navigate(typeof(global::iSee.MovieDetail), movieTitle);
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

        private async void UpdateTile()
        {

            //CreateTileUpdaterForApplication
            StorageFile xmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Tile.xml"));
            XmlDocument doc = await XmlDocument.LoadFromFileAsync(xmlFile);

            //Get the latest todoitem
            Movie lastMovie = ViewModel.GetLastMovie();
            if (lastMovie == null) return;

            //Get the text Node
            XmlNodeList textNodeList = doc.GetElementsByTagName("text");
            foreach (var node in textNodeList)
            {
                if (node.InnerText == "Title") node.InnerText = lastMovie.title;
                if (node.InnerText == "Details") node.InnerText = lastMovie.year;
            }

            //Get the image Node
            XmlNodeList imageNodeList = doc.GetElementsByTagName("image");
            foreach (var node in imageNodeList)
            {
                node.Attributes[0].NodeValue = lastMovie.url;
            }

            //Update the Tile
            TileNotification notifi = new TileNotification(doc);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Update(notifi);
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
