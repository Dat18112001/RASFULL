using System;
using System.Collections.Generic;

namespace HakunaMatata.Models.DataModels
{
    public partial class Newspicture
    {
        public int Id { get; set; }
        public string PictureName { get; set; }
        public int NewsID { get; set; }
        public string URL { get; set; }
        public bool? IsActive { get; set; }
        public int NativeHeight { get; set; }
        public int NativeWidth { get; set; }

        public virtual News News { get; set; }
    }
}
