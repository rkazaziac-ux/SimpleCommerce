namespace SimpleCommerce.Controllers;

[ApiController]
[Route("api/vendors")]
[Authorize(Roles = "Admin")] // vendor management is purely administrative
public class VendorsController(IVendorService vendorService) : ApiControllerBase
{
    /// <summary>Lists all vendors with their supplied products (Admin only).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<VendorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VendorDto>>> GetAll(CancellationToken cancellationToken) =>
        Ok(await vendorService.GetAllAsync(cancellationToken));

    /// <summary>Gets one vendor including its supply prices (Admin only).</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await vendorService.GetByIdAsync(id, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : NotFound(new { error = result.Error });
    }

    /// <summary>Creates a vendor (Admin only).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateVendorRequest request, CancellationToken cancellationToken)
    {
        var result = await vendorService.CreateAsync(request, cancellationToken);
        return result.Succeeded ? Ok(result.Value) : BadRequest(new { error = result.Error });
    }

    /// <summary>Updates a vendor's contact info (Admin only).</summary>
    [HttpPut("{id:guid}/contact")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateContact(Guid id, UpdateVendorContactRequest request, CancellationToken cancellationToken)
    {
        var result = await vendorService.UpdateContactAsync(id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value)
            : result.Error == "Vendor was not found." ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }

    /// <summary>Lets a vendor supply an existing product at a vendor-specific price (Admin only).</summary>
    [HttpPost("{id:guid}/products")]
    [ProducesResponseType(typeof(VendorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddProduct(Guid id, AddVendorProductRequest request, CancellationToken cancellationToken)
    {
        var result = await vendorService.AddProductAsync(id, request, cancellationToken);
        return result.Succeeded ? Ok(result.Value)
            : result.Error!.Contains("was not found") ? NotFound(new { error = result.Error })
            : BadRequest(new { error = result.Error });
    }
}
