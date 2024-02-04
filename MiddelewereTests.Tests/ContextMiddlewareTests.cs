using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;

namespace MiddelewereTests.Tests
{
    [TestClass]
    public class ContextMiddlewareTests
    {
        private IContextService _contextService;

        [TestMethod]
        public async Task ContextMiddleware_HappyFlow()
        {
            _contextService = A.Fake<IContextService>();

            A.CallTo(() => _contextService.GetContext("header")).Returns(new Context());
            A.CallTo(() => _contextService.GetContext("body")).Returns(new Context());
            A.CallTo(() => _contextService.GetContext("query")).Returns(new Context());
            A.CallTo(() => _contextService.GetContext("from")).Returns(new Context());

            var contextMiddleware = new ContextMiddleware(
                innerHttpContext => Task.CompletedTask,
                _contextService);

            var context = new DefaultHttpContext
            {
                Response = { Body = new MemoryStream() }
            };

            //add stuff to the request header
            context.Request.Headers.Append("x-httpContext", "header");

            //add data to request body
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{\"requestContext\" :\"body\"}"));

            
            context.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                { "form", "form" }
            });

            //add stuff to the query string
            context.Request.QueryString = context.Request.QueryString.Add("queryStringContext", "query");

            //Call the middleware
            await contextMiddleware.InvokeAsync(context);

            A.CallTo(() => _contextService.GetContext("header")).MustHaveHappened();
            A.CallTo(() => _contextService.GetContext("body")).MustHaveHappened();
            A.CallTo(() => _contextService.GetContext("query")).MustHaveHappened();
            A.CallTo(() => _contextService.GetContext("form")).MustHaveHappened();
            //context.Response.Body.Seek(0, SeekOrigin.Begin);
            //var body = new StreamReader(context.Response.Body).ReadToEnd();
        }
 
    }
}