using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace iSee
{
    public partial class MainPage : Page
    {
        public const string FEATURE_NAME = "Calendar C# sample";

        List<Scenario> scenarios = new List<Scenario>
        {
            new Scenario() { Symbol = "Video", Title = "正在热播", ClassType = typeof(global::iSee.TheRecentHit) },
            new Scenario() { Symbol = "Favorite", Title = "想看的", ClassType = typeof(global::iSee.WantToSee) },
            new Scenario() { Symbol = "List", Title = "已看电影", ClassType = typeof(global::iSee.AlreadySeen) },
            new Scenario() { Symbol = "Like", Title = "私人影院", ClassType = typeof(global::iSee.MovieRecommend) },
            //new Scenario() { Title = "Calendar with Unicode extensions in languages", ClassType = typeof(SDKTemplate.Scenario4_UnicodeExtensions) },
            //new Scenario() { Title = "Calendar time zone support", ClassType = typeof(SDKTemplate.Scenario5_TimeZone) }
        };
    }

    public class Scenario
    {
        public string Symbol { get; set; }
        public string Title { get; set; }
        public Type ClassType { get; set; }
    }
}
