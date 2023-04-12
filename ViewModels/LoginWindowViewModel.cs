using System.Linq;
using System.Windows.Input;
using HandyControl.Controls;
using NuCharacter.DataBase;
using NuCharacter.Helpers;
using NuCharacter.Infrastrucure.Commands;
using NuCharacter.Models;
using NuCharacter.ViewModels.Base;
using NuCharacter.Views;
using System.Windows;

namespace NuCharacter.ViewModels
{
    internal partial class LoginWindowViewModel : ViewModel
    {
        #region Properties
        private string login;
        public string Login { get => login; set => SetProperty(ref login, value); }


        #endregion
        public User FindUser => Local_DB.Users.
            FirstOrDefault(x => x.Login == Login);


        #region GoToMainWindow
        public ICommand GoToMainWindow { get; }
        private bool CanMainWindow(object obj)
        {
            if (!(obj is PasswordBox passwordBox)) return false;
            var password1 = passwordBox?.Password;

            if (string.IsNullOrEmpty(password1)|| string.IsNullOrEmpty(Login)) return false;
            else return true;
        }

        private User IsValidUser(object obj)
        {
            if (FindUser == null) return null;
            var user = FindUser;

            var psbox = obj as PasswordBox;
            if (user?.Password != Hash.Encrypt(psbox?.Password)) return null;
            else return user;
        }
        private void OnMainWindow(object obj)
        {
            if (IsValidUser(obj) == null)
            {
                HandyControl.Controls.MessageBox.Show(
                    "\tIncorrect login or password", 
                    "Error", 
                    MessageBoxButton.OK, 
                    MessageBoxImage.Information);
                return;
            }

            MainWindowViewModel.Acc = IsValidUser(obj);
            new MainWindow().Show();
            var window = App.Current.Windows[0];
            window?.Close();
        }

        #endregion

        #region GoToRegistrationWindow
        public ICommand GoToRegistrationWindow { get; }
        private bool CanRegistrationWindow(object obj) => true;

        private void OnRegistrationWindow(object obj)
        {
            new RegistrationWindow().Show();
            var window = App.Current.Windows[0];
            window?.Close();
        }
        #endregion

        public LoginWindowViewModel()
        {
            GoToMainWindow = new LambdaCommand(OnMainWindow, CanMainWindow);
            GoToRegistrationWindow = new LambdaCommand(OnRegistrationWindow, CanRegistrationWindow);

        }
    }
}
