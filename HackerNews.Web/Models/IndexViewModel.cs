using HackerNews.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HackerNews.Web.Models
{
    public class IndexViewModel
    {
        public IEnumerable<Post> Posts { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
    }
}