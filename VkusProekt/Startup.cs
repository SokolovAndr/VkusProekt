using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
using VkusProekt.Data;
using VkusProekt.Data.Interfaces;
using VkusProekt.Data.mocks;
using Microsoft.EntityFrameworkCore;
using VkusProekt.Data.Repositry;
using Microsoft.AspNetCore.Http;
using VkusProekt.Data.Models;

namespace VkusProekt
{
    public class Startup
    {
        private IConfigurationRoot _confstring;

        public Startup(IHostingEnvironment hostEnv) {
            _confstring = new ConfigurationBuilder().SetBasePath(hostEnv.ContentRootPath).AddJsonFile("dbsettings.json").Build();
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(option => option.EnableEndpointRouting = false);
            services.AddDbContext<AppDBContent>(options => options.UseSqlServer(_confstring.GetConnectionString("DefaultConnection")));

            services.AddTransient<IAllBludos, BludoRepository>();
            //��� ����������� ���������� � ������ ������� ��������� ���������
            //��������� IAllBludos ����������� � �����e MockBludos

            services.AddTransient<IBludosCategory, CategoryRepository>();
            //��������� IBludosCategory ����������� � �����e MockBludos
            services.AddTransient<IAllOrders, OrdersRepository>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(sp => ShopCart.GetCart(sp));

            services.AddMvc();
            services.AddMemoryCache();
            services.AddSession();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();
            // app.UseMvcWithDefaultRoute();
            app.UseMvc(routs =>
            {
                routs.MapRoute(name: "default", template: "{controller=Home}/{action=Index}/{id?}");
                routs.MapRoute(name: "categoryFilter", template: "Bludo/{action}/{category?}",
                    defaults: new { Controller = "Bludo", action = "List" });
            });


            using (var scope = app.ApplicationServices.CreateScope())
            {
               AppDBContent content = scope.ServiceProvider.GetRequiredService<AppDBContent>();
               DBObjects.Initial(content);
            }
            
        }
    }
}
