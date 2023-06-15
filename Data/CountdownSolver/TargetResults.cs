using System.Collections.Concurrent;

namespace BlazrCountdownApp.Data;

internal sealed class TargetResults
{
    /// <summary>
    /// thread synchronization objects
    /// </summary>
    private readonly object bagLock = new object();

    //Concurrent bag is used to aggregate the results
    public ConcurrentBag<string> targetSolutionsBag = new();

    /// <summary>
    /// aggregates the data from solving engines 
    /// that were run in parallel partitions
    /// </summary>
    /// <param name="solvingEngine"></param>
    public void AggregateData(SolvingEngine solvingEngine)
    {
        if ((solvingEngine is null) || (solvingEngine.TargetSolutions is null))
            throw new ArgumentNullException(nameof(solvingEngine));

        if (solvingEngine.TargetSolutions.Count > 0)
        {
            lock (bagLock) //lock the thread while doing this. The input param is simply an object reference.
            {
                foreach (string item in solvingEngine.TargetSolutions)
                {
                    targetSolutionsBag.Add(item);
                }
            }
        }
    }

}

