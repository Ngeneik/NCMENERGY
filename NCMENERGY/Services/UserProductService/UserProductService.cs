using Microsoft.EntityFrameworkCore;
using NCMENERGY.Data;
using NCMENERGY.Dtos;
using NCMENERGY.Response;

namespace NCMENERGY.Services.UserProductService
{
    public class UserProductService : IUserProductService
    {
        private readonly ApplicationDbContext _context;
        public UserProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GenericResponse> BuildSystem()
        {
            var Inverters = await _context.Products
                .Where(p => p.Related == "Inverters")
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .Select(p => new
                {
                    p.Id,
                    p.ProductName,
                    p.Price,
                    p.ThumbNail,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .ToListAsync();

            var Batteries = await _context.Products
                .Where(p => p.Related == "Batteries")
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .Select(p => new
                {
                    p.Id,
                    p.ProductName,
                    p.Price,
                    p.ThumbNail,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .ToListAsync();

            var Pannels = await _context.Products
                .Where(p => p.Related == "Panels")
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .Select(p => new
                {
                    p.Id,
                    p.ProductName,
                    p.Price,
                    p.ThumbNail,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .ToListAsync();

            return new GenericResponse
            {
                Success = true,
                Data = new
                {
                    Inverters,
                    Batteries,
                    Pannels
                }
            };
        }
        public async Task<GenericResponse> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .Select(p => new
                {
                    p.Id,
                    p.Category,
                    p.BrandName,
                    p.ProductName,
                    p.ThumbNail,
                    p.PercentOff,
                    p.ReviewPoint,
                    p.ReviewCount,
                    p.Price,
                    p.SlashedPrice,
                    p.Warranty,
                    p.InStock,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .Take(100) // optional limit for performance
                .ToListAsync();

            // shuffle in memory
            products = products.OrderBy(_ => Guid.NewGuid()).ToList();

            return new GenericResponse
            {
                Success = true,
                Data = products
            };
        }

        public async Task<GenericResponse> GetCartItems(GetCartDto request)
        {
            if (request?.CartIds == null || !request.CartIds.Any())
            {
                return new GenericResponse
                {
                    Success = false,
                    Message = "No cart items provided"
                };
            }

            var result = new List<object>();

            foreach (var cartItem in request.CartIds)
            {
                var product = await _context.Products
                    .Include(p => p.Specifications)
                        .ThenInclude(s => s.ProductSpecs)
                    .FirstOrDefaultAsync(p => p.Id == cartItem.Id);

                if (product == null) continue;

                result.Add(new
                {
                    product.Id,
                    ProductName = product.ProductName,
                    product.Price,
                    product.SlashedPrice,
                    product.ThumbNail,

                    Specifications = product.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                });
            }

            return new GenericResponse
            {
                Success = true,
                Data = result
            };
        }

        public async Task<GenericResponse> GetIndexProductById(Guid productId)
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

            var data = new
            {
                product.Id,
                product.BrandName,
                product.Category,
                product.Related,
                product.ReviewPoint,
                product.ReviewCount,
                product.Price,
                product.SlashedPrice,
                product.PercentOff,
                product.Yousave,
                product.InStock,
                product.Warranty,
                product.Description,
                InstallationNote = product.InstallationNote,
                product.ThumbNail,
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
        public async Task<GenericResponse> GetIndexProducts()
        {
            var products = await _context.Products
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .OrderBy(p => p.CreatedAt) // or OrderByDescending for latest
                .Take(4)
                .Select(p => new
                {
                    p.Id,
                    p.Category,
                    p.BrandName,
                    p.ProductName,
                    p.ThumbNail,
                    p.PercentOff,
                    p.ReviewPoint,
                    p.ReviewCount,
                    p.Price,
                    p.SlashedPrice,
                    p.Warranty,
                    p.InStock,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .ToListAsync();

            // shuffle in memory
            products = products.OrderBy(_ => Guid.NewGuid()).ToList();

            return new GenericResponse
            {
                Success = true,
                Data = products
            };
        }
        public async Task<GenericResponse> GetRelatedProducts(string relation)
        {
            var products = await _context.Products
                .Where(p => p.Related == relation)
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .Select(p => new
                {
                    p.Id,
                    p.Category,
                    p.BrandName,
                    p.ProductName,
                    p.ThumbNail,
                    p.PercentOff,
                    p.ReviewPoint,
                    p.ReviewCount,
                    p.Price,
                    p.SlashedPrice,
                    p.Warranty,
                    p.InStock,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .ToListAsync();

            // shuffle in memory
            products = products.OrderBy(_ => Guid.NewGuid()).ToList();

            return new GenericResponse
            {
                Success = true,
                Data = products
            };
        }
        public async Task<GenericResponse> GetRelatedProductsIndex(string relation)
        {
            var products = await _context.Products
                .Where(p => p.Related == relation)
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .OrderBy(p => p.CreatedAt) // or OrderByDescending for latest
                .Take(4)
                .Select(p => new
                {
                    p.Id,
                    p.Category,
                    p.BrandName,
                    p.ProductName,
                    p.ThumbNail,
                    p.PercentOff,
                    p.ReviewPoint,
                    p.ReviewCount,
                    p.Price,
                    p.SlashedPrice,
                    p.Warranty,
                    p.InStock,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3) // up to 3 values
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .ToListAsync();

            // shuffle in memory
            products = products.OrderBy(_ => Guid.NewGuid()).ToList();

            return new GenericResponse
            {
                Success = true,
                Data = products
            };
        }

        public async Task<GenericResponse> SearchQuery(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new GenericResponse { Success = true, Data = new List<object>() };

            query = query.Trim().ToLower(); // normalize

            var products = await _context.Products
                .Include(p => p.Specifications)
                    .ThenInclude(s => s.ProductSpecs)
                .Where(p =>
                    EF.Functions.Like(p.Category!.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(p.BrandName!.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(p.ProductName!.ToLower(), $"%{query}%") ||
                    EF.Functions.Like(p.Description!.ToLower(), $"%{query}%")
                )
                .Select(p => new
                {
                    p.Id,
                    p.Category,
                    p.BrandName,
                    p.ProductName,
                    p.ThumbNail,
                    p.PercentOff,
                    p.ReviewPoint,
                    p.ReviewCount,
                    p.Price,
                    p.SlashedPrice,
                    p.Warranty,
                    p.InStock,
                    Specifications = p.Specifications
                        .SelectMany(s => s.ProductSpecs)
                        .Take(3)
                        .Select(ps => ps.Value)
                        .ToList()
                })
                .Take(100)
                .ToListAsync();

            return new GenericResponse { Success = true, Data = products };
        }
    }
}
