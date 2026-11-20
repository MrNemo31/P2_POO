using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace LiteArt.Poo.Previa;

// Renderiza as Views existentes sem criar controllers ou duplicar o HTML.
internal sealed class PaginaRazor(string secao, string pagina, object? model = null) : IResult
{
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var routeData = new RouteData();
        routeData.Values["controller"] = secao;
        routeData.Values["action"] = pagina;
        var actionContext = new ActionContext(httpContext, routeData, new ActionDescriptor());
        var view = new ViewResult
        {
            ViewName = $"~/Views/{secao}/{pagina}.cshtml",
            ViewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), actionContext.ModelState) { Model = model }
        };
        await httpContext.RequestServices.GetRequiredService<IActionResultExecutor<ViewResult>>().ExecuteAsync(actionContext, view);
    }
}
