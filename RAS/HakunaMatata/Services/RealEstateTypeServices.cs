using HakunaMatata.Data;
using HakunaMatata.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace HakunaMatata.Services
{
    public interface IRealEstateTypeServices
    {
        IEnumerable<RealEstateType> GetList();
        RealEstateType GetDetails(int id);
        void Create(RealEstateType type);
        void UpdateType(RealEstateType type);
        void DeleteType(int id);
        bool IsExist(int id);
    }
    public class RealEstateTypeServices : IRealEstateTypeServices
    {
        private readonly HakunaMatataContext _context;
        public RealEstateTypeServices(HakunaMatataContext context)
        {
            _context = context;
        }
        public IEnumerable<RealEstateType> GetList()
        {
            return _context.RealEstateType.ToList();
        }
        public RealEstateType GetDetails(int id)
        {
            return _context.RealEstateType.Find(id);
        }

        public void Create(RealEstateType type)
        {
            try
            {
                _context.RealEstateType.Add(type);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateType(RealEstateType type)
        {
            try
            {
                var typ = _context.RealEstateType.Find(type.Id);
                if (typ != null)
                {
                    typ.RealEstateTypeName = type.RealEstateTypeName;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteType(int id)
        {
            try
            {
                var type = _context.RealEstateType.Find(id);
                if (type != null)
                {
                    _context.RealEstateType.Remove(type);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }
        public bool IsExist(int id)
        {
            return _context.RealEstateType.Any(type => type.Id == id);
        }
    }
}
