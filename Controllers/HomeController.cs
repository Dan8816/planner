using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Planner.Models;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Planner.Controllers
{

    public class HomeController : Controller
    {
        private YourContext _context;

        public HomeController(YourContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Route("Dashboard")]
        public IActionResult Dashboard()
        {
            System.Console.WriteLine("**********Hitting the Dashboard route in HomeController**********");
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }

            DashboardModel view = new DashboardModel()//instantiated a new instance called view of the DashboardModel class
            {
                users = new User(),//users = User object with inheritance from User class elsewhere
                weddings = new Wedding(),//weddings = Wedding object with inheritance from Wedding class elsewhere
                rsvps = new RSVP()////rsvps = RSVP objects with inheritance from RSVP class elsewhere
            };

            int? user_id = HttpContext.Session.GetInt32("user_id");//this says the nullable int called user_id is the value of the session key value called user_id
            User curruser = _context.users.Where(u => u.Id == user_id).SingleOrDefault();//instantiated an instance of User class called curruser from users db table where the id is the session user_id
            List<Wedding> allWeddings = _context.weddings//instantiated an instance of Wedding class callled allWeddings from weddings db table with the following
                                        .Include(w => w.rsvps)//including rsvps as list object from RSVP class
                                        .ThenInclude(g => g.RSVPguest)//this the User object from that class
                                        .ToList();//everything to a list object
            List<RSVP> allRSVPs = _context.rsvps//instantiated an instance of RVSP class call allRSVPs from the rsvps db table
                                    .Include(w => w.RSVPguest).ToList();//includes User objects of this table to a list
            ViewBag.RSVPs = allRSVPs;
            ViewBag.UserId = user_id;
            ViewBag.User = curruser;
            ViewBag.Weddings = allWeddings;
            return View(view);
        }

        [HttpGet]
        [Route("NewWedding")]
        public IActionResult NewWedding()
        {
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }
            return View();//goes to a page called NewWedding
        }

        [HttpPost]
        [Route("CreateWedding")]
        public IActionResult CreateWedding(NewWedding wedding)
        {
            if(HttpContext.Session.GetInt32("user_id") == null) {
                return RedirectToAction("Index", "User");
            }
            int? user_id = HttpContext.Session.GetInt32("user_id");
            if(ModelState.IsValid) {
                Wedding NewWedding = new Wedding
                {
                    WedderOne = wedding.WedderOne,
                    WedderTwo = wedding.WedderTwo,
                    WeddingDate = wedding.WeddingDate,
                    WeddingStrAdd = wedding.WeddingStrAdd,
                    created_at = DateTime.Now,
                    updated_at = DateTime.Now,
                    CreatorId = (int) user_id       
                };
                _context.Add(NewWedding);
                _context.SaveChanges();
                return RedirectToAction("Dashboard", "Home");
            }
            return View("NewWedding"); 
        }

        [HttpGet]
        [Route("Wedding/{WeddingId}")]
        public IActionResult Wedding(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }
            Wedding ThisWedding = _context.weddings//instantiated an instance of Wedding class called ThisWedding
                            .Include(w => w.rsvps)//including List objects of rsvps
                            .ThenInclude(g => g.RSVPguest)//including the instances of the User class
                            .SingleOrDefault(w => w.Id == WeddingId);//with ThisWedding id
            ViewBag.CurrentWedding = ThisWedding;
            ViewBag.WeddingGuests = ThisWedding.rsvps;
            return View();
        }

        [HttpGet]
        [Route("Delete/{WeddingId}")]
        public IActionResult Delete(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }
            Wedding ThisWedding = _context.weddings//instantiated an instance of Wedding class called ThisWedding
                            .Where(w => w.Id == WeddingId).SingleOrDefault();//where id == int WeddingId
            _context.weddings.Remove(ThisWedding);//deletes ThisWedding instance
            _context.SaveChanges();//Actually deletes the db row
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        [Route("RSVP/{WeddingId}")]
        public IActionResult RSVP(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null)//if user tried to hit the route through url without login or reg
            {
                return RedirectToAction("Index", "User");//no login or reg go back to the root index
            }
            int? user_id = HttpContext.Session.GetInt32("user_id");//instantiated an instance of Wedding class called ThisWedding
            User curruser = _context.users.Where(u => u.Id == user_id).SingleOrDefault();
            Wedding ThisWedding = _context.weddings
                            .Include(w => w.rsvps)
                            .ThenInclude(g => g.RSVPguest)
                            .SingleOrDefault(w => w.Id == WeddingId);
            RSVP newRSVP = new RSVP
            {
                RSVPguestId = curruser.Id,
                RSVPguest = curruser,
                WeddingId = ThisWedding.Id,
                weddings = ThisWedding
            };
            ThisWedding.rsvps.Add(newRSVP);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        

        [HttpGet]
        [Route("Undo/{WeddingId}")]
        public IActionResult Undo(int WeddingId)
        {
            if(HttpContext.Session.GetInt32("user_id") == null) {
                return RedirectToAction("Index", "User");
            }
            int? user_id = HttpContext.Session.GetInt32("user_id");
            User curruser = _context.users.Where(u => u.Id == user_id).SingleOrDefault();
            Wedding ThisWedding = _context.weddings
                            .Include(w => w.rsvps)
                            .ThenInclude(g => g.RSVPguest)
                            .SingleOrDefault(w => w.Id == WeddingId);
            RSVP oldRSVP = _context.rsvps.Where(i => i.RSVPguestId == user_id).Where(i => i.WeddingId == WeddingId).SingleOrDefault();
            ThisWedding.rsvps.Remove(oldRSVP);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
