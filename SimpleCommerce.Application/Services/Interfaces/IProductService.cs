namespace SimpleCommerce.Application.Services.Interfaces;

public interface IProductService
{
    Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default);
}
