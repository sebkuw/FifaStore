using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web;

namespace FifaStore.ViewModels
{
    public class LeaguesFileReq
    {
        [Display(Name = "Nazwa ligi"), Required, StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [Display(Name = "Skrót nazwy"), Required, StringLength(4, MinimumLength = 3)]
        public string Short { get; set; }
        [Display(Name = "Herb ligi"), Required]
        public IFormFile File { get; set; }

    }
}