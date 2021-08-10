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
    public class User : EntityBase<Model>
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set;  }
        public string FullName { get; set;  }
        public string Email { get; set; }
        public Role Role { get; set; }
        [InverseProperty(nameof(Post.Author))]
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        [InverseProperty(nameof(Vote.User))]
        public virtual ICollection<Vote> Votes { get; set; } = new HashSet<Vote>();
        [InverseProperty(nameof(Comment.Author))]
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public void Delete()
        {
            foreach(Post p in Model.Posts)
            {
                if (p.Author.UserId == this.UserId)
                    p.Delete();
            }
            foreach (Comment c in Model.Comments)
            {
                if (c.Author.UserId == this.UserId)
                    c.Delete();
            }
            Model.Users.Remove(this);
        }

        public User GetUserByPseudo(string pseudo)
        {
            foreach(User u in Model.Users)
            {
                if (u.UserName.Equals(pseudo))
                {
                    return (u);
                }
            }
            return (null);
        }
    }
}
