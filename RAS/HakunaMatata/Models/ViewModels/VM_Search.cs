using System.Collections.Generic;

namespace HakunaMatata.Models.ViewModels
{
    public class VM_Search
    {
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

        /// <summary>
        /// khu vuc tim kiem, co the la cac quan, phuong, duong
        /// </summary>
        public string SearchString { get; set; }
    }

    public class VM_Search_Result
    {
        public int Id { get; set; }

        public string Street { get; set; }

        public decimal Price { get; set; }

        public int Acreage { get; set; }

        /// <summary>
        /// real estate type id
        /// </summary>
        public int? Type { get; set; }

        public string PostTime { get; set; }

        public string ImageUrl { get; set; }

        public string AgentName { get; set; }
    }
    public class VM_Search_Result_Container
    {
        public VM_Search SearchObject { get; set; }

        public IEnumerable<VM_Search_Result> ResultList { get; set; }
    }
}
