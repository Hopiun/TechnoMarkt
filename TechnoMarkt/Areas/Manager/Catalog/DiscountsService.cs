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
                return (false, "Р—РЅРёР¶РєР° РјР°С” Р±СѓС‚Рё РїСЂРёРІ'СЏР·Р°РЅР° Р°Р±Рѕ РґРѕ С‚РѕРІР°СЂСѓ, Р°Р±Рѕ РґРѕ РєР°С‚РµРіРѕСЂС–С— вЂ” Р°Р»Рµ РЅРµ РґРѕ РѕР±РѕС….");

            if (newDiscount.DateFrom > newDiscount.DateTo)
                return (false, "Р”Р°С‚Р° РїРѕС‡Р°С‚РєСѓ РЅРµ РјРѕР¶Рµ Р±СѓС‚Рё РїС–Р·РЅС–С€Рµ РґР°С‚Рё Р·Р°РєС–РЅС‡РµРЅРЅСЏ.");

            bool activeExists = await _context.Discounts
                .Where(discount => discount.ItemId == newDiscount.ItemId && discount.CategoryId == newDiscount.CategoryId)
                .InPeriod(DateRange.Today).AnyAsync();

            if (activeExists)
                return (false, "РќР° С†РµР№ С‚РѕРІР°СЂ/РєР°С‚РµРіРѕСЂС–СЋ РІР¶Рµ С” Р°РєС‚РёРІРЅР° Р·РЅРёР¶РєР°. РЎРєРѕСЂРёСЃС‚Р°Р№С‚РµСЃСЏ СЂРµРґР°РіСѓРІР°РЅРЅСЏРј.");

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

                return (true, $"Р—РЅРёР¶РєСѓ {discount.Percent}% СѓСЃРїС–С€РЅРѕ РїСЂРёР·РЅР°С‡РµРЅРѕ.");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РїСЂРёР·РЅР°С‡РµРЅРЅСЏ Р·РЅРёР¶РєРё.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> UpdateDiscountAsync(DiscountFormVM updatedDiscount)
        {
            if (!updatedDiscount.Id.HasValue)
                return (false, "ID Р·РЅРёР¶РєРё РЅРµ РІРєР°Р·Р°РЅРѕ.");

            Discount? discount = await _context.Discounts.FindAsync(updatedDiscount.Id.Value);
            if (discount == null)
                return (false, $"Р—РЅРёР¶РєСѓ Р· ID:{updatedDiscount.Id} РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (updatedDiscount.DateFrom > updatedDiscount.DateTo)
                return (false, "Р”Р°С‚Р° РїРѕС‡Р°С‚РєСѓ РЅРµ РјРѕР¶Рµ Р±СѓС‚Рё РїС–Р·РЅС–С€Рµ РґР°С‚Рё Р·Р°РєС–РЅС‡РµРЅРЅСЏ.");

            bool otherActiveExists = await _context.Discounts
                .Where(discount => discount.DiscountId != updatedDiscount.Id)
                .Where(d => d.ItemId == updatedDiscount.ItemId && d.CategoryId == updatedDiscount.CategoryId)
                .InPeriod(DateRange.Today).AnyAsync();

            if (otherActiveExists)
                return (false, "Р”Р»СЏ С†СЊРѕРіРѕ С‚РѕРІР°СЂСѓ/РєР°С‚РµРіРѕСЂС–С— РІР¶Рµ С–СЃРЅСѓС” С–РЅС€Р° Р°РєС‚РёРІРЅР° Р·РЅРёР¶РєР°.");

            try
            {
                discount.Percent = (int)updatedDiscount.Percent;
                discount.DateFrom = updatedDiscount.DateFrom;
                discount.DateTo = updatedDiscount.DateTo;

                _context.Discounts.Update(discount);
                await _context.SaveChangesAsync();

                return (true, $"Р—РЅРёР¶РєСѓ {discount.Percent}% СѓСЃРїС–С€РЅРѕ РѕРЅРѕРІР»РµРЅРѕ.");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ РѕРЅРѕРІР»РµРЅРЅСЏ Р·РЅРёР¶РєРё.");
            }
        }

        public async Task<(bool Succeeded, string NotificationMessage)> RevokeDiscountAsync(int? itemId, int? categoryId)
        {
            bool bothNull = !itemId.HasValue && !categoryId.HasValue;
            bool bothFilled = itemId.HasValue && categoryId.HasValue;

            if (bothNull || bothFilled)
                return (false, "РќРµРѕР±С…С–РґРЅРѕ РІРєР°Р·Р°С‚Рё Р°Р±Рѕ С‚РѕРІР°СЂ, Р°Р±Рѕ РєР°С‚РµРіРѕСЂС–СЋ вЂ” Р°Р»Рµ РЅРµ РѕР±РёРґРІР°.");

            Discount? discount = await _context.Discounts
                .Where(discount => discount.ItemId == itemId && discount.CategoryId == categoryId)
                .InPeriod(DateRange.Today)
                .FirstOrDefaultAsync();

            if (discount == null)
                return (false, "РђРєС‚РёРІРЅСѓ Р·РЅРёР¶РєСѓ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            try
            {
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();

                return (true, "Р—РЅРёР¶РєСѓ СѓСЃРїС–С€РЅРѕ СЃРєР°СЃРѕРІР°РЅРѕ.");
            }
            catch (Exception)
            {
                return (false, "Р’РёРЅРёРєР»Р° РїРѕРјРёР»РєР° РїС–Рґ С‡Р°СЃ СЃРєР°СЃСѓРІР°РЅРЅСЏ Р·РЅРёР¶РєРё.");
            }
        }
    }
}





