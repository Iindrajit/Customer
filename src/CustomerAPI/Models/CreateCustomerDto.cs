using CustomerAPI.CustomAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerAPI.Models
{
    public class CreateCustomerDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "First name can have only 100 characters.")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Last name can have only 100 characters.")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [CustomDateRange(ErrorMessage ="Date of birth is out of Range.")]
        public DateTime DateOfBirth { get; set; }
    }
}
