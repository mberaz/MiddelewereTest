using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.Json.Nodes;

namespace MiddelewereTests
{
    public class ContextMiddleware
    {
        private readonly IContextService _contextService;
        private readonly RequestDelegate _next;

        public ContextMiddleware(RequestDelegate next, IContextService contextService)
        {
            _next = next;
            _contextService = contextService;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            httpContext.Request.Headers.TryGetValue("x-httpContext", out var contextFromHeader);

            var requestObject = await JsonNode.ParseAsync(httpContext.Request.Body);
            var requestContext = (string)requestObject["requestContext"];

            var formContext = httpContext.Request.Form["form"].ToString();
            
            var queryStringContext = httpContext.Request.Query["queryStringContext"].ToString();

            var contextByHeader = await _contextService.GetContext(contextFromHeader);
            var contextByBody = await _contextService.GetContext(requestContext);
            // Call the next delegate/middleware in the pipeline.
            await _next(httpContext);

            var contextByForm = await _contextService.GetContext(formContext);
            var contextByQuery = await _contextService.GetContext(queryStringContext);
        }
    }
}
