using ImageUplaodingASPCore.Models;
using Microsoft.AspNetCore.Mvc;

namespace ImageUplaodingASPCore.Controllers
{
    public class ProductController : Controller
    {
        imageDbContext context;
        IWebHostEnvironment env;

        public ProductController(imageDbContext context, IWebHostEnvironment env)
        {
            this.context = context;
            this.env = env;
        }
        public IActionResult Index()
        {
            return View(context.Products.ToList());
        }

        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProduct(ProductViewModel prod)
        {
            string fileName = "";
            if (prod.Photo != null)
            {
                var ext = Path.GetExtension(prod.Photo.FileName);
                var size = prod.Photo.Length;

                if (ext.Equals(".png") || ext.Equals(".jpg") || ext.Equals(".jpeg"))
                {
                    if (size <= 1000000)
                    {
                        string folder = Path.Combine(env.WebRootPath, "images");
                        fileName = Guid.NewGuid().ToString() + "_" + prod.Photo.FileName;
                        string filePath = Path.Combine(folder, fileName);
                        prod.Photo.CopyTo(new FileStream(filePath, FileMode.Create));

                        Product p = new Product()

                        {
                            Name = prod.Name,
                            Price = prod.Price,
                            ImagePath = fileName
                        };
                        context.Products.Add(p);
                        context.SaveChanges();
                        TempData["Success"] = "Product Added.....";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Size_Error"] = "Image Must be less than 1 MB";
                    }
                }
                else
                {
                    TempData["Ext_Error"] = "Only PNG, JPG, JPEG image allowed.";
                }
            }
            return View();
        }
    }
}
