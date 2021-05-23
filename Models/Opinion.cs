using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FifaStore.Models
{
    public class Opinion
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int CardID { get; set; }
        [Display(Name = "Treść"), Required, StringLength(1000, MinimumLength = 3)]
        public string Message { get; set; }
        [Display(Name = "Data utworzenia"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }
        public virtual Profile Profile { get; set; }
        public virtual Card Card { get; set; }
    }
}