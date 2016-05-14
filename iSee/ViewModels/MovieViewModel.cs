using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using iSee.Models;
using System.Diagnostics;

namespace iSee.ViewModels
{
    public class MovieViewModel
    {
        private ObservableCollection<Movie> allItems = new ObservableCollection<Movie>();
        public ObservableCollection<Movie> AllItems { get { return this.allItems; } }

        private Movie selectedItem = default(Movie);
        public Movie SelectedItem { get { return selectedItem; } set { this.selectedItem = value; } }

        public MovieViewModel()
        {
            this.allItems = new ObservableCollection<Movie>();
        }

        public void AddMovie(string user_name, string title, string tag, string act, string year, string url, int row)
        {
            this.allItems.Add(new Movie(user_name, title, tag, act, year, url, row));
            //this.allItems[this.allItems.Count - 1].save();
        }

        public void AddMovie(Movie movie)
        {
            this.allItems.Add(movie);
        }

        public Movie GetLastMovie()
        {
            return allItems[allItems.Count - 1];
        }

        public void RemoveAllMovie()
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                this.allItems.Remove(allItems[i]);
            }
        }

        public void RemoveMovie(string title)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (this.allItems[i].get_title() == title)
                {
                    this.allItems.Remove(allItems[i]);
                    break;
                }
            }
        }

        public Movie SearchMovie(string title)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (allItems[i].get_title() == title) return allItems[i];
            }
            return new Movie();
        }

        public void UpdateMovie(string title)
        {
            for (int i = 0; i < allItems.Count; i++)
            {
                if (this.allItems[i].get_title() == title)
                {
                    this.allItems[i].row = 2;
                    this.allItems[i].update();
                    Debug.WriteLine(this.allItems[i]);
                    break;
                }
            }
        }

        public int GetSize()
        {
            return allItems.Count;
        }
    }
}
