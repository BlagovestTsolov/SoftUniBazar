using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftUniBazar.Data;
using SoftUniBazar.Data.Models;
using SoftUniBazar.Models;
using System.Globalization;
using System.Security.Claims;
using System.Xml.Linq;
using static SoftUniBazar.Data.Constants.DataConstants;

namespace SoftUniBazar.Controllers
{
    [Authorize]
    public class AdController : Controller
    {
        private readonly BazarDbContext context;

        public AdController(BazarDbContext data)
        {
            context = data;
        }

        public async Task<IActionResult> All()
        {
            var ads = await context.Ads
                .AsNoTracking()
                .Select(a => new AdAllViewModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedOn = a.CreatedOn.ToString(NormalDateFormat),
                    Owner = a.Owner.UserName,
                    Price = a.Price,
                    Category = a.Category.Name,
                    ImageUrl = a.ImageUrl
                })
                .ToListAsync();

            return View(ads);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var model = new AdAddViewModel();
            model.Categories = await GetCategoriesAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AdAddViewModel model)
        {
            var ad = new Ad()
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                OwnerId = GetUserId(),
                ImageUrl = model.ImageUrl,
                CreatedOn = DateTime.Now,
                CategoryId = model.CategoryId
            };

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await context.Ads.AddAsync(ad);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> Cart()
        {
            var model = await context.AdBuyers
                .Where(ab => ab.BuyerId == GetUserId())
                .Select(ab => new AdAllViewModel()
                {
                    Id = ab.Ad.Id,
                    Name = ab.Ad.Name,
                })
                .ToListAsync();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ad = await context.Ads
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null)
            {
                return BadRequest();
            }

            var model = new AdAllViewModel()
            {
                Name = ad.Name,
                Description = ad.Description,
                ImageUrl = ad.ImageUrl,
                Price = ad.Price,
                CategoryId = ad.CategoryId
            };
            model.Categories = await GetCategoriesAsync();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AdAllViewModel model, int id)
        {
            var ad = await context.Ads
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null)
            {
                return BadRequest();
            }

            if (ad.OwnerId != GetUserId())
            {
                return Unauthorized();
            }

            ad.Name = model.Name;
            ad.Description = model.Description;
            ad.ImageUrl = model.ImageUrl;
            ad.Price = model.Price;
            ad.CategoryId = model.CategoryId;

            await context.SaveChangesAsync();
            return RedirectToAction(nameof(All));
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var ad = await context.Ads
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null)
            {
                return BadRequest();
            }

            var entity = new AdBuyer()
            {
                AdId = ad.Id,
                BuyerId = GetUserId()
            };

            if (await context.AdBuyers.ContainsAsync(entity))
            {
                return RedirectToAction(nameof(All));
            }

            await context.AdBuyers.AddAsync(entity);
            await context.SaveChangesAsync();

            return RedirectToAction(nameof(Cart));
        }

        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var ad = await context.Ads
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null)
            {
                return BadRequest();
            }

            var entity = new AdBuyer()
            {
                AdId = ad.Id,
                BuyerId = GetUserId()
            };

            if (await context.AdBuyers.ContainsAsync(entity))
            {
                context.AdBuyers.Remove(entity);
                await context.SaveChangesAsync();

            }

            return RedirectToAction(nameof(All));
        }

        private async Task<IList<CategoryViewModel>> GetCategoriesAsync()
        {
            var categories = await context.Categories
                .AsNoTracking()
                .Select(c => new CategoryViewModel()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            return categories;
        }
        private string GetUserId()
            => User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
    }
}
