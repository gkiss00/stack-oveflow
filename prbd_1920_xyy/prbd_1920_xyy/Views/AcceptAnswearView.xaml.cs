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
    public partial class AcceptAnswearView : UserControlBase
    {
        public Post Post { get; set; }
        public Post Ans { get; set; }

        public string CanBeAccepted { get; set; }
        public string CanBeRemoved { get; set; }

        public ICommand Accept { get; set; }
        public ICommand Remove { get; set; }
        public ICommand Back { get; set; }
        //******************************************
        //******************************************
        //CONSTRUCTOR
        //******************************************
        //******************************************
        public AcceptAnswearView(Post ans)
        {
            DataContext = this;

            Post = (Post)ans.Parent;
            Ans = ans;

            Accept = new RelayCommand(AcceptAction);

            Remove = new RelayCommand(RemoveAction);

            Back = new RelayCommand(BackAction);

            SetVisual();

            InitializeComponent();
        }
        //******************************************
        //******************************************
        //ACTIONS
        //******************************************
        //******************************************
        private void AcceptAction()
        {
            Post.AcceptedAnswer = Ans;
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_ANSWEAR_ACCEPTED, Post);
            Redirect();
        }

        private void RemoveAction()
        {
            Post.AcceptedAnswer = null;
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_ANSWEAR_ACCEPTED, Post);
            Redirect();
        }

        private void BackAction()
        {
            Redirect();
        }

        //******************************************
        //******************************************
        //VISUAL
        //******************************************
        //******************************************
        private void SetVisual()
        {
            if (Post.AcceptedAnswer == null)
            {
                CanBeAccepted = "Visible";
                CanBeRemoved = "Hidden";
            }
            else if (Post.AcceptedAnswer.PostId == Ans.PostId)
            {
                CanBeAccepted = "Hidden";
                CanBeRemoved = "Visible";
            }
            else
            {
                CanBeAccepted = "Visible";
                CanBeRemoved = "Hidden";
            }    
        }
        //******************************************
        //******************************************
        //REDIRECT
        //******************************************
        //******************************************
        private void Redirect()
        {
            App.NotifyColleagues(AppMessages.MSG_CLOSE_ACCEPT, Post);
            App.NotifyColleagues(AppMessages.MSG_OPEN_QUESTION, Post);
        }
    }
}
