using HakunaMatata.Models.DataModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HakunaMatata.Models.ViewModels
{
    public class VM_Agent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string ContactNumber { get; set; }

        public string Email { get; set; }
        public string Facebook { get; set; }
        public string Zalo { get; set; }
        public string Avatar { get; set; }

        public int ActivePosts { get; set; }

        public int TotalPosts { get; set; }

        public string Role { get; set; }

        public bool IsActive { get; set; }

        public bool IsConfirmedNumber { get; set; }

        public List<RealEstate> Posts { get; set; }
    }
}
