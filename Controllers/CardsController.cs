using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FifaStore.DAL;
using FifaStore.Models;
using PagedList;

namespace FifaStore.Controllers
{
    public class CardsController : Controller
    {
        private FSContext db = new FSContext();

        // GET: Cards
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string nationSortParm, string leagueSortParm, string cardTypeSortParm, string positionSortParm, string priceMin, string priceMax, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";
            ViewBag.OverallSortParm = sortOrder == "Overall" ? "overall_desc" : "Overall";
            ViewBag.AvaragePriceSortParm = sortOrder == "AvaragePrice" ? "avaragePrice_desc" : "AvaragePrice";
            ViewBag.RateSortParm = sortOrder == "Rate" ? "rate_desc" : "Rate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.LeagueSortParm = leagueSortParm;
            ViewBag.NationSortParm = nationSortParm;
            ViewBag.CardTypeSortParm = cardTypeSortParm;
            ViewBag.PositionSortParm = positionSortParm;
            ViewBag.PriceMin = priceMin;
            ViewBag.PriceMax = priceMax;

            var cards = from c in db.Cards select c;
            if (!String.IsNullOrEmpty(searchString))
            {
                cards = cards.Where(l => l.Footballer.FirstName.Contains(searchString) || l.Footballer.LastName.Contains(searchString) || l.Footballer.Alias.Contains(searchString)).Include(c => c.CardType).Include(c => c.Footballer);
            }
            if (!String.IsNullOrEmpty(nationSortParm))
            {
                int nationID = int.Parse(nationSortParm);
                cards = cards.Where(f => f.Footballer.NationalityID == nationID).Include(c => c.CardType).Include(c => c.Footballer);
            }
            if (!String.IsNullOrEmpty(leagueSortParm))
            {
                int leagueID = int.Parse(leagueSortParm);
                cards = cards.Where(f => f.Footballer.Club.LeagueID == leagueID).Include(c => c.CardType).Include(c => c.Footballer);
            }
            if (!String.IsNullOrEmpty(cardTypeSortParm))
            {
                int cardTypeID = int.Parse(cardTypeSortParm);
                cards = cards.Where(f => f.CardTypeID == cardTypeID).Include(c => c.CardType).Include(c => c.Footballer);
            }
            if (!String.IsNullOrEmpty(positionSortParm))
            {
                cards = cards.Where(f => f.Position.ToString() == positionSortParm).Include(c => c.CardType).Include(c => c.Footballer);
            }
            if (!String.IsNullOrEmpty(priceMin))
            {
                int min = int.Parse(priceMin);
                cards = cards.Where(f => f.AvaragePrice >= min).Include(c => c.CardType).Include(c => c.Footballer);
            }
            if (!String.IsNullOrEmpty(priceMax))
            {
                int max = int.Parse(priceMax);
                cards = cards.Where(f => f.AvaragePrice <= max).Include(c => c.CardType).Include(c => c.Footballer);
            }

            switch (sortOrder)
            {
                case "lastName_desc":
                    cards = cards.OrderByDescending(l => l.Footballer.LastName);
                    break;
                case "Overall":
                    cards = cards.OrderBy(l => l.Overall);
                    break;
                case "overall_desc":
                    cards = cards.OrderByDescending(l => l.Overall);
                    break;
                case "AvaragePrice":
                    cards = cards.OrderBy(l => l.AvaragePrice);
                    break;
                case "avaragePrice_desc":
                    cards = cards.OrderByDescending(l => l.AvaragePrice);
                    break;
                case "Rate":
                    cards = cards.OrderBy(l => l.AvarageRate);
                    break;
                case "rate_desc":
                    cards = cards.OrderByDescending(l => l.AvarageRate);
                    break;
                default:
                    cards = cards.OrderBy(l => l.Footballer.LastName);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var nations = from n in db.Nationalities select n;
            ViewBag.Nations = nations.ToList();
            var leagues = from l in db.Leagues select l;
            ViewBag.Leagues = leagues.ToList();
            var cardTypes = from c in db.CardTypes select c;
            ViewBag.CardTypes = cardTypes.ToList();
            var positions = Enum.GetValues(typeof(Position));
            ViewBag.Positions = positions;

            return View(cards.ToPagedList(pageNumber, pageSize));
        }

