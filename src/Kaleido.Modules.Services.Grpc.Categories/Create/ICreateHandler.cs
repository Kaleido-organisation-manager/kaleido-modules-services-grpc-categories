using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.Create;

public interface ICreateHandler : IBaseHandler<Category, CategoryResponse>;