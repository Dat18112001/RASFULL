using System;
using System.Collections.Generic;

namespace HakunaMatata.Models.DataModels
{
    public partial class HistoryPayment
    {
        public int PaymentID { get; set; } // Primary key
        public int Coin { get; set; }
        public int AgentID { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
