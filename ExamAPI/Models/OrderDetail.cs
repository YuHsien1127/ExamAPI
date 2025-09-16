using System;
using System.Collections.Generic;

namespace ExamAPI.Models
{
    public partial class OrderDetail
    {
        public int SerialNo { get; set; }
        public string OrderNo { get; set; } = null!;
        public string ProductNo { get; set; } = null!;
        public int Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; } = null!;
        public DateTime ModifyDate { get; set; }
        public string Modifier { get; set; } = null!;

        public virtual Order OrderNoNavigation { get; set; } = null!;
    }
}
