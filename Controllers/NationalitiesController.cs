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
    public class NationalitiesController : Controller
    {
        private FSContext db = new FSContext();

        // GET: Nationalities
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

            var nationalities = from n in db.Nationalities select n;

            if (!String.IsNullOrEmpty(searchString))
            {
                nationalities = nationalities.Where(n => n.Name.Contains(searchString) || n.Short.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    nationalities = nationalities.OrderByDescending(n => n.Name);
                    break;
                case "short_asc":
                    nationalities = nationalities.OrderBy(n => n.Short);
                    break;
                case "short_desc":
                    nationalities = nationalities.OrderByDescending(n => n.Short);
                    break;
                default:
                    nationalities = nationalities.OrderBy(n => n.Name);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(nationalities.ToPagedList(pageNumber, pageSize));
        }

        // GET: Nationalities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nationality nationality = db.Nationalities.Find(id);
            if (nationality == null)
            {
                return HttpNotFound();
            }
            return View(nationality);
        }

        // GET: Nationalities/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Nationalities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,Short,Flag")] Nationality nationality, HttpPostedFileBase files)
        {
            if (ModelState.IsValid && files != null && files.ContentLength > 0)
            {
                var fileName = Path.GetFileName(files.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/NationalityFlags/"), fileName);
                files.SaveAs(path);
                nationality.Flag = "NationalityFlags/" + fileName;

                db.Nationalities.Add(nationality);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(nationality);
        }

        // GET: Nationalities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nationality nationality = db.Nationalities.Find(id);
            if (nationality == null)
            {
                return HttpNotFound();
            }
            return View(nationality);
        }

        // POST: Nationalities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Short,Flag")] Nationality nationality, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(files.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/NationalityFlags/"), fileName);
                    files.SaveAs(path);
                    nationality.Flag = "NationalityFlags/" + fileName;
                }

                db.Entry(nationality).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(nationality);
        }

        // GET: Nationalities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Nationality nationality = db.Nationalities.Find(id);
            if (nationality == null)
            {
                return HttpNotFound();
            }
            return View(nationality);
        }

        // POST: Nationalities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Nationality nationality = db.Nationalities.Find(id);
            db.Nationalities.Remove(nationality);
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
