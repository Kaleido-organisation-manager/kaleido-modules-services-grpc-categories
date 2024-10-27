using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Delete;

public interface IDeleteHandler : IBaseHandler<CategoryRequest, CategoryResponse>;