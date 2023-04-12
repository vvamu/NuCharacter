using NuCharacter.ViewModels.Base;
using NuCharacter.Infrastrucure.Commands;
using System.Windows.Input;
using NuCharacter.Models;
using System.IO;
using System;
using NuCharacter.DataBase;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;
using Newtonsoft.Json;
using System.Windows;

namespace NuCharacter.ViewModels
{
    internal partial class MainWindowViewModel : ViewModel
    {
        private static User acc;
        public static User Acc { get => acc; set => acc = value; }
        private int acc_ID() => Acc.ID_User;

        private static MainWindowViewModel _instance;
        public static MainWindowViewModel Instance
        {
            get
            {
                if (_instance == null) _instance = new MainWindowViewModel();
                return _instance;
            }
        }



        public ICommand Click_GlobalGroup { get; }
        private bool CanClick_GlobalGroup(object obj) => true;
        private void OnClick_GlobalGroup(object obj)
        {
            SelectedGroup = null;
            All_Characters = new ObservableCollection<Character>(Local_DB.SelectAll<Character>().Where(x => x.PrivateStatus == false));
            OnPropertyChanged(nameof(All_Characters));
            FilterText = "";
        }


        #region Navigation


        private string frameSource = null;//"P_Views/MainPageView.xaml";
        public string FrameSource
        {
            get => frameSource;
            set => SetProperty(ref frameSource, value);
        }

        public ICommand GoToSecondPage { get; }
        private bool CanSecondPage(object obj) => true;
        private void OnSecondPage(object obj)
        {
            FrameSource = "P_Views/SecondPageView.xaml";
        }


        public ICommand GoToMainPage { get; }
        private bool CanMainPage(object obj) => true;
        private void OnMainPage(object obj = null)
        {
            FrameSource = "P_Views/MainPageView.xaml";
        }

        public ICommand OpenEditUser { get; }

        private bool CanEditUser(object obj) => true;
        private void OnEditUser(object obj) => FrameSource = "EditUserWindow.xaml";

        #endregion

        #region Save Charasteristics

        public ICommand SaveCharacteristics { get; }
        private bool CanSaveCharacteristics(object obj) => true;
        private void OnSaveCharacteristics(object obj)
        {
            Local_DB.Update(SelectedCharacter);
            SelectedCharacter = null;
        }

        #endregion

        #region Serialize and Deserialize
        public ICommand Serializing { get; }
        private bool CanSerializing(object obj) => true;//obj is Character ch && All_Characters.Contains(ch);
        private void OnSerializing(object obj)
        {
            var ch = obj as Character;

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() != true) return;

            var json = JsonConvert.SerializeObject(ch);
            File.WriteAllText(saveFileDialog.FileName + ".json", json);


        }


        public ICommand Deserialize { get; }
        private bool CanDeserialize(object obj) => obj is Group gr && gr != null ;
        
        private void OnDeserialise(object obj)
        {
            var gr = obj as Group;
            var gr_Id = gr.Id_Group;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() != true) return;

            if (!openFileDialog.FileName.Contains(".json"))
            { 
                HandyControl.Controls.MessageBox.Show("Sorry, but format is not compleate","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var json = File.ReadAllText(openFileDialog.FileName);
            
            var jsonCharacter = JsonConvert.DeserializeObject<Character>(json);

            jsonCharacter.Name = openFileDialog.SafeFileName.Replace(".json","");
            jsonCharacter.Id_Group = gr_Id;
            Local_DB.Insert(jsonCharacter);
            Refresh(gr);

        }
        #endregion

        #region Search

        private string filterText;
        public string FilterText
        {
            get => filterText;
            set
            {
                string text;
                if (!SetProperty(ref filterText, value)) return;

                if (xxxx == false)
                {
                    xxxx = true;
                    text = FilterText;
                    if (String.IsNullOrEmpty(text))
                        Refresh(_SelectedGroup);
                    else
                    {
                        OnSearch();
                    }
                    xxxx = false;

                }
            }
        }

        private void OnSearch(object obj = null)
        {

            var text = FilterText.ToLower();
            var buff = All_Characters;
            buff.Clear();

            #region Get All
            var bu = new ObservableCollection<Character>();
            if (SelectedGroup == null) 
            {
                var groups = Local_DB.SelectAll<Group>().Where(x => x.Id_User == Acc.ID_User);
                foreach (var group in groups)
                {
                    foreach(var ch in Local_DB.SelectAll<Character>().Where(x => x.Id_Group == group.Id_Group))
                    {
                        bu.Add(ch);
                    }
                }

            }
            else bu = new ObservableCollection<Character>(Local_DB.SelectAll<Character>().Where(x => x.Id_Group == Gr_ID));

            var bb = new ObservableCollection<Character>(bu);
            #endregion


            foreach (var item in bb)
            {
                var search_name = item.Name;


                if (item.Name.ToLower().Contains(FilterText))
                {
                    buff.Add(item);
                    All_Characters = buff;

                    OnPropertyChanged(nameof(All_Characters));

                }
                else
                {
                    buff.Remove(item);
                    All_Characters = buff;
                    OnPropertyChanged(nameof(All_Characters));


                }

            }
        }

        private string filterTextGroup;
        public string FilterTextGroup
        {
            get => filterTextGroup;
            set
            {
                string text;

                if (!SetProperty(ref filterTextGroup, value)) return;

                if (xxxx == false)
                {
                    xxxx = true;
                    text = filterTextGroup;
                    if (String.IsNullOrEmpty(text))
                        Refresh(_SelectedGroup);
                    else
                    {
                        OnSearchGroup();
                    }
                    xxxx = false;

                }
            }
        }

        private void OnSearchGroup(object obj = null)
        {

            var text = filterTextGroup.ToLower();
            var buff = All_Groups;

            buff.Clear();

            #region Get All
            IEnumerable<Group> bu;
            bu = All_User_Groups();

            var bb = new ObservableCollection<Group>(bu);
            #endregion


            foreach (var item in bb)
            {
                var search_name = item.Name;


                if (item.Name.ToLower().Contains(filterTextGroup))
                {
                    buff.Add(item);
                    All_Groups = buff;

                    OnPropertyChanged(nameof(All_Groups));

                }
                else
                {
                    buff.Remove(item);
                    All_Groups = buff;
                    OnPropertyChanged(nameof(All_Groups));
                }

            }

        }



        #endregion

        #region Choose Images
        public ICommand ChooseImage { get; }
        private bool CanChooseImage(object obj) => true;
        private void OnChooseImage(object obj)
        {
            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                if (dialog.ShowDialog() != true) return;

                var stream = dialog.OpenFile();
                var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuCharImg");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                var file = Path.Combine(folder, dialog.SafeFileName);
                if (!File.Exists(file))
                {
                    stream.CopyTo(File.Create(file));
                }

                SelectedCharacter.ImagePath = file;
                Local_DB.Update(SelectedCharacter);
                OnPropertyChanged("SelectedCharacter");
                Refresh(SelectedGroup);
            }
            catch (Exception) { }
        }

