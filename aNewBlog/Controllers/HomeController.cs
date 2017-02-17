using aNewBlog.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using aNewBlog;
using System.Threading.Tasks;

namespace aNewBlog.Controllers
{
    
    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contact([Bind(Include = "Id,FirstName,LastName,Email,Message,Phone")] Contact contact)
        {
            contact.Created = DateTime.Now;
            var newContact = contact.FirstName + " " + contact.LastName;

            var svc = new EmailService();
            var msg = new IdentityMessage();
            msg.Subject = "Contact From Portfolio Site";
            msg.Body = "\r\n You have recieved a request to contact from " + newContact + "(" + contact.Email + ")" + "\r\n"
                         + "\r\n With the following message: \r\n\t"
                         + contact.Message;


            await svc.SendAsync(msg);

            return View(contact);
        }

        public ActionResult MyAction()
        {
            ViewBag.Message = "Yeeeah. I'm pretty sure ... I RULE!!";

            return View();

        }

        public ActionResult FileUpload()
        {
            return View();
        }

       
    }
}