using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class Footballer
    {
        public int ID { get; set; }
        [Display(Name = "Imię"), Required, StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }
        [Display(Name = "Nazwisko"), Required, StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }
        [Display(Name = "Przydomek")]
        public string Alias { get; set; }
        [Display(Name = "Narodowość"), Required]
        public int NationalityID { get; set; }
        [Display(Name = "Klub"), Required]
        public int ClubID { get; set; }
        [Display(Name = "Zdjęcie zawodnika")]
        public string Photo { get; set; }
        public virtual Club Club { get; set; }
        public virtual Nationality Nationality { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
    }
}