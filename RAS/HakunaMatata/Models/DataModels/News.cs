using System;
using System.Collections.Generic;

namespace HakunaMatata.Models.DataModels
{
    public partial class News
    {
        public News()
        {
            Newspicture = new HashSet<Newspicture>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string NewsBody { get; set; }
        public int AgentId { get; set; }

        public virtual Agent Agent { get; set; }
        public virtual ICollection<Newspicture> Newspicture { get; set; }
    }
}
