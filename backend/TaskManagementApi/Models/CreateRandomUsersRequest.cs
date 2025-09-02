using System.ComponentModel.DataAnnotations;

namespace TaskManagementApi.Models
{
    public class CreateRandomUsersRequest
    {
        [Required]
        [Range(1, 10000)]
        public int Amount { get; set; }
        
        [Required]
        public string UserNameMask { get; set; } = string.Empty;
    }
}

