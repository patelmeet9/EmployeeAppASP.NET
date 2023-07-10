using Lane4Task.Data;
using Lane4Task.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Lane4Task.Controllers
{
    public class UserController : Controller
    {
        private readonly UserRepository _userRepository;
        private readonly ApplicationDbContext context;

        public UserController(UserRepository userRepository,ApplicationDbContext context)
        {
            _userRepository = userRepository;
            this.context = context;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (context != null)
                {
                    var user = context.Users.FirstOrDefault(u => u.Email == loggedInEmail);
                    if (user != null && user.Email == "admin@gmail.com" && user.Password == "Admin@123")
                    {
                        // If the logged-in user is the admin, redirect to the 'AdminIndex' view
                        return RedirectToAction("AdminIndex","Home");
                    }
                }
            }

            // For regular users or if the admin is not found, return the default 'Index' view
            List<User> userList = context.Users.ToList();
            return View(userList);
        }


        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                _userRepository.InsertUser(user);
                return RedirectToAction("Login");
            }

            // If the model state is not valid, return the view with validation errors
            return View("SignUp", user);
        }

        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var user = context.Users.FirstOrDefault(u => u.Email == loggedInEmail);

                if (user != null && user.Email == "admin@gmail.com" && user.Password == "Admin@123")
                {
                    // If the logged-in user is the admin, redirect to the 'AdminIndex' action in the 'Home' controller
                    return RedirectToAction("AdminIndex", "Home");
                }
                else
                {
                    // For regular users, redirect to the 'Index' action in the 'Home' controller
                    return RedirectToAction("Index", "Home");
                }
            }

            // If not authenticated, show the login view
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            if (model.Email == "admin@gmail.com" && model.Password == "Admin@123")
            {
                // Admin login
                // Set the FirstName, LastName, and Gender properties of the User model
                model.FirstName = "Admin";
                model.LastName = "Admin";
                model.Gender = "Male";

                // Create a claim with the user's email
                List<Claim> claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, model.Email),
        };

                // Create a claims identity and specify the authentication scheme
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create authentication properties
                AuthenticationProperties properties = new AuthenticationProperties()
                {
                    AllowRefresh = true,
                };

                // Sign in the user with the claims identity
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                // Set the 'Email' session value
                HttpContext.Session.SetInt32("Id", model.Id);
                HttpContext.Session.SetString("Email", model.Email);
                HttpContext.Session.SetString("FirstName", model.FirstName);

                // Redirect to the 'AdminIndex' action of the 'User' controller for the admin
                return RedirectToAction("AdminIndex", "Home");
            }

            var user = await context.Users.FirstOrDefaultAsync(e => e.Email == model.Email);

            if (user != null)
            {
                // User exists in the database
                if (model.Password == user.Password)
                {
                    // Password is correct

                    // Set the FirstName, LastName, and Gender properties of the User model
                    model.FirstName = user.FirstName;
                    model.LastName = user.LastName;
                    model.Gender = user.Gender;

                    // Create a claim with the user's email
                    List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, model.Email),
            };

                    // Create a claims identity and specify the authentication scheme
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Create authentication properties
                    AuthenticationProperties properties = new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                    };

                    // Sign in the user with the claims identity
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);

                    // Set the 'Email' session value
                    HttpContext.Session.SetString("Email", model.Email);

                    // Redirect to the 'Index' action of the 'User' controller
                    return RedirectToAction("Index", "Home");
                }

                // Password is incorrect
                ViewData["ValidateMessage"] = "Invalid password";
                return View();
            }
            else
            {
                // User not found in the database
                ViewData["ValidateMessage"] = "Email not found";
                return View();
            }
        }



    }
}
