using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace prbd_1920_xyy
{
    public partial class TagsView : UserControlBase
    {
        public string IsAdmin { get; set; }

        private string newTag;
        public string NewTag { get => newTag; set => SetProperty<string>(ref newTag, value, () => Validate()); }
        //TAG TO EDIT
        private string tagToEditName;
        public string TagToEditName { get => tagToEditName; set => SetProperty(ref tagToEditName, value); }

        public Tag TagToEdit { get; set; }
        //TAG TO DELETE
        private string tagToDeleteName;
        public string TagToDeleteName { get => tagToDeleteName; set => SetProperty<string>(ref tagToDeleteName, value); }

        public Tag TagToDelete { get; set; }
        //LISTE DES TAGS
        private ObservableCollection<Tag> tags;
        public ObservableCollection<Tag> Tags { get => tags; set => SetProperty(ref tags, value); }

        public ICommand AddTag { get; set; }
        public ICommand SelectTag { get; set; }
        public ICommand EditTag { get; set; }
        public ICommand DeleteTag { get; set; }
        public ICommand GoToIndex { get; set; }
        //****************************************************
        //****************************************************
        //CONSTRUCTOR
        //****************************************************
        //****************************************************
        public TagsView()
        {
            DataContext = this;

            TagToDelete = null;

            SetVisibility();

            Tags = GetAllTags();

            //COMMAND
            AddTag = new RelayCommand(AddTagAction, () => { return (!IsNull(newTag) && !Exists(newTag)); });

            SelectTag = new RelayCommand<Tag>(t =>
            {
                SelectTagAction(t);
            });

            EditTag = new RelayCommand(EditTagAction, () => { return (!IsNull(TagToEditName) && !Exists(TagToEditName)); });

            DeleteTag = new RelayCommand(DeleteTagAction, () => { return (!IsNull(TagToDeleteName)); });

            GoToIndex = new RelayCommand<Tag>(t =>
            {
                GoToIndexAction(t);
            });
            //LISTENER
            App.Register(this, AppMessages.MSG_TAG_ADDED, SetTags);

            App.Register(this, AppMessages.MSG_TAG_DELETED, SetTags);

            App.Register<Post>(this, AppMessages.MSG_POST_CHANGED, p =>
            {
                Tags = GetAllTags();
            });

            App.Register<Post>(this, AppMessages.MSG_POST_DELETED, p =>
            {
                Tags = GetAllTags();
            });

            InitializeComponent();
        }
        //****************************************************
        //****************************************************
        //GETTER
        //****************************************************
        //****************************************************
        private ObservableCollection<Tag> GetAllTags()
        {
            var query = from t in App.Model.Tags
                        select t;
            return (new ObservableCollection<Tag>(query));
        }
        //****************************************************
        //****************************************************
        //ACTIONS
        //****************************************************
        //****************************************************
        private void AddTagAction()
        {
            if (IsAllowed())
            {
                if (Validate())
                {
                    App.Model.CreateTag(NewTag);
                    App.Model.SaveChanges();
                    App.NotifyColleagues(AppMessages.MSG_TAG_ADDED);
                    NewTag = "";
                }
            }
        }

        private void SelectTagAction(Tag t)
        {
            if (IsAllowed())
            {
                TagToEditName = t.TagName;
                TagToEdit = t;
                TagToDeleteName = t.TagName;
                TagToDelete = t;
            }
        }

        private void EditTagAction()
        {
            if (IsAllowed() && !Exists(TagToEditName))
            {
                if (TagToEdit != null && !IsNull(TagToEditName))
                {
                    TagToEdit.TagName = TagToEditName;
                    App.Model.SaveChanges();
                    App.NotifyColleagues(AppMessages.MSG_TAG_ADDED);
                    Reset();
                }
            }
        }

        private void DeleteTagAction()
        {
            if (IsAllowed())
            {
                if (TagToDelete != null)
                {
                    TagToDelete.Delete();
                    App.Model.SaveChanges();
                    App.NotifyColleagues(AppMessages.MSG_TAG_DELETED);
                    Reset();
                }
            }
        }

        private void GoToIndexAction(Tag tt)
        {
            if (tt == null)
                return;
            App.NotifyColleagues(AppMessages.MSG_SORT_BY_TAG, tt.TagName);
            App.NotifyColleagues(AppMessages.MSG_OPEN_INDEX);
        }

        //****************************************************
        //****************************************************
        //VALIDATION AND PERMISSION
        //****************************************************
        //****************************************************

        public override bool Validate()
        {
            ClearErrors();
            if (!ValidateName())
                RaiseErrors();
            return (!HasErrors);
        }

        private bool ValidateName()
        {
            if (IsNull(NewTag))
            {
                AddError("NewTag", Properties.Resources.Error_Required);
            }
            else
            {
                if (Exists(NewTag))
                    AddError("NewTag", Properties.Resources.Error_AlreadyExist);
            }
            return (!HasErrors);
        }

        private bool IsAllowed()
        {
            if (App.CurrentUser.Role == Role.Admin)
                return (true);
            return (false);
        }

        //***************************************************************
        //***************************************************************
        //VISUAL
        //***************************************************************
        //***************************************************************

        private void SetVisibility()
        {
            if (IsAllowed())
                IsAdmin = "Visible";
            else
                IsAdmin = "Hidden";
        }

        //***************************************************************
        //***************************************************************
        //UTILS
        //***************************************************************
        //***************************************************************
        private void SetTags()
        {
            Tags = GetAllTags();
        }
        private void Reset()
        {
            TagToEditName = null;
            TagToEdit = null;
            TagToDeleteName = null;
            TagToDelete = null;
        }

        public bool Exists(string str)
        {
            if (str == null)
                return (true);    
            foreach (Tag tt in App.Model.Tags)
            {
                if (tt.TagName.Equals(str))
                    return (true);
            }
            return (false);
        }

        public bool IsNull(string str)
        {
            if (string.IsNullOrEmpty(str) || IsEmpty(str))
                return (true);
            return (false);
        }

        private bool IsEmpty(string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] != ' ')
                    return (false);
            }
            return (true);
        }
    }
}
