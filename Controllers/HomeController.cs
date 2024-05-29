using LibraryNET.Filters;
using LibraryNET.Implementation;
using LibraryNET.Implementation.AdminManagers;
using System.Linq;
using System.Web.Mvc;

namespace LibraryNET.Controllers
{
    [Session]
    public class HomeController : Controller
    {
        #region HomeMethods
        public ActionResult Index()
        {
            if (Session["UserId"] != null) {
                GenderManager genderManager = new GenderManager();
                UserManager userManager = new UserManager();
                BooksManager booksManager = new BooksManager();
                EditManager editManager = new EditManager();

                int Genders = genderManager.List().Count();
                int Users = userManager.List().Count();
                int Books = booksManager.List().Count();
                int Edits = editManager.List().Count();

                ViewBag.GenderCount = Genders;
                ViewBag.UsersCount = Users;
                ViewBag.BooksCount = Books;
                ViewBag.EditsCount = Edits;

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        #endregion
    }
}