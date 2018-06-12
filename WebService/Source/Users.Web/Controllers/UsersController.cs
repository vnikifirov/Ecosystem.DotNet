using System;
using System.Net;
using System.Web.Mvc;
using UserClient.MVC.BLL;
using Users.Web.MVC.BLL;
using Users.Web.MVC.Models;

namespace Users.Web.MVC.Controllers
{
    //[RouteArea("Users.Web")]
    public class UsersController : Controller
    {
        // W/O DI
        private static UsersList _userList = new UsersList();
        private static UserService _service = new UserService();

        // GET: Index
        [Route("")]
        [Route("Users")]
        [Route("Users/Index")]
        public ActionResult Index()
        {
            return View();
        }

        // GET: list
        [HttpGet]
        [Route("Users/List")]
        public JsonResult GetUsersList()
        {
            // Calling and storing web service output            
            try
            {
                _userList.GetUsers(_service);
                Response.StatusCode = (int)HttpStatusCode.OK;
                return Json(_userList.Users, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(ex.ToString(), JsonRequestBehavior.AllowGet);
            }
        }

        // POST: update
        [HttpPost]
        [Route("Users/Update")]
        public ActionResult Update(User user)
        {
            try
            {
                _userList.Users.RemoveAll(x => x.Picture == user.Picture);
                _userList.Users.Add(user);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }

        // POST: delete
        [HttpPost]
        [Route("Users/Delete")]
        public ActionResult Delete(User user)
        {
            try
            {
                _userList.Users.RemoveAll(x => x.Picture == user.Picture);
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.ToString());
            }
        }
    }
}