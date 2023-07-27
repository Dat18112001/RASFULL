using HakunaMatata.Data;
using HakunaMatata.Models.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HakunaMatata.Services
{
    public interface IPolicyServices
    {
        IEnumerable<Policy> GetListPolicies();
        Policy GetDetails(int id);
        void Create(Policy policy);
        void UpdatePolicy(Policy policy);
        void DeletePolicy(int id);
        bool IsExist(int id);
    }
    public class PolicyServices : IPolicyServices
    {
        private readonly HakunaMatataContext _context;
        public PolicyServices(HakunaMatataContext context)
        {
            _context = context;
        }
        public IEnumerable<Policy> GetListPolicies()
        {
            return _context.Policy.ToList();
        }

        public Policy GetDetails(int id)
        {
            return _context.Policy.Find(id);
        }

        public void Create(Policy policy)
        {
            try
            {
                _context.Policy.Add(policy);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdatePolicy(Policy policy)
        {
            try
            {
                var pl = _context.Policy.Find(policy.Id);
                if (pl != null)
                {
                    pl.PolicyContent = policy.PolicyContent;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeletePolicy(int id)
        {
            try
            {
                var a = _context.Policy.Find(id);
                if (a != null)
                {
                    _context.Policy.Remove(a);
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
            return _context.Policy.Any(p => p.Id == id);
        }


    }
}
