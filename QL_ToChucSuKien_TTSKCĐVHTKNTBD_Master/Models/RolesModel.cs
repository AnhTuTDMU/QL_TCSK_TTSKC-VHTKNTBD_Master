using System.ComponentModel.DataAnnotations;

namespace QL_ToChucSuKien_TTSKCĐVHTKNTBD_Master.Models
{
    public class RolesModel
    {
        [Key]
        public int RoleId { get; set; }

        [Required]
        public string RoleName { get; set; } = string.Empty;

        public List<UsersModel> Users { get; set; } = new List<UsersModel>();
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
