using System;
using System.Collections.Generic;
using System.Text;
using Entities;

namespace DomainModels
{
    public class ProfileViewModel
    {
        public User UserData { get; set; }
        public List<Entities.Post> Posts { get; set; }
        public bool CurrentUser { get; set; }
    }
}
