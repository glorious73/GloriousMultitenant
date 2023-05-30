using Application.DTO.Auth;
using Application.Logic.Account;
using Entity.Contracts;
using Entity.Database;

namespace WebAPI.Configuration.Db
{
    public class DbSeedConfig : IDbSeedConfig
    {
        private readonly ApplicationDbContext _context;
        private IAccountService _accountService;
        private IConfiguration _config { get; }
        private ILogger<DbSeedConfig> _logger;

        public DbSeedConfig(ApplicationDbContext context, IAccountService accountService, IConfiguration config, ILogger<DbSeedConfig> logger)
        {
            _context = context;
            _accountService = accountService;
            _config = config;
            _logger = logger;
        }

        public void Seed()
        {
            _logger.LogInformation("Starting to seed DB.");
            var defaultRoles = SeedDefaultRoles();
            var adminRole = _context.Role.Where(r => r.Value.ToLower().Contains("admin")).FirstOrDefault();
            if (adminRole == null)
                throw new Exception("Application cannot start without the default admin role.");
            var defaultUser = SeedDefaultUser(adminRole);
            if (defaultUser == null)
                throw new Exception("Application cannot start without the default admin user.");
            _logger.LogInformation("Seeded DB.");
        }

        // Accounts
        private List<Role> SeedDefaultRoles()
        {
            var defaultRoleNames = _config.GetSection("Roles").Get<string[]>();
            var Roles = new List<Role>();
            for (int i = 0; i < defaultRoleNames.Length; i++)
            {
                bool isRoleExistent = _context.Role.Any(r => r.Value == defaultRoleNames[i]);
                if (!isRoleExistent)
                {
                    Role defaultRole;
                    defaultRole = new Role()
                    {
                        Code = i + 1,
                        Value = defaultRoleNames[i],
                        IsSeeded = true
                    };
                    _context.Role.Add(defaultRole);
                    Roles.Add(defaultRole);
                }
            }
            _context.SaveChanges(); // to get the role Id back
            return Roles;
        }

        private ApplicationUser? SeedDefaultUser(Role adminRole)
        {
            ApplicationUser? defaultUser;
            if (!_context.User.Any(u => u.Username == "admin"))
            {
                defaultUser = new ApplicationUser()
                {
                    FirstName = "Admin",
                    LastName = "",
                    Email = "admin@gloriousmultitenant.com",
                    Username = "admin",
                    PhoneNumber = "966555555555",
                    RoleId = adminRole.Id,
                    IsSeeded = true
                };
                HashPasswordDTO passwordResult = _accountService.HashPassword(_config.GetSection("DefaultUser:Password").Get<string>());
                defaultUser.PasswordHash = passwordResult.Hash;
                defaultUser.PasswordSalt = Convert.ToBase64String(passwordResult.Salt);
                _context.User.Add(defaultUser);
                _context.SaveChanges();
            }
            else
                defaultUser = _context.User.FirstOrDefault(u => u.Username == "admin");
            return defaultUser;
        }
    }
}
