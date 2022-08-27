using System;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAutores.Utilidades
{
    public class SwaggerAgrupaPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace; // Controller v1
            var versionAPI = namespaceControlador.Split('.').Last().ToLower();
            controller.ApiExplorer.GroupName = versionAPI;
        }
    }
}