        // GET: PrettyCards
        public ActionResult PrettyCards()
        {
            var cards = from c in db.Cards orderby c.Footballer.LastName select c;
            return View(cards);
        }

        // GET: Cards/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }

            ViewBag.IsLiked = false;
            ViewBag.IsOwned = false;
            if (User.Identity.IsAuthenticated)
            {
                Profile profile = db.Profiles.Single(p => p.Username == User.Identity.Name);

                if (profile.Owned.Contains(card))
                {
                    ViewBag.IsOwned = true;
                }

                if (profile.Liked.Contains(card))
                {
                    ViewBag.IsLiked = true;
                }
            }

            if(card.Opinions.Count == 0)
            {
                ViewBag.IsComment = false;
            }
            else
            {
                ViewBag.IsComment = true;
                ViewBag.Comments = card.Opinions.ToList();
            }

            return View(card);
        }

        // GET: Cards/Create
        public ActionResult Create()
        {
            ViewBag.CardTypeID = new SelectList(db.CardTypes, "ID", "Name");
            ViewBag.FootballerID = new SelectList(db.Footballers, "ID", "FirstName");
            return View();
        }

        // POST: Cards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FootballerID,CardTypeID,AvaragePrice,Rate,Position,Overall,Pace,Shooting,Passing,Dribbling,Defending,Physicality")] Card card)
        {
            if (ModelState.IsValid)
            {
                db.Cards.Add(card);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CardTypeID = new SelectList(db.CardTypes, "ID", "Name", card.CardTypeID);
            ViewBag.FootballerID = new SelectList(db.Footballers, "ID", "FirstName", card.FootballerID);
            return View(card);
        }

        // GET: Cards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }
            ViewBag.CardTypeID = new SelectList(db.CardTypes, "ID", "Name", card.CardTypeID);
            ViewBag.FootballerID = new SelectList(db.Footballers, "ID", "FirstName", card.FootballerID);
            return View(card);
        }

        // POST: Cards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FootballerID,CardTypeID,AvaragePrice,Rate,Position,AddDate,Overall,Pace,Shooting,Passing,Dribbling,Defending,Physicality")] Card card)
        {
            if (ModelState.IsValid)
            {
                db.Entry(card).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CardTypeID = new SelectList(db.CardTypes, "ID", "Name", card.CardTypeID);
            ViewBag.FootballerID = new SelectList(db.Footballers, "ID", "FirstName", card.FootballerID);
            return View(card);
        }

        // GET: Cards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }
            return View(card);
        }

