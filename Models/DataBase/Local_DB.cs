using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NuCharacter.Models;
using SQLite;
using Group = NuCharacter.Models.Group;

namespace NuCharacter.DataBase
{
    internal static class Local_DB
    {
        public static SQLiteConnection db;

        public static List<User> Users => SelectAll<User>().ToList();
        public static List<Character> Characters => SelectAll<Character>().ToList();
        public static List<Group> Groups => SelectAll<Group>().ToList();
        static Local_DB()
        {
            //File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DataBase\NuCharacter.db"));
        }

        public static void Init()
        {
            if (db != null) return;

            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"DataBase\NuCharacter.db");
            db = new SQLiteConnection(path);

            #region Create tables
            db.CreateTable<User>();
            db.CreateTable<Group>();
            db.CreateTable<Character>();
            #endregion
        }

        public static void Insert(object obj)
        {
            Init();
            if (obj == null) return;


            db.Insert(obj);
        }

        public static void Remove(object obj)
        {
            Init();
            if (obj == null) return;

            db.Delete(obj);

        }

        public static void Update(object obj)
        {
            Init();
            if (obj == null) return;

            db.Update(obj);

        }
        public static T Select<T>(int id) where T : new()
        {
            Init();
            return db.Get<T>(id);

        }
        public static ObservableCollection<T> SelectAll<T>() where T : new()
        {
            Init();
            return new ObservableCollection<T>(db.Table<T>());
            
        }
    }
}
