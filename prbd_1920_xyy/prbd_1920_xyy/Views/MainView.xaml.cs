using PRBD_Framework;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace prbd_1920_xyy {
    public partial class MainView : WindowBase {

        public ICommand Logout { get; set; }
        public ICommand Ask { get; set; }
        public ICommand TagList { get; set; }
        //****************************************************************
        //****************************************************************
        //CONSTRUCTOR
        //****************************************************************
        //****************************************************************
        public MainView() {
            InitializeComponent();
            
            DataContext = this;

            App.Register<Post>(this, AppMessages.MSG_OPEN_DELETE, p =>
            {
                OpenDelete(p);
            });

            App.Register<Post>(this, AppMessages.MSG_CLOSE_DELETE, p => {
                CloseDelete(p);
            });

            App.Register<string>(this, AppMessages.MSG_CLOSE_ASK, str =>
            {
                CloseTab(str);
            });

            App.Register<Post>(this, AppMessages.MSG_OPEN_QUESTION, p => {
                OpenQuestion(p);
            });

            App.Register<Post>(this, AppMessages.MSG_CLOSE_QUESTION, p => {
                CloseQuestion(p);
            });

            App.Register<Post>(this, AppMessages.MSG_OPEN_EDIT, p => {
                OpenEdit(p);
            });

            App.Register<Post>(this, AppMessages.MSG_CLOSE_EDIT, p => {
                CloseEdit(p);
            });

            App.Register<Post>(this, AppMessages.MSG_OPEN_ACCEPT, p => {
                OpenAccept(p);
            });

            App.Register<Post>(this, AppMessages.MSG_CLOSE_ACCEPT, p => {
                CloseAccept(p);
            });

            App.Register<Post>(this, AppMessages.MSG_OPEN_COMMENT, p =>
            {
                OpenComment(p);
            });

            App.Register<Post>(this, AppMessages.MSG_CLOSE_COMMENT, p => {
                CloseComment(p);
            });

            App.Register<Post>(this, AppMessages.MSG_POST_DELETED, p => {
                CloseAll(p);
            });

            App.Register<Comment>(this, AppMessages.MSG_OPEN_EDIT_COMMENT, c => {
                OpenEditComment(c);
            });

            App.Register<Comment>(this, AppMessages.MSG_CLOSE_EDIT_COMMENT, c => {
                CloseEditComment(c);
            });

            App.Register<Post>(this, AppMessages.MSG_OPEN_VOTE, p =>
            {
                OpenVote(p);
            });

            App.Register<Post>(this, AppMessages.MSG_CLOSE_VOTE, p =>
            {
                CloseVote(p);
            });

            App.Register(this, AppMessages.MSG_OPEN_INDEX, OpenIndex);

            Logout = new RelayCommand(LogoutAction);
            Ask = new RelayCommand(OpenASk);
            TagList = new RelayCommand(TagListAction);
        }
        //****************************************************************
        //****************************************************************
        //INDEX
        //****************************************************************
        //****************************************************************
        private void OpenIndex()
        {
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Index"))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
        }
        //****************************************************************
        //****************************************************************
        //ASK
        //****************************************************************
        //****************************************************************
        private void OpenASk()
        {
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("New Q"))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "New Q",
                Content = new AskView()
            };
            ConfigTab(tab);
        }

        private void CloseTab(string str)
        {
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals(str))
                {
                    tabControl.Items.Remove(t);
                    return;
                }
            }
        }
        //****************************************************************
        //****************************************************************
        //QUESTION
        //****************************************************************
        //****************************************************************
        private void OpenQuestion(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Question " + p.PostId))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "Question " + p.PostId,
                Content = new QuestionView(p)
            };
            ConfigTab(tab);
        }

        private void CloseQuestion(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Question " + p.PostId))
                {
                    tabControl.Items.Remove(t);
                    return;
                }
            }
        }
        //****************************************************************
        //****************************************************************
        //EDIT
        //****************************************************************
        //****************************************************************
        private void OpenEdit(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("EditPost " + p.PostId))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "EditPost " + p.PostId,
                Content = new EditPostView(p)
            };
            ConfigTab(tab);
        }

        private void CloseEdit(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("EditPost " + p.PostId))
                {
                    tabControl.Items.Remove(t);
                    return;
                }
            }
        }
        //****************************************************************
        //****************************************************************
        //ACCEPT
        //****************************************************************
        //****************************************************************
        private void OpenAccept(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("AcceptPost " + p.Parent.PostId))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "AcceptPost " + p.Parent.PostId,
                Content = new AcceptAnswearView(p)
            };
            ConfigTab(tab);
        }

        private void CloseAccept(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("AcceptPost " + p.PostId))
                {
                    tabControl.Items.Remove(t);
                    return;
                }
            }
        }


        //****************************************************************
        //****************************************************************
        //DELETE
        //****************************************************************
        //****************************************************************
        private void OpenDelete(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Delete " + p.PostId))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "Delete " + p.PostId,
                Content = new DeleteView(p)
            };
            ConfigTab(tab);
        }

        private void CloseDelete(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Delete " + p.PostId))
                {
                    tabControl.Items.Remove(t);
                    App.NotifyColleagues(AppMessages.MSG_CLOSE_ACCEPT, p);
                    return;
                }
            }

        }

        //****************************************************************
        //****************************************************************
        //COMMENT
        //****************************************************************
        //****************************************************************
        private void OpenComment(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Comment " + p.PostId))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "Comment " + p.PostId,
                Content = new AddComment(p),
            };
            ConfigTab(tab);
        }

        private void CloseComment(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Comment " + p.PostId))
                {
                    tabControl.Items.Remove(t);
                    return;
                }
            }

        }


        //****************************************************************
        //****************************************************************
        //EDIT COMMENT
        //****************************************************************
        //****************************************************************
        private void OpenEditComment(Comment c)
        {
            if (c == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Edit Comment " + c.CommentId))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "Edit Comment " + c.CommentId,
                Content = new EditCommentView(c),
            };
            ConfigTab(tab);
        }

        private void CloseEditComment(Comment c)
        {
            if (c == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Edit Comment " + c.CommentId))
                {
                    tabControl.Items.Remove(t);
                    return;
                }
            }

        }

        //****************************************************************
        //****************************************************************
        //VOTE
        //****************************************************************
        //****************************************************************
        private void OpenVote(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Vote " + p.PostId))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "Vote " + p.PostId,
                Content = new VoteView(p),
            };
            ConfigTab(tab);
        }

        private void CloseVote(Post p)
        {
            if (p == null)
                return;
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("Vote " + p.PostId))
                {
                    tabControl.Items.Remove(t);
                    return;
                }
            }

        }

        //****************************************************************
        //****************************************************************
        //LOGIN LOGOUT TAGLIST
        //****************************************************************
        //****************************************************************
        private void TagListAction()
        {
            foreach (TabItem t in tabControl.Items)
            {
                if (t.Header.ToString().Equals("TagList"))
                {
                    Dispatcher.InvokeAsync(() => t.Focus());
                    return;
                }
            }
            var tab = new TabItem()
            {
                Header = "TagList",
                Content = new TagsView()
            };
            ConfigTab(tab);
        }

        private void Login() {
            var loginView = new LoginView();
            Visibility = Visibility.Hidden;
            var res = loginView.ShowDialog();
            if (res == true) {
                Visibility = Visibility.Visible;
            }
            else {
                Close();
            }
        }

        private void LogoutAction() {
            App.CurrentUser = null;
            for (int i = tabControl.Items.Count - 1; i > 0; i--) 
                tabControl.Items.RemoveAt(i);
            Login();
        }
        //****************************************************************
        //****************************************************************
        //CLOSE ALL
        //****************************************************************
        //****************************************************************
        private void CloseAll(Post p)
        {
            if (p == null)
                return;
            foreach (Post ans in p.Answears)
            {
                CloseAll(ans);
            }
            foreach (Comment c in p.Comments)
            {
                App.NotifyColleagues(AppMessages.MSG_CLOSE_EDIT_COMMENT, c);
            }
            App.NotifyColleagues(AppMessages.MSG_CLOSE_VOTE, p);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_QUESTION, p);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_EDIT, p);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_COMMENT, p);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_ACCEPT, p);
            App.NotifyColleagues(AppMessages.MSG_CLOSE_DELETE, p);
        }

        //****************************************************************
        //****************************************************************
        //CONFIG
        //****************************************************************
        //****************************************************************
        private void ConfigTab(TabItem tab)
        {
            tabControl.Items.Add(tab);
            tab.MouseDown += (o, e) => {
                if (e.ChangedButton == MouseButton.Middle &&
                    e.ButtonState == MouseButtonState.Pressed)
                {
                    tabControl.Items.Remove(o);
                    (tab.Content as UserControlBase).Dispose();
                }
            };
            tab.PreviewKeyDown += (o, e) => {
                if (e.Key == Key.W && Keyboard.IsKeyDown(Key.LeftCtrl))
                {
                    tabControl.Items.Remove(o);
                    (tab.Content as UserControlBase).Dispose();
                }
            };
            // exécute la méthode Focus() de l'onglet pour lui donner le focus (càd l'activer)
            Dispatcher.InvokeAsync(() => tab.Focus());
        }
    }
}
