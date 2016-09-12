using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BasicAuthentication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BasicAuthentication.Controllers
{
    public class RolesController : Controller
    {
        private readonly ApplicationDbContext _db;
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
    }
}
