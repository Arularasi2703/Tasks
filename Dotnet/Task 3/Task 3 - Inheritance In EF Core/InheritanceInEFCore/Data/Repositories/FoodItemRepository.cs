using InheritanceInEFCore.Data;
using InheritanceInEFCore.Interfaces;
using InheritanceInEFCore.Models;

namespace InheritanceInEFCore.Repositories{
    public class FoodItemRepository : IFoodItemRepository
    {
        private readonly FoodItemDbContext _foodItemDbContext;

        public FoodItemRepository(FoodItemDbContext foodItemDbContext)
        {
            _foodItemDbContext = foodItemDbContext;
        }

        public IEnumerable<FoodItem> GetAll()
        {
            return _foodItemDbContext.FoodItems.ToList();
        }

        public FoodItem GetById(int id)
        {
            return _foodItemDbContext.FoodItems.Find(id);
        }

        public void Add(FoodItem foodItem)
        {
            _foodItemDbContext.FoodItems.Add(foodItem);
            _foodItemDbContext.SaveChanges();
        }

        public void Update(FoodItem foodItem)
        {
            _foodItemDbContext.FoodItems.Update(foodItem);
            _foodItemDbContext.SaveChanges();
        }

        public void Remove(int id)
        {
            var foodItem = _foodItemDbContext.FoodItems.Find(id);
            if (foodItem != null){
                _foodItemDbContext.FoodItems.Remove(foodItem);
                _foodItemDbContext.SaveChanges();
            }
        }
    }
}