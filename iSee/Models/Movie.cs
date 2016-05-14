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
        public string user_name;
        public string title;
        public string tag;
        public string act;
        public string year;
        public string url;
        public int row;
        public Movie()
        {
            this.title = this.tag = this.act = this.year = this.url = null;
            this.user_name = "guest";
        }
        public string get_title()
        {
            return this.title;
        }
        public Movie(string user_name, string title, string tag, string act, string year, string url)
        {
            this.user_name = user_name;
            this.title = title;
            this.tag = tag;
            this.act = act;
            this.year = year;
            this.url = url;
            this.row = 1;
        }

        public Movie(string user_name, string title, string tag, string act, string year, string url, int row)
        {
            this.user_name = user_name;
            this.title = title;
            this.tag = tag;
            this.act = act;
            this.year = year;
            this.url = url;
            this.row = row;
        }

        public void save()
        {
            string sql = @"INSERT INTO movie VALUES (?, ?, ?, ?, ?, ?, ?)";
            using (var statement = App.conn.Prepare(sql))
            {
                statement.Bind(1, this.user_name);
                statement.Bind(2, this.title);
                statement.Bind(3, this.tag);
                statement.Bind(4, this.act);
                statement.Bind(5, this.year);
                statement.Bind(6, this.url);
                statement.Bind(7, this.row);
                statement.Step();
            }
        }

        public void remove()
        {
            Debug.WriteLine("Delete " + this.title + " " + this.row);
            string sql = @"DELETE FROM movie WHERE title = ? AND user_name = ? AND Row = ?";
            using (var statement = App.conn.Prepare(sql))
            {
                statement.Bind(1, this.title);
                statement.Bind(2, this.user_name);
                statement.Bind(3, this.row);
                statement.Step();
            }
        }

        public void update()
        {
            string sql = @"UPDATE movie SET Row = ? WHERE Title = ?";
            using (var statement = App.conn.Prepare(sql))
            {
                statement.Bind(1, 2);
                statement.Bind(2, this.title);
                statement.Step();
            }
        }
    }
}
