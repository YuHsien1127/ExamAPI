using System;
using System.Collections.Generic;

namespace ExamAPI.Models
{
    public partial class Material
    {
        public Material()
        {
            Boms = new HashSet<Bom>();
        }

        public string MaterialNo { get; set; } = null!;
        public string MaterialName { get; set; } = null!;
        public decimal MaterialCost { get; set; }
        public int CurrentStock { get; set; }
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; } = null!;
        public DateTime ModifyDate { get; set; }
        public string Modifier { get; set; } = null!;

        public virtual ICollection<Bom> Boms { get; set; }
    }
}
