using HackerNews.Data;
using HackerNews.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace HackerNews.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            var vm = new IndexViewModel();
            var Posts = repo.AllPosts();
            foreach(Post p in Posts)
            {
                var hrs = p.TimePosted.Hour - DateTime.Now.Hour;
                p.Score = GetScore(p.UpVotes, p.DownVotes, hrs);
            }
            vm.Posts = Posts.OrderByDescending(p => p.Score).Take(10);
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                User u = repo.GetByEmail(email);
                vm.User = u.FirstName + " " + u.LastName;
            }
            vm.Type = "Top 10 Articles";
            return View(vm);
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return Redirect("/home/index");
        }
        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            User user = repo.Login(email, password);
            if (user == null)
            {
                return View("Login");
            }
            FormsAuthentication.SetAuthCookie(email, true);
            return Redirect("/home/index");
        }
        public ActionResult Register()
        {
            return View();
        }
        public ActionResult addUser(User user, string password)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            repo.AddUser(user, password);
            FormsAuthentication.SetAuthCookie(user.Email, true);
            return Redirect("/home/index");
        }
        public ActionResult Newest()
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            var vm = new IndexViewModel();
            vm.Posts = repo.AllPosts().OrderByDescending(p => p.TimePosted).Take(10);
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                User u = repo.GetByEmail(email);
                vm.User = u.FirstName + " " + u.LastName;
            }
            vm.Type = "Newest 10 Articles";
            return View("Index", vm);
        }
        [Authorize]
        public ActionResult Submit()
        {
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult addPost(string linkTitle, string linkUrl)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            string email = User.Identity.Name;
            User u = repo.GetByEmail(email);
            Post post = new Post
            {
                LinkTitle = linkTitle,
                LinkUrl = linkUrl,
                TimePosted = DateTime.Now,
                UserId = u.Id
            };
            repo.AddPost(post);
            return Redirect("/home/index");
        }
        [Route("articles/{postId}/comments")]
        public ActionResult Comments(int postId)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            var vm = new CommentsViewModel();
            vm.Comments = repo.getComments(postId);
            vm.Post = repo.GetPostById(postId);
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                vm.User = repo.GetByEmail(email);
            }
            return View(vm);
        }
        private double GetScore(int upvotes, int downvotes, int hoursSinceSubmission)
        {
            return (upvotes - downvotes) / Math.Pow((hoursSinceSubmission + 2), 1.8);
        }
        [HttpPost]
        public ActionResult upVote(int postId)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            Post p = repo.UpVote(postId);
            return Json(new
            {
                votesUp = p.UpVotes,
                votesDown = p.DownVotes,
                postId = p.Id
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult downVote(int postId)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            Post p = repo.DownVote(postId);
            return Json(new
            {
                votesUp = p.UpVotes,
                votesDown = p.DownVotes,
                postId = p.Id
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UserPosts (int userId)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            var vm = new IndexViewModel();
            vm.Posts = repo.GetPostsForUser(userId).OrderByDescending(p => p.TimePosted);
            if (User.Identity.IsAuthenticated)
            {
                string email = User.Identity.Name;
                User u = repo.GetByEmail(email);
                vm.User = u.FirstName + " " + u.LastName;
            }
            User user = repo.GetUserById(userId);
            vm.Type = "Posts By " + user.FirstName + " " + user.LastName;
            return View("index", vm);
        }
        [Authorize]
        [HttpPost]
        public ActionResult addComment(string comment, int postId)
        {
            var repo = new HackerNewsRepository(Properties.Settings.Default.ConStr);
            string email = User.Identity.Name;
            User u = repo.GetByEmail(email);
            Comment c = new Comment
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                CommentText = comment,
                PostId = postId,
            };
            repo.AddComment(c);
            return Json(new {
                firstName = c.FirstName,
                lastName = c.LastName,
                commentText = c.CommentText
            });
        }
    }
}