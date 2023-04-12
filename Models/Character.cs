using SQLite;
using System;

namespace NuCharacter.Models
{
    public class Character
    {
        [PrimaryKey, AutoIncrement]
        public int Id_Character { get; set; }
        public string Name { get; set; }
        public int? Age { get; set; }
        public string DateBirth { get; set; }
        public DateTime DateCreated { get; set; }

        private string _imagePath;
        public string ImagePath { get  => _imagePath ?? System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\pic.jfif"); set =>_imagePath = value; }
        public int Id_Group { get; set; }
        public bool PrivateStatus { get; set; } = true;


        public string NoteInfo { get; set; }
        public string Race { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string Hair_Color { get; set; }
        public ConsoleColor Eyes_Color { get; set; }
        public string Behavior { get; set; }
        public string Dream { get; set; }
        public string Equipment { get; set; }
        public string Weapon { get; set; }
        

        public Character()
        {
            DateCreated = DateTime.Now;
            ImagePath = "";
        }
    }
}
