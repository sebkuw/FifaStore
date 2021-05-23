using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FifaStore.DAL;
using FifaStore.ViewModels;

namespace FifaStore.Controllers
{
    public class HomeController : Controller
    {
        private FSContext db = new FSContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            IQueryable<LeagueClubGroup> data = from club in db.Clubs
                                                   group club by club.League.Name into leagueGroup
                                                   select new LeagueClubGroup()
                                                   {
                                                       LeagueName = leagueGroup.Key,
                                                       ClubCount = leagueGroup.Count()
                                                   };
            return View(data.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}