using AutoMapper;
using Grpc.Core;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public class GetAllHandler : IGetAllHandler
{
    private readonly IGetAllManager _manager;
    private readonly ILogger<GetAllHandler> _logger;
    private readonly IMapper _mapper;

    public GetAllHandler(
        IGetAllManager manager,
        ILogger<GetAllHandler> logger,
        IMapper mapper
    )
    {
        _manager = manager;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<CategoryListResponse> HandleAsync(EmptyRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Handling GetAllCategories request");

        try
        {
            var result = await _manager.GetAllAsync(cancellationToken);
            return _mapper.Map<CategoryListResponse>(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling GetAllCategories request");
            throw new RpcException(new Status(StatusCode.Internal, ex.Message, ex));
        }
    }
}
