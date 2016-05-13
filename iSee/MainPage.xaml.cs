using SQLitePCL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;


//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace iSee
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public static MainPage Current;

        public MainPage()
        {
            this.InitializeComponent();
            Current = this;
            InitialzeTitleBar();
        }
        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // Populate the scenario list from the SampleConfiguration.cs file
            ScenarioControl.ItemsSource = scenarios;
            if (Window.Current.Bounds.Width < 640)
            {
                ScenarioControl.SelectedIndex = -1;
            }
            else
            {
                ScenarioControl.SelectedIndex = 0;
            }
        }

        private void ScenarioControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Clear the status block when navigating scenarios.
            NotifyUser(String.Empty, NotifyType.StatusMessage);

            ListView scenarioListView = sender as ListView;
            Scenario s = scenarioListView.SelectedItem as Scenario;
            if (s != null)
            {
                ScenarioFrame.Navigate(s.ClassType);
                if (Window.Current.Bounds.Width < 640)
                {
                    Splitter.IsPaneOpen = false;
                }
            }
        }

        public void NotifyUser(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    break;
                case NotifyType.ErrorMessage:
                    StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
            }
            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }
        }

        public List<Scenario> Scenarios
        {
            get { return this.scenarios; }
        }


        public void InitialzeTitleBar()
        {
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Color.FromArgb(255, 188, 47, 46);
            viewTitleBar.ButtonBackgroundColor = Color.FromArgb(255, 188, 47, 46);
            viewTitleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 232, 17, 35);
            viewTitleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 241, 112, 122);
            viewTitleBar.ButtonInactiveBackgroundColor = Color.FromArgb(255, 181, 113, 113);
            viewTitleBar.InactiveBackgroundColor = Color.FromArgb(255, 181, 113, 113);
            viewTitleBar.InactiveForegroundColor = Colors.White;
            viewTitleBar.ButtonInactiveForegroundColor = Colors.White;
            viewTitleBar.ButtonHoverForegroundColor = Colors.White;
        }

        public bool is_signin = false;

        private async void ShowSignInDialogButton_Click(object sender, RoutedEventArgs e)
        {
            if (is_signin == false)
            {
                SignInContentDialog signInDialog = new SignInContentDialog();
                await signInDialog.ShowAsync(); //获取返回值

                if (signInDialog.Result == SignInResult.SignInOK)
                {
                    // Sign in was successful.
                    is_signin = true;
                    current_user_name.Text = SignInContentDialog.current_user.name;

                    signin_button.Content = "注销";

                    // 更新用户的数据
                    ReloadAllMovies(is_signin);

                }
                else if (signInDialog.Result == SignInResult.SignInFail)
                {
                    // Sign in failed.
                }
                else if (signInDialog.Result == SignInResult.SignInCancel)
                {
                    // Sign in was cancelled by the user.
                }
            }
            else
            {
                is_signin = false;
                ReloadAllMovies(is_signin);
                //head_image.ImageSource = "Assets/UserImage.jpg";
                current_user_name.Text = "未登录";
                SignInContentDialog.current_user = null;
                signin_button.Content = "登录";
            }
        }

        private void Head_Picture_Click(object sender, RoutedEventArgs e)
        {
            this.SelectPictureButton_Click(sender, e);
        }

        public void ReloadAllMovies(bool is_signin)
        {
            App.AlreadySeenViewModel.RemoveAllMovie();
            App.WantToSeeViewModel.RemoveAllMovie();
            string movie_sql;
            movie_sql = "SELECT * FROM movie WHERE Row = ? AND User_name = ?";
            using (var statement = App.conn.Prepare(movie_sql))
            {
                statement.Bind(1, 1);
                if (is_signin)
                    statement.Bind(2, SignInContentDialog.current_user.name);
                else
                    statement.Bind(2, "guest");
                while (SQLiteResult.ROW == statement.Step())
                {
                    App.WantToSeeViewModel.AddMovie((string)statement[0], (string)statement[1], (string)statement[2], (string)statement[3], (string)statement[4], (string)statement[5]);
                }
            }
            using (var statement = App.conn.Prepare(movie_sql))
            {
                statement.Bind(1, 2);
                if (is_signin)
                    statement.Bind(2, SignInContentDialog.current_user.name);
                else
                    statement.Bind(2, "guest");
                while (SQLiteResult.ROW == statement.Step())
                {
                    App.AlreadySeenViewModel.AddMovie((string)statement[0], (string)statement[1], (string)statement[2], (string)statement[3], (string)statement[4], (string)statement[5]);
                }
            }

        }

        private async void ShowRegisterDialogButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterContentDialog registerDialog = new RegisterContentDialog();
            await registerDialog.ShowAsync(); //获取返回值
        }

        private async void SelectPictureButton_Click(object sender, RoutedEventArgs e)
        {
            if (is_signin == true)
            {
                FileOpenPicker picker = new FileOpenPicker();

                //设置打开时的默认路径，这里选择的是图片库
                picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                //添加可选择的文件类型
                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                //选择打开多个文件
                //IReadOnlyList<StorageFile> files = await picker.PickMultipleFilesAsync();
                //只选择一个文件
                StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    using (IRandomAccessStream fileStream =
                           await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                    {
                        BitmapImage bitmapImage = new BitmapImage();
                        await bitmapImage.SetSourceAsync(fileStream);
                        head_image.ImageSource = bitmapImage;
                    }
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string title = Search.Text;
            ScenarioFrame.Navigate(typeof(iSee.MovieDetail), title);
        }
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };

    public class ScenarioBindingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return s.Title;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }

    public class ScenarioBindingConverterSymbol : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Scenario s = value as Scenario;
            return s.Symbol;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return true;
        }
    }
}
