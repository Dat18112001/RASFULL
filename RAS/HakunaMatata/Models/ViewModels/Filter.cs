using System.Collections.Generic;

namespace HakunaMatata.Models.ViewModels
{
    public class Filter
    {
        public Condition Condition { get; set; }

        public List<Result> Results { get; set; }
    }

    public class Condition
    {
        /// <summary>
        /// khu vuc tim kiem, co the la cac quan, phuong, duong
        /// </summary>
        public string SearchString { get; set; }
        /// <summary>
        /// chon loai hinh phong tro
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// chon thanh phog
        /// </summary>
        public int City { get; set; }

        /// <summary>
        /// id cua quan
        /// </summary>
        public int District { get; set; }

        public int PriceRange { get; set; }

        public int AcreageRange { get; set; }


    }

    public class Result
    {
        public int Id { get; set; }

        public string Street { get; set; }

        public decimal Price { get; set; }

        public int Acreage { get; set; }

        /// <summary>
        /// loai phong
        /// </summary>
        public int? Type { get; set; }

        public string PostTime { get; set; }

        public string ImageUrl { get; set; }

        public string AgentName { get; set; }
        public string ContactNumber { get; set; }
        public string AgentAvatar { get; set; }
    }
}
