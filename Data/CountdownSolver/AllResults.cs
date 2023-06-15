using System.Collections.Concurrent;

namespace BlazrCountdownApp.Data;

internal sealed class AllResults
{
    /// <summary>
    /// thread synchronization objects
    /// </summary>
    private readonly object dictLock = new object();

    // collects a reference of each solver's solution list
    private List<List<(int calcResult, string calcSummary)>> SolverLists { get; } = new List<List<(int, string)>>(100);

    // used to aggregate all the solver list contents into a single list
    public List<(int calcResults, string calcSummaries)> Solutions { get; private set; } = new List<(int, string)>();

    //Use separate concurrent dictionaries as the running total count is updated far more often than the first solution.
    public ConcurrentDictionary<int, int> solutionCountDictionary = new();
    public ConcurrentDictionary<int, string?> firstSolutionDictionary = new();


    public AllResults() //initialise concurrent dictionaries.
    {
        for (int i = 100; i < 1000; i++)
        {
            solutionCountDictionary.TryAdd(i, 0);
            firstSolutionDictionary.TryAdd(i, null);
        }
    }

    /// <summary>
    /// aggregates the data from solving engines 
    /// that were run in parallel partitions
    /// </summary>
    /// <param name="solvingEngine"></param>
    public void AggregateData(SolvingEngine solvingEngine)
    {
        if ((solvingEngine is null) || (solvingEngine.AllSolutions is null))
            throw new ArgumentNullException(nameof(solvingEngine));

        lock (dictLock) //lock the thread while doing this. The input param is simply an object reference.
        {
            foreach(var item in solvingEngine.AllSolutions)
            {
                solutionCountDictionary.AddOrUpdate(item.calcResult, 0, (key, oldValue) => oldValue + 1);

                if (firstSolutionDictionary.TryGetValue(item.calcResult, out string? equationString))
                {
                    if(equationString is null)
                    {
                        firstSolutionDictionary.AddOrUpdate(item.calcResult, "", (key, oldValue) => item.calcSummary);
                    }
                }
            }
        }
    }

}
