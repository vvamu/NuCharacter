using SQLite;
using System;

namespace NuCharacter.Models
{
    public class User
    {
        [PrimaryKey,AutoIncrement]
        public int ID_User { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }
        private string _imagePath;
        public string ImagePath { get => _imagePath ?? System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\pic.jfif"); set => _imagePath = value; }

    }
}
