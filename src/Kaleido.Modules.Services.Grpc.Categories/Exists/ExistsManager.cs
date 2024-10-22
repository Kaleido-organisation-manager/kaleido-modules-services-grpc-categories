
using Kaleido.Modules.Services.Grpc.Categories.Common.Repositories.Interfaces;

namespace Kaleido.Modules.Services.Grpc.Categories.Exists;

public class ExistsManager : IExistsManager
{
    private readonly ICategoryRepository _repository;
    private readonly ILogger<ExistsManager> _logger;

    public ExistsManager(
        ICategoryRepository repository,
        ILogger<ExistsManager> logger
    )
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Checking if category with key {Key} exists", key);
        var guid = Guid.Parse(key);
        return _repository.ExistsAsync(guid, cancellationToken);
    }
}
