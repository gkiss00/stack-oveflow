using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class EditCommentView : UserControlBase
    {
        private string body;
        public string Body { get => body; set => SetProperty<string>(ref body, value, () => Validate()); }
        private Comment Comment { get; set; }
        private Post LastParent { get; set; }
        public ICommand Edit { get; set; }
        public ICommand Delete { get; set; }
        public ICommand Back { get; set; }
        //****************************************
        //****************************************
        //CONSTRUCTOR
        //****************************************
        //****************************************
        public EditCommentView(Comment c)
        {
            DataContext = this;

            Comment = c;

            LastParent = GetLastParent();

            Body = Comment.Body;          
            
            Edit = new RelayCommand(EditAction, () => { return Body != null && !HasErrors; });

            Delete = new RelayCommand(DeleteAction);

            Back = new RelayCommand(BackAction);

            InitializeComponent();
        }
        //****************************************
        //****************************************
        //GETTER
        //****************************************
        //****************************************
        private Post GetLastParent()
        {
            Post p = null;
            p = Comment.Post;
            if (p.Parent != null)
                p = p.Parent;
            return (p);
        }
        //****************************************
        //****************************************
        //ACTIONS
        //****************************************
        //****************************************
        private void EditAction()
        {
            Comment.Body = Body;
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_COMMENT_CHANGED);
            Redirect();
        }

        private void DeleteAction()
        {
            Comment.Delete();
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_COMMENT_CHANGED);
            Redirect();
        }

        private void BackAction()
        {
            Redirect();
        }
        //****************************************
        //****************************************
        //PERMISSION && VALIDATION
        //****************************************
        //****************************************
        private bool IsNull(string str)
        {
            if (string.IsNullOrEmpty(str) || IsEmpty(str))
                AddError("Body", Properties.Resources.Error_Required);
            return (!HasErrors);
        }

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
            App.NotifyColleagues(AppMessages.MSG_CLOSE_EDIT_COMMENT, Comment);
            App.NotifyColleagues(AppMessages.MSG_OPEN_QUESTION, LastParent);
        }
        //****************************************
        //****************************************
        //UTILS
        //****************************************
        //****************************************

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
