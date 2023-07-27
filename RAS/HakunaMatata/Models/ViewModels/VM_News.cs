using System;
using System.ComponentModel.DataAnnotations;

namespace HakunaMatata.Models.ViewModels
{
    public class VM_RealEstate
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string UrlImage { get; set; }

        [DisplayFormat(DataFormatString ="MM/dd/yyyy")]
        public DateTime PostTime { get; set; }

        [DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime? ExprireTime { get; set; }
        public int RealEstateTypeId { get; set; }
        public decimal Price { get; set; }
        public string AgentName { get; set; }
        public bool IsActive { get; set; }
        public string AvatarAgent { get; set; }
    }
}
