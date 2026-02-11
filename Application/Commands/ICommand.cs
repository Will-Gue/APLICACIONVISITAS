namespace Visitapp.Application.Commands
{
    public interface ICommand<TResult>
    {
    }

    public interface ICommand : ICommand<Unit>
    {
    }

    public struct Unit
    {
        public static readonly Unit Value = new();
    }
}