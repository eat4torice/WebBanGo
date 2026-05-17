using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanGo.Data;

namespace WebBanGo.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Products
        public async Task<IActionResult> Index(int? categoryId, string? search, int page = 1)
        {
            const int pageSize = 12;

            var query = _context.Products
                .Include(p => p.Category)
                .Where(p => p.IsActive);

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId.Value);
                ViewBag.CurrentCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == categoryId.Value);
            }

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Name.Contains(search) 
                    || p.WoodType.Contains(search));
                ViewBag.SearchQuery = search;
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var products = await query
                .OrderBy(p => p.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.Categories = await _context.Categories
                .Where(c => c.ParentId == null)
                .Include(c => c.SubCategories)
                .ToListAsync();

            return View(products);
        }

        // GET: /Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            // Sản phẩm liên quan (cùng danh mục)
            ViewBag.RelatedProducts = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsActive)
                .Take(4)
                .ToListAsync();

            return View(product);
        }
    }
}
