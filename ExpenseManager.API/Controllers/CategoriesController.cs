
using ExpenseManager.Application.Categories.CreateCategory;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler _createCategoryHandler;

    public CategoriesController(
        CreateCategoryHandler createCategoryHandler)
    {
        _createCategoryHandler = createCategoryHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var applicationRequest =
            new CreateCategoryRequest(
                request.Name,
                request.Type);

        var category = await _createCategoryHandler.HandleAsync(
            applicationRequest,
            cancellationToken);

        return Created(
            $"/api/categories/{category.Id}",
            category);
    }
}
