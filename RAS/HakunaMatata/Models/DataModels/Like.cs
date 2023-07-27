using System;
using System.Collections.Generic;
using HakunaMatata.Models.DataModels;

namespace HakunaMatata.Models.DataModels
{
    public partial class Like
    {
        public int Id { get; set; }
        public int ForumId { get; set; }
        public int AgentId { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual Agent Agent { get; set; }
        public virtual Forum Forum { get; set; }
    }
}
