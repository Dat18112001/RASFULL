using System;
using System.Collections.Generic;

namespace HakunaMatata.Models.DataModels
{
    public partial class Comment
    {
        public string Id { get; set; }
        public int ForumId { get; set; }
        public int AgentId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Agent Agent { get; set; }
        public virtual Forum Forum { get; set; }
    }
}
