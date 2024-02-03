using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FoodOrderingSystemAPI.Constraints{
  public class CategoryConstraintActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var categoryName = (string)context.ActionArguments["categoryName"];

        if (!IsValidCategoryName(categoryName))
        {
            var errorMessage = $"The value '{categoryName}' is not a valid category name. Category names can only contain letters.";
            context.Result = new BadRequestObjectResult(errorMessage);
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }

    private bool IsValidCategoryName(string categoryName)
    {
        return categoryName.All(char.IsLetter);
    }
}

}

