using Lane4Task.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Lane4Task.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Lane4Task.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            this.context = context;
        }

        public IActionResult Index()
        {
            List<User> userList = new List<User>();

            if (User.Identity.IsAuthenticated)
            {
                string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (context != null)
                {
                    // Get the IDs of the friends of the logged-in user
                    var friendIds = context.Friends
                        .Where(f => f.User.Email == loggedInEmail)
                        .Select(f => f.FriendId)
                        .ToList();

                    // Fetch users who are not the logged-in user or their friends
                    userList = context.Users
                        .Where(u => u.Email != loggedInEmail && !friendIds.Contains(u.Id))
                        .ToList();

                    ViewBag.Email = loggedInEmail; // Set the email value to ViewBag.Email
                }
            }
            else
            {
                if (context != null)
                {
                    userList = context.Users.ToList();
                }
            }

            // Pass the list of users to the view
            return View(userList);
        }



        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public IActionResult AddFriend(string friendEmail)
        {
            if (User.Identity.IsAuthenticated)
            {
                string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = context.Users.FirstOrDefault(u => u.Email == loggedInEmail);

                var friend = context.Users.FirstOrDefault(u => u.Email == friendEmail);

                if (user != null && friend != null)
                {
                    // Check if the friendship already exists
                    var existingFriendship = context.Friends.SingleOrDefault(f => f.UserId == user.Id && f.FriendId == friend.Id);
                    if (existingFriendship == null)
                    {
                        // Create a new friendship record
                        var friendship = new Friend
                        {
                            UserId = user.Id,
                            FriendId = friend.Id
                        };

                        context.Friends.Add(friendship);
                        context.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index");
        }


        public IActionResult UserDetails(int id)
        {
            var user = context.Users.FirstOrDefault(u => u.Id == id);

            if (user != null)
            {
                return View(user);
            }

            // If the user is not found, handle the error accordingly
            return RedirectToAction("AdminIndex");
        }


        [HttpPost]
        public IActionResult RemoveFriend(string friendEmail)
        {
            // Get the currently logged-in user's email
            string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Get the user and friend from the database based on the email
            var user = context.Users.FirstOrDefault(u => u.Email == loggedInEmail);
            var friend = context.Users.FirstOrDefault(u => u.Email == friendEmail);

            if (user != null && friend != null)
            {
                // Remove the friend from the user's friend list
                var friendship = context.Friends.FirstOrDefault(f => f.UserId == user.Id && f.FriendId == friend.Id);
                if (friendship != null)
                {
                    context.Friends.Remove(friendship);
                    context.SaveChanges();
                }
            }

            // Redirect back to the Index page
            return RedirectToAction("Friends");
        }



        public IActionResult Friends()
        {
            if (User.Identity.IsAuthenticated)
            {
                string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (context != null)
                {
                    
                    ViewBag.Email = loggedInEmail; // Set the email value to ViewBag.Email
                }
            }
            if (User.Identity.IsAuthenticated)
            {
                string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = context.Users.FirstOrDefault(u => u.Email == loggedInEmail);

                if (user != null)
                {
                    // Fetch the user's friends from the database
                    var friends = context.Friends
                        .Where(f => f.UserId == user.Id)
                        .Select(f => f.FriendUser)
                        .ToList();

                    // Pass the list of friends to the view
                    return View(friends);
                }
            }

            // If the user is not authenticated or user's friend data not found, return to Index page or handle the error
            return RedirectToAction("Index");
        }


        [HttpGet]
        public IActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = context.Users.FirstOrDefault(u => u.Email == loggedInEmail);

                if (user != null)
                {
                    return View(user);
                }
            }

            // If the user is not authenticated or user's profile not found, return to Index page or handle the error
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Profile(User updatedUser)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    string loggedInEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var user = context.Users.FirstOrDefault(u => u.Email == loggedInEmail);

                    if (user != null)
                    {
                        // Update the user's profile details with the updatedUser values
                        user.FirstName = updatedUser.FirstName;
                        user.LastName = updatedUser.LastName;
                        user.Email = updatedUser.Email;
                        user.MobileNumber = updatedUser.MobileNumber;
                        user.Password = updatedUser.Password;
                        user.Age = updatedUser.Age;
                        user.Gender = updatedUser.Gender;

                        context.SaveChanges();
                    }
                }

                return RedirectToAction("Index");
            }

            // If the model is not valid, return back to the profile page to display validation errors
            return View(updatedUser);
        }

        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            // Get the user from the database based on the provided userId
            var user = context.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                // Remove the user from the context and save changes
                context.Users.Remove(user);
                context.SaveChanges();
            }

            // Redirect back to the admin dashboard
            return RedirectToAction("AdminIndex");
        }


        [Authorize]
        public IActionResult AdminIndex()
        {
            List<User> userList = context.Users.Include(u => u.Friends).ToList();
            return View(userList);
        }



        public IActionResult AdminFriends()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Fetch all users from the database
                var users = context.Users.ToList();

                // Fetch the friend relationships from the database
                var friends = context.Friends.ToList();

                // Create a dictionary to store the friend lists for each user
                var friendLists = new Dictionary<User, List<User>>();

                foreach (var user in users)
                {
                    // Fetch the user's friend IDs
                    var friendIds = friends
                        .Where(f => f.UserId == user.Id)
                        .Select(f => f.FriendId)
                        .ToList();

                    // Fetch the user's friends from the dictionary using the friend IDs
                    var userFriends = users
                        .Where(u => friendIds.Contains(u.Id))
                        .ToList();

                    // Add the friend list to the dictionary
                    friendLists.Add(user, userFriends);
                }

                // Pass the dictionary of friend lists to the view
                return View(friendLists);
            }

            // If the user is not authenticated, return to the Index page or handle the error
            return RedirectToAction("Index");
        }







        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
