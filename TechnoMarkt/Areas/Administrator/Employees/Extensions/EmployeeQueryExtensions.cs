using TechnoMarkt.Areas.Administrator.Employees.Extensions;
using TechnoMarkt.Models;

namespace TechnoMarkt.Areas.Administrator.Employees.Extensions
{
    public static class EmployeeQueryExtensions
    {
        public static IQueryable<Employee> ByStore(this IQueryable<Employee> query, int storeId) =>
            query.Where(employee => employee.StoreId == storeId);

        public static IQueryable<Employee> WithRole(this IQueryable<Employee> query, EmployeeRole role) =>
            query.Where(employee => employee.Role == role);

        public static IQueryable<Employee> WithStatus(this IQueryable<Employee> query, EmployeeStatus status) =>
            query.Where(employee => employee.Status == status);

        public static IQueryable<Employee> Active(this IQueryable<Employee> query) =>
            query.WithStatus(EmployeeStatus.Present);
    }
}

