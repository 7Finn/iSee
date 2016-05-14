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
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Notifications;
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
    public sealed partial class TheRecentHit : Page
    {
        ViewModels.MovieViewModel ViewModel = App.RecentHitViewModel;


        public TheRecentHit()
        {
            this.InitializeComponent();
            if (ViewModel.GetSize() == 0)
            {
                DisplayLoadingImage();
                GetHit();
            }
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

        private void HideLoadingImage()
        {
            LoadingImage.Visibility = Visibility.Collapsed;
        }

        private void DisplayLoadingImage()
        {
            LoadingImage.Visibility = Visibility.Visible;
        }

        private async void GetHit()
        {

            // 创建一个HTTP client实例对象
            HttpClient httpClient = new HttpClient();

            // Add a user-agent header to the GET request. 
            /*
            默认情况下，HttpClient对象不会将用户代理标头随 HTTP 请求一起发送到 Web 服务。
            某些 HTTP 服务器（包括某些 Microsoft Web 服务器）要求从客户端发送的 HTTP 请求附带用户代理标头。
            如果标头不存在，则 HTTP 服务器返回错误。
            在 Windows.Web.Http.Headers 命名空间中使用类时，需要添加用户代理标头。
            我们将该标头添加到 HttpClient.DefaultRequestHeaders 属性以避免这些错误。
            */
            var headers = httpClient.DefaultRequestHeaders;

            // The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            // especially if the header value is coming from user input.
            string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";

            //在标头添加百度Api的apikey
            //headers.Add("apikey", "a4ea7cd8a04573dfc319f19394b99953");
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            string uri = "https://api.douban.com/v2/movie/in_theaters";

            //发送GET请求
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            // 确保返回值为成功状态
            response.EnsureSuccessStatusCode();

            // 因为返回的字节流中含有中文，传输过程中，所以需要编码后才可以正常显示
            Byte[] getByte = await response.Content.ReadAsByteArrayAsync();

            // 可以用来测试返回的结果
            //string returnContent = await response.Content.ReadAsStringAsync();

            // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
            Encoding code = Encoding.GetEncoding("UTF-8");
            string result = code.GetString(getByte, 0, getByte.Length);

            JsonTextReader json = new JsonTextReader(new StringReader(result));


            //获取用户名
            string username;
            if (SignInContentDialog.current_user != null)
            {
                username = SignInContentDialog.current_user.name;
            }
            else
            {
                username = "guest";
            }

            string title = "", tag = "", act = "", year = "", url;

            while (json.Read())
            {
                if (json.Value != null)
                {
                    if (json.Value.Equals("title"))
                    {
                        json.Read();
                        title = json.Value.ToString();
                    }
                    if (json.Value.Equals("images"))
                    {
                        for (int i = 0; i < 5; ++i) json.Read();
                        url = json.Value.ToString();
                        Movie movie = new Movie(username, title, tag, act, year, url);
                        ViewModel.AddMovie(movie);
                    }
                }
            }

            HideLoadingImage();
            UpdateTile();
        }

        private async void UpdateTile()
        {

            //CreateTileUpdaterForApplication
            StorageFile xmlFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Tile.xml"));
            XmlDocument doc = await XmlDocument.LoadFromFileAsync(xmlFile);

            //Get the latest todoitem
            Movie lastMovie = ViewModel.AllItems[0];

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
}
