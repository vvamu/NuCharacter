using System;
using NuCharacter.Models;
using NuCharacter.ViewModels.Base;
using System.Windows.Input;
using NuCharacter.Infrastrucure.Commands;
using NuCharacter.DataBase;
using Microsoft.Win32;
using System.IO;
using HandyControl.Controls;
using NuCharacter.Helpers;
using System.Windows;

namespace NuCharacter.ViewModels
{
    internal class EditUserViewModel : ViewModel
    {
        private User acc = MainWindowViewModel.Acc;
        public User ThisAcc { get => acc; set => SetProperty(ref acc, value); }

        public string ImageSource;
        private bool ImageChanged = false;

        public ICommand ChoosingImage { get; }
        private bool CanChoosingImage(object obj) => true;
        private void OnChoosingImage(object obj)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() != true) return;

            try
            {
                var stream = dialog.OpenFile();
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuCharImg");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var file = Path.Combine(folder, dialog.SafeFileName);
                if (!File.Exists(file))
                {
                    stream.CopyTo(File.Create(file));
                }
                ImageSource = file;
                
                ThisAcc.ImagePath = ImageSource;
                OnPropertyChanged("ThisAcc");
                ImageChanged = true;
            }
            catch
            {
                HandyControl.Controls.MessageBox.Show("\tSize of Image so much big", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
          
        }


        public ICommand CancelCloseWin { get; }
        private bool CanCancelCloseWin(object obj) => true;
        private void OnCancelCloseWin(object obj) => MainWindowViewModel.Instance.FrameSource = "P_Views/MainPageView.xaml";


        public ICommand OkCloseWin { get; }
        private bool CanOkCloseWin(object obj) => true;
        private void OnOkCloseWin(object obj)
        {
            try
            {
                var passwordBox = obj as object[];
                var password1 = (passwordBox[0] as PasswordBox)?.Password;
                var password2 = (passwordBox[1] as PasswordBox)?.Password;
                if (!string.IsNullOrEmpty(password2) && !string.IsNullOrEmpty(password1))
                {
                    if (password1 == password2)
                    {
                        HandyControl.Controls.MessageBox.Show("Password can't be equals", "Attention", MessageBoxButton.OK);
                        return;
                    }
                    if (Hash.Encrypt(password1) != ThisAcc?.Password)
                    {
                        HandyControl.Controls.MessageBox.Show("Old password not equal current password", "Attention", MessageBoxButton.OK);
                        return;
                    }
                    ThisAcc.Password = Hash.Encrypt(password2);
                }
                if (ImageChanged)
                {
                    HandyControl.Controls.MessageBox.Show("To change the profile picture, you need to restart the application", "Attention", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Local_DB.Update(ThisAcc);
                OnPropertyChanged("ThisAcc");
                MainWindowViewModel.Instance.Refresh();
                MainWindowViewModel.Instance.FrameSource = "P_Views/MainPageView.xaml";
            }
            catch (Exception) { }
        }


        public EditUserViewModel()
        {
            OkCloseWin = new LambdaCommand(OnOkCloseWin, CanOkCloseWin);
            CancelCloseWin = new LambdaCommand(OnCancelCloseWin, CanCancelCloseWin);
            ChoosingImage = new LambdaCommand(OnChoosingImage, CanChoosingImage);
        }

    }
}
