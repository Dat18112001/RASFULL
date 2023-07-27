using System;
using System.Collections.Generic;
using HakunaMatata.Models.DataModels;

namespace HakunaMatata.Models.DataModels
{
    public partial class Forum
    {
        public Forum()
        {
            Comment = new HashSet<Comment>();
            Like = new HashSet<Like>();
        }

        public int Id { get; set; }
        public int AgentId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual Agent Agent { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Like> Like { get; set; }
    }
}
