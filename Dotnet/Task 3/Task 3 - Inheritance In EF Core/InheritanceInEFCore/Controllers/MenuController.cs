using InheritanceInEFCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using InheritanceInEFCore.Models;
namespace InheritanceInEFCore.Controllers{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IFoodItemRepository _foodItemRepository;

        public MenuController(IFoodItemRepository foodItemRepository)
        {
            _foodItemRepository = foodItemRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var foodItems = _foodItemRepository.GetAll();
            return Ok(foodItems);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var foodItem = _foodItemRepository.GetById(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            return Ok(foodItem);
        }

        [HttpPost]
        public IActionResult Post([FromBody] FoodItem foodItem)
        {
            if (foodItem == null)
            {
                return BadRequest("Invalid data");
            }

            _foodItemRepository.Add(foodItem);

            return CreatedAtAction(nameof(Get), new { id = foodItem.FoodItemId }, foodItem);
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] FoodItem foodItem)
        {
            if (foodItem == null || id != foodItem.FoodItemId)
            {
                return BadRequest("Invalid data");
            }

            var existingFoodItem = _foodItemRepository.GetById(id);

            if (existingFoodItem == null)
            {
                return NotFound();
            }

            _foodItemRepository.Update(foodItem);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var foodItem = _foodItemRepository.GetById(id);

            if (foodItem == null)
            {
                return NotFound();
            }

            _foodItemRepository.Remove(id);

            return NoContent();
        }
    }
}

