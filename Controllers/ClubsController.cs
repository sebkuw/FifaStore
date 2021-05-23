using System;
using System.Collections.Generic;
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
    public class ClubsController : Controller
    {
        private FSContext db = new FSContext();

        // GET: Clubs
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ShortSortParm = sortOrder=="Short" ? "short_desc" : "Short";
            ViewBag.LeagueSortParm = sortOrder=="League" ? "league_desc" : "League";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var clubs = from l in db.Clubs select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                clubs = clubs.Where(l => l.Name.Contains(searchString) || l.Short.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    clubs = clubs.OrderByDescending(l => l.Name);
                    break;
                case "Short":
                    clubs = clubs.OrderBy(l => l.Short);
                    break;
                case "short_desc":
                    clubs = clubs.OrderByDescending(l => l.Short);
                    break;
                case "League":
                    clubs = clubs.OrderBy(l => l.League.Short);
                    break;
                case "league_desc":
                    clubs = clubs.OrderByDescending(l => l.League.Short);
                    break;
                default:
                    clubs = clubs.OrderBy(l => l.Name);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(clubs.ToPagedList(pageNumber, pageSize));
        }

        // GET: Clubs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // GET: Clubs/Create
        public ActionResult Create()
        {
            ViewBag.LeagueID = new SelectList(db.Leagues, "ID", "Name");
            return View();
        }

        // POST: Clubs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Short,ClubCrest,LeagueID")] Club club, HttpPostedFileBase files)
        {
            if (ModelState.IsValid && files != null && files.ContentLength > 0)
            {
                var fileName = Path.GetFileName(files.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/ClubCrests/"), fileName);
                files.SaveAs(path);
                club.ClubCrest = "ClubCrests/" + fileName;

                db.Clubs.Add(club);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LeagueID = new SelectList(db.Leagues, "ID", "Name", club.LeagueID);
            return View(club);
        }

        // GET: Clubs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            ViewBag.LeagueID = new SelectList(db.Leagues, "ID", "Name", club.LeagueID);
            return View(club);
        }

        // POST: Clubs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Short,ClubCrest,LeagueID")] Club club, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(files.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/ClubCrests/"), fileName);
                    files.SaveAs(path);
                    club.ClubCrest = "ClubCrests/" + fileName;
                }

                db.Entry(club).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LeagueID = new SelectList(db.Leagues, "ID", "Name", club.LeagueID);
            return View(club);
        }

        // GET: Clubs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Club club = db.Clubs.Find(id);
            if (club == null)
            {
                return HttpNotFound();
            }
            return View(club);
        }

        // POST: Clubs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Club club = db.Clubs.Find(id);
            db.Clubs.Remove(club);
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
