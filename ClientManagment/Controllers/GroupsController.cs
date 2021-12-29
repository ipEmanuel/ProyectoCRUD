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
        public IActionResult GetClientsListAsync(string filter = "", int pageIndex = 1)
        {
           ViewBag.clients = (PagingList<Client>)GetClients(filter, pageIndex).Result;
           return PartialView("~/Views/Groups/_ClientsList.cshtml");
        }

        public async Task<PagingList<Client>> GetClients(string filter = "", int pageIndex = 1)
        {
            var qry = _context.Clients.AsNoTracking().OrderBy(c => c.Surname).ThenBy(c => c.Name);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToUpper();
                qry = (IOrderedQueryable<Client>)qry.Where(c => c.Name.Contains(filter) || c.DocumentNumber.Contains(filter) || c.Surname.Contains(filter) || c.Patent.Contains(filter));
            }

            var model = await PagingList.CreateAsync(qry, 9, pageIndex);

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

                var dbClients = new List<Client>();

                if (clientsid.Count() > 0)
                    dbClients = await _context.Clients.ToListAsync();

                foreach (var clientId in clientsid)
                {
                    Client client = dbClients.Find(Guid.Parse(clientId.Value));

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

            var @group = await _context.Groups.Include(g => g.Clients).FirstOrDefaultAsync(g => g.GroupId == id);

            ViewBag.clients = (PagingList<Client>)GetClients(string.Empty, 1).Result;
            ViewBag.existingClients = @group.Clients.Select(c => c.ClientId).ToList();

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
        public async Task<IActionResult> Edit()
        {
            try
            {
                Guid groupId = Guid.Parse(Request.Form["groupId"]);
                Group @group = await _context.Groups.Include(g => g.Clients).FirstOrDefaultAsync(g => g.GroupId == groupId);

                if (@group == null)
                {
                    return NotFound();
                }

                var clientsid = JsonConvert.DeserializeObject<List<Guid>>(Request.Form["clientsId"]);
                string groupName = Request.Form["groupName"];
                string groupDescription = Request.Form["groupDescription"];

                List<Guid> newClientsIds = new List<Guid>();

                @group.Name = groupName;
                @group.Description = groupDescription;

                var dbClients = new List<Client>();

                if (clientsid.Count() > 0)
                    dbClients = await _context.Clients.ToListAsync();

                foreach (var clientId in clientsid)
                {
                    newClientsIds.Add(clientId);

                    //me fijo si el cliente que viene ya está en el grupo
                    var existe = @group.Clients.ToList().FirstOrDefault(c => c.ClientId == clientId);

                    if (existe == null)
                    {
                        //me quedo con el cliente que voy a agregar
                        Client client = dbClients.FirstOrDefault(c => c.ClientId == clientId);

                        if (client != null)
                            group.Clients.Add(client);
                    }
                }

                //quedate con los clientes que existan actualmente en el grupo pero que no lleguen chequeados (quiere decir que los deschequié)
                var result = @group.Clients.Where(p => newClientsIds.All(p2 => p2 != p.ClientId));

                foreach (var client in result)
                {
                    @group.Clients.Remove(client);
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
            catch(Exception ex)
            {
                throw ex;
            }

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
