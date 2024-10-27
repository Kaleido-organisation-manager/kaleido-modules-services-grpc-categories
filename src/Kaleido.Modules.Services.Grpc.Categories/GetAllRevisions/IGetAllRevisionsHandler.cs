using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllRevisions;

public interface IGetAllRevisionsHandler : IBaseHandler<CategoryRequest, CategoryListResponse>;