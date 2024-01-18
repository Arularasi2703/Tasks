using System.Collections.Generic;
using System.Threading.Tasks;
using FoodOrderingSystemAPI.Models;

namespace FoodOrderingSystemAPI.Interfaces{
  public interface IUserRepository
  {
      Task<List<SignupLogin>> GetUsersAsync();
      Task<bool> IsEmailAlreadyExistsAsync(string email);
      Task AddUserAsync(SignupLogin user);
      Task<SignupLogin> GetUsersByEmailAndPasswordAsync(string email, string password);
      Task<SignupLogin> GetUserByEmailAsync(string email);
      Task AddUserProfileAsync(UserProfile userProfile);
      Task<UserProfile> GetUserProfileByUserIdAsync(int userId);
      Task<bool> UpdateUserProfileAsync(UserProfile userProfile);
      Task DeleteUserProfileAsync(int profileId);
      Task<bool> UpdateUserAsync(SignupLogin user);
      Task<bool> SaveOTPAsync(string email, string otp, int userId);
      Task<bool> VerifyOTPAsync(string email, string otp);
      Task<bool> ResendOTPAsync(string email, string otp);


  }
}
