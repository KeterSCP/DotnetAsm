using Serilog;

namespace DotnetAsm.Core.Extensions;

public static class TaskExtensions
{
    private static readonly Action<Task> _continuation = result =>
    {
        if (result.Exception is not null)
        {
            Log.Error(result.Exception, "Not awaited task threw an exception");
        }
    };
    
    public static void NoAwait(this Task task)
    {
        task.ContinueWith(_continuation, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.NotOnRanToCompletion);
    }
}