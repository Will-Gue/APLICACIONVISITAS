using Visitapp.Application.Commands;

namespace Visitapp.Application.Common
{
    public interface IMediator
    {
        Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
        Task SendAsync(ICommand command, CancellationToken cancellationToken = default);
    }
}