using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using GroupSpace23.Data;
using Microsoft.AspNetCore.Identity;
using GroupSpace23.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using GroupSpace2022.Services;
using NETCore.MailKit.Infrastructure.Internal;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.OpenApi.Models;

namespace GroupSpace23
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("GroupSpaceContext");

            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'GroupSpaceContext' not found.")));

            builder.Services.AddDefaultIdentity<GroupSpace23User>((IdentityOptions options) => options.SignIn.RequireConfirmedAccount = true)
               .AddRoles<IdentityRole>()
               .AddEntityFrameworkStores<MyDbContext>();

            builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();
 
            // De volgende configuratie van de MailKit wordt toegevoegd als demonstratie, maar gebruiken we niet.
            // Deze is "overschreven" door het gebruik van de database-parameters in Globals, en ge�nitialiseerd in de data Initializer
            builder.Services.Configure<MailKitOptions>(options =>
            {
                options.Server = builder.Configuration["ExternalProviders:MailKit:SMTP:Address"];
                options.Port = Convert.ToInt32(builder.Configuration["ExternalProviders:MailKit:SMTP:Port"]);
                options.Account = builder.Configuration["ExternalProviders:MailKit:SMTP:Account"];
                options.Password = builder.Configuration["ExternalProviders:MailKit:SMTP:Password"];
                options.SenderEmail = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
                options.SenderName = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderName"];
                options.Security = true;  // true zet ssl or tls aan
            });

            // Add services for globalization/localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Translations");
            builder.Services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add services for RESTFull API
            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                                new OpenApiInfo { Title = "GroupSpace2023", Version = "v1" });
            });


            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });


            var app = builder.Build();
            Globals.App = app;          // Zorg ervoor dat we altijd een instantie van de huidige app bijhouden

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }
            else // Gebruik van RESTFull API tijdens ontwikkeling
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GroupSpace2023 v1"));
            }
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                MyDbContext context = new MyDbContext(services.GetRequiredService<DbContextOptions<MyDbContext>>());
                var userManager = services.GetRequiredService<UserManager<GroupSpace23User>>();
                await MyDbContext.DataInitializer(context, userManager);
            }

            var supportedCultures = new[] {"en", "fr", "nl" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

            app.Run();
        }
    }
}