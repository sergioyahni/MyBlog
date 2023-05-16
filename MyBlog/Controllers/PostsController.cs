using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MyBlog.Data;
using MyBlog.Models;

namespace MyBlog.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public PostsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
              return _context.Posts != null ? 
                          View(await _context.Posts.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Author,Date,Title,Image,ImageFile,ImageCredit,Intro,Article")] Posts posts)
        {
            if (ModelState.IsValid)
            {
                if (posts.ImageCredit == null || posts.ImageCredit.Length == 0) 
                {
                    posts.ImageCredit = " ";    
                }

                if(posts.Intro == null || posts.Intro.Length == 0)
                {
                    posts.Intro = " ";
                }

                if (posts.ImageFile == null || posts.ImageFile.Length == 0) 
                {
                    posts.Image = "8b8ddae6-1f5c-4d19-aa30-1fca2aed16cc.jpg";
                }
                else { 
                    // Save Uploaded Image to folder wwwroot/img
                    string wwwRootPath = _hostEnvironment.WebRootPath + "\\img\\"; 
                    string extensions = Path.GetExtension(posts.ImageFile.FileName);
                    string fileName = Guid.NewGuid().ToString();
                    posts.Image = fileName = fileName + extensions;
                    string path = Path.Combine(wwwRootPath, fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create)) 
                    { 
                        await posts.ImageFile.CopyToAsync(fileStream);
                    }
                }

                // Save record
                posts.Id = Guid.NewGuid();
                _context.Add(posts);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(posts);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts.FindAsync(id);
            if (posts == null)
            {
                return NotFound();
            }
            return View(posts);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Author,Date,Title, Image,ImageFile, ImageCredit,Intro,Article")] Posts posts)
        {
            if (id != posts.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (posts.ImageFile != null && posts.ImageFile.Length != 0)
                {
                    // Save Uploaded Image to folder wwwroot/img
                    string wwwRootPath = _hostEnvironment.WebRootPath + "\\img\\";
                    string extensions = Path.GetExtension(posts.ImageFile.FileName);
                    string fileName = Guid.NewGuid().ToString();
                    posts.Image = fileName = fileName + extensions;
                    string path = Path.Combine(wwwRootPath, fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await posts.ImageFile.CopyToAsync(fileStream);
                    }
                }


                try
                {
                    _context.Update(posts);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostsExists(posts.Id))
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
            return View(posts);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var posts = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (posts == null)
            {
                return NotFound();
            }

            return View(posts);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            // Delete image from wwwroot/img if it is no default
            Posts post = new Posts();
            if (post.Image != "8b8ddae6-1f5c-4d19-aa30-1fca2aed16cc.jpg") 
            {
                var imagePath = _hostEnvironment.WebRootPath + "\\img\\" + post.Image;
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            // Delete post from Database
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Posts'  is null.");
            }
            var posts = await _context.Posts.FindAsync(id);
            if (posts != null)
            {
                _context.Posts.Remove(posts);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostsExists(Guid id)
        {
          return (_context.Posts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
