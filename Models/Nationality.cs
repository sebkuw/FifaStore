using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class Nationality
    {
        public int ID { get; set; }
        [Display(Name = "Nazwa kraju"), Required, StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [Display(Name = "Skrót nazwy"), Required, StringLength(3, MinimumLength = 2)]
        public string Short { get; set; }
        [Display(Name = "Flaga")]
        public string Flag { get; set; }
        public virtual ICollection<Footballer> Footballers { get; set; }
    }
}