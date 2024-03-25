using Deepin.Domain;
using Deepin.Domain.Entities;
using Deepin.Infrastructure.EntityTypeConfigurations;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Deepin.Infrastructure;
public class DeepinDbContext : IdentityDbContext<User, IdentityRole, string>, IUnitOfWork
{
    private readonly IMediator _mediator = null;
    private IDbContextTransaction _currentTransaction;
    public IDbContextTransaction GetCurrentTransaction() => _currentTransaction;
    public bool HasActiveTransaction => _currentTransaction != null;
    public DeepinDbContext(DbContextOptions<DeepinDbContext> options, IMediator mediator = null) : base(options)
    {
        _mediator = mediator;
    }

    public const string DEFAULT_SCHEMA = "deepin";
    public const string IDENTITY_SCHEMA = "identity";
    public DbSet<Chat> Chats { get; set; }
    public DbSet<ChatMember> ChatMembers { get; set; }
    public virtual async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
    {

        // After executing this line all the changes (from the Command Handler and Domain Event Handlers) 
        // performed through the DbContext will be committed
        await base.SaveChangesAsync(cancellationToken);

        // Dispatch Domain Events collection. 
        // Choices:
        // A) Right BEFORE committing data (EF SaveChanges) into the DB will make a single transaction including  
        // side effects from the domain event handlers which are using the same DbContext with "InstancePerLifetimeScope" or "scoped" lifetime
        // B) Right AFTER committing data (EF SaveChanges) into the DB will make multiple transactions. 
        // You will need to handle eventual consistency and compensatory actions in case of failures in any of the Handlers. 
        if (_mediator != null)
            await _mediator.DispatchDomainEventsAsync(this);

        return true;
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<User>().ToTable("user", IDENTITY_SCHEMA);

        builder.Entity<IdentityUserLogin<string>>().ToTable("user_login", IDENTITY_SCHEMA);
        builder.Entity<IdentityUserClaim<string>>().ToTable("user_claim", IDENTITY_SCHEMA);
        builder.Entity<IdentityUserToken<string>>().ToTable("user_token", IDENTITY_SCHEMA);
        builder.Entity<IdentityRole>().ToTable("role", IDENTITY_SCHEMA);
        builder.Entity<IdentityRoleClaim<string>>().ToTable("role_claim", IDENTITY_SCHEMA);
        builder.Entity<IdentityUserRole<string>>().ToTable("user_role", IDENTITY_SCHEMA);

        builder.ApplyConfiguration(new ChatEntityTypeConfiguration());
        builder.ApplyConfiguration(new ChatMemberEntityTypeConfiguration());
    }
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null) return null;

        _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);

        return _currentTransaction;
    }

    public async Task CommitTransactionAsync(IDbContextTransaction transaction)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));
        if (transaction != _currentTransaction) throw new InvalidOperationException($"Transaction {transaction.TransactionId} is not current");

        try
        {
            await SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            RollbackTransaction();
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void RollbackTransaction()
    {
        try
        {
            _currentTransaction?.Rollback();
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }
}
