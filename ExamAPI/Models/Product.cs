using System;
using System.Collections.Generic;

namespace ExamAPI.Models
{
    public partial class Product
    {
        public Product()
        {
            Boms = new HashSet<Bom>();
        }

        public string ProductNo { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public decimal ProductPrice { get; set; }
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; } = null!;
        public DateTime ModifyDate { get; set; }
        public string Modifier { get; set; } = null!;

        public virtual ICollection<Bom> Boms { get; set; }
    }
}
