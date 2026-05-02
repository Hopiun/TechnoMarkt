using TechnoMarkt.Areas.Manager.Suppliers.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Forms;

namespace TechnoMarkt.Areas.Manager.Suppliers
{
    public class SuppliersService : ISuppliersService
    {
        private readonly AppDbContext _context;

        public SuppliersService(AppDbContext context) => _context = context;

        public async Task<List<SuppliersTableRow>> GetSuppliersAsync(SuppliersFilter? filter)
        {
            IQueryable<Supplier> query = _context.Suppliers.Include(supplier => supplier.Country).AsQueryable();

            if (filter != null)
            {
                if (!string.IsNullOrEmpty(filter.Search))
                    if (filter.Search.Contains('@'))
                        query = query.Where(supplier => supplier.Contact != null && supplier.Contact.Contains(filter.Search));
                    else
                        query = query.Where(supplier => supplier.Name.Contains(filter.Search) || (supplier.Contact != null && supplier.Contact.Contains(filter.Search)));

                if (filter.MinRating.HasValue && filter.MinRating.Value >= 0 && filter.MinRating.Value <= 5)
                    query = query.Where(supplier => supplier.Rating >= filter.MinRating.Value);

                if (filter.CountryId.HasValue)
                    query = query.Where(supplier => supplier.CountryId == filter.CountryId.Value);
            }

            return await query.Select(supplier => new SuppliersTableRow()
            {
                SupplierId = supplier.SupplierId,
                SupplierName = supplier.Name,
                Contact = supplier.Contact ?? "РќРµ РІРєР°Р·Р°РЅРѕ",
                Country = supplier.Country != null ? supplier.Country.Name : "РќРµРІС–РґРѕРјР° РєСЂР°С—РЅР°",
                Rating = supplier.Rating
            })
            .ToListAsync();
        }

        public async Task<Dictionary<string, List<SupplierItemsRow>>> GetSupplierItemsAsync(int supplierId)
        {
            var itemsData = await _context.Items
                .Where(item => item.SupplierId == supplierId)
                .Select(item => new
                {
                    CategoryName = item.Category != null ? item.Category.Name : "Р†РЅС€Рµ",
                    Row = new SupplierItemsRow()
                    {
                        ItemId = item.ItemId,
                        ItemName = item.Name,
                        Brand = item.Brand != null ? item.Brand.Name : "РќРµ РІРєР°Р·Р°РЅРѕ",
                        Weight = item.Weight ?? 0,
                        Price = item.Price
                    }
                })
                .ToListAsync();

            return itemsData
                .GroupBy(x => x.CategoryName)
                .ToDictionary(group => group.Key, group => group.Select(x => x.Row).ToList());
        }

        public async Task<SupplierFormVM?> GetSupplierFormAsync(int? supplierId)
        {
            List<SelectListItem> countriesList = await _context.Countries
                .Select(country => new SelectListItem()
                {
                    Value = country.CountryId.ToString(),
                    Text = country.Name
                })
                .ToListAsync();

            if (!supplierId.HasValue)
                return new SupplierFormVM()
                {
                    Action = FormAction.Add,
                    Countries = new SelectList(countriesList, "Value", "Text")
                };

            Supplier? supplier = await _context.Suppliers.FindAsync(supplierId);
            if (supplier == null) return null;

            return await _context.Suppliers
                .Where(supplier => supplier.SupplierId == supplierId)
                .Select(supplier => new SupplierFormVM()
                {
                    Action = FormAction.Edit,
                    Id = supplierId,
                    Name = supplier.Name,
                    Contact = supplier.Contact,
                    CountryId = supplier.CountryId,
                    Countries = new SelectList(countriesList, "Value", "Text", supplier.CountryId),
                    Rating = supplier.Rating
                })
                .FirstOrDefaultAsync();
        }

