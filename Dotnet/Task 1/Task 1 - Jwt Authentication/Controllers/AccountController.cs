// Title        : Food Ordering System ( FoodOrderingSystem API )
// Description  : Manages user authentication, account creation, login, profile management, cart operations,
//                and order handling for a food ordering system
// Author       : Arularasi J
// Created at   : 21/07/2023
// Updated at   : 20/12/2023
// Reviewed by  : 
// Reviewed at  : 

using System.Security.Claims;
using FoodOrderingSystemAPI.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FoodOrderingSystemAPI.Interfaces;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.IO;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
namespace FoodOrderingSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AccountController> _logger;
        // private const int ResendOtpIntervalSeconds = 30;
        private readonly EmailSettings _emailSettings;
        private readonly IOptions<OtpGenerationSettings> _otpGenerationSettings;

        public AccountController(IUserRepository userRepository, IConfiguration configuration, IOptionsMonitor<EmailSettings> emailSettings,IOptions<OtpGenerationSettings> otpGenerationSettings,ILogger<AccountController> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _emailSettings = emailSettings.CurrentValue;
            _otpGenerationSettings = otpGenerationSettings;
            _logger = logger;
        }


        [AllowAnonymous]
        [HttpGet("IsEmailAvailable")]
        public async Task<IActionResult> IsEmailAvailable(string email)
        {
            try
            {
                var isEmailAlreadyExists = await _userRepository.IsEmailAlreadyExistsAsync(email);
                return Ok(!isEmailAlreadyExists);
            }
            catch (Exception exception)
            {
                string error = _configuration["Messages:FailureMessages:IsEmailAvailableError"];
                _logger.LogError(exception, error);
                return StatusCode(500, error);
            }
        }

        [AllowAnonymous]
        [HttpGet("IsValidUser")]
        public async Task<IActionResult> IsValidUser(string email)
        {
            try
            {
                var isEmailAlreadyExists = await _userRepository.IsEmailAlreadyExistsAsync(email);
                return Ok(isEmailAlreadyExists);
            }
            catch (Exception exception)
            {
                // return StatusCode(500, exception.Message);
                 string error = _configuration["Messages:FailureMessages:IsValidUserError"];
                _logger.LogError(exception, error);
                return StatusCode(500, error);
            }
        }

        private string GenerateOTP()
        {
            Random random = new Random();
            int minimumOtpValue = _configuration.GetValue<int>("AppSettings:OtpGeneration:MinOtpValue");
            int maximumOtpValue = _configuration.GetValue<int>("AppSettings:OtpGeneration:MaxOtpValue");

            int otp = random.Next(minimumOtpValue, maximumOtpValue);
            return otp.ToString();
        }

        private async Task<bool> SendOTPByEmail(string email, string otp)
        {
            try
            {
                string fromMail = _configuration["EmailSettings:FromMail"];
                string fromPassword = _configuration["EmailSettings:FromPassword"];
                string otpEmailTemplate = _configuration["EmailSettings:OtpEmailTemplate"];
                string otpEmailSubject = _configuration["EmailSettings:OtpEmailSubject"];
                string smtpHost = _configuration["EmailSettings:SmtpHost"];
                int smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);

                MailMessage mailmessage = new MailMessage();
                mailmessage.From = new MailAddress(fromMail);
                mailmessage.Subject = otpEmailSubject;
                mailmessage.To.Add(new MailAddress(email));
                mailmessage.Body = otpEmailTemplate.Replace("{{otp}}", otp);
                mailmessage.IsBodyHtml = true;

                var smtpClient = new SmtpClient(smtpHost)
                {
                    Port = smtpPort,
                    Credentials = new NetworkCredential(fromMail, fromPassword),
                    EnableSsl = true,
                };
                await smtpClient.SendMailAsync(mailmessage);
                return true;
            }
            catch (Exception exception)
            {
                return false;
            }
        }


        [AllowAnonymous]
        [HttpPost("Signup")]
        public async Task<IActionResult> Signup([FromBody] SignupLogin signup)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _userRepository.AddUserAsync(signup);
                        string profilePicturePath = _configuration["ProfilePicturePath"];
                        byte[] profilePictureBytes = System.IO.File.ReadAllBytes(profilePicturePath);
                        var userProfile = new UserProfile
                        {
                            name = signup.name,
                            email = signup.email,
                            userId = signup.userId,
                            profilePicture = profilePictureBytes
                        };
                        await _userRepository.AddUserProfileAsync(userProfile);
                    // Generate OTP
                    string otp = GenerateOTP();

                    // Send OTP to the user's registered email
                    bool isOTPSent = await SendOTPByEmail(signup.email, otp);

                    if (isOTPSent)
                    {
                        // Store the OTP in the database for verification later
                        bool isOtpStored = await _userRepository.SaveOTPAsync(signup.email, otp,signup.userId);

                        if (!isOtpStored)
                        {
                            // return StatusCode(500, "Failed to store OTP in the database.");
                            string otpStoreFailureMessage = _configuration["Messages:FailureMessages:OtpStoreError"];
                            return StatusCode(500, otpStoreFailureMessage);
                        }

                        return Ok();
                    }
                    else
                    {
                        string otpSendFailureMessage = _configuration["Messages:FailureMessages:OtpSendError"];
                            return StatusCode(500, otpSendFailureMessage);
                        // return StatusCode(500, "Failed to send OTP to the user's email.");
                    }
                }
                else
                {
                    var errors = ModelState.SelectMany(modelStateEntry => modelStateEntry.Value.Errors)
                              .Select(errorEntry => errorEntry.ErrorMessage)
                              .ToList();
                    return BadRequest(errors);
                }

            }
            catch (Exception exception)
            {
                string error = _configuration["Messages:FailureMessages:SignupError"];
                _logger.LogError(exception, error);
                return StatusCode(500, error);
            }
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] Login login)
        {
            if (!ModelState.IsValid)
            {
                // Return validation errors
                return BadRequest(ModelState);
            }
            try
            {
                var user = await _userRepository.GetUsersByEmailAndPasswordAsync(login.email, login.password);

                if (user != null)
                {
                    // HttpContext.Session.SetInt32("UserId", user.userId);
                    string userIdKey = _configuration["SessionSettings:UserId"];
                    HttpContext.Session.SetInt32(userIdKey, user.userId);
                    var userIdClaimType = _configuration["JwtSettings:UserIdClaimType"];
                    List<Claim> claims = new List<Claim>(){
                        new Claim(ClaimTypes.NameIdentifier, login.email),
                        new Claim(userIdClaimType, user.userId.ToString())
                    };
                    if ((bool)user.isAdmin)
                    {
                        string adminRole = _configuration["RoleNames:Admin"];
                        claims.Add(new Claim(ClaimTypes.Role, adminRole));
                    }
                    else
                    {
                        string userRole = _configuration["RoleNames:User"];
                        claims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = GenerateJwtToken(claims);

                    return Ok(new { Token = token, User = user });
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception exception)
            {
                string loginFailureMessage = _configuration["Messages:FailureMessages:LoginError"];
                _logger.LogError(exception, loginFailureMessage);
                return StatusCode(500, loginFailureMessage);
            }
        }

        private string GenerateJwtToken(List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");
            var issuer = jwtSettings.GetValue<string>("Issuer");
            var audience = jwtSettings.GetValue<string>("Audience");

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7), // Set token expiration
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtSecurityTokenHandler.CreateToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(token);
        }
    
        
        [HttpGet("UserProfile/{userId}")]
        public async Task<IActionResult> UserProfile(int userId)
        {
            try
            {
                // int userId = HttpContext.Session.GetInt32("UserId") ?? 0;
                var userProfile = await _userRepository.GetUserProfileByUserIdAsync(userId);
                
                if (userProfile != null)
                {
                    return Ok(userProfile);
                }
                else
                {
                    string userProfileNotFound = _configuration["Messages:FailureMessages:UserProfileNotFoundError"];
                    return NotFound(userProfileNotFound);

                    
                }
            }
            catch (Exception exception)
            {
                // return StatusCode(500, "An error occurred while retrieving user profile.");
                    string userProfileErrorMessage = _configuration["Messages:FailureMessages:RetrieveUserProfileError"];
                    _logger.LogError(exception, userProfileErrorMessage);
                     return StatusCode(500, userProfileErrorMessage);
            }
        }

        [HttpPost("UserProfile")]
        public async Task<IActionResult> UpdateUserProfile([FromForm]UpdateUserProfileModel updateUserProfileModel)
        {
            try
            {
                int userId = updateUserProfileModel.userId;
                var existingProfile = await _userRepository.GetUserProfileByUserIdAsync(userId);

                if (existingProfile != null)
                {
                    existingProfile.name = updateUserProfileModel.name;
                    existingProfile.email = updateUserProfileModel.email;
                    existingProfile.mobileNumber = updateUserProfileModel.mobileNumber;
                    existingProfile.address = updateUserProfileModel.address;
                    existingProfile.pincode = updateUserProfileModel.pincode;

                    // Convert the profile picture to a byte array
                    if (updateUserProfileModel.profilePicture != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await updateUserProfileModel.profilePicture.CopyToAsync(memoryStream);
                            existingProfile.profilePicture = memoryStream.ToArray();
                        }
                    }

                    await _userRepository.UpdateUserProfileAsync(existingProfile);

                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception exception)
            {
                string updateUserProfileErrorMessage = _configuration["Messages:FailureMessages:UpdateProfileError"];
                _logger.LogError(exception, updateUserProfileErrorMessage);
                return StatusCode(500, updateUserProfileErrorMessage);
                // return StatusCode(500, "An error occurred while updating user profile.");
            }
        }


        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            try
            {
                var userIdentity = HttpContext.User.Identity as ClaimsIdentity;
                if (userIdentity != null)
                {
                    var userIdClaim = userIdentity.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null)
                    {
                        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        string logoutSuccessMessage = _configuration["Messages:SuccessMessages:LogoutSuccess"];
                        return Ok(logoutSuccessMessage);
                        // return Ok("User logged out successfully");

                    }
                }
                string invalidUserErrorMessage = _configuration["Messages:FailureMessages:InvalidUser"];
                return BadRequest(invalidUserErrorMessage);
            }
            catch (Exception exception)
            {
                string logoutFailureErrorMessage = _configuration["Messages:FailureMessages:LogoutError"];
                _logger.LogError(exception,logoutFailureErrorMessage);
                return StatusCode(500, logoutFailureErrorMessage);
            }
        }

        [AllowAnonymous]
        [HttpPost("VerifyOtp")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerificationModel otpverificationmodel)
        {
            try
            {
                bool isOtpVerified = await _userRepository.VerifyOTPAsync(otpverificationmodel.emailaddress, otpverificationmodel.otp);
                Console.WriteLine(otpverificationmodel.otp);

                if (isOtpVerified)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception exception)
            {
                string error = _configuration["Messages:FailureMessages:VerifyOtpError"];
                _logger.LogError(exception, error);
                return StatusCode(500, error);
            }
        }

        [AllowAnonymous]
        [HttpPost("ResendOtp")]
        public async Task<IActionResult> ResendOtp([FromBody] string email,int userid)
        {
            try
            {
                // Check if the resend interval has passed
                // var lastOtpRequestTime = HttpContext.Session.GetString("LastOtpRequestTime");

                int resendOtpIntervalSeconds = _configuration.GetValue<int>("AppSettings:ResendOtpIntervalSeconds");
                string lastOtpRequestTimeKey = _configuration["SessionSettings:LastOtpRequestTime"];
                var lastOtpRequestTime = HttpContext.Session.GetString(lastOtpRequestTimeKey);
                if (!string.IsNullOrEmpty(lastOtpRequestTime))
                {
                    var lastRequestTime = DateTime.Parse(lastOtpRequestTime);
                    if ((DateTime.Now - lastRequestTime).TotalSeconds < resendOtpIntervalSeconds)
                    {
                        // return BadRequest($"You can request OTP again after {ResendOtpIntervalSeconds} seconds.");
                        string resendOtpIntervalMessage = string.Format(_configuration["Messages:FailureMessages:ResendOtpInterval"], resendOtpIntervalSeconds);
                        return BadRequest(resendOtpIntervalMessage);
                    }
                }

                // Generate new OTP
                string otp = GenerateOTP();

                // Send OTP to the user's registered email
                bool isOtpResent = await SendOTPByEmail(email, otp);

                if (isOtpResent)
                {
                    // Update the last OTP request time in the session
                    HttpContext.Session.SetString(lastOtpRequestTimeKey, DateTime.Now.ToString());

                    // Store the new OTP in the database for verification later
                    bool isOtpStored = await _userRepository.SaveOTPAsync(email, otp,userid);

                    if (isOtpStored)
                    {
                        string otpResendSuccess = _configuration["Messages:SuccessMessages:OtpResent"];
                        return Ok(otpResendSuccess);
                    }
                    else
                    {
                        string otpStoreError = _configuration["Messages:FailureMessages:OtpStoreError"];
                        return StatusCode(500, otpStoreError);
                    }
                }
                else
                {
                    string otpResendError = _configuration["Messages:FailureMessages:OtpResendFailed"];
                        return StatusCode(500, otpResendError);
                }
            }
            catch (Exception exception)
            {
                string error = _configuration["Messages:FailureMessages:ResendOtpError"];
                _logger.LogError(exception, error);
                return StatusCode(500, error);
            }
        }
    }
}
