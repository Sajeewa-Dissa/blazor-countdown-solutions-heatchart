namespace BlazrCountdownApp.Data;

public interface ICountdownService
{
    Task<CountdownResultsModel> ExecuteSolveAllAsync(int[] numbers);
    Task<IEnumerable<string>> ExecuteSolveTargetAsync(int[] numbers, int target);
    Task<IEnumerable<string>> ExecuteSolveTargetTake50Async(int[] numbers, int target);
    IEnumerable<int> GetSixRandomTiles();
}

public class CountdownService : ICountdownService
{
    private static Random rnd = new Random();

    public IEnumerable<int> GetSixRandomTiles() => rnd.GetSixRandomTiles();


    public async Task<CountdownResultsModel> ExecuteSolveAllAsync(int[] numbers)
    {
        decimal countNonZeroResults = 0;
        AllResults results = await Task.Run(() => SolveAll(numbers));

        List<CountdownResultsModel.TileResult> tileResults = new();

        for (int i = 100; i < 1000; i++)
        {
            var tileResult = new CountdownResultsModel.TileResult()
            {
                Target = i,
                NumberOfSolutions = results.solutionCountDictionary[i],
                FirstSolution = results.firstSolutionDictionary[i]
            };
            tileResults.Add(tileResult);
            if (tileResult.NumberOfSolutions != 0) countNonZeroResults++;
        }

        return new CountdownResultsModel()
        {
            Coverage = decimal.Round(countNonZeroResults/900*100m, 1, MidpointRounding.AwayFromZero),
            TileResults = tileResults
        };
    }



    public async Task<IEnumerable<string>> ExecuteSolveTargetTake50Async(int[] numbers, int target)
    {
        return (await ExecuteSolveTargetAsync(numbers, target)).Take(50);
    }

    public async Task<IEnumerable<string>> ExecuteSolveTargetAsync(int[] numbers, int target)
    {
        // the data could change before the task is run
        TargetResults results = await Task.Run(() => SolveTarget(numbers, target));
        List<string> resultsList = new();

        if (results.targetSolutionsBag.Count == 0)
        {
            resultsList.Add("There are no solutions.");
        }
        else
        {
            foreach(string item in results.targetSolutionsBag)
            {
                resultsList.Add(item);
            }
            // guarantee ordering independent of parallel partition order
            //results.TargetSolutions.Sort((a, b) =>
            //{
            //    int lengthCompare = a.Length - b.Length;
            //    return lengthCompare == 0 ? string.Compare(b, a, StringComparison.CurrentCulture) : lengthCompare;
            //});
        }

        return resultsList;
    }


    private static AllResults SolveAll(int[] tiles)
    {
        AllResults results = new AllResults();

        ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = 8 };

        for (int k = 2; k <= tiles.Length; k++)
        {
            Combinations<int> combinations = new Combinations<int>(tiles, k);

            foreach (int[] combination in combinations)
            {
                Permutations<int> permutations = new Permutations<int>(combination);

                //Parallel.ForEach loop with partition-local variables
                Parallel.ForEach(permutations,           // source collection
                    parallelOptions,                     //set parallel options
                    () => new SolvingEngine(null),          // the partitions local solver method to initialize the local variable
                    (permutation, loopState, solvingEngine) => //the parallel action method invoked by the loop on each iteration
                    {
                        solvingEngine.SolveAll(permutation);  //modify local variable
                        return solvingEngine;              // value to be passed to next iteration
                    },
                    // Method to be executed when each partition has completed.
                    // solvingEngine is the final value of subtotal for a particular partition.
                    (solvingEngine) => results.AggregateData(solvingEngine));
            }
        }
        return results;
    }


    private static TargetResults SolveTarget(int[] tiles, int target)
    {
        TargetResults results = new TargetResults();

        ParallelOptions parallelOptions = new() { MaxDegreeOfParallelism = 8 };

        for (int k = 2; k <= tiles.Length; k++)
        {
            Combinations<int> combinations = new Combinations<int>(tiles, k);

            foreach (int[] combination in combinations)
            {
                Permutations<int> permutations = new Permutations<int>(combination);

                //Parallel.ForEach loop with partition-local variables
                Parallel.ForEach(permutations,              // source collection
                    parallelOptions,                         // the partitions local solver
                    () => new SolvingEngine(target),    //the partitions local solver method to initialize the local variable
                    (permutation, loopState, solvingEngine) => //the parallel action method invoked by the loop on each iteration
                    {
                        solvingEngine.SolveTarget(permutation);  //modify local variable
                        return solvingEngine;              // value to be passed to next iteration
                    },
                    // Method to be executed when each partition has completed.
                    // solvingEngine is the final value of subtotal for a particular partition.
                    (solvingEngine) => results.AggregateData(solvingEngine));
            }
        }
        return results;
    }

}