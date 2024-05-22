using System.ComponentModel.DataAnnotations;

namespace InformationOfStudent.Models
{
    public class StudentAddEdit
    {
        public Guid? StudentId { get; set; }

        [Required(ErrorMessage = "*Enrollment number should be unique and must have 12 digits")]
        [MaxLength(12)]
        [MinLength(12)]
        public string Enrollmentnumber { get; set;}

        [Required(ErrorMessage = "*Enter First name")]
        public string FirstName{ get; set;}

        [Required(ErrorMessage = "*Enter Middle name")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "*Enter Last name")]
        public string LastName { get; set;}
        
        [Required(ErrorMessage ="*Select Atleast one Branch")]        
        public Guid BranchId { get; set; }

        [Required(ErrorMessage ="*Must Select Gender")]
        public string Gender {  get; set; }
        
        [Required]
        [Phone(ErrorMessage = "*Enter valid mobile number")]
        [MinLength(10)]
        [MaxLength(10)]
        public string MobileNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; }


        public IFormFile ImagePath { get; set; }


    }
}
