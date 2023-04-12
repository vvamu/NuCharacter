using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using NuCharacter.DataBase;
using NuCharacter.Models;
using System.Windows;

namespace NuCharacter.ViewModels
{
    internal partial class MainWindowViewModel
    {

        private ObservableCollection<Group> _All_Groups;
        public ObservableCollection<Group> All_Groups { get => _All_Groups; set => SetProperty(ref _All_Groups, value); }

         
        private ObservableCollection<Group> All_User_Groups()
        {
            var a = Local_DB.SelectAll<Group>().Where(x => x.Id_User == Acc?.ID_User);
            return new ObservableCollection<Group>(a);
        }

        #region Selected Group

        bool xxxx = false;

        private Group _SelectedGroup;
        public Group SelectedGroup
        {
            get => _SelectedGroup;
            set
            {
                SetProperty(ref _SelectedGroup, value);
                OnPropertyChanged("All_Groups");
                OnPropertyChanged("SelectedGroup");

                if (xxxx == false)
                {
                    xxxx = true;
                    FilterText = "";
                    Refresh(SelectedGroup);
                    xxxx = false;

                }

            }
        }

        #endregion

        public int Gr_ID { get => _SelectedGroup.Id_Group; }

        public ICommand Click_AllGroups { get; }
        private bool CanClick_AllGroups(object obj) => true;
        private void OnClick_AllGroups(object obj)
        {
            var userID = Acc.ID_User;
            SelectedGroup = null;

            All_Groups = All_User_Groups();
            OnPropertyChanged(nameof(All_Groups));

            var allCharacter = new ObservableCollection<Character>();
            foreach(var groupCharacter in All_Groups)
            {
                foreach(var character in Local_DB.SelectAll<Character>().Where(x => x.Id_Group == groupCharacter.Id_Group))
                {
                    allCharacter.Add(character);
                }
            }
            

            All_Characters.Clear();
            All_Characters = allCharacter;
            OnPropertyChanged("All_Characters");

            FilterText = "";
        }

        #region Create Group
        public ICommand CreateGroup { get; }
        private bool CanCreateGroup(object obj) => true;
        private void OnCreateGroup(object obj)
        {

            var count = All_User_Groups().Count() + 1;

            var new_group = new Group() { Name = "Group " + count, Id_User = Acc.ID_User, CountCharacters = 0 };
            Local_DB.Insert(new_group);

            Refresh();

        }

        #endregion

        #region Remove Group

        public ICommand RemoveGroup { get; }
        private bool CanRemoveGroup(object obj) => obj is Group group && All_Groups.Contains(group) && SelectedGroup != null;

        private void OnRemoveGroup(object obj)
        {
            //MessageBox messageBox = new MessageBox("Do",);

            //MessageBox m = new MessageBox;
            MessageBoxResult result = HandyControl.Controls.MessageBox.Show("\tAll character in this group will be delete \n","Warning",MessageBoxButton.OKCancel,MessageBoxImage.Warning);
            if(result == MessageBoxResult.Cancel) return;
            foreach(var item in Local_DB.SelectAll<Character>().Where(x=>x.Id_Group == (obj as Group).Id_Group))
            {
                Local_DB.Remove(item);
            }
            //while(messageBox.ShowDialog() != ) {}
            var group = obj as Group;

            Local_DB.Remove(group);

            Refresh();
        }


        #endregion


        public void Refresh(Group group = null)
        {

            if (group != null)
            {
                All_Characters.Clear();
                var a = Local_DB.SelectAll<Character>().Where(x => x.Id_Group == Gr_ID);
                All_Characters = new ObservableCollection<Character>(a);
                OnPropertyChanged("All_Characters");

            }
            else
            {
                All_Groups.Clear();
                All_Groups = All_User_Groups();
                OnPropertyChanged(nameof(All_Groups));
            }
            OnPropertyChanged("Acc");
        }
    }
}
