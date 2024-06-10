using StudentStorage.DataAccess.Repository.IRepository;

namespace StudentStorage.Services;

public class AssignmentCheckBackgroundService : BackgroundService
{
    private readonly MailingService _mailingService;
    private readonly IUnitOfWork _unitOfWork;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            CheckAssignments();
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }
    }

    private void CheckAssignments()
    {
        // Logic to check assignments
        // This could involve checking a database for assignments that haven't been completed yet
    }
}
