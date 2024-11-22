using Kaleido.Common.Services.Grpc.Handlers;
using Kaleido.Grpc.Categories;

namespace Kaleido.Modules.Services.Grpc.Categories.GetAllByName;

public interface IGetAllFilteredHandler : IBaseHandler<GetAllCategoriesFilteredRequest, CategoryListResponse>;
