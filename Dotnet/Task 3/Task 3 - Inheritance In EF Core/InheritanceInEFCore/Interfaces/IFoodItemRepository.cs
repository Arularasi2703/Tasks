using InheritanceInEFCore.Models;

namespace InheritanceInEFCore.Interfaces{
    public interface IFoodItemRepository
    {
        IEnumerable<FoodItem> GetAll();
        FoodItem GetById(int id);
        void Add(FoodItem foodItem);
        void Update(FoodItem foodItem);
        void Remove(int id);
    }
}