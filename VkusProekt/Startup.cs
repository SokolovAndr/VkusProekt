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
 
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDBContent>(options => options.UseSqlServer(_confstring.GetConnectionString("DefaultConnection")));

            services.AddTransient<IAllBludos, BludoRepository>();
            //��� ����������� ���������� � ������ ������� ��������� ���������
            //��������� IAllBludos ����������� � �����e MockBludos

            services.AddTransient<IBludosCategory, CategoryRepository>();
            //��������� IBludosCategory ����������� � �����e MockBludos

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped(sp => ShopCart.GetCart(sp));

            services.AddMvc(option => option.EnableEndpointRouting = false);

            services.AddMemoryCache();
            services.AddSession();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvcWithDefaultRoute();
            
            using (var scope = app.ApplicationServices.CreateScope())
            {
                AppDBContent content = scope.ServiceProvider.GetRequiredService<AppDBContent>();
                DBObjects.Initial(content);
            }           
        }
    }
}
