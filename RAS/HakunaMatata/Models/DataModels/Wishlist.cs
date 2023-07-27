using System;
using System.Collections.Generic;
using HakunaMatata.Models.DataModels;

namespace HakunaMatata.Models.DataModels
{
    public partial class Wishlist
    {
        public int Id { get; set; }
        public int? AgentId { get; set; }
        public int? RealEstateId { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual Agent Agent { get; set; }
        public virtual RealEstate RealEstate { get; set; }
    }
}
