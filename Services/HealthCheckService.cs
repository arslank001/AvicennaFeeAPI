using AvicennaFeeAPI.Data;

public class HealthCheckService : IHealthCheckService
{
	private readonly ApplicationContext _context;

	public HealthCheckService(ApplicationContext context)
	{
		_context = context;
	}

	public async Task<bool> IsServiceHealthy()
	{
		return await IsDatabaseHealthy();
	}

	private async Task<bool> IsDatabaseHealthy()
	{
		try
		{
			return await _context.Database.CanConnectAsync();
		}
		catch
		{
			return false;
		}
	}
}
