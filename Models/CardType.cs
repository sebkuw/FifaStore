using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class CardType
    {
        public int ID { get; set; }
        [Display(Name = "Nazwa"), Required, StringLength(50, MinimumLength = 1)]
        public string Name { get; set; }
        [Display(Name = "Ramka"), Required]
        public string CardBorder { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
    }
}