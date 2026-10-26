using System;
using System.Collections.Generic;
using System.Text;
using DataBaseControl;

namespace Users
{
    class User
    {
        DataBase db = new DataBase();

        protected string Name;
        protected string Login;
        protected string Email;
        protected bool IsLoggedIn;
        public int Id { get; protected set; }

        public bool EnterToSystem(string login, string password)
        {

            return true;
        }
    }   
}
