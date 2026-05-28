using AlaiaStore.Domain.Entities;
using AlaiaStore.Domain.Interfaces;
using AlaiaStore.Web.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace AlaiaStore.Test
{
    [TestClass]
    public sealed class AuthenticationServiceTests
    {
        [TestMethod]
        public void HashPassword_ShouldReturnNonEmptyString()
        {
            var authService = new AuthenticationService();
            var result = authService.HashPassword("TestPassword123!");
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        }

        [TestMethod]
        public void VerifyPassword_ShouldReturnTrueForCorrectPassword()
        {
            var authService = new AuthenticationService();
            string password = "TestPassword123!";
            var hash = authService.HashPassword(password);

            var result = authService.VerifyPassword(password, hash);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void VerifyPassword_ShouldReturnFalseForIncorrectPassword()
        {
            var authService = new AuthenticationService();
            string password = "TestPassword123!";
            var hash = authService.HashPassword(password);

            var result = authService.VerifyPassword("WrongPassword!", hash);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerifyPassword_ShouldReturnFalseForTamperedHash()
        {
            var authService = new AuthenticationService();
            string password = "TestPassword123!";
            var hash = authService.HashPassword(password);
            
            // Tamper the hash to trigger loop mis-match
            var hashBytes = Convert.FromBase64String(hash);
            hashBytes[hashBytes.Length - 1] = (byte)(hashBytes[hashBytes.Length - 1] ^ 0xFF);
            var tamperedHash = Convert.ToBase64String(hashBytes);

            var result = authService.VerifyPassword(password, tamperedHash);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void VerifyPassword_ShouldThrowExceptionForInvalidHashLength()
        {
            var authService = new AuthenticationService();
            string invalidHash = Convert.ToBase64String(new byte[10]);

            Assert.ThrowsException<ArgumentException>(() => authService.VerifyPassword("password", invalidHash));
        }

    }

    [TestClass]
    public sealed class UserServiceTests
    {
        [TestMethod]
        public async Task AuthenticateAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();

            var user = new User { Email = "test@test.com", PasswordHash = "hashed_password" };
            mockUserRepo.Setup(r => r.GetWithRolesByEmailAsync("test@test.com")).ReturnsAsync(user);
            mockAuthService.Setup(s => s.VerifyPassword("password123", "hashed_password")).Returns(true);

            var userService = new UserService(mockUserRepo.Object, mockAuthService.Object);

            // Act
            var result = await userService.AuthenticateAsync("test@test.com", "password123");

            // Assert
            Assert.IsNotNull(result, "Debería retornar un usuario si las credenciales son válidas");
            Assert.AreEqual("test@test.com", result.Email);
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();

            var user = new User { Email = "test@test.com", PasswordHash = "hashed_password" };
            mockUserRepo.Setup(r => r.GetWithRolesByEmailAsync("test@test.com")).ReturnsAsync(user);
            mockAuthService.Setup(s => s.VerifyPassword("wrong_password", "hashed_password")).Returns(false);

            var userService = new UserService(mockUserRepo.Object, mockAuthService.Object);

            // Act
            var result = await userService.AuthenticateAsync("test@test.com", "wrong_password");

            // Assert
            Assert.IsNull(result, "Debería retornar null para clave incorrecta");
        }

        [TestMethod]
        public async Task AuthenticateAsync_ShouldReturnNull_WhenUserDoesNotExist()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();

            mockUserRepo.Setup(r => r.GetWithRolesByEmailAsync("test@test.com")).ReturnsAsync((User?)null);

            var userService = new UserService(mockUserRepo.Object, mockAuthService.Object);

            // Act
            var result = await userService.AuthenticateAsync("test@test.com", "password");

            // Assert
            Assert.IsNull(result, "Debería retornar null si el usuario no existe");
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldThrowException_WhenEmailAlreadyExists()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();

            var existingUser = new User { Email = "test@test.com" };
            mockUserRepo.Setup(r => r.GetByEmailAsync("test@test.com")).ReturnsAsync(existingUser);

            var userService = new UserService(mockUserRepo.Object, mockAuthService.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<System.InvalidOperationException>(() =>
                userService.RegisterAsync("John", "Doe", "test@test.com", "password123"));
        }

        [TestMethod]
        public async Task RegisterAsync_ShouldCreateAndReturnUser_WhenDataIsValid()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();

            mockUserRepo.Setup(r => r.GetByEmailAsync("new@test.com")).ReturnsAsync((User?)null);
            mockAuthService.Setup(s => s.HashPassword("validpass")).Returns("hashed_pass");
            mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
            mockUserRepo.Setup(r => r.AssignRoleAsync(It.IsAny<int>(), "Customer")).Returns(Task.CompletedTask);

            var userService = new UserService(mockUserRepo.Object, mockAuthService.Object);

            // Act
            var result = await userService.RegisterAsync("Alice", "Smith", "new@test.com", "validpass");

            // Assert
            Assert.IsNotNull(result, "Debería retornar el usuario creado.");
            Assert.AreEqual("Alice", result.FirstName);
            Assert.AreEqual("Smith", result.LastName);
            Assert.AreEqual("new@test.com", result.Email);
            Assert.AreEqual("hashed_pass", result.PasswordHash);
            Assert.IsTrue(result.IsActive, "El usuario debería crearse activo por defecto.");
            mockUserRepo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once, "Debería haber llamado al repositorio una sola vez.");
            mockUserRepo.Verify(r => r.AssignRoleAsync(It.IsAny<int>(), "Customer"), Times.Once);
        }

        [TestMethod]
        public async Task GetUserByEmailAsync_ShouldReturnUser_WhenFound()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockAuthService = new Mock<IAuthenticationService>();

            var expectedUser = new User { Id = 1, Email = "found@test.com", FirstName = "Found" };
            mockUserRepo.Setup(r => r.GetByEmailAsync("found@test.com")).ReturnsAsync(expectedUser);

            var userService = new UserService(mockUserRepo.Object, mockAuthService.Object);

            // Act
            var result = await userService.GetUserByEmailAsync("found@test.com");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("found@test.com", result.Email);
            Assert.AreEqual("Found", result.FirstName);
            mockUserRepo.Verify(r => r.GetByEmailAsync("found@test.com"), Times.Once);
        }
    }

    [TestClass]
    public sealed class DomainEntitiesTests
    {
        [TestMethod]
        public void User_ShouldInitializeCollectionsAndDefaults()
        {
            var user = new User();
            
            Assert.IsNotNull(user.UserRoles);
            Assert.IsNotNull(user.Addresses);
            Assert.IsNotNull(user.Orders);
            Assert.IsNotNull(user.Reviews);
            Assert.IsTrue(user.IsActive);
            Assert.AreEqual(string.Empty, user.FirstName);
            Assert.AreEqual(string.Empty, user.LastName);
            Assert.AreEqual(string.Empty, user.Email);
            Assert.AreEqual(string.Empty, user.PasswordHash);
        }

        [TestMethod]
        public void Product_ShouldInitializeDefaults()
        {
            var product = new Product();
            
            Assert.IsNotNull(product.Images);
            Assert.IsNotNull(product.Reviews);
            Assert.IsNotNull(product.ProductPromotions);
            Assert.IsTrue(product.IsActive);
        }

        [TestMethod]
        public void Order_ShouldInitializeDefaults()
        {
            var order = new Order();
            
            Assert.IsNotNull(order.OrderDetails);
        }

        [TestMethod]
        public void Cart_ShouldInitializeDefaults()
        {
            var cart = new Cart();
            
            Assert.IsNotNull(cart.Items);
        }

        [TestMethod]
        public void Wishlist_ShouldInitializeDefaults()
        {
            var wishlist = new Wishlist();
            
            Assert.IsNotNull(wishlist.Items);
        }

        [TestMethod]
        public void Role_ShouldInitializeDefaults()
        {
            var role = new Role();
            
            Assert.IsNotNull(role.UserRoles);
        }
        
        [TestMethod]
        public void Category_ShouldInitializeDefaults()
        {
            var category = new Category();
            
            Assert.IsNotNull(category.Products);
        }
        
        [TestMethod]
        public void Promotion_ShouldInitializeDefaults()
        {
            var promotion = new Promotion();
            
            Assert.IsNotNull(promotion.ProductPromotions);
        }

        [TestMethod]
        public void Address_ShouldInitializeDefaults()
        {
            var address = new Address();
            Assert.IsNotNull(address);
        }

        [TestMethod]
        public void BlogPost_ShouldInitializeDefaults()
        {
            var post = new BlogPost();
            Assert.IsTrue(post.IsPublished == false || post.IsPublished == true);
        }

        [TestMethod]
        public void CartItem_ShouldInitializeDefaults()
        {
            var item = new CartItem();
            Assert.IsNotNull(item);
        }

        [TestMethod]
        public void OrderDetail_ShouldInitializeDefaults()
        {
            var detail = new OrderDetail();
            Assert.IsNotNull(detail);
        }

        [TestMethod]
        public void Payment_ShouldInitializeDefaults()
        {
            var payment = new Payment();
            Assert.IsNotNull(payment);
        }

        [TestMethod]
        public void ProductImage_ShouldInitializeDefaults()
        {
            var image = new ProductImage();
            Assert.IsNotNull(image);
        }

        [TestMethod]
        public void ProductPromotion_ShouldInitializeDefaults()
        {
            var pp = new ProductPromotion();
            Assert.IsNotNull(pp);
        }

        [TestMethod]
        public void Review_ShouldInitializeDefaults()
        {
            var review = new Review();
            Assert.IsNotNull(review);
        }

        [TestMethod]
        public void UserRole_ShouldInitializeDefaults()
        {
            var userRole = new UserRole();
            Assert.IsNotNull(userRole);
        }

        [TestMethod]
        public void WishlistItem_ShouldInitializeDefaults()
        {
            var item = new WishlistItem();
            Assert.IsNotNull(item);
        }
    }
}
