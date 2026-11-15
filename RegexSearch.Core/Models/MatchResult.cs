namespace RegexSearch.Core.Models;

public sealed class MatchResult
{
    public int Number { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Position { get; set; } // Отсчёт от нуля
    public int Length { get; set; }
}
