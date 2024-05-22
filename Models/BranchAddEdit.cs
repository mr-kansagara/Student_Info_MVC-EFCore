using System.ComponentModel.DataAnnotations;

namespace InformationOfStudent.Models
{
    public class BranchAddEdit
    {
        public Guid? BranchID { get; set; }

        [Required(ErrorMessage ="Branch should not be empty")]
        public string BranchName { get; set; }

        public DateTime ModifyOn { get; set; }

        public DateTime? DeletedOn { get; set; }

    }
}
