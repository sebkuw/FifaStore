using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class League
    {
        public int ID { get; set; }
        [Display(Name = "Nazwa ligi"), Required, StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [Display(Name = "Skrót nazwy"), Required, StringLength(4, MinimumLength = 3)]
        public string Short { get; set; }
        [Display(Name = "Herb ligi")]
        public string LeagueCrest { get; set; }
        public virtual ICollection<Club> Clubs { get; set; }
    }
}