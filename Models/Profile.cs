using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class Profile
    {
        public int ID { get; set; }
        [Required]
        public string Username { get; set; }
        public string Avatar { get; set; }
        public virtual ICollection<Opinion> Opinions { get; set; }
        public virtual ICollection<Card> Owned { get; set; }
        public virtual ICollection<Card> Liked { get; set; }
    }
}