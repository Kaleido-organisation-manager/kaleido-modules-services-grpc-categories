using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Get;

public interface IGetHandler : IBaseHandler<CategoryRequest, CategoryResponse>;