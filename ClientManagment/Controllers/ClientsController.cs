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
using Microsoft.AspNetCore.Http;
using System.IO;
using ClientManagment.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Text.RegularExpressions;

namespace ClientManagment.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment hostingEnvironment;

        public ClientsController(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            hostingEnvironment = env;
        }

        public async Task<IActionResult> Index(string filter, int pageIndex = 1)
        {
            var qry = _context.Clients.AsNoTracking().OrderBy(c => c.Surname).ThenBy(c => c.Name);

            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.ToUpper();
                //TODO: CUANDO SUBAMOS A PRODUCCION SACAR EL ToUpper()
                qry = (IOrderedQueryable<Client>)qry.Where(c => c.Name.ToUpper().Contains(filter) || c.DocumentNumber.Contains(filter) || c.Surname.ToUpper().Contains(filter) || c.Patent.ToUpper().Contains(filter));
            }

            var model = await PagingList.CreateAsync(qry, 9, pageIndex);

            model.RouteValue = new RouteValueDictionary {
                { "filter", filter}
            };

            return View(model);
        }




        // GET: Clients/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // GET: Clients/Create
        public IActionResult Create()
        {
            return View();
        }

        public IActionResult Imagen(IFormFile file)
        {
            return RedirectToAction("Index");
        }

        // POST: Clients/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ClientId,Name,Surname,DocumentNumber,DateOfBirth,Description,isNotWished,ImagePath,Patent")] Client client, IFormFile file)
        {
            SetAttributesToUpper(client);

            if (ModelState.IsValid)
            {
                client.ClientId = Guid.NewGuid();
                //se agrega el cliente al contexto
                _context.Add(client);
                //acá se guarda el cliente en la base de datos
                await _context.SaveChangesAsync();

                //Código para guardar imagen
                if (file != null && file.Length > 0)
                {
                    await SaveClientImage(file, client);
                }

                //acá llama al método index que nos devuelve la vista index con todos los clientes
                return RedirectToAction(nameof(Index));
            }

            return View(client);
        }

        private async Task SaveClientImage(IFormFile file, Client client)
        {
            //nos quedamos con el nombre original del archivo
            FileInfo fi = new FileInfo(file.FileName);

            //asignarle el nuevo nombre al archivo
            var newFilename = client.ClientId + "_" + String.Format("{0:d}", (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;

            //nos quedamos con la carpeta wwwRoot
            var webPath = hostingEnvironment.WebRootPath;

            //nos quedamos con la ruta en la que se va a guardar la imagen
            var path = Path.Combine("", webPath + @"\clientsimages\" + newFilename);

            //acá le decimos qué es lo que vamos a guardar en wwwroot
            var pathToSave = @"/clientsimages/" + newFilename;

            //se guarda la imagen en la carpeta wwwroot
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //Asignamos la url de la imagen al atributo del cliente
            client.ImagePath = pathToSave;

            //actualizamos al cliente creado el atributo que hace referencia a la ruta de la imagen
            _context.Update(client);
            await _context.SaveChangesAsync();
        }

        // GET: Clients/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
            {
                return NotFound();
            }
            return View(client);
        }

        // POST: Clients/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ClientId,Name,Surname,DocumentNumber,DateOfBirth,Description,isNotWished,ImagePath,Patent")] Client client, IFormFile file)
        {

            SetAttributesToUpper(client);

            if (id != client.ClientId)
            {
                return NotFound();
            }

            //PARA NO PERDER LA IMAGEN QUE TENEMOS
            var auxiliarClientPath = _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == id).Result.ImagePath;

            if (client.ImagePath == null)
                client.ImagePath = auxiliarClientPath;

            if (ModelState.IsValid)
            {
                try
                {
                    if (file != null)
                    {
                        if(client.ImagePath == null)
                        {
                            string clientOriginalImagePath = _context.Clients.AsNoTracking().FirstOrDefaultAsync(c => c.ClientId == client.ClientId).Result.ImagePath;
                            client.ImagePath = clientOriginalImagePath;
                        }
                        
                        client.ImagePath = CargarArchivos(file, client, 1);
                    }

                    _context.Update(client);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ClientExists(client.ClientId))
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
            return View(client);
        }

        // GET: Clients/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var client = await _context.Clients
                .FirstOrDefaultAsync(m => m.ClientId == id);
            if (client == null)
            {
                return NotFound();
            }

            return View(client);
        }

        // POST: Clients/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var client = await _context.Clients.FindAsync(id);
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ClientExists(Guid id)
        {
            return _context.Clients.Any(e => e.ClientId == id);
        }

        private string CargarArchivos(IFormFile file, Client clientToUpdate, int n)
        {
            string newImagePath = string.Empty;

            if (file != null && file.Length > 0)
            {
                eliminarArchivoAnterior(clientToUpdate, n);

                newImagePath = AgregarArchivoNuevo(clientToUpdate, file, n);
            }

            return newImagePath;
        }

        private void eliminarArchivoAnterior(Client clientToUpdate, int n)
        {
            if (clientToUpdate.ImagePath != null)
            {
                string fileName = ("wwwroot" + clientToUpdate.ImagePath);

                if (fileName != null && fileName != string.Empty)
                {
                    if ((System.IO.File.Exists(fileName)))
                    {
                        System.IO.File.Delete(fileName);
                    }
                }
            }
        }

        private string AgregarArchivoNuevo(Client clientToUpdate, IFormFile file, int n)
        {
            // Code to upload image
            if (file != null && file.Length > 0)
            {
                FileInfo fi = new FileInfo(file.FileName);
                var newFilename = clientToUpdate.ClientId + "_" + String.Format("{0:d}", (DateTime.Now.Ticks / 10) % 100000000) + fi.Extension;
                var webPath = hostingEnvironment.WebRootPath;
                var path = Path.Combine("", webPath + @"\clientsimages\" + newFilename);
                var pathToSave = @"/clientsimages/" + newFilename;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                if (n == 1)
                {
                    //Código agregado por si tuviera + de una imagen
                    //if (prendaToUpdate.ImagePath == null)
                    //    prendaToUpdate.ImagePaths = new List<string>();

                    //prendaToUpdate.ImagePaths.Add(prendaToUpdate.ImagePath);
                    clientToUpdate.ImagePath = pathToSave;
                }

            }

            //transportBillControlToUpdate.Files.Add(file);
            //_context.Entry(prendaToUpdate).State = EntityState.Modified;
            return clientToUpdate.ImagePath;
        }

        private string UploadedFile(EmployeeViewModel model)
        {
            string uniqueFileName = null;
            var webPath = hostingEnvironment.WebRootPath;
            if (model.ProfileImage != null)
            {
                string uploadsFolder = Path.Combine(webPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfileImage.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfileImage.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        private void SetAttributesToUpper(Client client)
        {
            client.Name = client.Name.ToUpper();
            client.Name = Regex.Replace(client.Name, @"\s+", " ");

            client.Surname = client.Surname.ToUpper();
            client.Surname = Regex.Replace(client.Surname, @"\s+", " ");

            client.DocumentNumber = client.DocumentNumber.ToUpper();
            client.DocumentNumber = Regex.Replace(client.DocumentNumber, @"\s+", " ");

            if (!string.IsNullOrWhiteSpace(client.Patent))
            {
                client.Patent = client.Patent.ToUpper();
                client.Patent = Regex.Replace(client.Patent, @"\s+", " ");
            }

            if (!string.IsNullOrWhiteSpace(client.Description))
            {
                client.Description = client.Description.ToUpper();
                client.Description = Regex.Replace(client.Description, @"\s+", " ");
            }
        }
    }
}
