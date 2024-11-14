using AuthAlbiWebSchool.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AuthAlbiWebSchool.backgroundTasks;

public class AccountDeletionService : IHostedService, IDisposable
{
    private Timer _timer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AccountDeletionService> _logger;
    private bool _isDisposed;

    public AccountDeletionService(
        IServiceProvider serviceProvider,
        ILogger<AccountDeletionService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Account Deletion Service is starting.");
        
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        
        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        DoWorkAsync().GetAwaiter().GetResult();
    }

    private async Task DoWorkAsync()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            _logger.LogInformation("Starting scheduled account deletion check");
            
            await DeleteAccountsAsync(dbContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting accounts");
        }
    }

    private async Task DeleteAccountsAsync(ApplicationDbContext dbContext)
    {
        var usersToDelete = await dbContext.Users
            .Where(u => u.LockoutEnd < DateTimeOffset.Now && u.AccountDeletionRequested)
            .ToListAsync();

        if (!usersToDelete.Any())
        {
            _logger.LogInformation("No accounts found for deletion");
            return;
        }

        _logger.LogInformation("Found {Count} accounts to delete", usersToDelete.Count);

        foreach (var user in usersToDelete)
        {
            try
            {
                dbContext.Users.Remove(user);
                _logger.LogInformation("Deleting user with ID: {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", user.Id);
            }
        }

        await dbContext.SaveChangesAsync();
        _logger.LogInformation("Account deletion process completed");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Account Deletion Service is stopping.");
        
        _timer?.Change(Timeout.Infinite, 0);
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
        {
            _timer?.Dispose();
        }

        _isDisposed = true;
    }
}