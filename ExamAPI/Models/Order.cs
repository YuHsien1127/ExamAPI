using System;
using System.Collections.Generic;

namespace ExamAPI.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public string OrderNo { get; set; } = null!;
        public string? OrderSubject { get; set; }
        public string OrderApplicant { get; set; } = null!;
        public string Status { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; } = null!;
        public DateTime ModifyDate { get; set; }
        public string Modifier { get; set; } = null!;

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
