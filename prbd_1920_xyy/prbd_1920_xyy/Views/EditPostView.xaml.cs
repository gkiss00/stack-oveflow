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
    public partial class EditPostView : UserControlBase
    {
        public string IsQuestion { get; set; }
        public string CanBeDeleted { get; set; }

        private string title;
        public string Title { get => title; set => SetProperty<string>(ref title, value); }

        private string body;
        public string Body { get => body; set => SetProperty<string>(ref body, value, () => Validate()); }
        public Post Post { get; set; }

        private ObservableCollection<Tag> allTags;
        public ObservableCollection<Tag> AllTags { get => allTags; set => SetProperty(ref allTags, value); }

        private ObservableCollection<Tag> postTags;
        public ObservableCollection<Tag> PostTags { get => postTags; set => SetProperty(ref postTags, value); }

        public ICommand Edit { get; set; }

        public ICommand AddTag { get; set; }

        public ICommand RemoveTag { get; set; }

        public ICommand Delete {get; set; }
        //****************************************************
        //****************************************************
        //CONSTRUCTOR
        //****************************************************
        //****************************************************
        public EditPostView(Post p)
        {
            DataContext = this;

            Post = p;
            Title = Post.Title;
            Body = Post.Body;

            AllTags = GetAllTags();
            PostTags = GetPostTags();

            Edit = new RelayCommand(EditAction, () => { return Body != null && !HasErrors; });
            Delete = new RelayCommand(DeleteAction);

            AddTag = new RelayCommand<Tag>(t =>
            {
                AddTagAction(t);
            });

            RemoveTag = new RelayCommand<Tag>(t =>
            {
                RemoveTagAction(t);
            });

            App.Register<Post>(this, AppMessages.MSG_ANSWEAR_ACCEPTED, pp =>
            {
                CommentChanged();
            });

            App.Register<Post>(this, AppMessages.MSG_COMMENT_ADDED, pp =>
            {
                CommentChanged();
            });

            App.Register<Post>(this, AppMessages.MSG_COMMENT_ADDED, pp =>
            {
                CommentChanged();
            });

            App.Register(this, AppMessages.MSG_COMMENT_CHANGED, CommentChanged);

            App.Register(this, AppMessages.MSG_TAG_ADDED, SetTags);

            App.Register(this, AppMessages.MSG_TAG_DELETED, SetTags);

            SetVisual();

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

        private ObservableCollection<Tag> GetPostTags()
        {
            return (new ObservableCollection<Tag>(Post.Tags));
        }

        private Post GetLastParent()
        {
            Post LastParent = Post;
            if (Post.Parent != null)
                LastParent = Post.Parent;
            return (LastParent);
        }

        //****************************************************
        //****************************************************
        //ACTIONS
        //****************************************************
        //****************************************************
        private void EditAction()
        {
            if (!string.IsNullOrEmpty(Body))
            {
                Post.Body = Body;
                foreach (Tag t in Post.Tags)
                    t.Posts.Remove(Post);
                Post.Tags.Clear();
                foreach(Tag t in PostTags)
                {
                    Post.Tags.Add(t);
                    t.Posts.Add(Post);
                }    
                App.Model.SaveChanges();
                App.NotifyColleagues(AppMessages.MSG_POST_CHANGED, Post);
                Redirect();
            }
        }

        private void AddTagAction(Tag t)
        {
            if (PostTags.Count() < 3)
            {
                if (!PostTags.Contains(t))
                    PostTags.Add(t);
            }
        }

        private void RemoveTagAction(Tag t)
        {
            PostTags.Remove(t);
        }

        private void DeleteAction()
        {
            if (CanBeDel())
            {
                App.NotifyColleagues(AppMessages.MSG_OPEN_DELETE, Post);
            }
        }
        //****************************************************
        //****************************************************
        //PERMISSION && VALIDATION
        //****************************************************
        //****************************************************
        private bool CanBeDel()
        {
            if (Post.Parent.AcceptedAnswer != null && Post.Parent.AcceptedAnswer.PostId == Post.PostId)
                return (false);
            if (App.CurrentUser.Role.Equals(Role.Admin) || (App.CurrentUser.UserId == Post.Author.UserId && Post.Comments.Count() == 0))
                return (true);
            return (false);
        }

        

        public override bool Validate()
        {
            ClearErrors();
            if (IsNull(Body))
                RaiseErrors();
            return !HasErrors;
        }
        //****************************************************
        //****************************************************
        //VISUAL
        //****************************************************
        //****************************************************

        private void CommentChanged()
        {
            SetVisual();
        }

        private void SetVisual()
        {
            if (Post.Parent == null)
            {
                CanBeDeleted = "Hidden";
                IsQuestion = "Visible";
            }
            else
            {
                IsQuestion = "Hidden";
                Button b = (Button)delButton;
                if (Post.Parent.AcceptedAnswer != null && Post.Parent.AcceptedAnswer.PostId == Post.PostId)
                {
                    if (b != null)
                        b.Visibility = Visibility.Hidden;
                    CanBeDeleted = "Hidden";
                }
                    
                else if (App.CurrentUser.Role.Equals(Role.Admin) || (App.CurrentUser.UserId == Post.Author.UserId && Post.Comments.Count() == 0))
                {
                    if (b != null)
                        b.Visibility = Visibility.Visible;
                    CanBeDeleted = "Visible";
                }    
                else
                {
                    if (b != null)
                        b.Visibility = Visibility.Hidden;
                    CanBeDeleted = "Hidden";
                }   
            }  
        }
        //****************************************************
        //****************************************************
        //REDIRECT
        //****************************************************
        //****************************************************
        private void Redirect()
        {
            App.NotifyColleagues(AppMessages.MSG_CLOSE_EDIT, Post);
            App.NotifyColleagues(AppMessages.MSG_OPEN_QUESTION, GetLastParent());
        }

        //****************************************************
        //****************************************************
        //UTILS
        //****************************************************
        //****************************************************
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

        private void SetTags()
        {
            AllTags = GetAllTags();
            PostTags = GetPostTags();
        }
    }
}
