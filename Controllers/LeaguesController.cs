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
    public class LeaguesController : Controller
    {
        private FSContext db = new FSContext();

        // GET: Leagues
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.ShortSortParm = String.IsNullOrEmpty(sortOrder) ? "short_desc" : "short_asc";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var leagues = from l in db.Leagues select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                leagues = leagues.Where(l => l.Name.Contains(searchString) || l.Short.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    leagues = leagues.OrderByDescending(l => l.Name);
                    break;
                case "short_asc":
                    leagues = leagues.OrderBy(l => l.Short);
                    break;
                case "short_desc":
                    leagues = leagues.OrderByDescending(l => l.Short);
                    break;
                default:
                    leagues = leagues.OrderBy(l => l.Name);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(leagues.ToPagedList(pageNumber, pageSize));
        }

        // GET: Leagues/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // GET: Leagues/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Leagues/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Short,LeagueCrest")] League league, HttpPostedFileBase files)
        {
            if (ModelState.IsValid && files != null && files.ContentLength > 0)
            {
                var fileName = Path.GetFileName(files.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/LeagueCrests/"), fileName);
                files.SaveAs(path);
                league.LeagueCrest = "LeagueCrests/" + fileName;

                db.Leagues.Add(league);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(league);
        }

        // GET: Leagues/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Short,LeagueCrest")] League league, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(files.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/LeagueCrests/"), fileName);
                    files.SaveAs(path);
                    league.LeagueCrest = "LeagueCrests/" + fileName;
                }

                db.Entry(league).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(league);
        }

        // GET: Leagues/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            League league = db.Leagues.Find(id);
            if (league == null)
            {
                return HttpNotFound();
            }
            return View(league);
        }

        // POST: Leagues/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            League league = db.Leagues.Find(id);
            db.Leagues.Remove(league);
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
