using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace SimpleCommerce.Api;

/// <summary>
/// Maps EF Core concurrency failures to HTTP status codes:
/// DbUpdateConcurrencyException (two simultaneous orders deducting the same stock)
/// -> 409 Conflict, telling the client to retry.
/// </summary>
public class ConcurrencyExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is DbUpdateConcurrencyException)
        {
            context.Result = new ConflictObjectResult(new
            {
                error = "The record was modified by another request. Please retry."
            });
            context.ExceptionHandled = true;
        }
    }
}
