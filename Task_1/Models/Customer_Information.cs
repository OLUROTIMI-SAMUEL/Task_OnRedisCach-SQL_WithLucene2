using System.ComponentModel.DataAnnotations;

namespace Task_1.Models
{
    public class Customer_Information
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(35)]
        public string? Name { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string? Email_Address{ get; set; }

        //PURPOSELY DIDN'T INTRO REGEX FOR STRONGER "MODEL VALIDATIONS"

    }
}
