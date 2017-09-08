using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HackerNews.Data
{
    public class HackerNewsRepository
    {
        private string _connectionString;
        public HackerNewsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public IEnumerable<Post> AllPosts()
        {
            using(var context = new HackerNewsDataContext(_connectionString))
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Post>(p => p.User);
                context.LoadOptions = loadOptions;
                return context.Posts.ToList();
            }
        }
        public User Login(string email, string password)
        {
            User user = GetByEmail(email);
            if (user == null)
            {
                return null;
            }
            bool match = PasswordHelper.PasswordMatch(password, user.PasswordSalt, user.PasswordHash);
            if (!match)
            {
                return null;
            }
            return user;
        }
        public User GetByEmail(string email)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                return context.Users.First(u => u.Email == email);
            }
        }
        public void AddUser(User user, string password)
        {
            var salt = PasswordHelper.GenerateSalt();
            var hash = PasswordHelper.HashPassword(password, salt);
            user.PasswordHash = hash;
            user.PasswordSalt = salt;

            using (var context = new HackerNewsDataContext(_connectionString))
            {
                context.Users.InsertOnSubmit(user);
                context.SubmitChanges();
            }
        }
        public void AddPost(Post post)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                context.Posts.InsertOnSubmit(post);
                context.SubmitChanges();
            }
        }
        public IEnumerable<Comment> getComments(int postId)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Post>(p => p.User);
                loadOptions.LoadWith<Comment>(c => c.Post);
                context.LoadOptions = loadOptions;
                return context.Comments.Where(c => c.PostId == postId).ToList();
            }
        }
        public Post GetPostById(int postId)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                return context.Posts.First(p => p.Id == postId);
            }
        }
        public Post UpVote(int postId)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                Post p = GetPostById(postId);
                p.UpVotes++;
                context.ExecuteCommand("UPDATE Posts SET UpVotes = {0} WHERE Id = {1}", p.UpVotes, postId);
                return p;
            }
        }
        public Post DownVote(int postId)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                Post p = GetPostById(postId);
                p.DownVotes++;
                context.ExecuteCommand("UPDATE Posts SET DownVotes = {0} WHERE Id = {1}", p.DownVotes, postId);
                return p;
            }
        }
        public IEnumerable<Post> GetPostsForUser(int userId)
        {
            using(var context = new HackerNewsDataContext(_connectionString))
            {
                var loadOptions = new DataLoadOptions();
                loadOptions.LoadWith<Post>(p => p.User);
                context.LoadOptions = loadOptions;
                return context.Posts.Where(p => p.UserId == userId).ToList();
            }
        }
        public User GetUserById(int userId)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                return context.Users.First(u => u.Id == userId);
            }
        }
        public void AddComment(Comment comment)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                context.Comments.InsertOnSubmit(comment);
                context.SubmitChanges();
            }
        }
        public int CommentCount (int postId)
        {
            using (var context = new HackerNewsDataContext(_connectionString))
            {
                return context.Comments.Count(c => c.PostId == postId);
            }
        }
    }
}
