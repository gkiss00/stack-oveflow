using PRBD_Framework;
using System;
using System.Collections;
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
    public partial class IndexView : UserControlBase
    {
        private string SorterString { get; set; }

        private ObservableCollection<Post> posts;
        public ObservableCollection<Post> Posts { get => posts; set => SetProperty(ref posts, value); }

        private ObservableCollection<Tag> tags;
        public ObservableCollection<Tag> Tags { get => tags; set => SetProperty(ref tags, value); }

        private string filter;
        public string Filter
        {
            get => filter;
            set => SetProperty<string>(ref filter, value, ApplyFilterAction);
        }

        private Tag TagFilter;
        public ICommand ApplyFilter { get; set; }
        public ICommand QuestionDetails { get; set; }
        public ICommand AddTagToFilter { get; set; }
        //****************************************************
        //****************************************************
        //CONSTRUCTOR
        //****************************************************
        //****************************************************
        public IndexView()
        {
            DataContext = this;

            Posts = GetAllQuestions();

            Tags = GetAllTags();

            ApplyFilter = new RelayCommand(ApplyFilterAction);

            AddTagToFilter = new RelayCommand<Tag>(t =>
            {
                CheckTag(t);
                ApplyFilterAction();
            });

            QuestionDetails = new RelayCommand<Post>(p =>
            {
                App.NotifyColleagues(AppMessages.MSG_OPEN_QUESTION, p);
            });

            App.Register<Post>(this, AppMessages.MSG_POST_CHANGED, p =>
            {
                Posts = GetAllQuestions();
                ApplyFilterAction();
            });

            App.Register(this, AppMessages.MSG_TAG_DELETED, () =>
            {
                Posts = GetAllQuestions();
                Tags = GetAllTags();
                TagFilter = null;
                ApplyFilterAction();
            });

            App.Register(this, AppMessages.MSG_TAG_ADDED, () =>
            {
                Posts = GetAllQuestions();
                Tags = GetAllTags();
                TagFilter = null;
                ApplyFilterAction();
            });

            App.Register<Post>(this, AppMessages.MSG_ANSWEAR_ADDED, pp => {
                Posts = GetAllQuestions();
                ApplyFilterAction();
            });

            App.Register<Post>(this, AppMessages.MSG_ANSWEAR_ACCEPTED, pp => {
                Posts = GetAllQuestions();
                ApplyFilterAction();
            });

            App.Register<Post>(this, AppMessages.MSG_COMMENT_ADDED, ppp =>
            {
                Posts = GetAllQuestions();
                ApplyFilterAction();
            });

            App.Register<Post>(this, AppMessages.MSG_VOTE_CHANGED, ppp =>
            {
                Posts = GetAllQuestions();
                ApplyFilterAction();
            });

            App.Register(this, AppMessages.MSG_COMMENT_CHANGED, CommentChangedAction);

            App.Register<string>(this, AppMessages.MSG_SORT_BY_TAG, str =>
            {
                Tag tt = GetTag(str);
                if (tt != null)
                    TagFilter = tt;
                Posts = GetAllQuestions();
                ApplyFilterAction();
            });

            InitializeComponent();
        }

        

        //****************************************************
        //****************************************************
        //GETTER
        //****************************************************
        //****************************************************

        private Tag GetTag(string str)
        {
            Tag t = null;
            var query = from tt in App.Model.Tags
                        where tt.TagName == str
                        select tt;
            List<Tag> tmp = new List<Tag>(query);
            if (tmp.Count() > 0)
                t = tmp[0];
            return (t);
        }
        private ObservableCollection<Post> GetAllQuestions()
        {
            var query = from m in App.Model.Posts
                        where m.Parent == null
                        orderby m.TimeStamp descending
                        select m;
            return (new ObservableCollection<Post>(query));
        }

        private ObservableCollection<Tag> GetAllTags()
        {
            var query = from t in App.Model.Tags
                        select t;
            return (new ObservableCollection<Tag>(query));
        }

        private DateTime GetMostRecentDateTime(Post p)
        {
            DateTime t = p.TimeStamp;
            foreach(Post pt in p.Answears)
            {
                if (t.CompareTo(pt.TimeStamp) < 0)
                    t = pt.TimeStamp;
                foreach (Comment c in pt.Comments)
                {
                    if (t.CompareTo(c.TimeStamp) < 0)
                        t = c.TimeStamp;
                }
            }
            foreach (Comment c in p.Comments)
            {
                if (t.CompareTo(c.TimeStamp) < 0)
                    t = c.TimeStamp;
            }
            return (t);
        }
        //****************************************************
        //****************************************************
        //ACTIONS
        //****************************************************
        //****************************************************

        private void CommentChangedAction()
        {
            Posts = GetAllQuestions();
            ApplyFilterAction();
        }

        private void ApplyFilterAction()
        {
            SortBy();
            ApplyFIlterAct();
            ApplyTag();
        }
        //****************************************************
        //****************************************************
        //FILTER && SORTER
        //****************************************************
        //****************************************************
        private void ApplyFIlterAct()
        {
            if (Filter != null)
            {
                List<Post> tmp = new List<Post>();
                foreach (Post p in Posts)
                {
                    if (p.Title.Contains(Filter) || p.Body.Contains(Filter))
                    {
                        tmp.Add(p);
                    }
                }
                Posts = new ObservableCollection<Post>(tmp);
            }
        }
        //SELECTER LISTENER
        private void Sorter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var choice = (ComboBoxItem)sorter.SelectedValue;
            SorterString = (string)choice.Name;
            ApplyFilterAction();
        }
        //HUB
        private void SortBy()
        {
            Posts = GetAllQuestions();
            if (SorterString.Equals("newest"))
            {
                SortByNewest();
            }
            else if (SorterString.Equals("unansweared"))
            {
                SortByUnansweared();
            }
            else if (SorterString.Equals("active"))
            {
                SortByActive();
            }
            else if (SorterString.Equals("vote"))
            {
                SortByVote();
            }
            ApplyFIlterAct();
        }
        //NEWEST
        private void SortByNewest()
        {
            List<Post> tmp = new List<Post>(Posts);
            tmp.Sort((p1, p2) => p2.TimeStamp.CompareTo(p1.TimeStamp));
            Posts = new ObservableCollection<Post>(tmp);
        }
        //UNANSWEARED
        private void SortByUnansweared()
        {
            List<Post> tmp = new List<Post>(Posts);
            List<Post> tmp2 = new List<Post>();
            foreach (Post pt in tmp)
            {
                if (pt.AcceptedAnswer == null)
                    tmp2.Add(pt);
            }
            tmp2.Sort((p1, p2) => p2.TimeStamp.CompareTo(p1.TimeStamp));
            Posts = new ObservableCollection<Post>(tmp2);
        }
        //ACTIVE
        private void SortByActive()
        {
            List<Post> tmp = new List<Post>(Posts);
            List<DateTime> ldt = new List<DateTime>();
            foreach(Post p in tmp)
                ldt.Add(GetMostRecentDateTime(p));
            for (int i = 0; i < ldt.Count() - 1; ++i)
            {
                if(ldt[i].CompareTo(ldt[i + 1]) < 0)
                {
                    DateTime tx = ldt[i];
                    ldt[i] = ldt[i + 1];
                    ldt[i + 1] = tx;

                    Post tp = tmp[i];
                    tmp[i] = tmp[i + 1];
                    tmp[i + 1] = tp;
                    i = - 1;
                }
            }
            Posts = new ObservableCollection<Post>(tmp);
        }
        //VOTE
        private void SortByVote()
        {
            List<Post> tmp = new List<Post>(Posts);
            List<int> votes = new List<int>();
            foreach (Post pt in tmp)
            {
                var query = from v in App.Model.Votes
                            where v.Post.PostId == pt.PostId
                            select v;
                List<Vote> lv = new List<Vote>(query);
                int vote = 0;
                foreach(Vote v in lv)
                {
                    vote += v.UpDown;
                }
                votes.Add(vote);
            }
            int it;
            Post ptt;
            for (int i = 0; i < votes.Count() - 1; ++i)
            {
                if (votes[i] < votes[i + 1])
                {
                    it = votes[i + 1];
                    votes[i + 1] = votes[i];
                    votes[i] = it;

                    ptt = tmp[i + 1];
                    tmp[i + 1] = tmp[i];
                    tmp[i] = ptt;
                    i = -1;
                }
            }
            Posts = new ObservableCollection<Post>(tmp);
        }
        //BY TAG
        private void ApplyTag()
        {
            if (TagFilter != null)
            {
                List<Post> tmp = new List<Post>(Posts);
                List<Post> tmp2 = new List<Post>();
                foreach (Post pt in tmp)
                {
                    if (pt.Tags.Contains(TagFilter))
                        tmp2.Add(pt);
                }
                Posts = new ObservableCollection<Post>(tmp2);
            }
        }
        //****************************************************
        //****************************************************
        //UTILS
        //****************************************************
        //****************************************************
        private void CheckTag(Tag t)
        {
            if (TagFilter == null || !TagFilter.Equals(t))
                TagFilter = t;
            else
                TagFilter = null;
        }
    }
}
