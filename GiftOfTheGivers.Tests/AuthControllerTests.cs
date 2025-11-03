using GiftOfTheGivers.Controllers;
using GiftOfTheGivers.Models;
using GiftOfTheGivers.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace GiftOfTheGivers.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<SignInManager<ApplicationUser>> _signInManagerMock;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _userManagerMock = GetUserManagerMock();
            _signInManagerMock = GetSignInManagerMock();

            _controller = new AuthController(_userManagerMock.Object, _signInManagerMock.Object);
        }

        // ✅ Helper methods to mock Identity managers
        private Mock<UserManager<ApplicationUser>> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<ApplicationUser>>();
            return new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<ApplicationUser>> GetSignInManagerMock()
        {
            var userManager = GetUserManagerMock();
            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<ApplicationUser>>();
            return new Mock<SignInManager<ApplicationUser>>(
                userManager.Object, contextAccessor.Object, claimsFactory.Object,
                null, null, null, null);
        }

        [Fact]
        public async Task Register_ValidModel_RedirectsToUserIndex()
        {
            // Arrange
            var model = new RegisterViewModel
            {
                Email = "test@example.com",
                Password = "Password1!",
                Name = "John Doe"
            };

            var user = new ApplicationUser { Email = model.Email, UserName = model.Email, Role = "User" };

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), model.Password))
                            .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ApplicationUser>(), "User"))
                            .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("User", redirect.ControllerName);
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewWithModel()
        {
            // Arrange
            var model = new RegisterViewModel();
            _controller.ModelState.AddModelError("Email", "Required");

            // Act
            var result = await _controller.Register(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(model, viewResult.Model);
        }

        [Fact]
        public async Task Login_ValidCredentials_RedirectsToUserIndex()
        {
            // Arrange
            var model = new LoginViewModel { Email = "user@example.com", Password = "Password1" };

            var user = new ApplicationUser { Email = model.Email, UserName = model.Email };
            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, false, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _userManagerMock.Setup(x => x.FindByEmailAsync(model.Email))
                            .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user))
                            .ReturnsAsync(new List<string> { "User" });

            // Act
            var result = await _controller.Login(model);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
            Assert.Equal("User", redirect.ControllerName);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithError()
        {
            // Arrange
            var model = new LoginViewModel { Email = "user@example.com", Password = "wrong" };

            _signInManagerMock.Setup(x => x.PasswordSignInAsync(model.Email, model.Password, false, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.False(_controller.ModelState.IsValid); // Error added
        }

        [Fact]
        public async Task Logout_Always_RedirectsToLogin()
        {
            // Arrange
            _signInManagerMock.Setup(x => x.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Logout();

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirect.ActionName);
            Assert.Equal("Auth", redirect.ControllerName);
        }

    }

}
