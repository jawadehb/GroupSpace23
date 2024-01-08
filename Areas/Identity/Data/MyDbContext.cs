using GroupSpace23.Areas.Identity.Data;
using GroupSpace23.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace GroupSpace23.Data;

public class MyDbContext : IdentityDbContext<GroupSpace23User>
{
    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }


    public static async Task DataInitializer(MyDbContext context, UserManager<GroupSpace23User> userManager)
    {
        if (!context.Languages.Any())
        {
            context.AddRange(
                new Language { Id = "- ", Name = "-", IsSystemLanguage = false, IsAvailable = DateTime.MaxValue},
                new Language { Id = "en", Name = "English", IsSystemLanguage = true  },
                new Language { Id = "nl", Name = "Nederlands", IsSystemLanguage = true },
                new Language { Id = "fr", Name = "français", IsSystemLanguage = true },
                new Language { Id = "de", Name = "Deutsch", IsSystemLanguage = true }
                );
            context.SaveChanges();
        }

        Language.GetLanguages(context);

        if (!context.Users.Any())
        {
            GroupSpace23User dummyuser = new GroupSpace23User
            {
                Id = "Dummy",
                Email = "dummy@dummy.xx",
                UserName = "Dummy",
                FirstName = "Dummy",
                LastName = "Dummy",
                PasswordHash = "Dummy",
                LockoutEnabled = true,
                LockoutEnd = DateTime.MaxValue
            };
            context.Users.Add(dummyuser);
            context.SaveChanges();


            GroupSpace23User adminUser = new GroupSpace23User
            {
                Id = "Admin",
                Email = "admin@dummy.xx",
                UserName = "Admin",
                FirstName = "Administrator",
                LastName = "GroupSpace"
            };

            var result = await userManager.CreateAsync(adminUser, "Abc!12345");

        }


        GroupSpace23User dummy = context.Users.FirstOrDefault(u => u.UserName == "Dummy");
        GroupSpace23User admin = context.Users.FirstOrDefault(u => u.UserName == "Admin");


        AddParameters(context, admin);

        Globals.DummyUser = dummy;  // Make sure the dummy user is always available

        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new IdentityRole { Name = "SystemAdministrator", Id = "SystemAdministrator", NormalizedName = "SYSTEMADMINISTRATOR" },
                new IdentityRole { Name = "User", Id = "User", NormalizedName = "USER" },
                new IdentityRole { Name = "UserAdministrator", Id = "UserAdministrator", NormalizedName = "USERADMINISTRATOR" }
            );

            context.UserRoles.Add(new IdentityUserRole<string> { RoleId = "SystemAdministrator", UserId = admin.Id });
            context.UserRoles.Add(new IdentityUserRole<string> { RoleId = "UserAdministrator", UserId = admin.Id });

            context.SaveChanges();
        }


        if (!context.Groups.Any())
        {
            context.Groups.Add(new Group { Description = "Dummy", Name = "Dummy", Ended = DateTime.Now });
            context.SaveChanges();
        }
        Group dummyGroup = context.Groups.FirstOrDefault(g => g.Name == "Dummy");

        // Was nodig om bij de migratie een foreign-key constraint probleem te hebben
        //List <Group> groups = context.Groups.ToList();
        //foreach (Group g in groups)
        //{ 
        //    g.StartedById = dummy.Id;
        //    context.Update(g);
        //}
        //context.SaveChanges();

        if (!context.Messages.Any())
        {
            context.Messages.Add(new Message { Title = "Dummy", Body = "", Sent = DateTime.Now, Deleted = DateTime.Now, Recipient = dummyGroup });
            context.SaveChanges();
        }
        context.SaveChanges();
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    static void AddParameters(MyDbContext context, GroupSpace23User user)
    {
        if (!context.Parameters.Any())
        {
            context.Parameters.AddRange(
                new Parameter { Name = "Version", Value = "0.1.0", Description = "Huidige versie van de parameterlijst", Destination = "System", UserId = user.Id },
                new Parameter { Name = "Mail.Server", Value = "ergens.groupspace.be", Description = "Naam van de gebruikte pop-server", Destination = "Mail", UserId = user.Id },
                new Parameter { Name = "Mail.Port", Value = "25", Description = "Poort van de smtp-server", Destination = "Mail", UserId = user.Id },
                new Parameter { Name = "Mail.Account", Value = "SmtpServer", Description = "Acount-naam van de smtp-server", Destination = "Mail", UserId = user.Id },
                new Parameter { Name = "Mail.Password", Value = "xxxyyy!2315", Description = "Wachtwoord van de smtp-server", Destination = "Mail", UserId = user.Id },
                new Parameter { Name = "Mail.Security", Value = "true", Description = "Is SSL or TLS encryption used (true or false)", Destination = "Mail", UserId = user.Id },
                new Parameter { Name = "Mail.SenderEmail", Value = "administrator.groupspace.be", Description = "E-mail van de smtp-verzender", Destination = "Mail", UserId = user.Id },
                new Parameter { Name = "Mail.SenderName", Value = "Administrator", Description = "Naam van de smtp-verzender", Destination = "Mail", UserId = user.Id }
            );
            context.SaveChanges();
        }

        Globals.Parameters = new Dictionary<string, Parameter>();
        foreach (Parameter parameter in context.Parameters)
        {
            Globals.Parameters[parameter.Name] = parameter;
        }
        Globals.ConfigureMail();
    }

    public DbSet<GroupSpace23.Models.Group> Groups { get; set; } = default!;

    public DbSet<GroupSpace23.Models.Message> Messages { get; set; } = default!;

    public DbSet<GroupSpace23.Models.Parameter> Parameters { get; set; } = default!;

    public DbSet<GroupSpace23.Models.GroupMember> GroupMembers { get; set; } = default!;

    public DbSet<GroupSpace23.Models.Language> Languages { get; set; } = default!;


}
