using System.ComponentModel.DataAnnotations;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class RolesModel
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string ? RoleName { get; set; }

        // Danh sách các quyền hạn của vai trò
        public List<string> ? Permissions { get; set; }
    }
}
