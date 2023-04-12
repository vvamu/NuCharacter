using System;
using System.Windows;
using System.Windows.Input;
using HandyControl.Controls;
using NuCharacter.DataBase;
using NuCharacter.Helpers;
using NuCharacter.Infrastrucure.Commands;
using NuCharacter.Models;
using NuCharacter.ViewModels.Base;
using NuCharacter.Views;

namespace NuCharacter.ViewModels
{
    internal partial class RegistrationViewModel : ViewModel
    {

        #region Properties
        private string login;
        public string Login
        {
            get => login;
            set => SetProperty(ref login, value);
        }
        private string userName;
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        #endregion

        #region Navigation
        public ICommand GoToMainWindow { get; }
        private bool CanMainWindow(object obj)
        {

            if (obj == null) return false;
            var passwordBox = obj as object[];
            var password1 = (passwordBox[0] as PasswordBox)?.Password;
            var password2 = (passwordBox[1] as PasswordBox)?.Password;
            


            if (string.IsNullOrEmpty(password1) || string.IsNullOrEmpty(password2)
                || string.IsNullOrEmpty(Login) || string.IsNullOrEmpty(UserName)) return false;

            else return true;
        }

        public bool IsValidAccount(User user)
        {
            if (user == null || user.Login.Length < 3 || user.Password.Length < 3) 
            { 
                return false; 
            }
            else return true;
        }

        private void OnMainWindow(object obj)
        {
            var passwordBox = obj as object[];
            var password1 = (passwordBox[0] as PasswordBox)?.Password;
            var password2 = (passwordBox[1] as PasswordBox)?.Password;
            

            var a = AppDomain.CurrentDomain.BaseDirectory;
            if(password1 != password2)
            {
                HandyControl.Controls.MessageBox.Show("\tPasswords not equals", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var password = Hash.Encrypt(password1);

            User user = new User()
            {
                Login = Login,
                UserName = UserName,
                Password = password,
                ImagePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\pic.jfif")
            };

            if (!IsValidAccount(user) || password1.Length < 3 || password2.Length < 3) 
            {
               HandyControl.Controls.MessageBox.Show("\tIncorrect login, username or password\n\t Minimal length is 3 symbols", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Local_DB.Insert(user);
            MainWindowViewModel.Acc = user;
            new MainWindow().Show();
            var window = App.Current.Windows[0];
            window?.Close();
        }


        public ICommand GoToLoginWindow { get; }
        private bool CanLoginWindow(object obj) => true;
        private void OnLoginWindow(object obj)
        {
            new LoginWindow().Show();
            var window = App.Current.Windows[0];
            window?.Close();
        }
        #endregion


        internal RegistrationViewModel()
        {
            GoToMainWindow = new LambdaCommand(OnMainWindow, CanMainWindow);
            GoToLoginWindow = new LambdaCommand(OnLoginWindow,CanLoginWindow);

        }
    }
}
