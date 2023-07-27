using System.Threading.Tasks;
using HakunaMatata.Areas.AdminArea.Controllers;
using HakunaMatata.Models.ViewModels;
using HakunaMatata.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Xunit;

namespace HakunaMatata.Tests.Controllers
{
    public class AccountControllerTests
    {
        [Fact]
        public async Task Login_ValidCredentials_RedirectsToReturnUrl()
        {
            // Arrange
            var accountServicesMock = new Mock<IAccountServices>();
            var user = new VM_Login { Username = "testuser", Password = "testpassword", IsRememberMe = true };
            accountServicesMock.Setup(repo => repo.GetUser(user)).Returns(new User { /* User data */ });
            var controller = new AccountController(accountServicesMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            var returnUrl = "/some/url";

            // Act
            var result = await controller.Login(user);

            // Assert
            var redirectResult = Assert.IsType<RedirectResult>(result);
            Assert.Equal(returnUrl, redirectResult.Url);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsViewWithErrorMessage()
        {
            // Arrange
            var accountServicesMock = new Mock<IAccountServices>();
            var user = new VM_Login { Username = "testuser", Password = "invalidpassword", IsRememberMe = false };
            accountServicesMock.Setup(repo => repo.GetUser(user)).Returns((User)null);
            var controller = new AccountController(accountServicesMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };

            // Act
            var result = await controller.Login(user);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Should return the default view
            Assert.Equal("Sai tài khoản hoặc mật khẩu!", viewResult.ViewBag.Message);
        }

        [Fact]
        public async Task Register_ValidModel_RedirectsToLogin()
        {
            // Arrange
            var accountServicesMock = new Mock<IAccountServices>();
            accountServicesMock.Setup(repo => repo.RegisterUser(It.IsAny<VM_Register>())).ReturnsAsync(true);
            var controller = new AccountController(accountServicesMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            var newUser = new VM_Register { /* Fill valid user data */ };

            // Act
            var result = await controller.Register(newUser);

            // Assert
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Login", redirectResult.ActionName);
            Assert.Null(redirectResult.ControllerName); // Should stay in the same controller
        }

        [Fact]
        public async Task Register_InvalidModel_ReturnsViewWithModelError()
        {
            // Arrange
            var accountServicesMock = new Mock<IAccountServices>();
            var controller = new AccountController(accountServicesMock.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
            controller.ModelState.AddModelError("Email", "The Email field is required.");
            var newUser = new VM_Register { /* Fill invalid user data */ };

            // Act
            var result = await controller.Register(newUser);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Null(viewResult.ViewName); // Should return the default view
            Assert.False(viewResult.ViewData.ModelState.IsValid);
        }
    }
}
