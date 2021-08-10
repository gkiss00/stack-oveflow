using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace prbd_1920_xyy
{
    public partial class AddComment : UserControlBase
    {
        public Post Post { get; set; }

        private string body;
        public string Body { get => body; set => SetProperty<string>(ref body, value, () => Validate()); }

        public ICommand AddAComment { get; set; }

        public ICommand Cancel { get; set; }
        //***************************************
        //***************************************
        //CONSTRUCTOR
        //***************************************
        //***************************************
        public AddComment(Post p)
        {
            DataContext = this;

            Post = p;

            AddAComment = new RelayCommand(AddCommentAction, () => { return Body != null && !HasErrors; });

            Cancel = new RelayCommand(CancelAction);

            InitializeComponent();
        }
        //***************************************
        //***************************************
        //ACTIONS
        //***************************************
        //***************************************
        private void AddCommentAction()
        {
            App.Model.CreateComment(App.CurrentUser, Post, Body);
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_COMMENT_ADDED, Post);
            Redirect();
        }

        private void CancelAction()
        {
            Redirect();
        }
        //****************************************
        //****************************************
        //PERMISSION && VALIDATION
        //****************************************
        //****************************************

        public override bool Validate()
        {
            ClearErrors();
            if (IsNull(Body))
                RaiseErrors();
            return !HasErrors;
        }
        //****************************************
        //****************************************
        //REDIRECT
        //****************************************
        //****************************************
        private void Redirect()
        {
            Post LastParent = Post;
            if (Post.Parent != null)
                LastParent = Post.Parent;
            App.NotifyColleagues(AppMessages.MSG_CLOSE_COMMENT, Post);
            App.NotifyColleagues(AppMessages.MSG_OPEN_QUESTION, LastParent);
        }

        //****************************************
        //****************************************
        //UTILS
        //****************************************
        //****************************************

        private bool IsNull(string str)
        {
            if (string.IsNullOrEmpty(str) || IsEmpty(str))
                AddError("Body", Properties.Resources.Error_Required);
            return (!HasErrors);
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
