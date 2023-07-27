using HakunaMatata.Data;
using HakunaMatata.Models.DataModels;
using System.Collections.Generic;
using System.Linq;


namespace HakunaMatata.Services
{
    public interface IFAQServices
    {
        IEnumerable<Faq> GetListFAQs();
        Faq GetDetails(int id);
        void Create(Faq faq);
        void UpdateFAQ(Faq faq);
        void DeleteFAQ(int id);
        bool IsExist(int id);
    }

    public class FAQServices : IFAQServices
    {
        private readonly HakunaMatataContext _context;
        public FAQServices(HakunaMatataContext context)
        {
            _context = context;
        }


        public IEnumerable<Faq> GetListFAQs()
        {
            return _context.Faq.ToList();
        }

        public Faq GetDetails(int id)
        {
            return _context.Faq.Find(id);
        }

        public void Create(Faq faq)
        {
            try
            {
                _context.Faq.Add(faq);
                _context.SaveChanges();
            }
            catch
            {

                throw;
            }

        }
        public void UpdateFAQ(Faq faq)
        {
            try
            {
                var a = _context.Faq.Find(faq.Id);
                if (a != null)
                {
                    a.Question = faq.Question;
                    a.Answer = faq.Answer;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteFAQ(int id)
        {
            try
            {
                var a = _context.Faq.Find(id);
                if (a != null)
                {
                    _context.Faq.Remove(a);
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
            return _context.Faq.Any(f => f.Id == id);
        }


    }

}
