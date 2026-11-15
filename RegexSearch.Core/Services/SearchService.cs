using System.Text.RegularExpressions;
using RegexSearch.Core.Models;

namespace RegexSearch.Core.Services;

/// <summary>
/// Два независимых способа поиска самостоятельной последовательности
/// ровно из 16 цифр: библиотечный Regex и специализированный конечный автомат.
/// </summary>
public sealed class SearchService
{
    // Соседние буквы, цифры и '_' запрещают считать 16 цифр отдельным номером.
    private const string Pattern =
        @"(?<![A-Za-zА-Яа-яЁё0-9_])[0-9]{16}(?![A-Za-zА-Яа-яЁё0-9_])";

    private static readonly Regex NumberRegex = new(
        Pattern,
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public string RegularExpression => Pattern;

    public List<MatchResult> FindByRegex(string text)
    {
        var results = new List<MatchResult>();
        int number = 1;

        foreach (Match match in NumberRegex.Matches(text ?? string.Empty))
        {
            results.Add(new MatchResult
            {
                Number = number++,
                Value = match.Value,
                Position = match.Index,
                Length = match.Length
            });
        }

        return results;
    }

    public List<MatchResult> FindByAutomaton(string text)
    {
        text ??= string.Empty;
        var results = new List<MatchResult>();
        int number = 1;

        for (int start = 0; start < text.Length; start++)
        {
            if (!IsLeftBoundary(text, start))
                continue;

            State state = State.Start;
            int cursor = start;

            while (cursor < text.Length && state != State.Accept &&
                   state != State.Error)
            {
                state = Transition(state, text[cursor]);
                if (state != State.Error)
                    cursor++;
            }

            if (state == State.Accept && IsRightBoundary(text, cursor))
            {
                results.Add(new MatchResult
                {
                    Number = number++,
                    Value = text.Substring(start, 16),
                    Position = start,
                    Length = 16
                });
                start = cursor - 1;
            }
        }

        return results;
    }

    private static State Transition(State state, char symbol)
    {
        bool isDigit = symbol >= '0' && symbol <= '9';

        return state switch
        {
            State.Start  => isDigit ? State.Digit1 : State.Error,
            State.Digit1 => isDigit ? State.Digit2 : State.Error,
            State.Digit2 => isDigit ? State.Digit3 : State.Error,
            State.Digit3 => isDigit ? State.Digit4 : State.Error,
            State.Digit4 => isDigit ? State.Digit5 : State.Error,
            State.Digit5 => isDigit ? State.Digit6 : State.Error,
            State.Digit6 => isDigit ? State.Digit7 : State.Error,
            State.Digit7 => isDigit ? State.Digit8 : State.Error,
            State.Digit8 => isDigit ? State.Digit9 : State.Error,
            State.Digit9 => isDigit ? State.Digit10 : State.Error,
            State.Digit10 => isDigit ? State.Digit11 : State.Error,
            State.Digit11 => isDigit ? State.Digit12 : State.Error,
            State.Digit12 => isDigit ? State.Digit13 : State.Error,
            State.Digit13 => isDigit ? State.Digit14 : State.Error,
            State.Digit14 => isDigit ? State.Digit15 : State.Error,
            State.Digit15 => isDigit ? State.Digit16 : State.Error,
            State.Digit16 => State.Accept,
            _ => State.Error
        };
    }

    private static bool IsLeftBoundary(string text, int index) =>
        index == 0 || !IsIdentifierChar(text[index - 1]);

    private static bool IsRightBoundary(string text, int index) =>
        index >= text.Length || !IsIdentifierChar(text[index]);

    private static bool IsIdentifierChar(char c) =>
        (c >= 'A' && c <= 'Z') ||
        (c >= 'a' && c <= 'z') ||
        (c >= 'А' && c <= 'Я') ||
        (c >= 'а' && c <= 'я') ||
        c == 'Ё' || c == 'ё' ||
        (c >= '0' && c <= '9') || c == '_';
}
