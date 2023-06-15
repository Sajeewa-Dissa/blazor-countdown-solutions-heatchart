namespace BlazrCountdownApp.Components;

public enum TileStatusEnum
{
    UnchosenSmall, UnchosenLarge, ChosenSmall, ChosenLarge, Chosen
}

public enum TileTypeEnum
{
    Small, Large
}

public class TileModel
{
    public int Id { get; set; }

    public required string Caption { get; init; }

    public TileStatusEnum Status { get; set; }

    public TileTypeEnum Type { get; set; }
}



