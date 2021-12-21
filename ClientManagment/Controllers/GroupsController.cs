using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClientManagment.Data;
using ClientManagment.Models;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;

namespace ClientManagment.Controllers
{
    public class GroupsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GroupsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Groups
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Groups.ToListAsync());
        //}

        public async Task<IActionResult> Index(string filter, int pageIndex = 1)
        {
            IOrderedQueryable<Group> qry = _context.Groups.AsNoTracking().OrderBy(c => c.Name);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToUpper();
                qry = (IOrderedQueryable<Group>)qry.Where(c => c.Name.ToUpper().Contains(filter));
            }

            var model = await PagingList.CreateAsync(qry, 9, pageIndex);

            model.RouteValue = new RouteValueDictionary {
                { "filter", filter}
            };

            return View(model);
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.Include(g => g.Clients)
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // GET: Groups/Create
        public IActionResult Create()
        {
            ViewBag.clients = (PagingList<Client>)GetClients(string.Empty, 1).Result;
            return View();
        }

        // GET: Groups/GetClientsList
        public IActionResult GetClientsList(int pageIndex = 1)
        {
            ViewBag.clients = (PagingList<Client>)GetClients(string.Empty, pageIndex).Result;
            return PartialView("~/Views/Groups/_ClientsList.cshtml");
            //return new PartialViewResult
            //{
            //    ViewName = "_ClientsList",
            //    ViewBag.clients = (PagingList<Client>)GetClients(string.Empty, pageIndex).Result
            //}
        }

        public async Task<PagingList<Client>> GetClients(string filter, int pageIndex = 1)
        {
            var qry = _context.Clients.AsNoTracking().OrderBy(c => c.Surname).ThenBy(c => c.Name);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToUpper();
                qry = (IOrderedQueryable<Client>)qry.Where(c => c.Name.ToUpper().Contains(filter) || c.DocumentNumber.Contains(filter));
            }

            var model = await PagingList.CreateAsync(qry, 9, pageIndex);

            model.RouteValue = new RouteValueDictionary {
                { "filter", filter}
            };

            return model;
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public async Task<IActionResult> CreateGroup()
        {
            try
            {
                Group @group = new Group();

                dynamic clientsid = JsonConvert.DeserializeObject(Request.Form["clientsId"]);
                string groupName = Request.Form["groupName"];
                string groupDescription = Request.Form["groupDescription"];

                @group.Name = groupName;
                @group.Description = groupDescription;
                group.Clients = new List<Client>();

                foreach (var clientId in clientsid)
                {
                    Client client = await _context.Clients.FindAsync(Guid.Parse(clientId.Value));

                    if (client != null)
                        group.Clients.Add(client);
                }

                if (ModelState.IsValid)
                {
                    @group.GroupId = Guid.NewGuid();
                    _context.Add(@group);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }

                return View(@group);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups.FindAsync(id);
            if (@group == null)
            {
                return NotFound();
            }
            return View(@group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("GroupId,Name,Description")] Group @group)
        {
            if (id != @group.GroupId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(@group.GroupId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @group = await _context.Groups
                .FirstOrDefaultAsync(m => m.GroupId == id);
            if (@group == null)
            {
                return NotFound();
            }

            return View(@group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var @group = await _context.Groups.FindAsync(id);
            _context.Groups.Remove(@group);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(Guid id)
        {
            return _context.Groups.Any(e => e.GroupId == id);
        }
    }
}
