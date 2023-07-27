using HakunaMatata.Data;
using HakunaMatata.Models.DataModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HakunaMatata.Models.ViewModels;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace HakunaMatata.Services
{
    public interface ILevelServices
    {
        Task<List<ChartReportSummaryModel>> GenerateOrderReport(int year, int month);
        IEnumerable<Level> GetListLevels();
        IEnumerable<HistoryPayment> GetHistoryPayments();

        IEnumerable<PaymentHistoryDetailModel> GetHistoryPaymentsByDate(DateTime? startDate, DateTime? endDate,
            int userId = 0);

        IEnumerable<News> GetListNews();
        Level GetDetails(int id);
        Agent GetCoin(int id);
        Agent GetPackage(int id);

        News GetNewsDetails(int id);
        Newspicture GetNewsPictureDetails(int id);
        void Create(Level level);
        void CreateNews(News news);
        void CreateNewsPicture(Newspicture newspic);
        void CreateHistoryPayment(Agent agent);
        void UpdateLevel(Level level);
        void UpdateCoinAgent(Agent agent);
        void UpdateCoinPackageAgent(Agent agent);
        void UpdateNews(News news);
        void UpdateNewsPicture(Newspicture newspic);
        void DeleteLevel(int id);
        void DeleteNews(int id);
        bool IsExist(int id);
        bool IsExistNews(int id);
    }

    public class LevelServices : ILevelServices
    {
        private readonly HakunaMatataContext _context;

        public LevelServices(HakunaMatataContext context)
        {
            _context = context;
        }


        public async Task<List<ChartReportSummaryModel>> GenerateOrderReport(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var orders = await _context.HistoryPayment
                .Where(s => s.PaymentDate >= startDate && s.PaymentDate < endDate)
                .ToListAsync();

            var reportData = new List<ChartReportSummaryModel>();

            for (var day = 1; day <= DateTime.DaysInMonth(year, month); day++)
            {
                var ordersInDay = orders.Where(s => s.PaymentDate.Day == day).ToList();
                var totalAmount = (double)ordersInDay.Sum(s => s.Coin);

                var chartModel = new ChartReportSummaryModel
                {
                    ValueX = day,
                    ValueY = totalAmount
                };

                reportData.Add(chartModel);
            }

            return reportData;
        }

        public IEnumerable<Level> GetListLevels()
        {
            return _context.Level.ToList();
        }

        public IEnumerable<HistoryPayment> GetHistoryPayments()
        {
            return _context.HistoryPayment.ToList();
        }

        public IEnumerable<PaymentHistoryDetailModel> GetHistoryPaymentsByDate(DateTime? startDate, DateTime? endDate,
            int userId = 0)
        {
            var agents = _context.Agent.ToList();
            if (userId > 0)
            {
                var histories = _context.HistoryPayment
                    .Where(h => h.AgentID == userId && ((startDate == null || endDate == null) ||
                                                        (h.PaymentDate.Date >= startDate.Value.Date &&
                                                         h.PaymentDate.Date <= endDate.Value.Date)))
                    .OrderByDescending(s => s.PaymentDate)
                    .ToList();
                
                var dataJoin = from history in histories
                    join agent in agents on history.AgentID equals agent.Id
                    select new
                    {
                        history.PaymentID,
                        history.Coin,
                        history.AgentID,
                        EmailAgent = agent.Email,
                        history.PaymentDate,
                    };

                return dataJoin.Adapt<List<PaymentHistoryDetailModel>>();
            }
            else
            {
                var histories =  _context.HistoryPayment
                    .Where(h => (startDate == null || endDate == null) ||
                                (h.PaymentDate.Date >= startDate.Value.Date && h.PaymentDate.Date <= endDate.Value.Date))
                    .OrderByDescending(s => s.PaymentDate)
                    .ToList();
            
                var dataJoin = from history in histories
                    join agent in agents on history.AgentID equals agent.Id
                    select new
                    {
                        history.PaymentID,
                        history.Coin,
                        history.AgentID,
                        EmailAgent = agent.Email,
                        history.PaymentDate,
                    };

                return dataJoin.Adapt<List<PaymentHistoryDetailModel>>();
            }
        }


        public IEnumerable<News> GetListNews()
        {
            return _context.News.ToList();
        }

        public Level GetDetails(int id)
        {
            return _context.Level.Find(id);
        }

        public Agent GetCoin(int id)
        {
            return _context.Agent.Find(id);
        }

        public Agent GetPackage(int id)
        {
            return _context.Agent.Find(id);
        }

        public News GetNewsDetails(int id)
        {
            return _context.News.Find(id);
        }

        public Newspicture GetNewsPictureDetails(int id)
        {
            return _context.Newspicture.Find(id);
        }

        public void Create(Level level)
        {
            try
            {
                _context.Level.Add(level);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void CreateNews(News news)
        {
            try
            {
                news.AgentId = 5; // Set AgentId to 5
                _context.News.Add(news);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void CreateNewsPicture(Newspicture newspic)
        {
            try
            {
                newspic.IsActive = true;
                newspic.NativeWidth = 0;
                newspic.NativeHeight = 0;
                newspic.URL = newspic.PictureName;
                _context.Newspicture.Add(newspic);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public void UpdateLevel(Level level)
        {
            try
            {
                var lv = _context.Level.Find(level.Id);
                if (lv != null)
                {
                    lv.LevelName = level.LevelName;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateCoinAgent(Agent agent)
        {
            try
            {
                agent.Coin ??= 0;
                var ag = _context.Agent.Find(agent.Id);
                if (ag != null)
                {
                    ag.Coin ??= 0;
                    ag.Coin += agent.Coin / 1000;
                    _context.Agent.Update(ag);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateCoinPackageAgent(Agent agent)
        {
            try
            {
                var ag = _context.Agent.Find(agent.Id);
                if (ag != null)
                {
                    ag.Package = agent.Package;

                    // Deduct coins based on the package duration
                    if (ag.Package == 3)
                        ag.Coin -= 1000;
                    else if (ag.Package == 6)
                        ag.Coin -= 2000;
                    else if (ag.Package == 12)
                        ag.Coin -= 3000;

                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }


        public void CreateHistoryPayment(Agent agent)
        {
            try
            {
                var historyPayment = new HistoryPayment
                {
                    Coin = agent.Coin ?? 0, // Assign 0 if agent.Coin is null
                    PaymentDate = DateTime.Now,
                    AgentID = agent.Id,
                };

                _context.HistoryPayment.Add(historyPayment);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }


        public void UpdateNews(News news)
        {
            try
            {
                var n = _context.News.Find(news.Id);
                if (n != null)
                {
                    n.Title = news.Title;
                    n.NewsBody = news.NewsBody;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateNewsPicture(Newspicture newspic)
        {
            try
            {
                var n = _context.Newspicture.Find(newspic.Id);
                if (n != null)
                {
                    n.PictureName = newspic.PictureName;
                    n.NewsID = newspic.NewsID;
                    n.URL = newspic.URL;
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteLevel(int id)
        {
            try
            {
                var a = _context.Level.Find(id);
                if (a != null)
                {
                    _context.Level.Remove(a);
                    _context.SaveChanges();
                }
            }
            catch
            {
                throw;
            }
        }

        public void DeleteNews(int id)
        {
            try
            {
                var a = _context.News.Find(id);
                if (a != null)
                {
                    _context.News.Remove(a);
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
            return _context.Level.Any(f => f.Id == id);
        }

        public bool IsExistNews(int id)
        {
            return _context.News.Any(f => f.Id == id);
        }
    }
}