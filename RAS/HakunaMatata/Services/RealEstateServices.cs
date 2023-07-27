using System;
using System.Linq;
using HakunaMatata.Data;
using HakunaMatata.Helpers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using HakunaMatata.Models.DataModels;
using HakunaMatata.Models.ViewModels;


namespace HakunaMatata.Services
{
    public interface IRealEstateServices
    {

        List<VM_RealEstate> GetList();
        List<VM_WishList> GetWishList(string agentId);

        List<VM_News> GetListNews();
        VM_News GetNewsById(int id);
        List<string> GetPicturesByRealEstateId(int realEstateId);
        List<VM_Location> GetAllActiveLocation();

        /// <summary>
        /// use for paging, search in admin page
        /// </summary>
        /// <returns></returns>
        IQueryable<RealEstateViewModel> GetRealEstates(string searchKey);

        /// <summary>
        /// User's all post
        /// </summary>
        /// <returns></returns>
        List<RealEstateViewModel> GetUserAllPosts(int? userId);

        /// <summary>
        /// Get all waiting confirm post
        /// </summary>
        /// <returns></returns>
        List<RealEstateViewModel> GetCustomerConFirmList();
        List<RealEstateViewModel> CustomerExpireList();

        /// <summary>
        /// use for client, filter
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        IQueryable<Result> Filter(Condition condition);
        Task<VM_RealEstateDetails> GetRealEstateDetails(int? id);
        List<VM_RealEstateDetails> GetRealEstateDetailHome();


        int AddNewRealEstate(RealEstate realEstate);
        int AddNewWishList(Wishlist wishlist);
        int DeleteWishlist(int id);
        bool AddRealEstateDetails(RealEstateDetail details);
        bool AddMapForRealEstate(Map map);
        int AddCompleteRealEstate(VM_RealEstateDetails details, int agentId);
        bool UpdateRealEstate(VM_RealEstateDetails details);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <returns> < 1 - loi, 1 - thanh cong, 2 - user khong hop le </returns>
        int DeleteRealEstate(int id, int userId);
        bool DisableRealEstate(int id);
        bool BookedRealEstate(int id);
        bool UnbookRealEstate(int id);
        /// <summary>
        /// activate post when create success
        /// </summary>
        /// <param name="id"></param>
        void ActiveRealEstate(int id);

        /// <summary>
        /// Admin confirm about customer's post
        /// </summary>
        /// <param name="id"></param>
        /// <param name="confirmType">1:confirmed, 2: denied</param>
        /// <returns></returns>
        bool ConfirmRealEsate(int id, int confirmType);
        IEnumerable<RealEstateType> GetRealEstateTypeList();
        IEnumerable<City> GetCityList();
        IEnumerable<District> GetDistrictList();
        bool IsExistRealEstate(int id);

        Tuple<int?, int?, int> GetLocation(string address);
        List<Result> GetRecommendList(int? id);
        
    }

    public class RealEstateServices : IRealEstateServices
    {
        private readonly HakunaMatataContext _context;
        public RealEstateServices(HakunaMatataContext context)
        {
            _context = context;
        }

        public List<VM_RealEstate> GetList()
        {
            var list = new List<VM_RealEstate>();

            var details = _context.RealEstateDetail
                .Include(detail => detail.RealEstate)
                    .ThenInclude(detail => detail.Agent)
                .Include(detail => detail.RealEstate)
                    .ThenInclude(detail => detail.ReaEstateType)
                .Include(detail => detail.RealEstate)
                    .ThenInclude(detail => detail.Picture) // Include the Pictures table
                .ToList();

            foreach (var item in details)
            {
                var vm_rt = new VM_RealEstate()
                {
                    Id = item.RealEstate.Id,
                    Title = item.Title,
                    ExprireTime = item.RealEstate.ExprireTime,
                    RealEstateTypeId = item.RealEstate.ReaEstateType.Id,
                    Price = item.Price,
                    AgentName = item.RealEstate.Agent.AgentName,
                    IsActive = item.RealEstate.IsActive,
                    UrlImage = item.RealEstate.Picture.FirstOrDefault()?.Url,
                    AvatarAgent = item.RealEstate.Agent.Avatar,
                };

                list.Add(vm_rt);
            }

            // Sort the list by ExprireTime in descending order
            list = list.OrderByDescending(rt => rt.ExprireTime).ToList();

            return list;
        }

