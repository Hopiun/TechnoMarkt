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
                Contact = supplier.Contact ?? "Не вказано",
                Country = supplier.Country != null ? supplier.Country.Name : "Невідома країна",
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
                    CategoryName = item.Category != null ? item.Category.Name : "Інше",
                    Row = new SupplierItemsRow()
                    {
                        ItemId = item.ItemId,
                        ItemName = item.Name,
                        Brand = item.Brand != null ? item.Brand.Name : "Не вказано",
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
                return (false, "Постачальник з таким іменем або контактними даними вже існує.");

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

                return (true, $"Постачальника {supplier.Name} (ID: {supplier.SupplierId}) успішно додано.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час додавання постачальника.");
            }
        }

        public async Task<(bool Succeeded, string NotificationText)> UpdateSupplierAsync(int supplierId, SupplierFormVM updatedSupplier)
        {
            Supplier? supplier = await _context.Suppliers.FindAsync(supplierId);

            if (supplier == null)
                return (false, $"Постачальника з ID:{supplierId} не знайдено.");

            bool similarSupplierExists = await _context.Suppliers
                .AnyAsync(supplier => supplier.SupplierId != updatedSupplier.Id &&
                (supplier.Name.ToLower() == updatedSupplier.Name.ToLower() ||
                (updatedSupplier.Contact != null && supplier.Contact == updatedSupplier.Contact)));

            if (similarSupplierExists)
                return (false, "Постачальник з таким іменем або контактними даними вже існує.");

            try
            {
                supplier.Name = updatedSupplier.Name;
                supplier.Contact = updatedSupplier.Contact;
                supplier.CountryId = updatedSupplier.CountryId;
                supplier.Rating = updatedSupplier.Rating;

                _context.Suppliers.Update(supplier);
                await _context.SaveChangesAsync();

                return (true, $"Успішно змінено дані постачальника з ID:{supplier.SupplierId}");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час оновлення постачальника.");
            }
        }

        public async Task<(bool Succeeded, string NotificationText)> DeleteSupplierAsync(int supplierId)
        {
            Supplier? supplier = await _context.Suppliers.FindAsync(supplierId);

            if (supplier == null)
                return (false, $"Постачальника з ID:{supplierId} не знайдено.");

            bool hasItems = await _context.Items.AnyAsync(item => item.SupplierId == supplierId);
            if (hasItems)
            {
                return (false, $"Неможливо видалити {supplier.Name}, оскільки за ним закріплені товари в каталозі.");
            }

            try
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();

                return (true, $"Постачальника з ID:{supplierId} успішно видалено.");
            }
            catch (Exception)
            {
                return (false, $"Виникла непередбачувана помилка під час видалення постачальника.");
            }
        }
    }
}




