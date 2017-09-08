using HackerNews.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerNews.Web.Models
{
    public class CommentsViewModel
    {
        public IEnumerable<Comment> Comments { get; set; }
        public Post Post { get; set; }
        public User User { get; set; }
    }
}