        public List<VM_News> GetListNews()
        {
            var newsList = _context.News
                .Select(n => new VM_News
                {
                    Id = n.Id,
                    Title = n.Title,
                    NewsBody = n.NewsBody,
                    UrlImage = _context.Newspicture
                        .Where(p => p.NewsID == n.Id && p.IsActive == true)
                        .Select(p => p.URL)
                        .FirstOrDefault() ?? "2aac9f91-2b80-4e47-abf8-4a462f0f461a-skyhome3.jpg"
                })
                .ToList();

            return newsList;
        }

        public VM_News GetNewsById(int id)
        {
            var news = _context.News
                .Where(n => n.Id == id)
                .Select(n => new VM_News
                {
                    Id = n.Id,
                    Title = n.Title,
                    NewsBody = n.NewsBody,
                    UrlImage = _context.Newspicture
                        .Where(p => p.NewsID == n.Id && (p.IsActive == true || p.IsActive == null))
                        .Select(p => p.URL)
                        .FirstOrDefault() ?? "2aac9f91-2b80-4e47-abf8-4a462f0f461a-skyhome3.jpg"
                })
                .FirstOrDefault();

            return news;
        }














        public List<VM_WishList> GetWishList(string agentId)
        {
            // Chuyển đổi agentId sang kiểu int (nếu cần)
            int parsedAgentId = int.Parse(agentId);

            var wishlist = _context.Wishlist
                .Where(w => w.AgentId == parsedAgentId) // Lọc theo AgentId
                .Join(_context.RealEstate, w => w.RealEstateId, re => re.Id, (w, re) => new { Wishlist = w, RealEstate = re })
                .Join(_context.RealEstateDetail, x => x.RealEstate.Id, red => red.RealEstateId, (x, red) => new { x.Wishlist, x.RealEstate, RealEstateDetail = red })
                .Join(_context.Map, x => x.RealEstate.Id, m => m.RealEstateId, (x, m) => new { x.Wishlist, x.RealEstate, x.RealEstateDetail, Map = m })
                .Join(_context.Picture, x => x.RealEstate.Id, p => p.RealEstateId, (x, p) => new { x.Wishlist, x.RealEstate, x.RealEstateDetail, x.Map, Picture = p })
                .GroupBy(x => new { x.Wishlist.Id, x.Wishlist.RealEstateId, x.RealEstateDetail.Title, x.RealEstateDetail.Price, x.Map.Address })
                .Select(g => new VM_WishList
                {
                    Id = g.Key.Id,
                    RealEstateId = g.Key.RealEstateId ?? 3, // Retrieve RealEstateId from Wishlist
                    Title = g.Key.Title,
                    Price = g.Key.Price,
                    Address = g.Key.Address,
                })
                .ToList();

            return wishlist;
        }



        public List<string> GetPicturesByRealEstateId(int realEstateId)
        {
            var pictures = _context.Picture
                .Where(p => p.RealEstateId == realEstateId)
                .Select(p => p.Url)
                .ToList();

            return pictures;
        }











