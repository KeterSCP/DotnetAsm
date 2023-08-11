using System.Diagnostics;

using Serilog;

namespace DotnetAsm.Core.Tools;

internal static class Measure
{
    internal static async Task AsyncAction(Func<Task> action, string actionName)
    {
        var start = Stopwatch.GetTimestamp();

        await action();

        var end = Stopwatch.GetTimestamp();
        Log.Information("{ActionName} took {Elapsed}", actionName, new TimeSpan(end - start));
    }
}
