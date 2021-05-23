using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class Club
    {
        public int ID { get; set; }
        [Display(Name = "Nazwa klubu"), Required, StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }
        [Display(Name = "Skrót nazwy"), Required, StringLength(3, MinimumLength = 2)]
        public string Short { get; set; }
        [Display(Name = "Herb klubu")]
        public string ClubCrest { get; set; }
        [Required]
        public int LeagueID { get; set; }
        public virtual League League { get; set; }
        [Display(Name = "Zawodnicy")]
        public virtual ICollection<Footballer> Footballers { get; set; }
    }
}