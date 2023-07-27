using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace HakunaMatata.Models.DataModels
{
    public partial class AboutUs
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "This Field is required.")]
        public string Content { get; set; }
    }
}
