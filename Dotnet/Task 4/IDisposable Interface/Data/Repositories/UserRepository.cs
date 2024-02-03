using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountAPI.Data;
using FoodOrderingSystemAPI.Models;
using FoodOrderingSystemAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrderingSystemAPI.Data.Repositories{
    public class UserRepository : IUserRepository
    {
        private readonly FoodAppDbContext _foodAppDbContext;

        public UserRepository(FoodAppDbContext foodAppDbContext)
        {
            _foodAppDbContext = foodAppDbContext;
        }
        public async Task<List<SignupLogin>> GetUsersAsync()
        {
            return await _foodAppDbContext.SignupLogin.ToListAsync();
        }


        public async Task<bool> IsEmailAlreadyExistsAsync(string email)
        {
            return await Task.Run(() => _foodAppDbContext.SignupLogin.Any(users => users.email == email));
        }

        public async Task AddUserAsync(SignupLogin user)
        {
            await _foodAppDbContext.SignupLogin.AddAsync(user);
            await _foodAppDbContext.SaveChangesAsync();
        }
        public async Task AddUserProfileAsync(UserProfile userProfile)
        {
            await _foodAppDbContext.UserProfile.AddAsync(userProfile);
            await _foodAppDbContext.SaveChangesAsync();
        }

        public async Task<SignupLogin> GetUsersByEmailAndPasswordAsync(string email, string password)
        {
                return await _foodAppDbContext.SignupLogin.FirstOrDefaultAsync(users => users.email.Equals(email) && users.password.Equals(password));
        }

        public async Task<SignupLogin> GetUserByEmailAsync(string email)
        {
                return await _foodAppDbContext.SignupLogin.FirstOrDefaultAsync(users => users.email.Equals(email));
        }
        
        public async Task<UserProfile> GetUserProfileByUserIdAsync(int userId)
        {
            return await _foodAppDbContext.UserProfile.FirstOrDefaultAsync(userProfile => userProfile.userId == userId);
        }

         public async Task<bool> UpdateUserProfileAsync(UserProfile userProfile)
        {
            var existingProfile = await _foodAppDbContext.UserProfile.FirstOrDefaultAsync(profile => profile.userId == userProfile.userId);


            if (existingProfile == null)
            {
                return false;
            }

            existingProfile.name = userProfile.name;
            existingProfile.mobileNumber = userProfile.mobileNumber;
            existingProfile.address = userProfile.address;
            existingProfile.pincode = userProfile.pincode;

            try
            {
                _foodAppDbContext.Entry(existingProfile).State = EntityState.Modified;
                await _foodAppDbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> UpdateUserAsync(SignupLogin user)
        {
            try
            {
                _foodAppDbContext.Entry(user).State = EntityState.Modified;
                await _foodAppDbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task DeleteUserProfileAsync(int profileId)
        {
            var userProfile = await _foodAppDbContext.UserProfile.FindAsync(profileId);
            if (userProfile != null)
            {
                _foodAppDbContext.UserProfile.Remove(userProfile);
                await _foodAppDbContext.SaveChangesAsync();
            }
        }

         public async Task<bool> SaveOTPAsync(string email, string otp, int userId)
        {
            try
            {
                // Check if an OTP record already exists for the given userId and IsVerified status
                var existingOtpModel = _foodAppDbContext.otpModels.FirstOrDefault(otpDetail => otpDetail.userId == userId && !otpDetail.isVerified);

                if (existingOtpModel != null)
                {
                    // If an existing record is found, update the OTPValue and mark it as Modified
                    existingOtpModel.otpValue = otp;
                    _foodAppDbContext.Entry(existingOtpModel).State = EntityState.Modified;
                    await _foodAppDbContext.SaveChangesAsync();
                }
                else
                {
                    // If no existing record is found, create a new OTPModel
                    var otpModel = new OtpModel
                    {
                        email = email,
                        otpValue = otp,
                        isVerified = false,
                        userId = userId,
                    };
                    _foodAppDbContext.otpModels.Add(otpModel);
                    await _foodAppDbContext.SaveChangesAsync();

                }


                return true;
            }
            catch (Exception)
            {
                return false; 
            }
        }



        public async Task<bool> VerifyOTPAsync(string email, string otp)
        {
            try
            {
               var otpModel = _foodAppDbContext.otpModels.FirstOrDefault(otpModel => otpModel.email == email && !otpModel.isVerified);
                if (otpModel == null)
                {
                    return false; 
                }

                if (otp == otpModel.otpValue)
                {
                    otpModel.isVerified = true;
                    await _foodAppDbContext.SaveChangesAsync();
                    return true; 
                }

                return false; 
            }
            catch (Exception)
            {
                return false; 
            }
        }

        public async Task<bool> ResendOTPAsync(string email, string otp)
        {
            try
            {
                var otpModel = _foodAppDbContext.otpModels.FirstOrDefault(otpModel => otpModel.email == email && !otpModel.isVerified);
                if (otpModel != null)
                {
                    otpModel.otpValue = otp;
                    await _foodAppDbContext.SaveChangesAsync();
                }

                return true; 
            }
            catch (Exception)
            {
                return false; 
            }
        }


    }
}
