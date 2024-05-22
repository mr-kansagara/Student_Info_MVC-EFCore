using Microsoft.CodeAnalysis.Operations;
using Microsoft.Extensions.Hosting;

namespace InformationOfStudent.Models.Tables
{
    public class Branch
    {
        public Guid BranchID { get; set; }

        public string BranchName { get; set; }

        public string IsActive { get; set; }

        public DateTime ModifyOn { get; set; }

        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<Student> Student { get; set; }
        public Branch()
        {
            Student = new List<Student>();
        }



    }
}
