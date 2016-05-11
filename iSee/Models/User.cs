using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iSee
{
    public class User
    {
        public string name { get; set; }
        public string password { get; set; }
        public string id;

        public string getID()
        {
            return this.id;
        }
        public void change(string name, string password)
        {
            this.name = name;
            this.password = password;
        }

        public User(string name, string password)
        {
            this.id = Guid.NewGuid().ToString(); //生成id
            this.name = name;
            this.password = password;
        }

        public User(string id , string name, string password)
        {
            this.id = id;
            this.name = name;
            this.password = password;
        }

        public User()
        {
            this.id = "";
            this.name = "";
            this.password = "";
        }
    }
}
