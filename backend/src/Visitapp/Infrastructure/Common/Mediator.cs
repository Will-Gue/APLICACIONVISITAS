using Visitapp.Application.Commands;
using Visitapp.Application.Common;

namespace Visitapp.Infrastructure.Common
{
    public class Mediator : IMediator
    {
        private readonly IServiceProvider _serviceProvider;

        public Mediator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(command.GetType(), typeof(TResult));
            var handler = _serviceProvider.GetRequiredService(handlerType);
            
            var method = handlerType.GetMethod("HandleAsync");
            if (method == null)
                throw new InvalidOperationException($"Handler for {command.GetType().Name} not found");

            var result = await (Task<TResult>)method.Invoke(handler, new object[] { command, cancellationToken })!;
            return result;
        }

        public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            await SendAsync<Unit>(command, cancellationToken);
        }
    }
}