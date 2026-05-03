using TechnoMarkt.Areas.Manager.Catalog.ViewModels;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Models;
using TechnoMarkt.Models.Utilities;
using TechnoMarkt.Extensions.Query;
using TechnoMarkt.Shared.Common.ViewModels.Forms;

namespace TechnoMarkt.Areas.Manager.Catalog
{
    public class DiscountsService : IDiscountsService
    {
        private readonly AppDbContext _context;

        public DiscountsService(AppDbContext context) => _context = context;

        public async Task<DiscountFormVM?> GetDiscountFormAsync(int? itemId, int? categoryId)
        {
            bool bothNull = !itemId.HasValue && !categoryId.HasValue;
            bool bothFilled = itemId.HasValue && categoryId.HasValue;

            if (bothNull || bothFilled) return null;

            string? name = null;

            if (itemId.HasValue)
            {
                Item? item = await _context.Items.FindAsync(itemId.Value);
                if (item == null) return null;
                name = item.Name;
            }
            else if (categoryId.HasValue)
            {
                Category? category = await _context.Categories.FindAsync(categoryId.Value);
                if (category == null) return null;
                name = category.Name;
            }

            Discount? discount = await _context.Discounts
                .InPeriod(DateRange.Today)
                .Where(discount => discount.ItemId == itemId && discount.CategoryId == categoryId)
                .FirstOrDefaultAsync();

            if (discount == null)
                return new DiscountFormVM()
                {
                    Action = FormAction.Add,
                    ItemId = itemId,
                    CategoryId = categoryId,
                    Name = name ?? string.Empty,
                };
            else
                return new DiscountFormVM()
                {
                    Action = FormAction.Edit,
                    Id = discount.DiscountId,
                    ItemId = itemId,
                    CategoryId = categoryId,
                    Name = name,
                    Percent = discount.Percent,
                    DateFrom = discount.DateFrom,
                    DateTo = discount.DateTo
                };
        }

        public async Task<(bool Succeeded, string NotificationMessage)> AssignDiscountAsync(DiscountFormVM newDiscount)
        {
            bool bothNull = !newDiscount.ItemId.HasValue && !newDiscount.CategoryId.HasValue;
            bool bothFilled = newDiscount.ItemId.HasValue && newDiscount.CategoryId.HasValue;

            if (bothNull || bothFilled)
                return (false, "Знижка має бути прив'язана або до товару, або до категорії — але не до обох.");

            if (newDiscount.DateFrom > newDiscount.DateTo)
                return (false, "Дата початку не може бути пізніше дати завершення.");

            bool activeExists = await _context.Discounts
                .Where(discount => discount.ItemId == newDiscount.ItemId && discount.CategoryId == newDiscount.CategoryId)
                .InPeriod(DateRange.Today).AnyAsync();

            if (activeExists)
                return (false, "На цей товар/категорію вже є активна знижка. Скористайтеся редагуванням.");

            try
            {
                Discount discount = new Discount()
                {
                    ItemId = newDiscount.ItemId,
                    CategoryId = newDiscount.CategoryId,
                    Percent = (int)newDiscount.Percent,
                    DateFrom = newDiscount.DateFrom,
                    DateTo = newDiscount.DateTo
                };

                _context.Discounts.Add(discount);
                await _context.SaveChangesAsync();

                return (true, $"Знижку {discount.Percent}% успішно призначено.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час призначення знижки.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> UpdateDiscountAsync(DiscountFormVM updatedDiscount)
        {
            if (!updatedDiscount.Id.HasValue)
                return (false, "ID знижки не вказано.");

            Discount? discount = await _context.Discounts.FindAsync(updatedDiscount.Id.Value);
            if (discount == null)
                return (false, $"Знижку з ID:{updatedDiscount.Id} не знайдено.");

            if (updatedDiscount.DateFrom > updatedDiscount.DateTo)
                return (false, "Дата початку не може бути пізніше дати завершення.");

            bool otherActiveExists = await _context.Discounts
                .Where(discount => discount.DiscountId != updatedDiscount.Id)
                .Where(d => d.ItemId == updatedDiscount.ItemId && d.CategoryId == updatedDiscount.CategoryId)
                .InPeriod(DateRange.Today).AnyAsync();

            if (otherActiveExists)
                return (false, "Для цього товару/категорії вже існує інша активна знижка.");

            try
            {
                discount.Percent = (int)updatedDiscount.Percent;
                discount.DateFrom = updatedDiscount.DateFrom;
                discount.DateTo = updatedDiscount.DateTo;

                _context.Discounts.Update(discount);
                await _context.SaveChangesAsync();

                return (true, $"Знижку {discount.Percent}% успішно оновлено.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час оновлення знижки.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> RevokeDiscountAsync(int? itemId, int? categoryId)
        {
            bool bothNull = !itemId.HasValue && !categoryId.HasValue;
            bool bothFilled = itemId.HasValue && categoryId.HasValue;

            if (bothNull || bothFilled)
                return (false, "Необхідно вказати або товар, або категорію — але не обидва.");

            Discount? discount = await _context.Discounts
                .Where(discount => discount.ItemId == itemId && discount.CategoryId == categoryId)
                .InPeriod(DateRange.Today)
                .FirstOrDefaultAsync();

            if (discount == null)
                return (false, "Активну знижку не знайдено.");

            try
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();

                return (true, "Знижку успішно скасовано.");
            }
            catch (Exception)
            {
                return (false, "Виникла помилка під час скасування знижки.");
            }
        }
    }
}



