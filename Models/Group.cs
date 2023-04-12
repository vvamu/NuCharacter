using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuCharacter.Models
{
    public class Group
    {
        [PrimaryKey, AutoIncrement]
        public int Id_Group { get; set; }
        public string Name { get; set; }
        public DateTime DateCreated { get; set; }
        public int CountCharacters { get; set; }
        public int Id_User { get; set; }


        public Group() 
        {
            DateCreated = DateTime.Now;
        }
    }
}
