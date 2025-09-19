using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;

namespace ExamAPI.Dto.Request
{
    public class UserRequest
    {
        public string UserAccount { get; set; } = null!;
        public string UserPassword { get; set; } = null!;
        public string UserName { get; set; } = null!;
        /// <summary>
        /// 使用者在系統中的角色，例如 admin、customer、staff
        /// </summary>
        public string UserRole { get; set; } = null!;
        public string UserEmail { get; set; } = null!;
    }
}
