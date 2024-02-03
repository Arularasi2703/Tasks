    using NUnit.Framework;
    using Moq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using FoodOrderingSystemAPI.Controllers;
    using FoodOrderingSystemAPI.Interfaces;
    using FoodOrderingSystemAPI.Models;
    using System.Threading.Tasks;
    using System;
    using Microsoft.AspNetCore.Http;
    using System.Text;
    namespace FoodOrderingSystem.Tests{

        [TestFixture]
        public class AccountControllerTests{
            private AccountController _accountController;
            private Mock<IUserRepository> _userRepositoryMock;
            private Mock<IConfiguration> _configurationMock;
            private Mock<ILogger<AccountController>> _loggerMock;
            private Mock<IOptionsMonitor<EmailSettings>> _emailSettingsMock;
            private Mock<IOptions<OtpGenerationSettings>> _otpGenerationSettingsMock;

            [SetUp]
            public void Setup()
            {
                _userRepositoryMock = new Mock<IUserRepository>();
                _configurationMock = new Mock<IConfiguration>();
                _loggerMock = new Mock<ILogger<AccountController>>();
                _emailSettingsMock = new Mock<IOptionsMonitor<EmailSettings>>();
                _otpGenerationSettingsMock = new Mock<IOptions<OtpGenerationSettings>>();

                _accountController = new AccountController(
                    _userRepositoryMock.Object,
                    _configurationMock.Object,
                    _emailSettingsMock.Object,
                    _otpGenerationSettingsMock.Object,
                    _loggerMock.Object
                );
            }


            [Test]
            public async Task UserProfile_Returns_OkResult_When_UserProfileExists()
            {
                // Arrange
                int userId = 1;

                var expectedUserProfile = new UserProfile
                {
                    profileId = 1,
                    name = "Arularasi J",
                    email = "arularasi2002@gmail.com",
                    mobileNumber = "+919361604668",
                    profilePicture = new byte[] { 0x00, 0x01, 0x02, 0x03, 0x04, 0x05 },
                    address =  "No 2nd Rajaji Street, Nellikuppam,Cuddalore",
                    pincode = 607105,
                    userId = 1              
                };

                _userRepositoryMock.Setup(userRepositoryMock => userRepositoryMock.GetUserProfileByUserIdAsync(userId))
                    .ReturnsAsync(expectedUserProfile);

                // Act
                var result = await _accountController.UserProfile(userId);

                // Assert
                Assert.IsInstanceOf<OkObjectResult>(result);
                var okResult = (OkObjectResult)result;
                Assert.IsNotNull(okResult.Value);
            }

            [Test]
            public async Task UserProfile_Returns_NotFoundResult_When_UserProfileNotFound()
            {
                // Arrange
                int userId = 18;

                _userRepositoryMock.Setup(repo => repo.GetUserProfileByUserIdAsync(userId))
                    .ReturnsAsync((UserProfile)null);

                // Act
                var result = await _accountController.UserProfile(userId);

                // Assert
                Assert.IsInstanceOf<NotFoundObjectResult>(result);
                var notFoundResult = (NotFoundObjectResult)result;

                Assert.IsNull(notFoundResult.Value);
                
            }

            
            [Test]
            public async Task UpdateUserProfile_Returns_OkResult_When_UserProfileExists()
            {
                // Arrange
                var updateUserProfileModel = new UpdateUserProfileModel
                {
                    userId = 1,
                    name = "Arularasi J",
                    email = "arularasi2002@gmail.com",
                    mobileNumber = "+919876543210",
                    address = "No 1 Rajaji Street, Nellikuppam, Cuddalore",
                    pincode = 123456,
                    profilePicture = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("UserProfile")), 0, 0, "profilePicture", "profilePicture.jpg")
                };

                var existingProfile = new UserProfile
                {
                    profileId = 1,
                    name = "Arul J",
                    email = "arularasi@gmail.com",
                    mobileNumber = "+919876543210",
                    address = "No 2,Nehru Street,Cuddalore",
                    pincode = 789012,
                    profilePicture = Encoding.UTF8.GetBytes("UserProfile") 
                };

                _userRepositoryMock.Setup(repo => repo.GetUserProfileByUserIdAsync(updateUserProfileModel.userId))
                    .ReturnsAsync(existingProfile);

                _userRepositoryMock.Setup(repo => repo.UpdateUserProfileAsync(It.IsAny<UserProfile>()))
                    .ReturnsAsync(true);

                // Act
                var result = await _accountController.UpdateUserProfile(updateUserProfileModel);

                // Assert
                Assert.IsInstanceOf<OkResult>(result);

                _userRepositoryMock.Verify(userRepositoryMock => userRepositoryMock.GetUserProfileByUserIdAsync(updateUserProfileModel.userId), Times.Once);
                _userRepositoryMock.Verify(userRepositoryMock => userRepositoryMock.UpdateUserProfileAsync(It.IsAny<UserProfile>()), Times.Once);
            }



        [Test]
        [TestCase("arularasi2002@gmail.com")] // Negative Test Case 1: Email already registered
        [TestCase("invalidemail")]            // Negative Test Case 2: Invalid email format
        [TestCase("")]                        // Negative Test Case 3: empty email
        public async Task IsEmailAvailable_InvalidEmail_ReturnsFalse(string email)
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.IsEmailAlreadyExistsAsync(email))
                .ReturnsAsync(false);

            // Act
            var result = await _accountController.IsEmailAvailable(email) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsFalse((bool)result.Value);
        }


        [Test]
        [TestCaseSource(nameof(IsEmailAvailableTestData))]
        public async Task IsEmailAvailable_InvalidEmail_ReturnsFalse(string email, bool expectedResult)
        {
            // Arrange
            _userRepositoryMock.Setup(repo => repo.IsEmailAlreadyExistsAsync(email))
                .ReturnsAsync(false);

            // Act
            var result = await _accountController.IsEmailAvailable(email) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedResult, (bool)result.Value);
        }

        private static IEnumerable<TestCaseData> IsEmailAvailableTestData()
        {
            yield return new TestCaseData("arularasi2002@gmail.com", false).SetName("Email already registered");
            yield return new TestCaseData("invalidemail", false).SetName("Invalid email format");
            yield return new TestCaseData("", false).SetName("Empty email");
        }



            



            










        }
    }
