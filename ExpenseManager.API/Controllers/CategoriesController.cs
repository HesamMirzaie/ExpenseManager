
using ExpenseManager.API.Contracts.Categories;
using CreateCategoryHandler = ExpenseManager.Application.Categories.CreateCategory.CreateCategoryHandler;
using ApplicationCreateCategoryRequest = ExpenseManager.Application.Categories.CreateCategory.CreateCategoryRequest;
using GetCategoriesHandler = ExpenseManager.Application.Categories.GetCategories.GetCategoriesHandler;
using Microsoft.AspNetCore.Mvc;
using ExpenseManager.Application.Categories.GetCategoryById;

namespace ExpenseManager.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CategoriesController : ControllerBase
{
    private readonly CreateCategoryHandler _createCategoryHandler;

    private readonly GetCategoriesHandler _getCategoriesHandler;

    private readonly GetCategoryByIdHandler _getCategoryByIdHandler;


    public CategoriesController(
        CreateCategoryHandler createCategoryHandler,
        GetCategoriesHandler getCategoriesHandler,
        GetCategoryByIdHandler getCategoryByIdHandler)
    {
        _createCategoryHandler = createCategoryHandler;
        _getCategoriesHandler = getCategoriesHandler;
        _getCategoryByIdHandler = getCategoryByIdHandler;
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var applicationRequest =
            new ApplicationCreateCategoryRequest(
                request.Name,
                request.Type);

        var category = await _createCategoryHandler.HandleAsync(
            applicationRequest,
            cancellationToken);

        return Created(
            $"/api/categories/{category.Id}",
            category);
    }
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CategoryResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var categories = await _getCategoriesHandler.HandleAsync(
            cancellationToken);

        var response = categories
            .Select(category => new CategoryResponse(
                category.Id,
                category.Name,
                (int)category.Type))
            .ToList();

        return Ok(response);
    }
    [HttpGet("{id:guid}")]
public async Task<ActionResult<CategoryResponse>> GetById(
    Guid id,
    CancellationToken cancellationToken)
{
    var category = await _getCategoryByIdHandler.HandleAsync(
        id,
        cancellationToken);

    if (category is null)
    {
        return NotFound();
    }

    var response = new CategoryResponse(
        category.Id,
        category.Name,
        (int)category.Type);

    return Ok(response);
}
}
