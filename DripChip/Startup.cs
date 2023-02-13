using DripChip.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using DripChip.DataBase.Repositories;

namespace DripChip
{
    public class Startup
    {
        private readonly string _connectionString;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options => options.LoginPath = "/registration");

            services.AddDbContext<DataContext>(options => options.UseSqlServer(_connectionString));

            services.AddTransient<IAccountRepository, AccountRepository>();
            services.AddTransient<IAnimalRepository, AnimalRepository>();
            services.AddTransient<IAnimalTypeRepository, AnimalTypeRepository>();
            services.AddTransient<ILocationPointRepository, LocationPointRepository>();
        }

        public void Configure(IApplicationBuilder app, IHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            try
            {
                app.UseRouting();
                app.UseStaticFiles();
                app.UseHttpsRedirection();
                app.UseAuthentication();
                app.UseAuthorization();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}