using Entity.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Database
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Accounts
        public DbSet<ApplicationUser> User { get; set; }
        public DbSet<Role> Role { get; set; }

        // Organization
        public DbSet<Organization> Organization { get; set; }

        // Relation
        public DbSet<OrganizationUser> OrganizationUser { get; set; }
        public DbSet<Employment> Employment { get; set; }

        // License
        public DbSet<License> License { get; set; }
        public DbSet<LicenseType> LicenseType { get; set; }

        // Transaction
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<TransactionType> TransactionType { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            /*-- Soft Delete --*/
            bool result = CreateIsDeletedQueryFilter(builder);
            /*-- /Soft Delete --*/

            /*-- User --*/
            // Email
            builder.Entity<ApplicationUser>()
                .HasAlternateKey(u => u.Email);
            // Username
            builder.Entity<ApplicationUser>()
                .HasAlternateKey(u => u.Username);
            // Role
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.Role)
                .WithMany()
                .IsRequired(true);
            /*-- /User --*/

            /*-- Role --*/
            builder.Entity<Role>()
                .HasAlternateKey(r => r.Code);
            builder.Entity<Role>()
                .HasAlternateKey(r => r.Value);
            /*-- /Role --*/

            /*-- Organization --*/
            builder.Entity<Organization>()
                .HasAlternateKey(o => o.Name);
            builder.Entity<Organization>()
                .HasAlternateKey(o => o.CommercialRegistration);
            /*-- /Organization --*/

            /*-- Relation --*/
            builder.Entity<Employment>()
                .HasOne(e => e.ApplicationUser)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);
            builder.Entity<Employment>()
                .HasOne(e => e.Organization)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            builder.Entity<OrganizationUser>()
                .HasOne(o => o.ApplicationUser)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);
            builder.Entity<OrganizationUser>()
                .HasOne(o => o.Organization)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);
            /*-- /Relation --*/

            /*-- License --*/
            builder.Entity<License>()
                .HasOne(l => l.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);
            /*-- /License --*/

            /*-- Transaction --*/
            builder.Entity<Transaction>()
                .HasOne(t => t.CreatedBy)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(true);

            builder.Entity<TransactionType>()
                .HasAlternateKey(t => t.Code);
            builder.Entity<TransactionType>()
                .HasAlternateKey(t => t.Value);
            /*-- /Transaction --*/
        }

        private bool CreateIsDeletedQueryFilter(ModelBuilder builder)
        {
            Expression<Func<Base.Entity, bool>> filterExpression = e => !e.isDeleted;
            foreach (var mutableEntityType in builder.Model.GetEntityTypes())
            {
                // check if current entity type is child of BaseModel
                if (mutableEntityType.ClrType.IsAssignableTo(typeof(Base.Entity)))
                {
                    // modify expression to handle correct child type
                    var parameter = Expression.Parameter(mutableEntityType.ClrType);
                    var body = ReplacingExpressionVisitor.Replace(filterExpression.Parameters.First(), parameter, filterExpression.Body);
                    var lambdaExpression = Expression.Lambda(body, parameter);

                    // set filter
                    mutableEntityType.SetQueryFilter(lambdaExpression);
                }
            }
            return true;
        }
    }
}
