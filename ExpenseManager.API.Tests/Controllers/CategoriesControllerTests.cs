using ExpenseManager.API.Controllers;
using ExpenseManager.API.Contracts.Categories;
using ExpenseManager.Application.Categories.CreateCategory;
using ExpenseManager.Application.Categories.GetCategories;
using ExpenseManager.Application.Categories.GetCategoryById;
using ExpenseManager.Application.Interfaces;
using ExpenseManager.Domain.Entities;
using ExpenseManager.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ExpenseManager.API.Tests.Controllers;

public sealed class CategoriesControllerTests
{
    [Fact]
    public async Task GetById_ExistingCategory_ReturnsMappedResponse()
    {
        var category = new Category("Food", TransactionType.Expense);
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(x => x.GetByIdAsync(category.Id, It.IsAny<CancellationToken>())).ReturnsAsync(category);
        var controller = new CategoriesController(
            new CreateCategoryHandler(repository.Object, Mock.Of<IUnitOfWork>()),
            new GetCategoriesHandler(repository.Object),
            new GetCategoryByIdHandler(repository.Object));

        var result = await controller.GetById(category.Id, CancellationToken.None);

        var response = Assert.IsType<CategoryResponse>(Assert.IsType<OkObjectResult>(result.Result).Value);
        Assert.Equal(category.Id, response.Id);
        Assert.Equal("Food", response.Name);
        Assert.Equal((int)TransactionType.Expense, response.Type);
    }

    [Fact]
    public async Task GetById_MissingCategory_ReturnsNotFound()
    {
        var id = Guid.NewGuid();
        var repository = new Mock<ICategoryRepository>();
        repository.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Category?)null);
        var controller = new CategoriesController(
            new CreateCategoryHandler(repository.Object, Mock.Of<IUnitOfWork>()),
            new GetCategoriesHandler(repository.Object),
            new GetCategoryByIdHandler(repository.Object));

        var result = await controller.GetById(id, CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }
}
