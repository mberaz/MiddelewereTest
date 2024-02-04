namespace MiddelewereTests
{
    public class Context
    {

    }
    public interface IContextService
    {
        Task<Context> GetContext(string contextName);
    }
    public class ContextService:IContextService
    {
        public Task<Context> GetContext(string contextName)
        {
            return Task.FromResult(new Context());
        }
    }
}
