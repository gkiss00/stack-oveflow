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
    public partial class VoteView : UserControlBase
    {
        private string yourVote;
        public string YourVote { get => yourVote; set => SetProperty<string>(ref yourVote, value); }
        private Post Post { get; set; }
        public ICommand Like { get; set; }
        public ICommand Dislike { get; set; }
        public ICommand Back { get; set; }
        //**********************************
        //**********************************
        //CONSTRUCTOR
        //**********************************
        //**********************************
        public VoteView(Post p)
        {
            DataContext = this;

            Post = p;

            Like = new RelayCommand(LikeAction);
            Dislike = new RelayCommand(DislikeAction);
            Back = new RelayCommand(BackAction);

            SetYourVote();

            InitializeComponent();
        }
        //**********************************
        //**********************************
        //GETTER
        //**********************************
        //**********************************
        private Vote getVote()
        {
            foreach (Vote v in App.CurrentUser.Votes)
            {
                if (v.Post.PostId == Post.PostId)
                {
                    return (v);
                }
            }
            return (null);
        }
        //**********************************
        //**********************************
        //ACTIONS
        //**********************************
        //**********************************
        private void LikeAction()
        {
            Vote v = getVote();
            if(v == null)
            {
                App.Model.CreateVote(App.CurrentUser, Post, 1);
            }
            else
            {
                if (v.UpDown == 1)
                {
                    v.Delete();
                }
                else
                {
                    v.UpDown = 1;
                }
            }
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_VOTE_CHANGED, Post);
            SetYourVote();
        }

        private void DislikeAction()
        {
            Vote v = getVote();
            if (v == null)
            {
                App.Model.CreateVote(App.CurrentUser, Post, -1);
            }
            else
            {
                if (v.UpDown == -1)
                {
                    v.Delete();
                }
                else
                {
                    v.UpDown = -1;
                }
            }
            App.Model.SaveChanges();
            App.NotifyColleagues(AppMessages.MSG_VOTE_CHANGED, Post);
            SetYourVote();
        }

        private void BackAction()
        {
            App.NotifyColleagues(AppMessages.MSG_CLOSE_VOTE, Post);
        }
        //**********************************
        //**********************************
        //VISIBILITY
        //**********************************
        //**********************************
        private void SetYourVote()
        {
            Vote v = getVote();

            if (v == null)
                YourVote = "0";
            else
                YourVote = v.UpDown.ToString();
        }
    }
}
