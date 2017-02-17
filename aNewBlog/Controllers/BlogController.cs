using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using aNewBlog.Models;
using System.IO;
using PagedList;
using Microsoft.AspNet.Identity;

namespace aNewBlog.Controllers
{
    public class BlogController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //GET: BlogPosts
        public ActionResult Index(int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.FName = db.Users.Find(User.Identity.GetUserId()).FirstName;
            }
            int pageSize = 5;       // the number of posts you want to display per page
            int pageNumber = (page ?? 1);

            var listPosts = db.BlogPosts.AsQueryable();
            return View(listPosts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Index(string searchStr, int? page)
        {
            //var result = db.BlogPosts.Where(p => p.Body.Contains(searchStr))
            //        .Union(db.BlogPosts.Where(p => p.Title.Contains(searchStr)))
            //        .Union(db.BlogPosts.Where(p => p.Comments.Any(c => c.Body.Contains(searchStr))))
            //        .Union(db.BlogPosts.Where(p => p.Comments.Any(c => c.Author.DisplayName.Contains(searchStr))))
            //        .Union(db.BlogPosts.Where(p => p.Comments.Any(c => c.Author.FirstName.Contains(searchStr))))
            //        .Union(db.BlogPosts.Where(p => p.Comments.Any(c => c.Author.LastName.Contains(searchStr))))
            //        .Union(db.BlogPosts.Where(p => p.Comments.Any(c => c.Author.UserName.Contains(searchStr))))
            //        .Union(db.BlogPosts.Where(p => p.Comments.Any(c => c.Author.Email.Contains(searchStr))))
            //        .Union(db.BlogPosts.Where(p => p.Comments.Any(c => c.UpdateReason.Contains(searchStr))));

            var listPosts = db.BlogPosts.AsQueryable();
            listPosts = listPosts.Where(p => p.Title.Contains(searchStr) ||
                                             p.Body.Contains(searchStr) ||
                                             p.Comments.Any(c => c.Body.Contains(searchStr) ||
                                                                 c.Author.FirstName.Contains(searchStr) ||
                                                                 c.Author.LastName.Contains(searchStr) ||
                                                                 c.Author.DisplayName.Contains(searchStr) ||
                                                                 c.Author.Email.Contains(searchStr)));

            int pageSize = 5;
            int pageNumber = page ?? 1;

            //IList<BlogPost> plist = result.ToList();

            return View(listPosts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));

        }


        public ActionResult Details(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost post = db.BlogPosts.FirstOrDefault(p => p.Slug == slug);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        // GET: BlogPosts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Created,Updated,Title,Slug,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                // restricting the valid file formats to images only
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/images/blog/"), fileName));
                    blogPost.MediaURL = "~/images/blog/" + fileName;
                }

                var Slug = StringUtilities.URLFriendly(blogPost.Title);

                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPost);
                }
                if (db.BlogPosts.Any(b => b.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique.");
                    return View(blogPost);
                }

                blogPost.Slug = Slug;
                blogPost.Created = new DateTimeOffset(DateTime.Now);
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Created,Updated,Title,Slug,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {

                // restricting the valid file formats to images only
                if (ImageUploadValidator.IsWebFriendlyImage(image))
                {
                    var fileName = Path.GetFileName(image.FileName);
                    image.SaveAs(Path.Combine(Server.MapPath("~/images/blog/"), fileName));
                    blogPost.MediaURL = "~/images/blog/" + fileName;
                }

                blogPost.Updated = new DateTimeOffset(DateTime.Now);
                db.Entry(blogPost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }

        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
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
