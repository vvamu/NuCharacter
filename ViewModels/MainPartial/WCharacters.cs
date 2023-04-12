using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows;
using System.Windows.Input;
using NuCharacter.DataBase;
using NuCharacter.Infrastrucure.Commands.Base;
using NuCharacter.Models;
using HandyControl.Tools.Extension;

namespace NuCharacter.ViewModels
{
    partial class MainWindowViewModel
    {

        private ObservableCollection<Character> _All_Characters;
        public ObservableCollection<Character> All_Characters { get => _All_Characters ?? new ObservableCollection<Character>(); set { _All_Characters = value; } }

        #region CharactersValues




        public List<string> AutoCompleteRaces { get => DataBase.Local_DB.Characters.Select(x=>x.Race).ToList() ; } 
        private string _autoCompleteRace;
        public string AutoCompleteRace
        {
            get { return DataBase.Local_DB.Characters.Where(x => x.Id_Character == SelectedCharacter.Id_Character).Select(x=>x.Race).FirstOrDefault(); }
            set
            {
                _autoCompleteRace = value;
                OnPropertyChanged(nameof(_autoCompleteRace));
                UpdateAutocompleteList();
            }
        }
        private void UpdateAutocompleteList()
        {
            var filteredList = AutoCompleteRaces
                .Where(s => string.IsNullOrEmpty(AutoCompleteRace) || s.Contains(AutoCompleteRace))
                .ToList();

            AutoCompleteRaces.Clear();
            foreach (var item in filteredList)
            {
                AutoCompleteRaces.Add(item);
            }
        }

        #endregion


        #region Selected Character


        private bool x = false;
        private Character _SelectedCharacter;
        public Character SelectedCharacter
        {
            get => _SelectedCharacter ?? DataBase.Local_DB.Characters.FirstOrDefault();
            set
            {
                SetProperty(ref _SelectedCharacter, value);

                if (x == false)
                {
                    x = true;
                    if (SelectedCharacter != null)
                    {
                        FrameSource = "P_Views/MainPageView.xaml";
                        StatusCharacter = SelectedCharacter.PrivateStatus ? "Status: Own" : "Status: Global";
                    }
                    else FrameSource = null;
                    x = false;
                    //Refresh();
                }

            }
        }
        #endregion

        #region Create Char
        public ICommand CreateCharacter { get; }
        private bool CanCreateCharacter(object obj) => obj is Group group && group == SelectedGroup && obj != null;
        private void OnCreateCharacter(object obj)
        {
            var group = obj as Group;

            var new_character = new Character();

            Local_DB.Insert(new_character);


            new_character.ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Data\pic.jfif");
            new_character.Id_Group = Gr_ID;
            new_character.Name = $"Character {All_Characters.Count}";
            new_character.PrivateStatus = true;
            Local_DB.Update(new_character);

            group.CountCharacters++;
            OnPropertyChanged(nameof(group));

            Refresh(group);


        }
        #endregion

        #region Remove Char
        public ICommand RemoveCharacter { get; }
        private bool CanRemoveCharacter(object obj) => SelectedGroup != null && SelectedCharacter != null;
        private void OnRemoveCharacter(object obj)
        {
            if (!(obj is Character character)) return;

            Local_DB.Remove(character);
            Refresh(SelectedGroup);
        }
        #endregion


        
        public ICommand ShareStatus { get; }
        private bool CanSharaStatus(object obj) => obj is Character ch && ch != null;
        private void OnShareStatus(object obj)
        {
            var ch = obj as Character;
            ch.PrivateStatus = !ch.PrivateStatus;
            OnPropertyChanged(nameof(StatusCharacter));
            
        }

        private string statusCharacter;
        public string StatusCharacter
        {
            get => statusCharacter;
            set
            {
                if (SelectedCharacter == null) return;
                if (SelectedCharacter.PrivateStatus == false) SetProperty(ref statusCharacter, "Status: Global");
                if (SelectedCharacter.PrivateStatus == true) SetProperty(ref statusCharacter, "Status: Own");
            }
        }


    }
}
