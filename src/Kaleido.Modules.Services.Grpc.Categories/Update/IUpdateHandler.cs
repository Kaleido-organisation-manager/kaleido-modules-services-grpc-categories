using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Update;

public interface IUpdateHandler : IBaseHandler<CategoryActionRequest, CategoryResponse>;