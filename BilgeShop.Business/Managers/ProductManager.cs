using BilgeShop.Business.Dtos;
using BilgeShop.Business.Services;
using BilgeShop.Data.Entities;
using BilgeShop.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Business.Managers
{
    public class ProductManager : IProductService
    {
        private readonly IRepository<ProductEntity> _productRepository;
        public ProductManager(IRepository<ProductEntity> productRepository)
        {
            _productRepository = productRepository;
        }
        public void AddProduct(AddProductDto addProductDto)
        {
            var productEntity = new ProductEntity()
            {
                Name = addProductDto.Name,
                Description = addProductDto.Description,
                UnitInStock = addProductDto.UnitInStock,
                UnitPrice = addProductDto.UnitPrice,
                CategoryId = addProductDto.CategoryId,
                ImageUrl = addProductDto.ImagePath
            };

            _productRepository.Add(productEntity);
            
        }

        public void DeleteProduct(int id)
        {
            _productRepository.Delete(id);
        }

        public void EditProduct(EditProductDto editProductDto)
        {
            var productEntity = _productRepository.GetById(editProductDto.Id);

            productEntity.Name = editProductDto.Name;
            productEntity.Description = editProductDto.Description;
            productEntity.UnitPrice = editProductDto.UnitPrice;
            productEntity.UnitInStock= editProductDto.UnitInStock;
            productEntity.CategoryId = editProductDto.CategoryId;

            if(editProductDto.ImagePath != null)
            {
                productEntity.ImageUrl = editProductDto.ImagePath;
            }

            _productRepository.Update(productEntity);
        }

        public EditProductDto GetProductById(int id)
        {
            var productEntity=_productRepository.GetById(id);
            var editProductDto = new EditProductDto()
            {
                Id = productEntity.Id,
                Name = productEntity.Name,
                Description = productEntity.Description,
                UnitInStock = productEntity.UnitInStock,
                UnitPrice = productEntity.UnitPrice,
                CategoryId = productEntity.CategoryId,
                ImagePath=productEntity.ImageUrl
            };

            return editProductDto;
        }

        public List<ListProductDto> GetProducts()
        {
            var productEntities=_productRepository.GetAll().OrderBy(x=> x.Category.Name).ThenBy(x=> x.Name);

            var productDtoList=productEntities.Select(x=> new ListProductDto()
            {
                Id = x.Id,
                Name = x.Name,
                UnitPrice= x.UnitPrice, 
                CategoryId = x.CategoryId,
                UnitInStock=x.UnitInStock,
                CategoryName=x.Category.Name,
                ImagePath=x.ImageUrl

            }).ToList();

            return productDtoList;
        }

        public List<ListProductDto> GetProductsByCategoryId(int? categoryId)
        {
            if(categoryId.HasValue)
            {
                var productEntites = _productRepository.GetAll(x => x.CategoryId == categoryId).OrderBy(x => x.Name);

                var productDtos = productEntites.Select(x => new ListProductDto()
                {
                    Id = x.Id,
                    Name = x.Name,
                    UnitPrice = x.UnitPrice,
                    CategoryId = x.CategoryId,
                    UnitInStock = x.UnitInStock,
                    CategoryName = x.Category.Name,
                    ImagePath = x.ImageUrl
                }).ToList();

                return productDtos;
            }
            else
            {
                return GetProducts();
            }
        }
    }
}
