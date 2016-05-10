using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSee
{
    public class UserViewModel
    {
        private ObservableCollection<User> all_users = new ObservableCollection<User>();
        public ObservableCollection<User> All_users { get { return this.all_users; } }

        private User selectedUser = default(User);
        public User SelectedUser { get { return selectedUser; } set { this.selectedUser = value; } }
        

        public UserViewModel()
        {
            DateTimeOffset curtime = DateTimeOffset.Now;
        }

        public void AddUser(string name, string password)
        {
            this.all_users.Add(new User(name, password));
        }

        public void UpdateUser(string id, string name, string password)
        {
            // DIY
            for (int i = 0; i < all_users.Count; i++)
            {
                if (all_users[i].getID() == id)
                {
                    all_users[i].change(name, password);
                }
            }
            // set selectedUser to null after update
            this.selectedUser = null;
        }
    }
}
