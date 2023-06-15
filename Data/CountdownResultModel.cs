namespace BlazrCountdownApp.Data;

public class CountdownResultsModel
{
    public decimal Coverage { get; init; }

    public required IEnumerable<TileResult> TileResults { get; init; }

    public class TileResult
    {
        public int Target { get; set; }
        public int NumberOfSolutions { get; set; }
        public string? FirstSolution { get; set; }
    }
}