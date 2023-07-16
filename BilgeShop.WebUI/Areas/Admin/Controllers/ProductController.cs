using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.WebUI.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BilgeShop.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _environment;
        private readonly IProductService _productService;
        public ProductController(ICategoryService categoryService, IWebHostEnvironment environment, IProductService productService)
        {
            _categoryService = categoryService;
            _environment = environment;
            _productService = productService;

        }

        public IActionResult List()
        {
            var productDtos = _productService.GetProducts();

            var viewModel= productDtos.Select(x=> new ProductListViewModel
            {
                Id= x.Id,
                Name= x.Name,
                CategoryId= x.CategoryId,
                CategoryName= x.CategoryName,
                UnitInStock= x.UnitInStock,
                UnitPrice= x.UnitPrice,
                ImagePath= x.ImagePath,
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult New()
        {
            ViewBag.Categories= _categoryService.GetCategories();
            return View("Form",new ProductFormViewModel());
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var editProductDto = _productService.GetProductById(id);

            var viewModel = new ProductFormViewModel()
            {
                Id = editProductDto.Id,
                Name = editProductDto.Name,
                CategoryId = editProductDto.CategoryId,
                Description = editProductDto.Description,
                UnitPrice = editProductDto.UnitPrice,
                UnitStock = editProductDto.UnitInStock
            };

            ViewBag.Categories = _categoryService.GetCategories();

            ViewBag.ImagePath = editProductDto.ImagePath;

            return View("Form",viewModel);
        }

        [HttpPost]
        public IActionResult Save(ProductFormViewModel formData)
        {
            if (!ModelState.IsValid)
            {
                if(formData.CategoryId==0)
                {
                    ViewBag.CatError = "Kategori alanı zorunludur";
                }
                ViewBag.Categories = _categoryService.GetCategories();
                return View("Form", formData);
            }

            var newFileName = "";

            if (formData.File is not null)
            {
                var allowedFileTypes = new string[] { "image/jpeg", "image/jpg", "image/png", "image/jfif" };

                var allowedFileExtensions = new string[] { ".jpg", ".jpeg", ".png", ".jfif" };

                var fileContentType=formData.File.ContentType;

                var fileNameWithoutExtension=Path.GetFileNameWithoutExtension(formData.File.FileName);

                var fileExtensions=Path.GetExtension(formData.File.FileName);

                if (!allowedFileTypes.Contains(fileContentType) || !allowedFileExtensions.Contains(fileExtensions))
                {
                    ViewBag.FileError = "Geçersiz dosya formatı.";

                    ViewBag.Categories=_categoryService.GetCategories();

                    return View("Form",formData);
                }

                newFileName=fileNameWithoutExtension+"-"+Guid.NewGuid()+fileExtensions;

                var folderPath = Path.Combine("images", "products");

                var wwwrootFolderpath=Path.Combine(_environment.WebRootPath, folderPath);

                var wwwrootFilePath = Path.Combine(wwwrootFolderpath, newFileName);

                Directory.CreateDirectory(wwwrootFolderpath);

                using(var fileStream=new FileStream(wwwrootFilePath,FileMode.Create))
                {
                    formData.File.CopyTo(fileStream);
                }
            }

            if (formData.Id==0)
            {
                var addProductDto = new AddProductDto()
                {
                    Name = formData.Name,
                    Description = formData.Description,
                    UnitInStock = formData.UnitStock,
                    UnitPrice = formData.UnitPrice,
                    CategoryId = formData.CategoryId,
                    ImagePath=newFileName
                };

                _productService.AddProduct(addProductDto);
                RedirectToAction("List");
            }
            else
            {
                var editProductDto = new EditProductDto()
                {
                    Id = formData.Id,
                    Name = formData.Name,
                    Description = formData.Description,
                    UnitInStock = formData.UnitStock,
                    UnitPrice = formData.UnitPrice,
                    CategoryId = formData.CategoryId,
                };

                if (formData.File is not null)
                {
                    editProductDto.ImagePath = newFileName;
                }

                _productService.EditProduct(editProductDto);
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            _productService.DeleteProduct(id);

            return RedirectToAction("List");
        }
    }
}