        /// <summary>
        /// Paging in AdminArea/RealEsate/Index2, order by post time
        /// </summary>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public IQueryable<RealEstateViewModel> GetRealEstates(string searchKey)
        {
            var source = _context.RealEstate
                            .Include(r => r.RealEstateDetail)
                            .Include(r => r.Agent)
                            .Include(r => r.Map)
                            .Where(r => r.Map.Address.Contains(searchKey)
                                     || r.Agent.AgentName.Contains(searchKey)
                                     || r.ReaEstateType.RealEstateTypeName.Contains(searchKey)
                                     || r.RealEstateDetail.Price.ToString().Contains(searchKey)
                                  )
                            .OrderByDescending(r => r.IsActive)
                            .ThenByDescending(r => r.IsAvaiable)
                            .ThenByDescending(r => r.ExprireTime)
                            .AsQueryable();

            //if (!string.IsNullOrEmpty(searchKey))
            //{
            //    source = source.Where(s => s.Map.Address.Contains(searchKey)
            //                             || s.Agent.AgentName.Contains(searchKey)
            //                             || s.ReaEstateType.RealEstateTypeName.Contains(searchKey)
            //                             || EF.Functions.Contains(s.PostTime.ToString("dd/MM/yyyy"), searchKey)
            //                             || EF.Functions.Contains(s.ExprireTime.Value.ToString("dd/MM/yyyy"), searchKey)
            //                             || EF.Functions.Contains(s.RealEstateDetail.Price.ToString(), searchKey)
            //                          );
            //}

            IQueryable<RealEstateViewModel> results = (from item in source
                                                       select new RealEstateViewModel
                                                       {
                                                           Id = item.Id,
                                                           Street = item.Map.Address,
                                                           Price = Helper.VNCurrencyFormat(item.RealEstateDetail.Price.ToString()),
                                                           PostDate = item.PostTime.ToString("dd/MM/yyyy"),
                                                           ExpireTime = item.ExprireTime == null ? string.Empty : item.ExprireTime.Value.ToString("dd/MM/yyyy"),
                                                           Agent = item.Agent.AgentName,
                                                           Type = item.ReaEstateType.RealEstateTypeName,
                                                           Status = Helper.GetStatus(item)
                                                       });

            return results;
        }

        /// <summary>
        /// Get all user's posts
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<RealEstateViewModel> GetUserAllPosts(int? userId)
        {
            var results = new List<RealEstateViewModel>();
            var source = _context.RealEstate
                           .Include(r => r.RealEstateDetail)
                           .Include(r => r.ReaEstateType)
                           .Include(r => r.Agent)
                           .Include(r => r.Map)
                           .Where(r => r.AgentId == userId)
                           .OrderByDescending(r => r.PostTime)
                           .ThenByDescending(r => r.IsActive)
                           .ToList();

            foreach (var item in source)
            {
                var viewModelItem = new RealEstateViewModel
                {
                    Id = item.Id,
                    AgentId = item.Agent.Id,
                    Street = item.Map.Address,
                    Price = Helper.VNCurrencyFormat(item.RealEstateDetail.Price.ToString()),
                    PostDate = item.PostTime.ToString("dd/MM/yyyy"),
                    BeginTime = item.BeginTime.ToString("dd/MM/yyyy"),
                    ExpireTime = item.ExprireTime == null ? string.Empty : item.ExprireTime.Value.ToString("dd/MM/yyyy"),
                    Type = item.ReaEstateType.RealEstateTypeName,
                    Status = Helper.GetStatus(item)
                };
                results.Add(viewModelItem);
            }

            return results;
        }

