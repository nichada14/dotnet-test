using System.ComponentModel.DataAnnotations;

namespace TestZen.Models
{
    public class User
    {
        public int id { get; set; } 

        [Required]
        public string? fullname { get; set; } 

        [Required]
        [EmailAddress]
        public required string email { get; set; } 

        [Phone]
        public string? phone { get; set; } 

        [Required]
        public string? address { get; set; } 

        [Required]
        public string? password { get; set; } 
    }
}