        #endregion

        #region CutUp Copy Paste

        private static bool flagPaste = false;
        public ICommand CutUp { get; }
        private bool CanCutUp(object obj) => SelectedGroup != null && SelectedCharacter != null;
        private void OnCutUp(object obj)
        {

            var ch = SelectedCharacter;
            var json = JsonConvert.SerializeObject(ch);

            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuCharImg");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, "buff.json");
            
            File.WriteAllText(file, json);
            All_Characters.Remove(ch);
            Local_DB.Remove(ch);

            var gr = Local_DB.Select<Group>(ch.Id_Group);
            OnPropertyChanged(nameof(All_Characters));

            flagPaste = true;
        }
        public ICommand Copy { get; }
        private bool CanCopy(object obj) => SelectedCharacter != null;
        private void OnCopy(object obj)
        {
            var ch = SelectedCharacter;
            var json = JsonConvert.SerializeObject(ch);

            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuCharImg");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, "buff.json");
            

            File.WriteAllText(file, json);

            var gr = Local_DB.Select<Group>(ch.Id_Group);

            Refresh(gr);
            flagPaste = true;
        }
        public ICommand Paste { get; }
        private bool CanPaste(object obj) => obj is Group gr && gr != null && flagPaste != false;
        private void OnPaste(object obj)
        {

            var group = obj as Group;
            var id_gr = group.Id_Group;


            var gr = obj as Group;
            var gr_Id = gr.Id_Group;

            var folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuCharImg");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            var file = Path.Combine(folder, "buff.json");

            if (!File.Exists(file))
            {
                HandyControl.Controls.MessageBox.Show("\tYou dont copy or paste character", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var json = File.ReadAllText(file);
            var jsonCharacter = JsonConvert.DeserializeObject<Character>(json);


            jsonCharacter.Id_Group = gr_Id;
            Local_DB.Insert(jsonCharacter);
            Refresh(gr);
            flagPaste = false;

        }
        #endregion


        public MainWindowViewModel()
        {
            #region Commands

            CreateCharacter = new LambdaCommand(OnCreateCharacter, CanCreateCharacter);
            RemoveCharacter = new LambdaCommand(OnRemoveCharacter, CanRemoveCharacter);

            Click_AllGroups = new LambdaCommand(OnClick_AllGroups, CanClick_AllGroups);
            Click_GlobalGroup = new LambdaCommand(OnClick_GlobalGroup, CanClick_GlobalGroup);

            CreateGroup = new LambdaCommand(OnCreateGroup, CanCreateGroup);
            RemoveGroup = new LambdaCommand(OnRemoveGroup, CanRemoveGroup);

            SaveCharacteristics = new LambdaCommand(OnSaveCharacteristics, CanSaveCharacteristics);

            Deserialize = new LambdaCommand(OnDeserialise,CanDeserialize);
            Serializing = new LambdaCommand(OnSerializing, CanSerializing);

            Copy = new LambdaCommand(OnCopy, CanCopy);
            Paste = new LambdaCommand(OnPaste, CanPaste);
            CutUp = new LambdaCommand(OnCutUp, CanCutUp);


            GoToSecondPage = new LambdaCommand(OnSecondPage, CanSecondPage);
            GoToMainPage = new LambdaCommand(OnMainPage, CanMainPage);

            OpenEditUser = new LambdaCommand(OnEditUser, CanEditUser);
            ChooseImage = new LambdaCommand(OnChooseImage, CanChooseImage);

            ShareStatus = new LambdaCommand(OnShareStatus, CanSharaStatus);

            //AutoCompleteRaces = new LambdaCommand(OnShareStatus, CanSharaStatus);

            #endregion


            #region AutoCompleteCharacterProperties

           

            #endregion

            OnAppearing();
        }
        private void OnAppearing()
        {
            All_Groups = new ObservableCollection<Group>(All_User_Groups());
            OnPropertyChanged("All_Groups");

            var allCharacter = new ObservableCollection<Character>();
            foreach (var groupCharacter in All_Groups)
            {
                foreach (var character in Local_DB.SelectAll<Character>().Where(x => x.Id_Group == groupCharacter.Id_Group))
                {
                    allCharacter.Add(character);
                }
            }
            All_Characters.Clear();
            All_Characters = allCharacter;
            OnPropertyChanged("All_Characters");




        }
    }


}
