using System;
using System.ComponentModel.DataAnnotations;

namespace HakunaMatata.Models.ViewModels
{
    public class VM_WishList
    {
        public int Id { get; set; }
        public int IdRealEstate { get; set; }

        public string Title { get; set; }
        public int RealEstateId { get; set; }
        public string Url { get; set; }
        public string Address { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }
}
