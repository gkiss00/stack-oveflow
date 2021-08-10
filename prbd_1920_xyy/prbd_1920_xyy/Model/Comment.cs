using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1920_xyy
{
    public class Comment : EntityBase<Model>
    {
        [Key]
        public int CommentId { get; set; }
        public string Body { get; set; }
        public virtual User Author { get; set; }
        public virtual Post Post { get; set; }
        public DateTime TimeStamp { get; set; }

        public void Delete()
        {
            Author.Comments.Remove(this);
            Post.Comments.Remove(this);
            Author = null;
            Post = null;
            Model.Comments.Remove(this);
        }
    }
}