        // POST: Cards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Card card = db.Cards.Find(id);
            db.Cards.Remove(card);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddPrice(int id, string tbPrice)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }
            card.FullPrice += int.Parse(tbPrice);
            card.AvaragePriceCounter++;

            db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult AddRate(int id, string tbRate)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }
            card.FullRate += int.Parse(tbRate);
            card.RateCounter++;

            db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult AddToOwned(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }

            Profile profile = db.Profiles.Single(p => p.Username == User.Identity.Name);
            profile.Owned.Add(card);

            db.Entry(profile).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult AddToLiked(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Card card = db.Cards.Find(id);
            if (card == null)
            {
                return HttpNotFound();
            }

            Profile profile = db.Profiles.Single(p => p.Username == User.Identity.Name);
            profile.Liked.Add(card);

            db.Entry(profile).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult AddOpinion(int id, string opinion)
        {
            if (id == 0 || opinion.Equals(""))
            {
                return RedirectToAction("Details", new { id = id });
            }
            Card card = db.Cards.Find(id);
            Profile profile = db.Profiles.Single(p => p.Username == User.Identity.Name);
            if (card == null || profile == null)
            {
                return HttpNotFound();
            }

            Opinion op = new Opinion
            {
                Card = card,
                CardID = card.ID,
                Profile = profile,
                Time = DateTime.Now,
                Message = opinion
            };

            db.Opinions.Add(op);
            db.SaveChanges();

            return RedirectToAction("Details", new { id = id });
        }

        // GET: Cards/MyLikedCards
        public ActionResult MyLikedCards(string sortOrder, string currentFilter, string searchString, string nationSortParm, string leagueSortParm, string cardTypeSortParm, string positionSortParm, string priceMin, string priceMax, int? page)
        {
            Profile profile = db.Profiles.Single(p => p.Username == User.Identity.Name);
            if (profile == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";
            ViewBag.OverallSortParm = sortOrder == "Overall" ? "overall_desc" : "Overall";
            ViewBag.AvaragePriceSortParm = sortOrder == "AvaragePrice" ? "avaragePrice_desc" : "AvaragePrice";
            ViewBag.RateSortParm = sortOrder == "Rate" ? "rate_desc" : "Rate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.LeagueSortParm = leagueSortParm;
            ViewBag.NationSortParm = nationSortParm;
            ViewBag.CardTypeSortParm = cardTypeSortParm;
            ViewBag.PositionSortParm = positionSortParm;
            ViewBag.PriceMin = priceMin;
            ViewBag.PriceMax = priceMax;

            var cards = profile.Liked;

            if (!String.IsNullOrEmpty(searchString))
            {
                cards = cards.Where(l => l.Footballer.FirstName.Contains(searchString) || l.Footballer.LastName.Contains(searchString) || l.Footballer.Alias.Contains(searchString)).ToList();
            }
            if (!String.IsNullOrEmpty(nationSortParm))
            {
                int nationID = int.Parse(nationSortParm);
                cards = cards.Where(f => f.Footballer.NationalityID == nationID).ToList();
            }
            if (!String.IsNullOrEmpty(leagueSortParm))
            {
                int leagueID = int.Parse(leagueSortParm);
                cards = cards.Where(f => f.Footballer.Club.LeagueID == leagueID).ToList();
            }
            if (!String.IsNullOrEmpty(cardTypeSortParm))
            {
                int cardTypeID = int.Parse(cardTypeSortParm);
                cards = cards.Where(f => f.CardTypeID == cardTypeID).ToList();
            }
            if (!String.IsNullOrEmpty(positionSortParm))
            {
                cards = cards.Where(f => f.Position.ToString() == positionSortParm).ToList();
            }
            if (!String.IsNullOrEmpty(priceMin))
            {
                int min = int.Parse(priceMin);
                cards = cards.Where(f => f.AvaragePrice >= min).ToList();
            }
            if (!String.IsNullOrEmpty(priceMax))
            {
                int max = int.Parse(priceMax);
                cards = cards.Where(f => f.AvaragePrice <= max).ToList();
            }

            switch (sortOrder)
            {
                case "lastName_desc":
                    cards = cards.OrderByDescending(l => l.Footballer.LastName).ToList();
                    break;
                case "Overall":
                    cards = cards.OrderBy(l => l.Overall).ToList();
                    break;
                case "overall_desc":
                    cards = cards.OrderByDescending(l => l.Overall).ToList();
                    break;
                case "AvaragePrice":
                    cards = cards.OrderBy(l => l.AvaragePrice).ToList();
                    break;
                case "avaragePrice_desc":
                    cards = cards.OrderByDescending(l => l.AvaragePrice).ToList();
                    break;
                case "Rate":
                    cards = cards.OrderBy(l => l.AvarageRate).ToList();
                    break;
                case "rate_desc":
                    cards = cards.OrderByDescending(l => l.AvarageRate).ToList();
                    break;
                default:
                    cards = cards.OrderBy(l => l.Footballer.LastName).ToList();
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var nations = from n in db.Nationalities select n;
            ViewBag.Nations = nations.ToList();
            var leagues = from l in db.Leagues select l;
            ViewBag.Leagues = leagues.ToList();
            var cardTypes = from c in db.CardTypes select c;
            ViewBag.CardTypes = cardTypes.ToList();
            var positions = Enum.GetValues(typeof(Position));
            ViewBag.Positions = positions;

            return View(cards.ToPagedList(pageNumber, pageSize));
        }

        // GET: Cards/MyOwnedCards
        public ActionResult MyOwnedCards(string sortOrder, string currentFilter, string searchString, string nationSortParm, string leagueSortParm, string cardTypeSortParm, string positionSortParm, string priceMin, string priceMax, int? page)
        {
            Profile profile = db.Profiles.Single(p => p.Username == User.Identity.Name);
            if (profile == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";
            ViewBag.OverallSortParm = sortOrder == "Overall" ? "overall_desc" : "Overall";
            ViewBag.AvaragePriceSortParm = sortOrder == "AvaragePrice" ? "avaragePrice_desc" : "AvaragePrice";
            ViewBag.RateSortParm = sortOrder == "Rate" ? "rate_desc" : "Rate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;
            ViewBag.LeagueSortParm = leagueSortParm;
            ViewBag.NationSortParm = nationSortParm;
            ViewBag.CardTypeSortParm = cardTypeSortParm;
            ViewBag.PositionSortParm = positionSortParm;
            ViewBag.PriceMin = priceMin;
            ViewBag.PriceMax = priceMax;

            var cards = profile.Owned;

            if (!String.IsNullOrEmpty(searchString))
            {
                cards = cards.Where(l => l.Footballer.FirstName.Contains(searchString) || l.Footballer.LastName.Contains(searchString) || l.Footballer.Alias.Contains(searchString)).ToList();
            }
            if (!String.IsNullOrEmpty(nationSortParm))
            {
                int nationID = int.Parse(nationSortParm);
                cards = cards.Where(f => f.Footballer.NationalityID == nationID).ToList();
            }
            if (!String.IsNullOrEmpty(leagueSortParm))
            {
                int leagueID = int.Parse(leagueSortParm);
                cards = cards.Where(f => f.Footballer.Club.LeagueID == leagueID).ToList();
            }
            if (!String.IsNullOrEmpty(cardTypeSortParm))
            {
                int cardTypeID = int.Parse(cardTypeSortParm);
                cards = cards.Where(f => f.CardTypeID == cardTypeID).ToList();
            }
            if (!String.IsNullOrEmpty(positionSortParm))
            {
                cards = cards.Where(f => f.Position.ToString() == positionSortParm).ToList();
            }
            if (!String.IsNullOrEmpty(priceMin))
            {
                int min = int.Parse(priceMin);
                cards = cards.Where(f => f.AvaragePrice >= min).ToList();
            }
            if (!String.IsNullOrEmpty(priceMax))
            {
                int max = int.Parse(priceMax);
                cards = cards.Where(f => f.AvaragePrice <= max).ToList();
            }

            switch (sortOrder)
            {
                case "lastName_desc":
                    cards = cards.OrderByDescending(l => l.Footballer.LastName).ToList();
                    break;
                case "Overall":
                    cards = cards.OrderBy(l => l.Overall).ToList();
                    break;
                case "overall_desc":
                    cards = cards.OrderByDescending(l => l.Overall).ToList();
                    break;
                case "AvaragePrice":
                    cards = cards.OrderBy(l => l.AvaragePrice).ToList();
                    break;
                case "avaragePrice_desc":
                    cards = cards.OrderByDescending(l => l.AvaragePrice).ToList();
                    break;
                case "Rate":
                    cards = cards.OrderBy(l => l.AvarageRate).ToList();
                    break;
                case "rate_desc":
                    cards = cards.OrderByDescending(l => l.AvarageRate).ToList();
                    break;
                default:
                    cards = cards.OrderBy(l => l.Footballer.LastName).ToList();
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var nations = from n in db.Nationalities select n;
            ViewBag.Nations = nations.ToList();
            var leagues = from l in db.Leagues select l;
            ViewBag.Leagues = leagues.ToList();
            var cardTypes = from c in db.CardTypes select c;
            ViewBag.CardTypes = cardTypes.ToList();
            var positions = Enum.GetValues(typeof(Position));
            ViewBag.Positions = positions;

            return View(cards.ToPagedList(pageNumber, pageSize));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
