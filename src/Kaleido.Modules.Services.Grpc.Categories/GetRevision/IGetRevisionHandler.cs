using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetRevision;

public interface IGetRevisionHandler : IBaseHandler<GetCategoryRevisionRequest, CategoryResponse>;