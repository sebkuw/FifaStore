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
    public class FootballersController : Controller
    {
        private FSContext db = new FSContext();

        // GET: Footballers
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, string nationSortParm, string leagueSortParm, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "lastName_desc" : "";

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

            var footballers = from l in db.Footballers select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                footballers = footballers.Where(l => l.FirstName.Contains(searchString) || l.LastName.Contains(searchString) || l.Alias.Contains(searchString)).Include(f => f.Nationality);
            }
            if (!String.IsNullOrEmpty(nationSortParm))
            {
                int nationID = int.Parse(nationSortParm);
                footballers = footballers.Where(f => f.NationalityID == nationID).Include(f => f.Club).Include(f => f.Nationality);
            }
            if (!String.IsNullOrEmpty(leagueSortParm))
            {
                int leagueID = int.Parse(leagueSortParm);
                footballers = footballers.Where(f => f.Club.LeagueID == leagueID).Include(f => f.Club).Include(f => f.Nationality);
            }
            switch (sortOrder)
            {
                case "lastName_desc":
                    footballers = footballers.OrderByDescending(l => l.LastName).Include(f => f.Club).Include(f => f.Nationality);
                    break;
                default:
                    footballers = footballers.OrderBy(l => l.LastName).Include(f => f.Club).Include(f => f.Nationality);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            var nations = from n in db.Nationalities select n;
            ViewBag.Nations = nations.ToList();
            var leagues = from l in db.Leagues select l;
            ViewBag.Leagues = leagues.ToList();

            return View(footballers.ToPagedList(pageNumber, pageSize));
        }

        // GET: Footballers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Footballer footballer = db.Footballers.Find(id);
            if (footballer == null)
            {
                return HttpNotFound();
            }
            return View(footballer);
        }

        // GET: Footballers/Create
        public ActionResult Create()
        {
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name");
            ViewBag.NationalityID = new SelectList(db.Nationalities, "ID", "Name");
            return View();
        }

        // POST: Footballers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,Alias,NationalityID,ClubID,Photo")] Footballer footballer, HttpPostedFileBase files)
        {
            if (ModelState.IsValid && files != null && files.ContentLength > 0)
            {
                var fileName = Path.GetFileName(files.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/FootballerPortraits/"), fileName);
                files.SaveAs(path);
                footballer.Photo = "FootballerPortraits/" + fileName;

                db.Footballers.Add(footballer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", footballer.ClubID);
            ViewBag.NationalityID = new SelectList(db.Nationalities, "ID", "Name", footballer.NationalityID);
            return View(footballer);
        }

        // GET: Footballers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Footballer footballer = db.Footballers.Find(id);
            if (footballer == null)
            {
                return HttpNotFound();
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", footballer.ClubID);
            ViewBag.NationalityID = new SelectList(db.Nationalities, "ID", "Name", footballer.NationalityID);
            return View(footballer);
        }

        // POST: Footballers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,Alias,NationalityID,ClubID,Photo")] Footballer footballer, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(files.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/FootballerPortraits/"), fileName);
                    files.SaveAs(path);
                    footballer.Photo = "FootballerPortraits/" + fileName;
                }

                db.Entry(footballer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ClubID = new SelectList(db.Clubs, "ID", "Name", footballer.ClubID);
            ViewBag.NationalityID = new SelectList(db.Nationalities, "ID", "Name", footballer.NationalityID);
            return View(footballer);
        }

        // GET: Footballers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Footballer footballer = db.Footballers.Find(id);
            if (footballer == null)
            {
                return HttpNotFound();
            }
            return View(footballer);
        }

        // POST: Footballers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Footballer footballer = db.Footballers.Find(id);
            db.Footballers.Remove(footballer);
            db.SaveChanges();
            return RedirectToAction("Index");
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
