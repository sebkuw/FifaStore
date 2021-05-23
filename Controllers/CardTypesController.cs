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
    public class CardTypesController : Controller
    {
        private FSContext db = new FSContext();

        // GET: CardTypes
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var cardTypes = from l in db.CardTypes select l;

            if (!String.IsNullOrEmpty(searchString))
            {
                cardTypes = cardTypes.Where(l => l.Name.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    cardTypes = cardTypes.OrderByDescending(l => l.Name);
                    break;
                default:
                    cardTypes = cardTypes.OrderBy(l => l.Name);
                    break;
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(cardTypes.ToPagedList(pageNumber, pageSize));
        }

        // GET: CardTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardType cardType = db.CardTypes.Find(id);
            if (cardType == null)
            {
                return HttpNotFound();
            }
            return View(cardType);
        }

        // GET: CardTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CardTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,CardBorder")] CardType cardType, HttpPostedFileBase files)
        {
            if (ModelState.IsValid && files != null && files.ContentLength > 0)
            {
                var fileName = Path.GetFileName(files.FileName);
                var path = Path.Combine(Server.MapPath("~/Images/CardBorders/"), fileName);
                files.SaveAs(path);
                cardType.CardBorder = "CardBorders/" + fileName;

                db.CardTypes.Add(cardType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cardType);
        }

        // GET: CardTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardType cardType = db.CardTypes.Find(id);
            if (cardType == null)
            {
                return HttpNotFound();
            }
            return View(cardType);
        }

        // POST: CardTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,CardBorder")] CardType cardType, HttpPostedFileBase files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(files.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images/CardBorders/"), fileName);
                    files.SaveAs(path);
                    cardType.CardBorder = "CardBorders/" + fileName;
                }

                db.Entry(cardType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cardType);
        }

        // GET: CardTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardType cardType = db.CardTypes.Find(id);
            if (cardType == null)
            {
                return HttpNotFound();
            }
            return View(cardType);
        }

        // POST: CardTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CardType cardType = db.CardTypes.Find(id);
            db.CardTypes.Remove(cardType);
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
