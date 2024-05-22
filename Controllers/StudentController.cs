using InformationOfStudent.Database;
using InformationOfStudent.Models;
using InformationOfStudent.Models.Tables;
using InformationOfStudent.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationOfStudent.Controllers
{

    public class StudentController : Controller
    {

        private readonly IGenericServices<Student> _studentService;
        private readonly IGenericServices<Branch> _branchService;
        private readonly IWebHostEnvironment _webHost;
        //Constructor(services)
        public StudentController(IGenericServices<Student> studentService, IGenericServices<Branch> branchService, IWebHostEnvironment webHost)
        {
            _studentService = studentService;
            _branchService = branchService;
            _webHost = webHost;
        }


        //Get All Students
        [HttpGet]
        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 3, string searchQuery = "", string StudentCategory = "")
        {
            IQueryable<Student> query = _studentService.Query();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                switch (StudentCategory)
                {
                    case "EnrollmentNumber":
                        query = query.Where(s => s.Enrollmentnumber.Contains(searchQuery));
                        break;
                    case "Gender":
                        query = query.Where(s => s.Gender.Contains(searchQuery));
                        break;
                    case "Branch":
                        query = query.Where(s => s.Branch.BranchName.Contains(searchQuery));
                        break;
                    case "MobileNumber":
                        query = query.Where(s => s.MobileNumber.Contains(searchQuery));
                        break;
                    case "Email":
                        query = query.Where(s => s.Email.Contains(searchQuery));
                        break;
                    case "FirstName":
                        query = query.Where(s => s.FirstName.Contains(searchQuery));
                        break;
                    case "LastName":
                        query = query.Where(s => s.LastName.Contains(searchQuery));
                        break;
                }
            }
            int totalStudents = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalStudents / (double)pageSize);
            currentPage = Math.Max(1, Math.Min(currentPage, totalPages));

            List<Student> students = await query
               .Skip((currentPage - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();

            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = currentPage;
            ViewData["SearchQuery"] = searchQuery;
            ViewData["StudentCategory"] = StudentCategory;

            var branch = await _branchService.GetAllAsync();

            ViewBag.branch = branch;

            return View(students);
        }

        //Student Add Edit Get method
        [HttpGet]
        public async Task<IActionResult> StudentAddEdit(Guid id)
        {
            var branch = await _branchService.Query().Where(a => a.IsActive == "1").ToListAsync();
            //for add Student
            if (id != Guid.Empty)
            {
                var student = await _studentService.GetByIdAsync(id);
                if (student != null)
                {
                    var EditModel = new StudentAddEdit()
                    {
                        StudentId = student.StudentId,
                        Enrollmentnumber = student.Enrollmentnumber,
                        FirstName = student.FirstName,
                        MiddleName = student.MiddleName,
                        LastName = student.LastName,
                        BranchId = student.BranchId,
                        Gender = student.Gender,
                        MobileNumber = student.MobileNumber,
                        Email = student.Email,
                    };
                    ViewBag.Branch = branch;
                    return View(EditModel);
                }
            }
            ViewBag.Branch = branch;
            return View();
        }

        //Student Add Edit Post method
        [HttpPost]
        public async Task<IActionResult> StudentAddEdit(StudentAddEdit student, IFormFile ImagePath)
        {
            var branch = await _branchService.Query().Where(a => a.IsActive == "1").ToListAsync();
            if (student.StudentId == null)
            {
                if (ModelState.IsValid)
                {
                    if (await _studentService.IsStudentUniqueAsync((student.Enrollmentnumber).Trim()))
                    {

                        string uploadFolder = Path.Combine(_webHost.WebRootPath, "StudentImage");
                        if (!Directory.Exists(uploadFolder))
                        {
                            Directory.CreateDirectory(uploadFolder);
                        }
                        string FileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagePath.FileName);
                        String FileUploadPathInDataBase = Path.Combine("StudentImage", FileName);

                        string FileUploadPath = Path.Combine(uploadFolder, FileName);

                        using (FileStream stream = new FileStream(FileUploadPath, FileMode.Create))
                        {
                            await ImagePath.CopyToAsync(stream);
                        }

                        Student studentAdd = new Student()
                        {
                            StudentId = Guid.NewGuid(),
                            Enrollmentnumber = student.Enrollmentnumber,
                            FirstName = student.FirstName,
                            MiddleName = student.MiddleName,
                            LastName = student.LastName,
                            BranchId = student.BranchId,
                            MobileNumber = student.MobileNumber,
                            Gender = student.Gender,
                            Email = student.Email,
                            ImagePath = FileUploadPathInDataBase,
                            ModifyOn = DateTime.Now,
                        };
                        await _studentService.AddAsync(studentAdd);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        // EnrollmentNumber is not unique, add error message to ModelState
                        ModelState.AddModelError("Enrollmentnumber", "Enrollment number must be unique.");
                    }
                }

            }
            else
            {
                if (ModelState.IsValid)
                {
                    var studentEdit = await _studentService.GetByIdAsync(student.StudentId);
                    if (studentEdit != null)
                    {
                        studentEdit.Enrollmentnumber = student.Enrollmentnumber;
                        studentEdit.FirstName = student.FirstName;
                        studentEdit.MiddleName = student.MiddleName;
                        studentEdit.LastName = student.LastName;
                        studentEdit.BranchId = student.BranchId;
                        studentEdit.Gender = student.Gender;
                        studentEdit.MobileNumber = student.MobileNumber;
                        studentEdit.Email = student.Email;
                        studentEdit.ModifyOn = DateTime.Now;

                        // Update image if new image is provided
                        if (ImagePath != null)
                        {
                            string uploadFolder = Path.Combine(_webHost.WebRootPath, "StudentImage");
                            if (!Directory.Exists(uploadFolder))
                            {
                                Directory.CreateDirectory(uploadFolder);
                            }
                            string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagePath.FileName);
                            string fileUploadPathInDatabase = Path.Combine("StudentImage", fileName);
                            string fileUploadPath = Path.Combine(uploadFolder, fileName);

                            using (FileStream stream = new FileStream(fileUploadPath, FileMode.Create))
                            {
                                await ImagePath.CopyToAsync(stream);
                            }

                            // Delete old image if exists
                            if (!string.IsNullOrEmpty(studentEdit.ImagePath))
                            {
                                var oldImagePath = Path.Combine(_webHost.WebRootPath, studentEdit.ImagePath);
                                if (System.IO.File.Exists(oldImagePath))
                                {
                                    System.IO.File.Delete(oldImagePath);
                                }
                            }
                            studentEdit.ImagePath = fileUploadPathInDatabase;
                        }
                        try
                        {
                            await _studentService.UpdateAsync(studentEdit);
                            return RedirectToAction("Index");
                        }
                        catch (Exception)
                        {
                            return RedirectToAction("ErrorPage");
                        }
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            ViewBag.Branch = branch;
            return View(student);
        }

        //Delete Student
        public async Task<IActionResult> StudentDelete(Guid id)
        {
            try
            {
                var student = await _studentService.GetByIdAsync(id);
                if (student != null)
                {
                    // Delete related photo from wwwroot folder
                    if (!string.IsNullOrEmpty(student.ImagePath))
                    {
                        var imagePath = Path.Combine(_webHost.WebRootPath, student.ImagePath);
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    await _studentService.DeleteAsync(student);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Error");
            }

            return RedirectToAction("Index");
        }



        [HttpGet]
        public IActionResult ErrorPage() => View();

    }
}






/*      [HttpGet]
      public async Task<IActionResult> StudentAdd()
      {
          var branch = await databaseContext.Branch.Where(a => a.IsActive == "1").ToListAsync();
          ViewBag.Branch = branch;
          return View();
      }*/

/*[HttpGet]
        public async Task<IActionResult> StudentEdit(Guid id)
        {
            var branch = await databaseContext.Branch.ToListAsync();
            var student = await databaseContext.Student.FirstOrDefaultAsync(temp => temp.StudentId == id);
            if (student != null)
            {
                var EditModel = new StudentAddEdit()
                {
                    StudentId = student.StudentId,
                    Enrollmentnumber = student.Enrollmentnumber,
                    FirstName = student.FirstName,
                    MiddleName = student.MiddleName,
                    LastName = student.LastName,
                    BranchId = student.BranchId,
                    Gender = student.Gender,
                    MobileNumber = student.MobileNumber,
                    Email = student.Email,
                };
                ViewBag.Branch = branch;
                return View(EditModel);
            }

            ViewBag.Branch = branch;
            return View();
        }*/

/* [HttpPost]
 public async Task<IActionResult> StudentAdd(StudentAddEdit addStudent, IFormFile ImagePath)
 {

     if (ModelState.IsValid)
     {
         if (await IsEnrollmentNumberUnique(addStudent.Enrollmentnumber))
         {

             string uploadFolder = Path.Combine(webHost.WebRootPath, "StudentImage");
             if (!Directory.Exists(uploadFolder))
             {
                 Directory.CreateDirectory(uploadFolder);
             }
             string FileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagePath.FileName);
             String FileUploadPathInDataBase = Path.Combine("StudentImage", FileName);

             string FileUploadPath = Path.Combine(uploadFolder, FileName);

             using (FileStream stream = new FileStream(FileUploadPath, FileMode.Create))
             {
                 await ImagePath.CopyToAsync(stream);
             }

             Student student = new Student()
             {
                 StudentId = Guid.NewGuid(),
                 Enrollmentnumber = addStudent.Enrollmentnumber,
                 FirstName = addStudent.FirstName,
                 MiddleName = addStudent.MiddleName,
                 LastName = addStudent.LastName,
                 BranchId = addStudent.BranchId,
                 MobileNumber = addStudent.MobileNumber,
                 Gender = addStudent.Gender,
                 Email = addStudent.Email,
                 ImagePath = FileUploadPathInDataBase,
                 ModifyOn = DateTime.Now,
             };
             await databaseContext.Student.AddAsync(student);
             await databaseContext.SaveChangesAsync();
             return RedirectToAction("Index");

         }
         else
         {
             // EnrollmentNumber is not unique, add error message to ModelState
             ModelState.AddModelError("Enrollmentnumber", "Enrollment number must be unique.");
         }

     }
     var branch = await databaseContext.Branch.Where(a => a.IsActive == "1").ToListAsync();
     ViewBag.Branch = branch;
     return View(addStudent);
 }
*/

/* [HttpPost]
 public async Task<IActionResult> StudentEdit(StudentAddEdit editStudent, IFormFile ImagePath)
 {
     if (ModelState.IsValid)
     {
         var student = await databaseContext.Student.FindAsync(editStudent.StudentId);
         if (student != null)
         {
             student.StudentId = editStudent.StudentId;
             student.Enrollmentnumber = editStudent.Enrollmentnumber;
             student.FirstName = editStudent.FirstName;
             student.MiddleName = editStudent.MiddleName;
             student.LastName = editStudent.LastName;
             student.BranchId = editStudent.BranchId;
             student.Gender = editStudent.Gender;
             student.MobileNumber = editStudent.MobileNumber;
             student.Email = editStudent.Email;
             student.ModifyOn = DateTime.Now;

             // Update image if new image is provided
             if (ImagePath != null)
             {
                 string uploadFolder = Path.Combine(webHost.WebRootPath, "StudentImage");
                 if (!Directory.Exists(uploadFolder))
                 {
                     Directory.CreateDirectory(uploadFolder);
                 }
                 string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagePath.FileName);
                 string fileUploadPathInDatabase = Path.Combine("StudentImage", fileName);
                 string fileUploadPath = Path.Combine(uploadFolder, fileName);

                 using (FileStream stream = new FileStream(fileUploadPath, FileMode.Create))
                 {
                     await ImagePath.CopyToAsync(stream);
                 }

                 // Delete old image if exists
                 if (!string.IsNullOrEmpty(student.ImagePath))
                 {
                     var oldImagePath = Path.Combine(webHost.WebRootPath, student.ImagePath);
                     if (System.IO.File.Exists(oldImagePath))
                     {
                         System.IO.File.Delete(oldImagePath);
                     }
                 }
                 student.ImagePath = fileUploadPathInDatabase;
             }
             try
             {
                 await databaseContext.SaveChangesAsync();
                 return RedirectToAction("Index");
             }
             catch (Exception ex)
             {
                 return RedirectToAction("ErrorPage");
             }
         }
         else
         {
             return NotFound();
         }

     }
     var branch = await databaseContext.Branch.ToListAsync();
     ViewBag.Branch = branch;
     return View(editStudent);
 }*/