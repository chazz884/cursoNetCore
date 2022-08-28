using System;
using Microsoft.AspNetCore.Authorization;
using WebApiAutores.Controllers.V1;
using WebAPIConsole.Tests.Mocks;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
// Implementando test cuando existen dependencias de otras clases usando moks
namespace WebAPIConsole.Tests.PruebasUnitarias
{
    [TestClass]
    public class RootControllerTest
    {
        [TestMethod]
        public async Task SiUsuarioEsAdmin_Obtenemos4Links()
        {
            // preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Ejecucion
            var resultado = await rootController.Get();

            // Verificacion
            Assert.AreEqual(4, resultado.Value.Count());
        }
        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2Links()
        {
            // preparacion
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Resultado = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            rootController.Url = new URLHelperMock();

            // Ejecucion
            var resultado = await rootController.Get();

            // Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }

        [TestMethod]
        public async Task SiUsuarioNoEsAdmin_Obtenemos2Links_UsandoMoq()
        {
            // preparacion
            var mockAuth = new Mock<IAuthorizationService>();
            mockAuth.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
            )).Returns(Task.FromResult(AuthorizationResult.Failed()));
            mockAuth.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
            )).Returns(Task.FromResult(AuthorizationResult.Failed()));
            
            var mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()
            )).Returns(string.Empty);

            var rootController = new RootController(mockAuth.Object);
            rootController.Url = mockUrlHelper.Object;

            // Ejecucion
            var resultado = await rootController.Get();

            // Verificacion
            Assert.AreEqual(2, resultado.Value.Count());
        }
    }
}

