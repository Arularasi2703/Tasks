// Title: Food Ordering System ( Menu API )
// Author : Arularasi J
// Created at : 03/07/2023
// Updated at : 20/12/2023
// Reviewed by: 
// Reviewed at: 

using FoodOrderingSystemAPI.Constraints;
using FoodOrderingSystemAPI.Interfaces;
using FoodOrderingSystemAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks; 
namespace FoodOrderingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IFoodMenuService _foodMenuService;
        // Menu
        private readonly ILogger<MenuController> _logger;
        private readonly IConfiguration _configuration;

        public MenuController(IFoodMenuService foodMenuService, ILogger<MenuController> logger,IConfiguration configuration)
        {
            _foodMenuService = foodMenuService;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFoodItemsAsync()
        {
            try
            {
                var foodItems = await _foodMenuService.GetAllFoodItemsAsync();
                return Ok(foodItems);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, "Error occurred while retrieving all food items.");
                // return StatusCode(500, "An error occurred while retrieving all food items.");

                 _logger.LogError(exception, _configuration["Messages:Error:RetrieveAllFoodItems"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveAllFoodItems"]);
            }
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> GetAllFoodCategoriesAsync()
        {
            try
            {
                var foodItems = await _foodMenuService.GetFoodCategoriesAsync();
                return Ok(foodItems);
            }
            catch (Exception exception)
            {
                // _logger.LogError(exception, "Error occurred while retrieving all food items.");
                // return StatusCode(500, "An error occurred while retrieving all food items.");
                _logger.LogError(exception, _configuration["Messages:Error:RetrieveFoodCategories"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveFoodCategories"]);
            }
        }
        

        // [HttpGet("{name:length(5,20):alpha}")]
            
        // [HttpGet("Categories/{categoryName:category}")]

        [HttpGet("Categories/{categoryName}")]
        // [ServiceFilter(typeof(CategoryConstraintActionFilter))]
        public async Task<IActionResult> GetFoodItemsByCategoryAsync(string categoryName)
        {
            try
            {
                var foodItems = await _foodMenuService.GetFoodItemsByCategoryAsync(categoryName);

                if (foodItems == null || !foodItems.Any())
                {
                    return NotFound();
                }

                return Ok(foodItems);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, $"Error occurred while retrieving food items with category {categoryName}.");
                // return StatusCode(500, $"An error occurred while retrieving food items with category {categoryName}.");
                _logger.LogError(exception, _configuration["Messages:Error:RetrieveFoodItemsByCategory"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveFoodItemsByCategory"]);
            }
        }

        [HttpGet("Price/{price:decimal}")]
        public async Task<IActionResult> GetFoodItemsByPriceAsync(decimal price)
        {
            try
            {
                var foodItems = await _foodMenuService.GetFoodItemsByPriceAsync(price);

                if (foodItems == null || !foodItems.Any())
                {
                    return NotFound();
                }

                return Ok(foodItems);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, $"Error occurred while retrieving food items with price {price}.");
                // return StatusCode(500, $"An error occurred while retrieving food items with price {price}.");
                _logger.LogError(exception, _configuration["Messages:Error:RetrieveFoodItemsByPrice"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveFoodItemsByPrice"]);
            }
        }

        [HttpGet("Vegan/{isVegan:bool}")]
        public async Task<IActionResult> GetFoodItemsByVegan(bool isVegan)
        {
            try
            {
                var foodItems = await _foodMenuService.GetFoodItemsByVeganAsync(isVegan);

                if (foodItems == null || !foodItems.Any())
                {
                    return NotFound();
                }

                return Ok(foodItems);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, $"Error occurred while retrieving food items by vegan: {isVegan}.");
                // return StatusCode(500, $"An error occurred while retrieving food items by vegan: {isVegan}.");
                _logger.LogError(exception, _configuration["Messages:Error:RetrieveFoodItemsByPrice"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveFoodItemsByPrice"]);
            }
        }


        [HttpGet("{id:int:min(1):max(12)}")]
        public async Task<IActionResult> GetFoodItemAsync(int id)
        {
            try
            {
                var foodItem = await _foodMenuService.GetFoodItemAsync(id);

                if (foodItem == null)
                {
                    return NotFound();
                }

                return Ok(foodItem);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, $"Error occurred while retrieving food item with id {id}.");
                // return StatusCode(500, $"An error occurred while retrieving food item with id {id}.");
                _logger.LogError(exception, _configuration["Messages:Error:RetrieveFoodItem"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveFoodItem"]);
            }
        }

        [HttpGet("Calories/{maxCalories:float:min(0)}")]
        public async Task<IActionResult> GetFoodItemsByCaloriesAsync(float maximumCalories)
        {
            try
            {
                var foodItems = await _foodMenuService.GetFoodItemsByCaloriesAsync(maximumCalories);

                if (foodItems == null || !foodItems.Any())
                {
                    return NotFound();
                }

                return Ok(foodItems);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, $"Error occurred while retrieving food items with calories <= {maxCalories}.");
                // return StatusCode(500, $"An error occurred while retrieving food items with calories <= {maxCalories}.");
                _logger.LogError(exception, _configuration["Messages:Error:RetrieveFoodItemsByCalories"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveFoodItemsByCalories"]);
            }
        }


        [HttpPost]  
         public async Task<IActionResult> AddFoodItemAsync([FromForm] FoodItemRequest foodItemRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                byte[] imageBytes = null;

                using (var memorystream = new MemoryStream())
                {
                    foodItemRequest.image.OpenReadStream().CopyTo(memorystream);
                    imageBytes = memorystream.ToArray();
                }

                var foodItem = new FoodItem
                {
                    name = foodItemRequest.name,
                    price = foodItemRequest.price,
                    description = foodItemRequest.description,
                    imageUrl = imageBytes,
                    category = foodItemRequest.category,
                    isVegan = foodItemRequest.isVegan,
                    calories = foodItemRequest.calories
                };

                await _foodMenuService.AddFoodItemAsync(foodItem);
                return Ok(foodItem);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, "Error occurred while adding a new food item.");
                // return StatusCode(500, "An error occurred while adding a new food item.");
                _logger.LogError(exception, _configuration["Messages:Error:AddFoodItem"]);
                return StatusCode(500, _configuration["Messages:Error:AddFoodItem"]);
            }
        }

        [HttpPut("{id:int:range(1, 12)}")]
        public async Task<IActionResult> UpdateFoodItemAsync(int id, [FromBody] FoodItem foodItem)
        {
            try
            {
                if (id != foodItem.id)
                {
                    return BadRequest();
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _foodMenuService.UpdateFoodItemAsync(foodItem);
                return Ok(foodItem);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, $"Error occurred while updating food item with id {id}.");
                // return StatusCode(500, $"An error occurred while updating food item with id {id}.");
                _logger.LogError(exception, _configuration["Messages:Error:UpdateFoodItem"]);
                return StatusCode(500, _configuration["Messages:Error:UpdateFoodItem"]);
            }
        }

        [HttpDelete("{id:int:min(1)}")]
        public async Task<IActionResult> DeleteFoodItemAsync(int id)
        {
            try
            {
                var foodItem = await _foodMenuService.GetFoodItemAsync(id);

                if (foodItem == null)
                {
                    return NotFound();
                }

                await _foodMenuService.DeleteFoodItemAsync(id);
                return Ok(foodItem);
            }
            catch (Exception exception)
            {
                // _logger.LogError(ex, $"Error occurred while deleting food item with id {id}.");
                // return StatusCode(500, $"An error occurred while deleting food item with id {id}.");
                _logger.LogError(exception, _configuration["Messages:Error:DeleteFoodItem"]);
                return StatusCode(500, _configuration["Messages:Error:DeleteFoodItem"]);
            }
        }

        [HttpGet("Gallery")]
        public async Task<IActionResult> GetGalleryItemsAsync()
        {
            try
            {
                var galleryItems = await _foodMenuService.GetGalleryItemsAsync();

                if (galleryItems == null || !galleryItems.Any())
                {
                    return NotFound();
                }

                return Ok(galleryItems);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, _configuration["Messages:Error:RetrieveGalleryItems"]);
                return StatusCode(500, _configuration["Messages:Error:RetrieveGalleryItems"]);
            }
        }

    }
}
