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
    public partial class AskView : UserControlBase
    {
        private static int maxtag = 3;

        private string title;
        public string Title { get => title;
            set => SetProperty<string>(ref title, value, () => Validate());
        }

        private string body;
        public string Body
        {
            get => body;
            set => SetProperty<string>(ref body, value, () => Validate());
        }

        private string tag1;
        public String Tag1 {get => tag1; set => SetProperty<string>(ref tag1, value); }

        private string tag2;
        public String Tag2 { get => tag2; set => SetProperty<string>(ref tag2, value); }

        private string tag3;
        public String Tag3 { get => tag3; set => SetProperty<string>(ref tag3, value); }

        private ObservableCollection<Tag> tags;
        public ObservableCollection<Tag> Tags { get => tags; set => SetProperty(ref tags, value); }

        List<Tag> l = new List<Tag>();
        public ICommand Ask { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand AddTag { get; set; }
        //****************************************************
        //****************************************************
        //CONSTRUCTOR
        //****************************************************
        //****************************************************
        public AskView()
        {
            DataContext = this;

            Ask = new RelayCommand(AskAction, () => { return Body != null && Title != null && !HasErrors; });
            Cancel = new RelayCommand(CancelAction);
            AddTag = new RelayCommand<Tag>(t => {
                AddTagAction(t);
            });

            Tags = GetAllTags();

            App.Register(this, AppMessages.MSG_TAG_ADDED, () =>
            {
                Tags = GetAllTags();
                BindVisual();
            });

            App.Register(this, AppMessages.MSG_TAG_DELETED, () =>
            {
                Tags = GetAllTags();
                l.Clear();
                BindVisual();
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
            var query = from m in App.Model.Tags
                        select m;
            return (new ObservableCollection<Tag>(query));
        }
        //****************************************************
        //****************************************************
        //ACTIONS
        //****************************************************
        //****************************************************
        private void AddTagAction(Tag t)
        {
            if (l.Contains(t))
            {
                l.Remove(t);
            }
            else
            {
                if (l.Count() == maxtag)
                {
                    l.Remove(l[0]);
                }
                l.Add(t);
            }
            BindVisual();
        }

        private void AskAction()
        {
            if (Validate())
            {
                if (l.Count() == 0)
                {
                    l = null;
                }
                App.Model.CreatePost(App.CurrentUser, title, body, null, l);
                App.Model.SaveChanges();
                App.NotifyColleagues(AppMessages.MSG_POST_CHANGED, null);
                CancelAction();
            }
        }

        private void CancelAction()
        {
            App.NotifyColleagues(AppMessages.MSG_CLOSE_ASK, "New Q");
            App.NotifyColleagues(AppMessages.MSG_OPEN_INDEX);
        }

        //****************************************************
        //****************************************************
        //PERMISSION AND VALIDATION
        //****************************************************
        //****************************************************

        public override bool Validate()
        {
            ClearErrors();
            if (!ValidateTitle() || !ValidateBody())
                RaiseErrors();
            return !HasErrors;
        }

        private bool ValidateTitle()
        {
            if (IsNull(title))
                AddError("Title", Properties.Resources.Error_Required);
            return (!HasErrors);
        }

        private bool ValidateBody()
        {
            if (IsNull(body))
                AddError("Body", Properties.Resources.Error_Required);
            return (!HasErrors);
        }
        //****************************************************
        //****************************************************
        //VISUAL
        //****************************************************
        //****************************************************
        private void BindVisual()
        {
            if (l.Count() == 0)
            {
                Tag1 = null;
                Tag2 = null;
                Tag3 = null;
            }
            if (l.Count() == 1)
            {
                Tag1 = l[0].TagName;
                Tag2 = null;
                Tag3 = null;
            }
            if (l.Count() == 2)
            {
                Tag1 = l[0].TagName;
                Tag2 = l[1].TagName;
                Tag3 = null;
            }
            if (l.Count() == 3)
            {
                Tag1 = l[0].TagName;
                Tag2 = l[1].TagName;
                Tag3 = l[2].TagName;
            }
        }

        //****************************************************
        //****************************************************
        //UTILS
        //****************************************************
        //****************************************************
        private bool IsNull(string str)
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
