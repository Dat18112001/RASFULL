namespace HakunaMatata.Models.DataModels
{
    public partial class Picture
    {
        public int Id { get; set; }
        public string PictureName { get; set; }
        public int NativeWidth { get; set; }
        public int NativeHeight { get; set; }
        public int? RealEstateId { get; set; }
        public string Url { get; set; }
        public bool IsActive { get; set; }

        public virtual RealEstate RealEstate { get; set; }
    }
}
