namespace InformationOfStudent.Models.Tables
{
    public class Student
    {
        public Guid StudentId { get; set; }

        public string Enrollmentnumber { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public Guid BranchId { get; set; }

        public string Gender { get; set; }

        public string MobileNumber { get; set; }

        public string Email { get; set; }

        public string ImagePath { get; set; }

        public DateTime ModifyOn { get; set; }

        public Branch Branch { get; set; }


    }
}
