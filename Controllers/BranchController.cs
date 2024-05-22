using InformationOfStudent.Database;
using InformationOfStudent.Models;
using InformationOfStudent.Models.Tables;
using InformationOfStudent.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InformationOfStudent.Controllers
{
    public class BranchController : Controller
    {
        private readonly IGenericServices<Branch> _genericServices;

        public BranchController(IGenericServices<Branch> genericServices) {
            _genericServices = genericServices;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int currentPage = 1, int pageSize = 5, string searchCategory = "")
        {
            IQueryable<Branch> query = _genericServices.Query();

            switch (searchCategory)
            {
                case "All":
                    query = query;
                    break;
                case "Active":
                    query = query.Where(b => b.IsActive == "1");
                    break;
                case "Deactive":    
                    query = query.Where(b => b.IsActive == "0");
                    break;
            }
            int totalBranches = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalBranches / (double)pageSize);
            currentPage = Math.Max(1, Math.Min(currentPage, totalPages));

            List<Branch> branches = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize).OrderBy(b => b.ModifyOn)
                .ToListAsync();

            ViewData["TotalPages"] = totalPages;
            ViewData["CurrentPage"] = currentPage;
            ViewData["ItemsPerPage"] = pageSize;    
            ViewData["searchCategory"] = searchCategory;

            return View(branches);
        }

        [HttpGet]
        public async Task<IActionResult> BranchAddEdit(Guid id)
        {
            //AddViewAPage
            if (id != Guid.Empty)
            {
                var branch = await _genericServices.GetByIdAsync(id);
                if (branch != null)
                {
                    var editModel = new BranchAddEdit()
                    {
                        BranchID = branch.BranchID,
                        BranchName = branch.BranchName,

                    };
                    return View(editModel);
                }
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BranchAddEdit(BranchAddEdit branch)
        {
            // Add Branch 
            if (branch.BranchID == null)
            {
                if (ModelState.IsValid)
                {
                    if (await _genericServices.IsBranchUniqueAsync((branch.BranchName).Trim()))
                    {
                        try
                        {
                            Branch addBranch = new Branch()
                            {
                                BranchID = Guid.NewGuid(),
                                BranchName = branch.BranchName,
                                ModifyOn = DateTime.Now,
                                IsActive = "1"
                            };
                            await _genericServices.AddAsync(addBranch);
                            return RedirectToAction("Index");
                        }
                        catch (Exception)
                        {
                            return RedirectToAction("ErrorPage");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("BranchName", "Branch already exists If you want to add Branch so, kindly reactive");
                    }
                }
                return View(branch);
            }
            //edit Branch
            else
            {
                var addBranch = await _genericServices.GetByIdAsync(branch.BranchID);

                if ((addBranch != null))
                {
                    if (ModelState.IsValid)
                    {
                        addBranch.BranchName = branch.BranchName;
                        addBranch.ModifyOn = DateTime.Now;
                    }
                    try
                    {
                        await _genericServices.UpdateAsync(addBranch);
                        return RedirectToAction("Index");
                    }
                    catch (Exception)
                    {
                        return RedirectToAction("ErrorPage");
                    }
                }
                return View(branch);
            }
        }

        /*[HttpPost]
        //public async Task<JsonResult> InactiveActiveBranch(Guid id)
        public async Task<IActionResult> InactiveActiveBranch(Guid id)
        {
            try
            {
                var branchToDelete = await _genericServices.GetByIdAsync(id);

                if (branchToDelete != null)
                {
                    if (branchToDelete.IsActive != "1")
                    {
                        branchToDelete.IsActive = "1";
                        branchToDelete.DeletedOn = DateTime.Now;
                    }
                    else
                    {
                        branchToDelete.IsActive = "0";
                    }
                }
                else
                {
                    return NotFound();
                }
                var result = await _genericServices.SaveChangesAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json("Error : " + ex.Message);
            }
        }*/


        [HttpPost]
        public async Task<IActionResult> InactiveActiveBranch(Guid id)
        {
            try
            {
                var branchToUpdate = await _genericServices.GetByIdAsync(id);
                if (branchToUpdate != null)
                {
                    branchToUpdate.IsActive = branchToUpdate.IsActive != "1" ? "1" : "0";
                    branchToUpdate.DeletedOn = branchToUpdate.IsActive == "0" ? DateTime.Now : (DateTime?)null;
                    await _genericServices.UpdateAsync(branchToUpdate);
                    return RedirectToAction("Index");
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return Json("Error: " + ex.Message);
            }
        }

        [HttpGet]
        public IActionResult ErrorPage() => View();

    }
}
