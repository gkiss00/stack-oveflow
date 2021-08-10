using PRBD_Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1920_xyy
{
    public class Tag : EntityBase<Model>
    {
        [Key]
        public int TagId { get; set; }
        public string TagName { get; set; }
        public virtual ICollection<Post> Posts { get; set; } = new HashSet<Post>();

        public int NbPosts { get
            {
                return (Posts.Count());
            } }
        public void Delete()
        {
            foreach (Post p in Posts)
            {
                p.Tags.Remove(this);
            }
            Posts.Clear();
            Model.Tags.Remove(this);
        }
    }
}
