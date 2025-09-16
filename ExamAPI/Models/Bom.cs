using System;
using System.Collections.Generic;

namespace ExamAPI.Models
{
    public partial class Bom
    {
        public int SerialNo { get; set; }
        public string ProductNo { get; set; } = null!;
        public string MaterialNo { get; set; } = null!;
        public int MaterialUseQuantity { get; set; }
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; } = null!;
        public DateTime ModifyDate { get; set; }
        public string Modifier { get; set; } = null!;

        public virtual Material MaterialNoNavigation { get; set; } = null!;
        public virtual Product ProductNoNavigation { get; set; } = null!;
    }
}