        public List<RealEstateViewModel> GetCustomerConFirmList()
        {
            var results = new List<RealEstateViewModel>();
            var source = _context.RealEstate
                           .Include(r => r.RealEstateDetail)
                           .Include(r => r.ReaEstateType)
                           .Include(r => r.Agent)
                           .Include(r => r.Map)
                           .Where(r => !r.IsConfirm)
                           .OrderByDescending(r => r.PostTime)
                           .ToList();

            foreach (var item in source)
            {
                var viewModelItem = new RealEstateViewModel
                {
                    Id = item.Id,
                    Street = item.Map.Address,
                    Price = Helper.VNCurrencyFormat(item.RealEstateDetail.Price.ToString()),
                    Agent = item.Agent.AgentName,
                    PostDate = item.PostTime.ToString("dd/MM/yyyy"),
                    BeginTime = item.BeginTime.ToString("dd/MM/yyyy"),
                    ExpireTime = item.ExprireTime == null ? string.Empty : item.ExprireTime.Value.ToString("dd/MM/yyyy"),
                    Type = item.ReaEstateType.RealEstateTypeName,
                    Status = Helper.GetStatus(item)
                };
                results.Add(viewModelItem);
            }

            return results;
        }
        public List<RealEstateViewModel> CustomerExpireList()
        {
            var results = new List<RealEstateViewModel>();
            var source = _context.RealEstate
                           .Include(r => r.RealEstateDetail)
                           .Include(r => r.ReaEstateType)
                           .Include(r => r.Agent)
                           .Include(r => r.Map)
                           .Where(r => r.IsActive && r.ExprireTime < DateTime.Now)
                           .OrderByDescending(r => r.PostTime)
                           .ToList();

            foreach (var item in source)
            {
                var viewModelItem = new RealEstateViewModel
                {
                    Id = item.Id,
                    Street = item.Map.Address,
                    Price = Helper.VNCurrencyFormat(item.RealEstateDetail.Price.ToString()),
                    Agent = item.Agent.AgentName,
                    PostDate = item.PostTime.ToString("dd/MM/yyyy"),
                    BeginTime = item.BeginTime.ToString("dd/MM/yyyy"),
                    ExpireTime = item.ExprireTime == null ? string.Empty : item.ExprireTime.Value.ToString("dd/MM/yyyy"),
                    Type = item.ReaEstateType.RealEstateTypeName,
                    Status = Helper.GetStatus(item)
                };
                results.Add(viewModelItem);
            }

            return results;
        }

        public async Task<VM_RealEstateDetails> GetRealEstateDetails(int? id)
        {
            var info = await _context.RealEstate.Where(r => r.Id == id)
                           .Include(r => r.RealEstateDetail)
                           .Include(r => r.ReaEstateType)
                           .Include(r => r.Map)
                           .Include(r => r.Agent)
                           .SingleOrDefaultAsync();
            if (info != null)
            {
                if (info.Map == null)
                {
                    info.Map = new Map();
                }
                VM_RealEstateDetails result = Helper.MappingFromRealEstate(info);
                return result;
            }

            return null;
        }

        public List <VM_RealEstateDetails> GetRealEstateDetailHome()
        {
/*            var info = new List<RealEstate>();
            info = _context.RealEstateDetail.Include(r => r.RealEstateDetail)
                           .Include(r => r.ReaEstateType)
                           .Include(r => r.Map)
                           .Include(r => r.Agent)
            if (info != null)
            {
                if (info.Map == null)
                {
                    info.Map = new Map();
                }
                VM_RealEstateDetails result = Helper.MappingFromRealEstate(info);
                return result;
            }
*/
            return null;
        }

        #region comment out
        //public VM_RealEstateDetails GetById(int? id)
        //{
        //    var details = _context.RealEstateDetail
        //        .Where(x => x.RealEstateId == id)
        //        .Include(detail => detail.RealEstate)
        //            .ThenInclude(detail => detail.Agent)
        //        .Include(detail => detail.RealEstate)
        //            .ThenInclude(detail => detail.ReaEstateType)
        //        .SingleOrDefault();

        //    var map = _context.Map.Where(m => m.RealEstateId == id).SingleOrDefault();

        //    var images = _context.Picture
        //        .Where(pic => pic.RealEstateId == id && pic.IsActive)
        //        .ToList();

        //    var imgUrls = new List<string>();
        //    foreach (var img in images)
        //    {
        //        var tempUrl = GetLinkImage(img);
        //        imgUrls.Add(tempUrl);
        //    }

        //    if (details != null)
        //    {
        //        if (map == null)
        //        {
        //            map = new Map();
        //        }

