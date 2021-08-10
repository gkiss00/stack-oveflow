using PRBD_Framework;
using System;
using System.Collections.Generic;
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

namespace prbd_1920_xyy
{
    public partial class DeleteView : UserControlBase
    {
        private Post Post { get; set; }

        public ICommand Del { get; set; }
        public ICommand Back { get; set; }
        //****************************************************
        //****************************************************
        //CONSTRUCTOR
        //****************************************************
        //****************************************************
        public DeleteView(Post p)
        {
            DataContext = this;

            Post = p;
            Del = new RelayCommand(DelAction);
            Back = new RelayCommand(BackAction);

            App.Register<Post>(this, AppMessages.MSG_ANSWEAR_ADDED, pp =>
            {
                if (App.CurrentUser.Role != Role.Admin)
                    App.NotifyColleagues(AppMessages.MSG_CLOSE_DELETE, pp);
            });

            App.Register<Post>(this, AppMessages.MSG_COMMENT_ADDED, pp =>
            {
                if (App.CurrentUser.Role != Role.Admin)
                    App.NotifyColleagues(AppMessages.MSG_CLOSE_DELETE, pp);
            });

            App.Register<Post>(this, AppMessages.MSG_ANSWEAR_ACCEPTED, pp =>
            {
                App.NotifyColleagues(AppMessages.MSG_CLOSE_DELETE, pp.AcceptedAnswer);
            });

            InitializeComponent();
        }
        //****************************************************
        //****************************************************
        //ACTIONS
        //****************************************************
        //****************************************************
        private void BackAction()
        {
            App.NotifyColleagues(AppMessages.MSG_CLOSE_DELETE, Post);
        }

        private void DelAction()
        {
            if (CanBeDeleted())
            {
                Post parent = Post.Parent;
                App.NotifyColleagues(AppMessages.MSG_POST_DELETED, Post);
                Post.Delete();
                App.Model.SaveChanges();
                if (Parent != null)
                    App.NotifyColleagues(AppMessages.MSG_ANSWEAR_DELETED);
                App.NotifyColleagues(AppMessages.MSG_POST_CHANGED, Post);
                Redirect(parent);
            }
        }
        //****************************************************
        //****************************************************
        //VALIDATION && PERMISSION
        //****************************************************
        //****************************************************
        private bool CanBeDeleted()
        {
            if (Post.Parent != null && Post.Parent.AcceptedAnswer != null && Post.Parent.AcceptedAnswer.PostId == Post.PostId)
                return (false);
            if (App.CurrentUser.Role.Equals(Role.Admin))
                return (true);
            else
            {
                if (Post.Answears.Count() != 0 || Post.Comments.Count() != 0)
                    return (false);
            }
            return (true);
        }
        //****************************************************
        //****************************************************
        //REDIRECT
        //****************************************************
        //****************************************************

        private void Redirect(Post parent)
        {
            if (Parent == null)
                App.NotifyColleagues(AppMessages.MSG_OPEN_INDEX);
            else
                App.NotifyColleagues(AppMessages.MSG_OPEN_QUESTION, parent);
        }
    }
}
