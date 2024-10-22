namespace Kaleido.Modules.Services.Grpc.Categories.Exists;

public interface IExistsManager
{
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
}