        //        var vm_rt_details = new VM_RealEstateDetails()
        //        {
        //            Id = details.RealEstate.Id,
        //            Title = details.Title,
        //            Address = map.Address ?? string.Empty,
        //            Price = details.Price,
        //            Acreage = details.Acreage,
        //            PostTime = details.RealEstate.PostTime.ToString("dd/MM/yyyy"),
        //            LastUpdate = details.RealEstate.LastUpdate?.ToString("dd/MM/yyyy"),
        //            ExprireTime = details.RealEstate.ExprireTime?.ToString("dd/MM/yyyy"),
        //            RoomNumber = details.RoomNumber,
        //            Description = details.Description,
        //            AgentName = details.RealEstate.Agent.AgentName,
        //            HasPrivateWc = details.HasPrivateWc,
        //            HasMezzanine = details.HasMezzanine,
        //            AllowCook = details.AllowCook,
        //            FreeTime = details.FreeTime,
        //            SecurityCamera = details.SecurityCamera,
        //            WaterPrice = details.WaterPrice == null ? 0 : details.WaterPrice,
        //            ElectronicPrice = details.ElectronicPrice == null ? 0 : details.ElectronicPrice,
        //            WifiPrice = details.WifiPrice,
        //            Latitude = map.Latitude,
        //            Longtitude = map.Longtitude,
        //            RealEstateTypeId = details.RealEstate.ReaEstateType.Id,
        //            IsActive = details.RealEstate.IsActive,
        //            ImageUrls = imgUrls
        //        };
        //        return vm_rt_details;
        //    }

        //    return new VM_RealEstateDetails();
        //}
        #endregion

        public int AddNewRealEstate(RealEstate realEstate)
        {
            try
            {
                _context.RealEstate.Add(realEstate);
                _context.SaveChanges();
                return realEstate.Id;
            }
            catch
            {
                return -1;
            }
        }

        public int AddNewWishList(Wishlist wishlist)
        {
            try
            {
                _context.Wishlist.Add(wishlist);
                _context.SaveChanges();
                return wishlist.Id;
            }
            catch
            {
                return -1;
            }
        }


