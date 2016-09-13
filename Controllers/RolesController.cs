using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasicAuthentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BasicAuthentication.Controllers
{
    public class RolesController : Controller
    {
        private ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ApplicationDbContext db, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _roleManager = roleManager;
        }
        public IActionResult Index()
        {
            var roles = _db.Roles.ToList();
            return View(roles);
        }

        private void PrepopulateDropDownMenus()
        {
            var rolesList = _db.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            var usersList = _db.Users.OrderBy(u => u.UserName).ToList().Select(uu => new SelectListItem { Value = uu.UserName.ToString(), Text = uu.UserName }).ToList();
            ViewBag.Roles = rolesList;
            ViewBag.Users = usersList;
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(string rolename)
        {
            try
            {
                _db.Roles.Add(new IdentityRole()
                {
                    Name = rolename
                });
                _db.SaveChanges();
                ViewBag.ResultMessage = "Role created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View();
            }
        }

        public IActionResult Delete(string rolename)
        {
            var thisRole = _db.Roles.Where(r => r.Name.Equals(rolename, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            _db.Roles.Remove(thisRole);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(string rolename)
        {
            var thisRole = _db.Roles.Where(r => r.Name.Equals(rolename, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            return View(thisRole);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityRole role)
        {
            try
            {
                _db.Roles.Attach(role);
                _db.Entry(role).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine(ex);
                return View();
            }
        }

        public IActionResult Manage()
        {
            PrepopulateDropDownMenus();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRoleToUser(string UserName, string RoleName)
        {
            try
            {
                var RoleNameUpper = RoleName.ToUpper();

                ApplicationUser user = _db.Users.Where(u => u.UserName.Equals(UserName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
                await _userManager.AddToRoleAsync(user, RoleNameUpper);
                PrepopulateDropDownMenus();
                ViewBag.ResultMessage = "Role created successfully!";
                return View("Manage", "Roles");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return View("Manage");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetRoles(string userName)
        {
            ApplicationUser user = _db.Users.Where(u => u.UserName.Equals(userName, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            var account = new AccountController(_userManager, _signInManager, _db);
            ViewBag.RolesForThisUser = await account._userManager.GetRolesAsync(user);
            PrepopulateDropDownMenus();
            return View("Manage");
        }
    }
}
