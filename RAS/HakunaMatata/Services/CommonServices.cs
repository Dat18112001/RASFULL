using HakunaMatata.Data;
using HakunaMatata.Models.DataModels;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Commom
{
    public interface ICommonServices
    {
        Task<IEnumerable<District>> GetDistrictsByCity(int? cityId);
        int? GetDistrictByAddress(string address);
        int? GetWardByAddress(string address);
    }

    public class CommonServices : ICommonServices
    {
        private readonly HakunaMatataContext _context;
        public CommonServices(HakunaMatataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<District>> GetDistrictsByCity(int? cityId)
        {
            return await _context.District.Where(d => d.CityId == cityId).ToListAsync();
        }

        public int? GetWardByAddress(string address)
        {
            var wardList = _context.Ward.ToList();
            foreach (var ward in wardList)
            {
                if (address.Contains(ward.WardName)) return ward.Id;
            }
            return null;
        }
        public int? GetDistrictByAddress(string address)
        {
            var districtList = _context.District.ToList();
            foreach (var d in districtList)
            {
                if (address.Contains(d.DistrictName)) return d.Id;
            }
            return null;
        }
    }
}


