using ExpenseManager.API.ExceptionHandling;
using ExpenseManager.Application.Transactions.CreateTransaction;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace ExpenseManager.API.Tests.ExceptionHandling;

public sealed class GlobalExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsync_ArgumentException_WritesBadRequestProblemDetails()
    {
        var context = new DefaultHttpContext();
        context.Request.Path = "/api/transactions";
        await using var body = new MemoryStream();
        context.Response.Body = body;

        var handled = await new GlobalExceptionHandler().TryHandleAsync(context, new ArgumentException("Invalid amount."), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        body.Position = 0;
        using var document = await JsonDocument.ParseAsync(body);
        Assert.Equal("Bad Request", document.RootElement.GetProperty("title").GetString());
        Assert.Equal("Invalid amount.", document.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task TryHandleAsync_CategoryNotFound_WritesNotFound()
    {
        var context = new DefaultHttpContext();
        await using var body = new MemoryStream();
        context.Response.Body = body;

        var handled = await new GlobalExceptionHandler().TryHandleAsync(context, new CategoryNotFoundException(Guid.NewGuid()), CancellationToken.None);

        Assert.True(handled);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
    }
}
