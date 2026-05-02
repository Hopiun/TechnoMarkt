using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TechnoMarkt.Data;
using TechnoMarkt.Controllers;
using TechnoMarkt.Interfaces;
using TechnoMarkt.Shared.Orders.Services;
using TechnoMarkt.Shared.Orders.ViewModels.Tables;
using TechnoMarkt.Shared.Stock.Services;
using TechnoMarkt.Services;
using TechnoMarkt.Models.Identity;

// Administrator
using TechnoMarkt.Areas.Administrator.Dashboard;
using TechnoMarkt.Areas.Administrator.Employees;
using TechnoMarkt.Areas.Administrator.Finance;
using TechnoMarkt.Areas.Administrator.Orders;
using TechnoMarkt.Areas.Administrator.Settings;
using TechnoMarkt.Areas.Administrator.Warehouse;
// Manager
using TechnoMarkt.Areas.Manager.Catalog;
using TechnoMarkt.Areas.Manager.Catalog.ViewModels;
using TechnoMarkt.Areas.Manager.Suppliers;
using TechnoMarkt.Areas.Manager.Warehouse;
// Operator
using TechnoMarkt.Areas.Operator.Orders;
using TechnoMarkt.Areas.Operator.Warehouse;
// Home
using TechnoMarkt.Areas.Home.Home;
using TechnoMarkt.Areas.Home.Catalog;
using TechnoMarkt.Areas.Home.Catalog.ViewModels;
// Client
using TechnoMarkt.Areas.Client.Checkout;
using TechnoMarkt.Areas.Client.Orders;
using TechnoMarkt.Areas.Client.Wallet;
using TechnoMarkt.Areas.Client.Profile;

namespace TechnoMarkt.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["ConnectionStrings:DefaultConnection"];

            services.AddDbContextFactory<AppDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            services.AddScoped<AppDbContext>(sp => sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

            services.AddWarehouseService();
            services.AddOrdersService();
            services.AddCartService();
            services.AddControllersServices();

            services.AddAdministratorServices();
            services.AddManagerServices();
            services.AddClientServices();
            services.AddHomeServices();

            services.AddReportServices();
            
            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, IdentityRole<int>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
            });

            return services;
        }

        private static IServiceCollection AddWarehouseService(this IServiceCollection services)
        {
            services.AddScoped<IStockReadService, StockReadService>();
            services.AddScoped<IStockStatsService, StockStatsService>();
            services.AddScoped<IStockEditService, StockEditService>();
            services.AddScoped<IOperatorStockService, OperatorStockService>();

            return services;
        }

        private static IServiceCollection AddOrdersService(this IServiceCollection services)
        {
            services.AddScoped<IOrdersReadService<OrdersFilter>, OrdersReadService>();
            services.AddScoped<IOrdersStatsService, OrdersStatsService>();
            services.AddScoped<IOrdersEditService, OrdersEditService>();
            services.AddScoped<IOperatorOrderService, OperatorOrderService>();

            return services;
        }

        private static IServiceCollection AddAdministratorServices(this IServiceCollection services)
        {
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IFinanceService, FinanceService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IAdministratorOrderService, AdministratorOrderService>();
            services.AddScoped<IAdministratorStockService, AdministratorStockService>();
            services.AddScoped<ISettingsService, SettingsService>();

            return services;
        }

        private static IServiceCollection AddManagerServices(this IServiceCollection services)
        {
            services.AddScoped<ISuppliersService, SuppliersService>();
            services.AddScoped<ICategoriesReadService, CategoriesService>();
            services.AddScoped<IManagerCategoriesService, ManagerCategoriesService>();
            services.AddScoped<IManagerItemsService, ManagerItemsService>();
            services.AddScoped<IDiscountsService, DiscountsService>();
            services.AddScoped<IManagerStockService, ManagerStockService>();

            return services;
        }

        private static IServiceCollection AddHomeServices(this IServiceCollection services)
        {
            services.AddScoped<IHomeService, HomeService>();
            services.AddScoped<IItemsReadService<HomeItemsFilter>, HomeItemsService>();

            return services;
        }

        private static IServiceCollection AddReportServices(this IServiceCollection services)
        {
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<ReportService>();

            return services;
        }

        private static IServiceCollection AddCartService(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<CartService>();
            services.AddScoped<IEventLogsService, EventLogsService>();
            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(24);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            return services;
        }

        private static IServiceCollection AddControllersServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<ICartFacadeService, CartFacadeService>();
            return services;
        }

        private static IServiceCollection AddClientServices(this IServiceCollection services)
        {
            services.AddScoped<ClientOrdersService>();
            services.AddScoped<IClientOrdersService, ClientAreaOrdersService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IClientCheckoutService, ClientCheckoutService>();

            return services;
        }
    }
}

