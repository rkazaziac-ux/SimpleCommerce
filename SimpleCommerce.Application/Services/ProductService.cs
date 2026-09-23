namespace SimpleCommerce.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDto>> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Code))
            return Result<ProductDto>.Fail("Product code is required.");
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductDto>.Fail("Product name is required.");
        if (request.Price < 0)
            return Result<ProductDto>.Fail("Product price cannot be negative.");
        if (request.StockQuantity < 0)
            return Result<ProductDto>.Fail("Product stock cannot be negative.");

        var code = request.Code.Trim();
        if (await _productRepository.CodeExistsAsync(code, cancellationToken))
            return Result<ProductDto>.Fail($"A product with code '{code}' already exists.");

        var product = new Product
        {
            Code = code,
            Name = request.Name.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        await _productRepository.AddAsync(product, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Ok(ToDto(product));
    }

    public async Task<Result<ProductDto>> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        if (product is null)
            return Result<ProductDto>.Fail("Product was not found.");
        if (string.IsNullOrWhiteSpace(request.Name))
            return Result<ProductDto>.Fail("Product name is required.");
        if (request.Price < 0)
            return Result<ProductDto>.Fail("Product price cannot be negative.");

        product.Name = request.Name.Trim();
        product.Price = request.Price;
        await _productRepository.SaveChangesAsync(cancellationToken);

        return Result<ProductDto>.Ok(ToDto(product));
    }

    public async Task<Result<ProductDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(id, cancellationToken);
        return product is null
            ? Result<ProductDto>.Fail("Product was not found.")
            : Result<ProductDto>.Ok(ToDto(product));
    }

    public async Task<List<ProductDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var products = await _productRepository.GetAllAsync(cancellationToken);
        return products.Select(ToDto).ToList();
    }

    private static ProductDto ToDto(Product product) =>
        new(product.Id, product.Code, product.Name, product.Price, product.StockQuantity);
}
