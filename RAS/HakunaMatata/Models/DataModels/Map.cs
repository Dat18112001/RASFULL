using System;
using System.Collections.Generic;

namespace HakunaMatata.Models.DataModels
{
    public partial class Map
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longtitude { get; set; }
        public int? WardId { get; set; }
        public int? DistrictId { get; set; }
        public int? CityId { get; set; }
        public int? RealEstateId { get; set; }

        public virtual City City { get; set; }
        public virtual District District { get; set; }
        public virtual RealEstate RealEstate { get; set; }
        public virtual Ward Ward { get; set; }
    }
}
