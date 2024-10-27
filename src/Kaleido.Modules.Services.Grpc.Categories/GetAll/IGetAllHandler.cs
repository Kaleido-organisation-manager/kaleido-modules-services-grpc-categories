using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAll;

public interface IGetAllHandler : IBaseHandler<EmptyRequest, CategoryListResponse>;