using System;
using System.Collections.Generic;

namespace ExamAPI.Models
{
    public partial class User
    {
        public int SerialNo { get; set; }
        public string UserAccount { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
        public string UserRole { get; set; } = null!;
        public DateTime CreateDate { get; set; }
        public string Creator { get; set; } = null!;
        public DateTime ModifyDate { get; set; }
        public string Modifier { get; set; } = null!;
    }
}