        public bool AddRealEstateDetails(RealEstateDetail details)
        {
            try
            {
                _context.RealEstateDetail.Add(details);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddMapForRealEstate(Map map)
        {
            try
            {
                var location = GetLocation(map.Address);
                map.WardId = location.Item1;
                map.DistrictId = location.Item2;
                map.CityId = location.Item3;

                _context.Map.Add(map);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary>

        /// </summary>
        /// <param name="details"></param>
        /// <param name="agentId"></param>
        /// <returns></returns> 
        public int AddCompleteRealEstate(VM_RealEstateDetails details, int agentId)
        {
            var realEstate = new RealEstate()
            {
                PostTime = DateTime.Now,
                LastUpdate = DateTime.Now,
                BeginTime = Convert.ToDateTime(details.BeginTime),
                ExprireTime = Convert.ToDateTime(details.ExprireTime),
                RealEstateTypeId = details.RealEstateTypeId,
                AgentId = agentId,
                ContactNumber = details.ContactNumber,
                IsActive = false,
                IsConfirm = agentId == 1,
                IsAvaiable = true,
                ConfirmStatus = agentId == 1 ? 1 : 0
            };
            var realEstateId = AddNewRealEstate(realEstate);

            //tao real estate thanh cong
            if (realEstateId != -1)
            {
                //add details
                var rtDetails = new RealEstateDetail()
                {
                    RealEstateId = realEstateId,
                    Title = details.Title,
                    Price = details.Price,
                    Acreage = details.Acreage,
                    RoomNumber = details.RoomNumber,
                    Description = details.Description,
                    HasPrivateWc = details.HasPrivateWc,
                    HasMezzanine = details.HasMezzanine,
                    AllowCook = details.AllowCook,
                    FreeTime = details.FreeTime,
                    SecurityCamera = details.SecurityCamera,
                    WaterPrice = details.IsFreeWater ? 0 : Convert.ToInt32(details.WaterPrice),
                    ElectronicPrice = details.IsFreeElectronic ? 0 : Convert.ToInt32(details.ElectronicPrice),
                    WifiPrice = details.IsFreeWifi ? 0 : details.WifiPrice
                };

                var isSuccessAddDetails = AddRealEstateDetails(rtDetails);

                //add map
                var map = new Map()
                {
                    Address = details.Address,
                    Latitude = details.Latitude,
                    Longtitude = details.Longtitude,
                    RealEstateId = realEstateId
                };
                var isSuccessAddMap = AddMapForRealEstate(map);

                //neu add thong tin chi tiet va map thanh cong thi active
                if (isSuccessAddDetails && isSuccessAddMap)
                {
                    ActiveRealEstate(realEstateId);
                    return realEstateId;
                }
                else
                {
                    return -1;
                }
            }

            return realEstateId;
        }
        public bool UpdateRealEstate(VM_RealEstateDetails details)
        {

            var rt = _context.RealEstate.Find(details.Id);
            if (rt != null)
            {
                var rt_detail = _context.RealEstateDetail.FirstOrDefault(d => d.RealEstateId == rt.Id);

                if (rt_detail != null)
                {
                    //it's update time
                    rt.LastUpdate = DateTime.Now;
                    rt.BeginTime = DateTime.Parse(details.BeginTime);
                    rt.ExprireTime = DateTime.Parse(details.ExprireTime);
                    rt.ContactNumber = details.ContactNumber;
                    rt_detail.Title = details.Title;
                    rt_detail.Price = details.Price;
                    rt_detail.Acreage = details.Acreage;
                    rt_detail.RoomNumber = details.RoomNumber;
                    rt_detail.Description = details.Description;
                    rt_detail.HasPrivateWc = details.HasPrivateWc;
                    rt_detail.HasMezzanine = details.HasMezzanine;
                    rt_detail.AllowCook = details.AllowCook;
                    rt_detail.FreeTime = details.FreeTime;
                    rt_detail.SecurityCamera = details.SecurityCamera;
                    rt_detail.WaterPrice = Convert.ToInt32(details.WaterPrice);
                    rt_detail.ElectronicPrice = Convert.ToInt32(details.ElectronicPrice);
                    rt_detail.WifiPrice = details.WifiPrice;

                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }

            else return false;
        }

        public int DeleteRealEstate(int id, int userId)
        {
            try
            {
                var realEstate = _context.RealEstate.Find(id);
                var user = _context.Agent.Find(userId);

                if (realEstate != null && user != null)
                {
                    if (Convert.ToInt32(realEstate.AgentId) == user.Id || user.LevelId == 1)
                    {
                        _context.RealEstate.Remove(realEstate);
                        _context.SaveChanges();
                        return 1;   //thanh cong
                    }
                    else return 2;  //user khong hop le
                }
                return 0;   //not found
            }
            catch (Exception)
            {
                return -1;  //loi he thong
            }

        }

        public int DeleteWishlist(int id)
        {
            try
            {
                var wishlist = _context.Wishlist.Find(id);

                if (wishlist != null)
                {
                    _context.Wishlist.Remove(wishlist);
                    _context.SaveChanges();
                    return 1; // Success
                }

                return 0; // Not found
            }
            catch (Exception)
            {
                return -1; // System error
            }
        }


        public bool DisableRealEstate(int id)
        {
            var realEstate = _context.RealEstate.Find(id);
            if (realEstate != null && realEstate.IsActive)
            {
                realEstate.IsActive = false;
                realEstate.LastUpdate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool BookedRealEstate(int id)
        {
            var realEstate = _context.RealEstate.Find(id);
            if (realEstate != null && realEstate.IsAvaiable)
            {
                realEstate.IsAvaiable = false;
                realEstate.LastUpdate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UnbookRealEstate(int id)
        {
            var realEstate = _context.RealEstate.Find(id);
            if (realEstate != null && !realEstate.IsAvaiable)
            {
                realEstate.IsAvaiable = true; // Set the property back to available
                realEstate.LastUpdate = DateTime.Now;
                _context.SaveChanges();
                return true;
            }
            return false;
        }


        public void ActiveRealEstate(int id)
        {
            var realEstate = _context.RealEstate.Find(id);
            if (realEstate != null && !realEstate.IsActive)
            {
                realEstate.IsActive = true;
                _context.SaveChanges();

            }
        }

        public bool IsExistRealEstate(int id)
        {
            return _context.RealEstate.Any(r => r.Id == id);
        }

        public IEnumerable<RealEstateType> GetRealEstateTypeList()
        {
            return _context.RealEstateType.ToList();
        }

        public IEnumerable<City> GetCityList()
        {
            return _context.City.ToList();
        }

        public IEnumerable<District> GetDistrictList()
        {
            return _context.District.ToList();
        }

        /// <summary>
        /// Tim kiem phuong, quan, thanh pho tu dia chi
        /// </summary>
        /// <param name="address"></param>
        /// <returns>id cua phuong, quan va thanh pho</returns>
        public Tuple<int?, int?, int> GetLocation(string address)
        {
            int cityId = 1;
            int? districtId = null;
            int? wardId = null;


            var cityList = _context.City.ToList();

            foreach (var city in cityList)
            {
                if (address.Contains(city.CityName))
                {
                    cityId = city.Id;
                    break;
                }
            }

            var districtList = _context.District.Where(d => d.CityId == cityId).ToList();
            if (districtList.Count > 0)
            {
                foreach (var district in districtList)
                {
                    if (address.Contains(district.DistrictName))
                    {
                        districtId = district.Id;
                        break;
                    }
                }
                var wardList = _context.Ward.Where(w => w.DistrictId == districtId).ToList();
                if (wardList.Count > 0)
                {
                    foreach (var ward in wardList)
                    {
                        if (address.Contains(ward.WardName))
                        {
                            wardId = ward.Id;
                            break;
                        }
                    }
                }
            }
            var result = Tuple.Create(wardId, districtId, cityId);
            return result;
        }

        /*
         ------------Client services-------------------------------
         */

        public IQueryable<Result> Filter(Condition condition)
        {
            try
            {
                var source = _context.RealEstate.Where(r => r.IsActive && r.ConfirmStatus == 1)
                                .Include(r => r.RealEstateDetail)
                                .Include(r => r.Agent)
                                .Include(r => r.Map)
                                .Include(r => r.Picture)
                                .OrderByDescending(r => r.IsAvaiable)
                                .ThenByDescending(r => r.PostTime)
                                .ThenByDescending(r => r.ExprireTime)
                                .AsQueryable();

                if (source != null && condition != null)
                {
                    if (condition.Type > 0)
                        source = source.Where(s => s.RealEstateTypeId == condition.Type);
                    if (condition.City > 0)
                        source = source.Where(x => x.Map.CityId == condition.City);
                    if (condition.District > 0)
                        source = source.Where(x => x.Map.DistrictId == condition.District);

                    var priceRange = Helper.GetPriceRange(condition.PriceRange);
                    var minPrice = priceRange[0];
                    var maxPrice = priceRange[1];
                    source = source.Where(x =>
                        x.RealEstateDetail.Price >= minPrice && x.RealEstateDetail.Price <= maxPrice);

                    var acreageRange = Helper.GetAcreageRange(condition.AcreageRange);
                    var minAcreage = acreageRange[0];
                    var maxAcreage = acreageRange[1];
                    source = source.Where(x =>
                       x.RealEstateDetail.Acreage >= minAcreage && x.RealEstateDetail.Acreage <= maxAcreage);

                    if (!string.IsNullOrEmpty(condition.SearchString.Trim()))
                    {
                        if (DateTime.TryParse(condition.SearchString, out DateTime searchDate))
                        {
                            source = source.Where(x => x.PostTime < searchDate && x.ExprireTime > searchDate);
                        }
                        else
                        {
                            source = source.Where(x => x.Map.Address.Contains(condition.SearchString)
                                                     || x.RealEstateDetail.Price.ToString().Contains(condition.SearchString));
                        }
                    }
                }

                IQueryable<Result> results = (from item in source
                                              select new Result
                                              {
                                                  Id = item.Id,
                                                  Street = Helper.GetStreet(item.Map.Address),
                                                  Price = item.RealEstateDetail.Price,
                                                  Acreage = item.RealEstateDetail.Acreage,
                                                  Type = item.RealEstateTypeId,
                                                  PostTime = item.PostTime.ToString("dd/MM/yyyy"),
                                                  ImageUrl = Helper.GetRealEstateAvatar(item.Picture.FirstOrDefault()),
                                                  AgentName = item.Agent.AgentName,
                                                  ContactNumber = item.ContactNumber,
                                                  AgentAvatar = item.Agent.Avatar
                                              });

                return results;

            }
            catch
            {
                return null;
            }
        }

        public bool ConfirmRealEsate(int id, int confirmType)
        {
            try
            {
                if (id <= 0 || confirmType <= 0) return false;

                var post = _context.RealEstate.Find(id);
                if (post == null) return false;

                post.IsConfirm = true;
                post.ConfirmStatus = confirmType;
                _context.SaveChanges();

                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// Get recommend real estate list by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Result> GetRecommendList(int? id)
        {
            var realEstate = _context.RealEstate.Where(r => r.Id == id)
                                .Include(r => r.RealEstateDetail)
                                .Include(r => r.Map)
                                .Include(r => r.Agent)
                                .Include(r => r.Map).SingleOrDefault();

            if (realEstate == null)
            {
                return null;
            }
            else
            {
                var source = _context.RealEstate.Include(r => r.RealEstateDetail)
                                                .Include(r => r.Map)
                                                .Include(r => r.Picture)
                                                .Include(r => r.Agent)
                                                .Include(r => r.ReaEstateType)
                                                .Where(r => r.Id != realEstate.Id
                                                && r.RealEstateTypeId == realEstate.RealEstateTypeId
                                                && r.IsActive && r.ConfirmStatus == 1 && r.ExprireTime > DateTime.Now && r.IsAvaiable
                                                && (r.Map.WardId == realEstate.Map.WardId
                                                || r.Map.DistrictId == realEstate.Map.DistrictId))
                                                .OrderByDescending(r => r.PostTime)
                                                .Take(4);

                var result = (from item in source
                              select new Result
                              {
                                  Id = item.Id,
                                  Street = Helper.GetStreet(item.Map.Address),
                                  Price = item.RealEstateDetail.Price,
                                  Acreage = item.RealEstateDetail.Acreage,
                                  Type = item.RealEstateTypeId,
                                  PostTime = item.PostTime.ToString("dd/MM/yyyy"),
                                  ImageUrl = Helper.GetRealEstateAvatar(item.Picture.FirstOrDefault()),
                                  AgentName = item.Agent.AgentName,
                                  ContactNumber = item.ContactNumber
                              }).ToList();
                return result;
            }
        }

        public List<VM_Location> GetAllActiveLocation()
        {
            var source = _context.RealEstate.Where(r => r.IsActive && r.IsAvaiable && r.ExprireTime > DateTime.Now)
                .Include(r => r.RealEstateDetail).Include(r => r.Map).ToList();

            var result = (from item in source
                          select new VM_Location
                          {
                              Id = item.Id,
                              Address = Helper.GetStreet(item.Map.Address),
                              Price = item.RealEstateDetail.Price,
                              Latitude = item.Map.Latitude,
                              Longtitude = item.Map.Longtitude
                          }).ToList();

            return result;
        }

        List<VM_RealEstateDetails> IRealEstateServices.GetRealEstateDetailHome()
        {
            throw new NotImplementedException();
        }
    }

}
