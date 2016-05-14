using iSee.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    public sealed partial class AlreadySeen : Page
    {
        ViewModels.MovieViewModel ViewModel = App.AlreadySeenViewModel;

        public AlreadySeen()
        {
            this.InitializeComponent();
        }

        private void DeleteAppBarButton_Click(object sender, RoutedEventArgs e)
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

        private void LikeAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            AppBarButton apt = sender as AppBarButton;
            apt.Foreground = new SolidColorBrush(Color.FromArgb(255, 204, 51, 0));
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

    }
}
