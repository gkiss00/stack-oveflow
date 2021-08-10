using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1920_xyy
{
    public class Post : EntityBase<Model>
    {
        [Key]
        public int PostId { get; set; }
        public virtual User Author { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime TimeStamp { get; set; }
        public virtual Post AcceptedAnswer { get; set; }
        public virtual Post Parent { get; set; }
        public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
        [InverseProperty(nameof(Comment.Post))]
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        [InverseProperty(nameof(Vote.Post))]
        public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
        [InverseProperty(nameof(Parent))]
        public virtual ICollection<Post> Answears { get; set; } = new HashSet<Post>();

        public int NbVote { get
            {
                var sum = 0;
                var query = from v in App.Model.Votes
                            where v.Post.PostId == PostId
                            select v;
                List<Vote> l = new List<Vote>(query);
                foreach(Vote v in l)
                {
                    sum += v.UpDown;
                }
                return (sum);
            }
        }
        public bool IsAccepted { get
            {
                if (Parent != null && Parent.AcceptedAnswer != null && Parent.AcceptedAnswer.PostId == PostId)
                    return (true);
                return (false);
            } 
        }

        public bool CanBeVoted { get
            {
                if (App.CurrentUser != null && App.CurrentUser.UserId == Author.UserId)
                    return (false);
                return (true);
            }
        }
        /*
        public void Delete()
        {
            //supprime réponse acceptée
            AcceptedAnswer = null;
            // question et tout ce qui en découle
            foreach(Post r in Answears)
            {
                r.Delete();
            }
            // supprimer chaque tags
            foreach (Tag t in Tags)
                t.Posts.Remove(this);
            Tags.Clear();
            // supprimer chaque comments
            foreach(Comment c in Comments)
            {
                c.Author.Comments.Remove(c);
            }
            Model.Comments.RemoveRange(Comments);
            Comments.Clear();
            // supprimer chaque votes
            foreach (Vote v in Votes)
            {
                v.User.Votes.Remove(v);
            }
            Model.Votes.RemoveRange(Votes);
            Votes.Clear();
            // supprimer chaque réponses
            Model.Posts.RemoveRange(Answears);
            Answears.Clear();
            //supprime la question
            if (Parent == null)
                Model.Posts.Remove(this);
        }*/

        public void Delete()
        {
            Model.Comments.RemoveRange(Comments);
            Model.Votes.RemoveRange(Votes);
            AcceptedAnswer = null;
            Model.Posts.RemoveRange(Answears);
            foreach (Tag t in Tags)
                t.Posts.Remove(this);
            Tags.Clear();
            Model.SaveChanges();
            Model.Posts.Remove(this);
            Model.SaveChanges();
        }
    }
}
