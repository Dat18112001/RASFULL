using System;

namespace HakunaMatata.Models.ViewModels
{
    public class PaymentHistoryDetailModel
    {
        public int PaymentID { get; set; } // Primary key
        public int Coin { get; set; }
        public int AgentID { get; set; }
        public string EmailAgent { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}