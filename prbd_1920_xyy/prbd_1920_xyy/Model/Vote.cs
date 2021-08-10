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
    public class Vote : EntityBase<Model>
    {
        [Key]
        public int VoteId { get; set; }
        public virtual User User { get; set; }
        public virtual Post Post { get; set; }
        public int UpDown { get; set; }

        public void Delete()
        {
            User.Votes.Remove(this);
            Post.Votes.Remove(this);
            Model.Votes.Remove(this);
        }
    }
}
