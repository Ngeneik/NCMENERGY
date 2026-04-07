using Microsoft.EntityFrameworkCore;
using NCMENERGY.Data;
using NCMENERGY.Dtos;
using NCMENERGY.Models;
using NCMENERGY.Response;
using NCMENERGY.Services.FileUploadService;
using System.Text.Json;

namespace NCMENERGY.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        private readonly IWebHostEnvironment _env;

        public ProductService(ApplicationDbContext context, IFileUploadService fileUploadService, IWebHostEnvironment env)
        {
            _context = context;
            _fileUploadService = fileUploadService;
            _env = env;
        }

        public async Task<GenericResponse> CreateProduct(CreateProductDto request)
        {
            string? thumbNailUrl = null;

            // 1. Upload thumbnail if provided
            if (request.ThumbNail != null)
            {
                var thumbResponse = await _fileUploadService.UploadFile(new FileUploadDto
                {
                    Files = new List<IFormFile> { request.ThumbNail }
                });

                if (thumbResponse.Success && thumbResponse.Data is List<string> thumbUrls && thumbUrls.Any())
                {
                    thumbNailUrl = thumbUrls.First();
                }
            }

            // 2. Calculate SlashedPrice and Yousave
            decimal slashedPrice = request.Price;
            decimal youSave = 0;

            if (request.PercentOff.HasValue && request.PercentOff.Value > 0)
            {
                slashedPrice = request.Price + (request.Price * request.PercentOff.Value / 100);
                youSave = slashedPrice - request.Price;
            }

            // 3. Create product
            var product = new Product
            {
                Category = request.Category,
                BrandName = request.BrandName,
                ProductName = request.ProductName,
                ReviewPoint = request.ReviewPoint,
                ReviewCount = request.ReviewCount,
                Price = request.Price,
                SlashedPrice = slashedPrice,
                PercentOff = request.PercentOff,
                Stock = request.Stock,
                Yousave = youSave,
                InStock = request.InStock,
                Warranty = request.Warranty,
                Description = request.Description,
                InstallationNote = request.InstallationNote,
                Related = request.Related,
                ThumbNail = thumbNailUrl,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // 4. Upload other images
            if (request.Images != null && request.Images.Any())
            {
                var uploadResponse = await _fileUploadService.UploadFile(new FileUploadDto
                {
                    Files = request.Images
                });

                if (uploadResponse.Success && uploadResponse.Data is List<string> imageUrls)
                {
                    foreach (var url in imageUrls)
                    {
                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = url
                        });
                    }
                }
            }

            // 5. Save specifications (UPDATED)
            if (request.Specifications != null && request.Specifications.Any())
            {
                foreach (var specJson in request.Specifications)
                {
                    var specDto = JsonSerializer.Deserialize<SpecificationDto>(specJson);
                    if (specDto == null) continue;

                    var spec = new Specification
                    {
                        ProductId = product.Id
                    };

                    var productSpec = new ProductSpec
                    {
                        Metrics = specDto.metrics,
                        Value = specDto.value
                    };

                    spec.ProductSpecs.Add(productSpec);
                    _context.Specifications.Add(spec);
                }
            }

            await _context.SaveChangesAsync();

            return new GenericResponse
            {
                Success = true,
                Message = "Product created successfully"
            };
        }
        public async Task<GenericResponse> DeleteProduct(Guid productId)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return new GenericResponse
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            // 1. Delete thumbnail file
            if (!string.IsNullOrEmpty(product.ThumbNail))
            {
                var fileName = Path.GetFileName(product.ThumbNail);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // 2. Delete image files
            foreach (var image in product.Images)
            {
                if (string.IsNullOrEmpty(image.ImageUrl)) continue;

                var fileName = Path.GetFileName(image.ImageUrl);
                var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            // 3. Remove images from DB
            _context.ProductImages.RemoveRange(product.Images);

            // 4. Remove specifications and ProductSpecs
            foreach (var spec in product.Specifications)
            {
                if (spec.ProductSpecs != null && spec.ProductSpecs.Any())
                {
                    _context.Set<ProductSpec>().RemoveRange(spec.ProductSpecs);
                }
            }
            _context.Specifications.RemoveRange(product.Specifications);

            // 5. Remove product
            _context.Products.Remove(product);

            await _context.SaveChangesAsync();

            return new GenericResponse
            {
                Success = true,
                Message = "Product deleted successfully"
            };
        }
        public async Task<GenericResponse> EditProduct(Guid productId, CreateProductDto request)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return new GenericResponse
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            // 1. Calculate SlashedPrice and Yousave
            decimal slashedPrice = request.Price;
            decimal youSave = 0;

            if (request.PercentOff.HasValue && request.PercentOff.Value > 0)
            {
                slashedPrice = request.Price + (request.Price * request.PercentOff.Value / 100);
                youSave = slashedPrice - request.Price;
            }

            // 2. Update product fields
            product.Category = request.Category;
            product.BrandName = request.BrandName;
            product.ProductName = request.ProductName;
            product.ReviewPoint = request.ReviewPoint;
            product.ReviewCount = request.ReviewCount;
            product.Price = request.Price;
            product.SlashedPrice = slashedPrice;
            product.PercentOff = request.PercentOff;
            product.Stock = request.Stock;
            product.Yousave = youSave;
            product.InStock = request.InStock;
            product.Warranty = request.Warranty;
            product.Description = request.Description;
            product.InstallationNote = request.InstallationNote;
            product.Related = request.Related;
            product.ModifiedAt = DateTime.UtcNow;

            // 3. Update thumbnail if provided
            if (request.ThumbNail != null)
            {
                if (!string.IsNullOrEmpty(product.ThumbNail))
                {
                    var fileName = Path.GetFileName(product.ThumbNail);
                    var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                var thumbResponse = await _fileUploadService.UploadFile(new FileUploadDto
                {
                    Files = new List<IFormFile> { request.ThumbNail }
                });

                if (thumbResponse.Success && thumbResponse.Data is List<string> thumbUrls && thumbUrls.Any())
                {
                    product.ThumbNail = thumbUrls.First();
                }
            }

            // 4. Replace images if new ones are provided
            if (request.Images != null && request.Images.Any())
            {
                foreach (var image in product.Images)
                {
                    if (!string.IsNullOrEmpty(image.ImageUrl))
                    {
                        var fileName = Path.GetFileName(image.ImageUrl);
                        var filePath = Path.Combine(_env.WebRootPath, "uploads", fileName);
                        if (System.IO.File.Exists(filePath))
                            System.IO.File.Delete(filePath);
                    }
                }

                _context.ProductImages.RemoveRange(product.Images);

                var uploadResponse = await _fileUploadService.UploadFile(new FileUploadDto
                {
                    Files = request.Images
                });

                if (uploadResponse.Success && uploadResponse.Data is List<string> imageUrls)
                {
                    foreach (var url in imageUrls)
                    {
                        _context.ProductImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = url
                        });
                    }
                }
            }

            // 5. Replace specifications if new ones are provided
            if (request.Specifications != null && request.Specifications.Any())
            {
                foreach (var spec in product.Specifications)
                {
                    if (spec.ProductSpecs != null && spec.ProductSpecs.Any())
                    {
                        _context.Set<ProductSpec>().RemoveRange(spec.ProductSpecs);
                    }
                }

                _context.Specifications.RemoveRange(product.Specifications);

                foreach (var specJson in request.Specifications)
                {
                    var specDto = JsonSerializer.Deserialize<SpecificationDto>(specJson);
                    if (specDto == null) continue;

                    var spec = new Specification
                    {
                        ProductId = product.Id
                    };

                    var productSpec = new ProductSpec
                    {
                        Metrics = specDto.metrics,
                        Value = specDto.value
                    };

                    spec.ProductSpecs.Add(productSpec);
                    _context.Specifications.Add(spec);
                }
            }

            await _context.SaveChangesAsync();

            return new GenericResponse
            {
                Success = true,
                Message = "Product updated successfully"
            };
        }
        public async Task<GenericResponse> ViewAllProducts()
        {
            var products = await _context.Products
                .Select(p => new
                {
                    p.Id,
                    p.ProductName,
                    p.Category,
                    p.Price,
                    p.Stock
                })
                .ToListAsync();

            return new GenericResponse
            {
                Success = true,
                Data = products
            };
        }
        public async Task<GenericResponse> ViewProductById(Guid productId)
        {
            var product = await _context.Products
                .Include(p => p.Images)
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return new GenericResponse
                {
                    Success = false,
                    Message = "Product not found"
                };
            }

            // return product with images, specifications, Related, and ThumbNail
            var data = new
            {
                product.Id,
                product.Category,
                product.BrandName,
                product.ProductName,
                product.ReviewPoint,
                product.ReviewCount,
                product.Price,
                product.SlashedPrice,
                product.PercentOff,
                product.Stock,
                product.Yousave,
                product.InStock,
                product.Warranty,
                product.Description,
                product.InstallationNote,
                product.Related,
                product.ThumbNail,
                product.CreatedAt,
                product.ModifiedAt,
                Images = product.Images.Select(i => i.ImageUrl).ToList(),
                Specifications = product.Specifications.Select(s => new
                {
                    s.Id,
                    ProductSpecs = s.ProductSpecs.Select(ps => new
                    {
                        ps.Id,
                        ps.Metrics,
                        ps.Value
                    }).ToList()
                }).ToList()
            };

            return new GenericResponse
            {
                Success = true,
                Data = data
            };
        }
    }
}
