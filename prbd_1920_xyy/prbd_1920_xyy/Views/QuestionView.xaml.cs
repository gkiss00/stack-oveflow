using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class QuestionView : UserControlBase
    {
        public string IsAuthor { get; set; }
        public string CanBeDel { get; set; }
        private string postBody;
        public string PostBody { get => postBody; set => SetProperty<string>(ref postBody, value); }

        private string tag1;
        public string Tag1 { get => tag1; set => SetProperty<string>(ref tag1, value); }

        private string tag2;
        public string Tag2 { get => tag2; set => SetProperty<string>(ref tag2, value); }

        private string tag3;
        public string Tag3 { get => tag3; set => SetProperty<string>(ref tag3, value); }
        public Post Post { get; set; }

        private ObservableCollection<Post> answears;
        public ObservableCollection<Post> Answears { get => answears; set => SetProperty(ref answears, value); }

        private ObservableCollection<Comment> comments;
        public ObservableCollection<Comment> Comments { get => comments; set => SetProperty(ref comments, value); }
        private string rep;
        public string Rep { get => rep; set => SetProperty<string>(ref rep, value, () => Validate()); }

        public ICommand Poste { get; set; }

        public ICommand Del { get; set; }

        public ICommand EditPost { get; set; }

        public ICommand EditQuestion { get; set; }

        public ICommand AccepteAnswear { get; set; }

        public ICommand Comment { get; set; }

        public ICommand EditComment { get; set; }

        public ICommand Vote { get; set; }

        public ICommand GoToIndex { get; set; }
        //***************************************************************
        //***************************************************************
        //CONSTRUCTOR
        //***************************************************************
        //***************************************************************
        public QuestionView(Post p)
        {
            DataContext = this;

            Rep = null;

            Post = p;

            SetVisibility();

            PostBody = Post.Body;

            Answears = GetAllAnswears();

            Comments = GetAllComments();

            //COMMAND
            Poste = new RelayCommand(PosteAction, () => { return Rep != null && !HasErrors;});

            Del = new RelayCommand(DelAction);

            EditPost = new RelayCommand<Post>(pp =>
            {
                if (IsAllowed(pp))
                    App.NotifyColleagues(AppMessages.MSG_OPEN_EDIT, pp);
            });

            EditQuestion = new RelayCommand(EditQuestionAction);

            AccepteAnswear = new RelayCommand<Post>(ans =>
            {
                if (IsAuthorQ())
                {
                    App.NotifyColleagues(AppMessages.MSG_OPEN_ACCEPT, ans);
                }
            });

            Comment = new RelayCommand<Post>(pp =>
            {         
                App.NotifyColleagues(AppMessages.MSG_OPEN_COMMENT, pp);
            });

            EditComment = new RelayCommand<Comment>(c =>
            {
                if (CanEdit(c))
                    App.NotifyColleagues(AppMessages.MSG_OPEN_EDIT_COMMENT, c);
            });

            Vote = new RelayCommand<Post>(pp =>
            {
                if (CanVote(pp))
                    App.NotifyColleagues(AppMessages.MSG_OPEN_VOTE, pp);
            });

            GoToIndex = new RelayCommand<string>(str =>
            {
                if (str != null)
                {
                    App.NotifyColleagues(AppMessages.MSG_SORT_BY_TAG, str);
                    App.NotifyColleagues(AppMessages.MSG_OPEN_INDEX);
                }
                    
            });
            //MESSAGE LISTENER
            App.Register<Post>(this, AppMessages.MSG_ANSWEAR_ADDED, pp => {
                Answears = GetAllAnswears();
                SetVisibility();
            });

            App.Register(this, AppMessages.MSG_TAG_DELETED, () =>
            {
                SetTags();
            });

            App.Register(this, AppMessages.MSG_TAG_ADDED, () =>
            {
                SetTags();
            });

            App.Register<Post>(this, AppMessages.MSG_POST_CHANGED, ppp =>
            {
                Answears = GetAllAnswears();
                var query = from po in App.Model.Posts
                            where po.PostId == Post.PostId
                            select po;
                List<Post> tmp = new List<Post>(query);
                if (tmp.Count() > 0)
                    Post = tmp[0];
                PostBody = Post.Body;
                SetTags();
                SetVisibility();
            });

            App.Register<Post>(this, AppMessages.MSG_ANSWEAR_ACCEPTED, ppp =>
            {
                Answears = GetAllAnswears();
            });

            App.Register(this, AppMessages.MSG_ANSWEAR_DELETED, AnsweardDeleted);

            App.Register<Post>(this, AppMessages.MSG_VOTE_CHANGED, ppp =>
            {
                Answears = GetAllAnswears();
            });

            App.Register<Post>(this, AppMessages.MSG_COMMENT_ADDED, ppp =>
            {
                CommentCanged();
            });

            App.Register(this, AppMessages.MSG_COMMENT_CHANGED, CommentCanged);

            SetTags();

            InitializeComponent();
        }
        
        //***************************************************************
        //***************************************************************
        //GETTER
        //***************************************************************
        //***************************************************************

        private ObservableCollection<Comment> GetAllComments()
        {
            var query = from c in App.Model.Comments
                        where c.Post.PostId == Post.PostId
                        select c;
            List<Comment> tmp = new List<Comment>(query);
            //Sort by date
            for (int i = 0; i < tmp.Count() - 1; ++i)
            {
                if (tmp[i].TimeStamp.CompareTo(tmp[i + 1].TimeStamp) > 0)
                {

                    Comment t2 = tmp[i];
                    tmp[i] = tmp[i + 1];
                    tmp[i + 1] = t2;
                    i = -1;
                }
            }
            return (new ObservableCollection<Comment>(tmp));
        }
            
        private ObservableCollection<Post> GetAllAnswears()
        {
            //Get all answears
            var query = (from m in App.Model.Posts
                        where m.Parent.PostId == Post.PostId
                        select m);
            List<Post> tmp = new List<Post>(query);
           
            //Sort by score
            List<int> tmp2 = new List<int>();
            foreach(Post p1 in tmp)
                tmp2.Add(p1.NbVote);
            for (int i = 0; i < tmp2.Count() - 1; ++i)
            {
                if (tmp2[i] < tmp2[i + 1])
                {
                    int t = tmp2[i];
                    tmp2[i] = tmp2[i + 1];
                    tmp2[i + 1] = t;

                    Post t2 = tmp[i];
                    tmp[i] = tmp[i + 1];
                    tmp[i + 1] = t2;
                    i = -1;
                }
            }
            //Sort by date
            for (int i = 0; i < tmp2.Count() - 1; ++i)
            {
                if (tmp2[i] == tmp2[i + 1])
                {
                    if (tmp[i].TimeStamp.CompareTo(tmp[i + 1].TimeStamp) < 0)
                    {
                        int t = tmp2[i];
                        tmp2[i] = tmp2[i + 1];
                        tmp2[i + 1] = t;

                        Post t2 = tmp[i];
                        tmp[i] = tmp[i + 1];
                        tmp[i + 1] = t2;
                        i = -1;
                    }
                }
            }
            //Find the validated answear
            List<Post> tmp3 = new List<Post>(tmp);
            for (int i = 0; i < tmp.Count(); ++i)
            {
                if (tmp[i].Parent.AcceptedAnswer != null && tmp[i].Parent.AcceptedAnswer.PostId == tmp[i].PostId)
                {
                    tmp3.Clear();
                    tmp3.Add(tmp[i]);
                    tmp.Remove(tmp[i]);
                    tmp3.AddRange(tmp);
                    break;
                }
            }
            return (new ObservableCollection<Post>(tmp3));
        }
        //***************************************************************
        //***************************************************************
        //ACTION
        //***************************************************************
        //***************************************************************
        private void DelAction()
        {
            if (CanBeDeleted())
            {
                App.NotifyColleagues(AppMessages.MSG_OPEN_DELETE, Post);
            }
        }

        private void PosteAction()
        {
            App.Model.CreatePost(App.CurrentUser, null, Rep, Post, null);
            App.Model.SaveChanges();
            if (App.CurrentUser.Role != Role.Admin)
                App.NotifyColleagues(AppMessages.MSG_CLOSE_DELETE, Post);
            App.NotifyColleagues(AppMessages.MSG_ANSWEAR_ADDED, Post);
            Rep = null;
        }

        private void EditQuestionAction()
        {
            if (IsAllowed(Post))
                App.NotifyColleagues(AppMessages.MSG_OPEN_EDIT, Post);
        }

        //***************************************************************
        //***************************************************************
        //PERMISSION AND VALIDATION
        //***************************************************************
        //***************************************************************

        private bool CanVote(Post p)
        {
            if (p == null)
                return (false);
            if (App.CurrentUser.UserId == p.Author.UserId)
                return (false);
            return (true);
        }

        private bool CanEdit(Comment c)
        {
            if (c == null)
                return (false);
            if (App.CurrentUser.UserId == c.Author.UserId || App.CurrentUser.Role == Role.Admin)
                return (true);
            return (false);
        }

        private bool IsAuthorQ()
        {
            if (App.CurrentUser.UserId == Post.Author.UserId)
                return (true);
            return (false);
        }
        private bool IsAllowed(Post p)
        {
            if (p == null)
                return (false);
            if (App.CurrentUser.Role == Role.Admin || App.CurrentUser.UserId == p.Author.UserId)
                return (true);
            return (false);
        }

        private bool CanBeDeleted()
        {
            if(App.CurrentUser.Role == Role.Admin)
                return (true);
            if(App.CurrentUser.UserId == Post.Author.UserId)
            {

                if (Post.Answears.Count() == 0 && Post.Comments.Count() == 0)
                    return (true);
                else
                    return (false);
            }
            return (false);
        }

        private bool IsNull(string str)
        {
            if (string.IsNullOrEmpty(str) || IsEmpty(str))
                AddError("Rep", Properties.Resources.Error_Required);
            return (!HasErrors);
        }

        public override bool Validate()
        {
            ClearErrors();
            if (IsNull(Rep))
                RaiseErrors();
            return (!HasErrors);
        }

        //***************************************************************
        //***************************************************************
        //VISUAL
        //***************************************************************
        //***************************************************************
        private void SetTags()
        {
            if (Post.Tags.Count() == 0)
            {
                Tag1 = null;
                Tag2 = null;
                Tag3 = null;
            }
            if (Post.Tags.Count() == 1)
            {
                Tag1 = Post.Tags.ElementAt(0).TagName;
                Tag2 = null;
                Tag3 = null;
            }
            if (Post.Tags.Count() == 2)
            {
                Tag1 = Post.Tags.ElementAt(0).TagName;
                Tag2 = Post.Tags.ElementAt(1).TagName;
                Tag3 = null;
            }
            if (Post.Tags.Count() == 3)
            {
                Tag1 = Post.Tags.ElementAt(0).TagName;
                Tag2 = Post.Tags.ElementAt(1).TagName;
                Tag3 = Post.Tags.ElementAt(2).TagName;
            }
        }

        private void SetVisibility()
        {
            if (IsAllowed(Post))
                IsAuthor = "Visible";
            else
                IsAuthor = "Hidden";

            if (CanBeDeleted())
            {
                CanBeDel = "Visible";
                Button b = (Button)delButon;
                if (b != null)
                    b.Visibility = Visibility.Visible;
            }
                
            else
            {
                CanBeDel = "Hidden";
                Button b = (Button)delButon;
                if (b != null)
                    b.Visibility = Visibility.Hidden;
            }
                
        }
        //***************************************************************
        //***************************************************************
        //UTILS
        //***************************************************************
        //***************************************************************
        private void CommentCanged()
        {
            Comments = GetAllComments();
            Answears = GetAllAnswears();
        }

        private void AnsweardDeleted()
        {
            Answears = GetAllAnswears();
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
