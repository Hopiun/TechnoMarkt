using TechnoMarkt.Areas.Manager.Suppliers.ViewModels;
using TechnoMarkt.Models;
using TechnoMarkt.Shared.Common.ViewModels.Filters;
using TechnoMarkt.Shared.EventLogs.ViewModels.Filters;

namespace TechnoMarkt.Areas.Manager.Suppliers
{
    public interface ISuppliersService
    {
        public Task<List<SuppliersTableRow>> GetSuppliersAsync(SuppliersFilter? filter);
        public Task<Dictionary<string, List<SupplierItemsRow>>> GetSupplierItemsAsync(int supplierId);
        public Task<SupplierFormVM?> GetSupplierFormAsync(int? supplierId);
        public Task<(bool Succeeded, string NotificationText)> AddSupplierAsync(SupplierFormVM newSupplier);
        public Task<(bool Succeeded, string NotificationText)> UpdateSupplierAsync(int supplierId, SupplierFormVM updatedSupplier);
        public Task<(bool Succeeded, string NotificationText)> DeleteSupplierAsync(int supplierId);
    }
}




