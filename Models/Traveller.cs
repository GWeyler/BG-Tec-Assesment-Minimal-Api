using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BG_Tec_Assesment_Minimal_Api.Utils;




namespace BG_Tec_Assesment_Minimal_Api.Models
{
    [Microsoft.EntityFrameworkCore.Index(nameof(DocumentNumberSHA), IsUnique = true)]
    public class Traveller
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "{0} contains invalid characters. Only alphanumeric characters are allowed.")]
        public string Forename { get; set; }
        [Required]
        [Column(TypeName = "varchar(255)")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "{0} contains invalid characters. Only alphanumeric characters are allowed.")]
        public string Surname { get; set; }
        [Required]
        [DateValidationAttribute]
        public DateOnly? Dob { get; set; }
        
        public List<Flight> Flights { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        [Column(TypeName = "varchar(60)")]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessage = "{0} contains invalid characters. Only alphanumeric characters are allowed.")]
        [StringLength(60, MinimumLength = 6,ErrorMessage ="The {0} must be atleast {2} characters long")]
        public string DocumentNumber { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string DocumentNumberSHA { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        //This functions is needed to prevent a new traveller with different details but same name from bing checked in a flight
        public bool SoftEquals(Traveller other)
        {
            if (other == null) return false;
            return string.Equals(Forename, other.Forename, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(Surname, other.Surname, StringComparison.OrdinalIgnoreCase) &&
                   Dob == other.Dob;
        }
    }
}
