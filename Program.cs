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

            // Add services to the container.

            builder.Services.AddDbContext<MyDbContext>(options =>
                options.UseSqlServer(connectionString ?? throw new InvalidOperationException("Connection string 'GroupSpaceContext' not found.")));

            builder.Services.AddDefaultIdentity<GroupSpace23User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<MyDbContext>();

            builder.Services.AddTransient<IEmailSender, MailKitEmailSender>();

            // Configure MailKit (Optional, if needed for sending emails)
            builder.Services.Configure<MailKitOptions>(options =>
            {
                options.Server = builder.Configuration["ExternalProviders:MailKit:SMTP:Address"];
                options.Port = Convert.ToInt32(builder.Configuration["ExternalProviders:MailKit:SMTP:Port"]);
                options.Account = builder.Configuration["ExternalProviders:MailKit:SMTP:Account"];
                options.Password = builder.Configuration["ExternalProviders:MailKit:SMTP:Password"];
                options.SenderEmail = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderEmail"];
                options.SenderName = builder.Configuration["ExternalProviders:MailKit:SMTP:SenderName"];
                options.Security = true; // Enables SSL or TLS
            });

            // Add services for globalization/localization
            builder.Services.AddLocalization(options => options.ResourcesPath = "Translations");
            builder.Services.AddMvc()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
                .AddDataAnnotationsLocalization();

            builder.Services.AddControllersWithViews();
            builder.Services.AddControllers();

            // Configure Swagger for API documentation
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GroupSpace2023", Version = "v1" });
            });

            // Configure Identity options
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = false;
            });

            // Configure CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            // Ensure we always have an instance of the current app
            Globals.App = app;

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Enforce HTTPS
            }
            else
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GroupSpace2023 v1"));
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<CustomErrorHandlingMiddleware>();

            // Database initialization and seeding
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    MyDbContext context = new MyDbContext(services.GetRequiredService<DbContextOptions<MyDbContext>>());
                    var userManager = services.GetRequiredService<UserManager<GroupSpace23User>>();
                    await MyDbContext.DataInitializer(context, userManager);
                }
                catch (Exception ex)
                {
                    // Log the error (uncomment ex variable name and write a log.)
                    // logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            // Configure localization
            var supportedCultures = new[] { "en", "fr", "nl" };
            var localizationOptions = new RequestLocalizationOptions().SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);

            // Configure endpoints
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
                endpoints.MapFallbackToController("Error", "Home");
            });

            app.Run();
        }
    }
}