        public async Task<(bool Succeeded, string NotificationText)> AddSupplierAsync(SupplierFormVM newSupplier)
        {
            bool similarSupplierExists = await _context.Suppliers
                .AnyAsync(supplier => supplier.Name.ToLower() == newSupplier.Name.ToLower() ||
                                      (newSupplier.Contact != null && supplier.Contact == newSupplier.Contact));

            if (similarSupplierExists)
                return (false, "РџРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРє Р· С‚Р°РєРёРј С–РјРµРЅРµРј Р°Р±Рѕ РєРѕРЅС‚Р°РєС‚РЅРёРјРё РґР°РЅРёРјРё РІР¶Рµ С–СЃРЅСѓС”.");

            try
            {
                Supplier supplier = new Supplier()
                {
                    Name = newSupplier.Name,
                    Contact = newSupplier.Contact,
                    CountryId = newSupplier.CountryId,
                    Rating = newSupplier.Rating
                };

                _context.Suppliers.Add(supplier);
                await _context.SaveChangesAsync();

                return (true, $"РџРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР° {supplier.Name} (ID: {supplier.SupplierId}) СѓСЃРїС–С€РЅРѕ РґРѕРґР°РЅРѕ.");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РґРѕРґР°РІР°РЅРЅСЏ РїРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР°.");
            }
        }

        public async Task<(bool Succeeded, string NotificationText)> UpdateSupplierAsync(int supplierId, SupplierFormVM updatedSupplier)
        {
            Supplier? supplier = await _context.Suppliers.FindAsync(supplierId);

            if (supplier == null)
                return (false, $"РџРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР° Р· ID:{supplierId} РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            bool similarSupplierExists = await _context.Suppliers
                .AnyAsync(supplier => supplier.SupplierId != updatedSupplier.Id &&
                (supplier.Name.ToLower() == updatedSupplier.Name.ToLower() ||
                (updatedSupplier.Contact != null && supplier.Contact == updatedSupplier.Contact)));

            if (similarSupplierExists)
                return (false, "РџРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРє Р· С‚Р°РєРёРј С–РјРµРЅРµРј Р°Р±Рѕ РєРѕРЅС‚Р°РєС‚РЅРёРјРё РґР°РЅРёРјРё РІР¶Рµ С–СЃРЅСѓС”.");

            try
            {
                supplier.Name = updatedSupplier.Name;
                supplier.Contact = updatedSupplier.Contact;
                supplier.CountryId = updatedSupplier.CountryId;
                supplier.Rating = updatedSupplier.Rating;

                _context.Suppliers.Update(supplier);
                await _context.SaveChangesAsync();

                return (true, $"РЈСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ РґР°РЅС– РїРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР° Р· ID:{supplier.SupplierId}");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РѕРЅРѕРІР»РµРЅРЅСЏ РїРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР°.");
            }
        }

        public async Task<(bool Succeeded, string NotificationText)> DeleteSupplierAsync(int supplierId)
        {
            Supplier? supplier = await _context.Suppliers.FindAsync(supplierId);

            if (supplier == null)
                return (false, $"РџРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР° Р· ID:{supplierId} РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            bool hasItems = await _context.Items.AnyAsync(item => item.SupplierId == supplierId);
            if (hasItems)
            {
                return (false, $"РќРµРјРѕР¶Р»РёРІРѕ РІРёРґР°Р»РёС‚Рё {supplier.Name}, РѕСЃРєС–Р»СЊРєРё Р·Р° РЅРёРј Р·Р°РєСЂС–РїР»РµРЅС– С‚РѕРІР°СЂРё РІ РєР°С‚Р°Р»РѕР·С–.");
            }

            try
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                return (true, $"РџРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР° Р· ID:{supplierId} СѓСЃРїС–С€РЅРѕ РІРёРґР°Р»РµРЅРѕ.");
            }
            catch (Exception)
            {
                return (false, $"Р’РёРЅРёРєР»Р° РЅРµРїРµСЂРµРґР±Р°С‡СѓРІР°РЅР° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РІРёРґР°Р»РµРЅРЅСЏ РїРѕСЃС‚Р°С‡Р°Р»СЊРЅРёРєР°.");
            }
        }
    }
}






