using SimpleCommerce.Application.Services;
using SimpleCommerce.UnitTests.Application;
using Xunit;

namespace SimpleCommerce.UnitTests.Application;

public class ProductServiceTests
{
    private readonly FakeProductRepository _productRepository = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _service = new ProductService(_productRepository);
    }

    [Fact]
    public async Task Create_WithUniqueCode_Succeeds()
    {
        var result = await _service.CreateAsync(new("P-100", "Laptop", 2500m, 10));

        Assert.True(result.Succeeded);
        Assert.Equal("P-100", result.Value!.Code);
        Assert.Single(_productRepository.Products);
        Assert.Equal(1, _productRepository.SaveCount);
    }

    [Fact]
    public async Task Create_WithDuplicateCode_FailsWithoutSaving()
    {
        _productRepository.Products.Add(new Product { Code = "P-100", Name = "Laptop", Price = 2500m, StockQuantity = 10 });

        var result = await _service.CreateAsync(new("P-100", "Another Laptop", 1000m, 5));

        Assert.False(result.Succeeded);
        Assert.Contains("already exists", result.Error);
        Assert.Equal(0, _productRepository.SaveCount);
    }

    [Fact]
    public async Task Update_ChangesNameAndPrice()
    {
        var product = new Product { Code = "P-100", Name = "Laptop", Price = 2500m, StockQuantity = 10 };
        _productRepository.Products.Add(product);

        var result = await _service.UpdateAsync(product.Id, new("Laptop Pro", 3000m));

        Assert.True(result.Succeeded);
        Assert.Equal("Laptop Pro", product.Name);
        Assert.Equal(3000m, product.Price);
    }
}
