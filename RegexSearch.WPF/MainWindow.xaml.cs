using System.Windows;
using RegexSearch.Core.Models;
using RegexSearch.Core.Services;

namespace RegexSearch.WPF;

public partial class MainWindow : Window
{
    private readonly SearchService _service = new();

    public MainWindow()
    {
        InitializeComponent();
        PatternText.Text = _service.RegularExpression;
        InputText.Text =
            "Примеры корректных номеров:\n" +
            "1234567890123456, 9876543210987654; 0000111122223333.\n" +
            "Полис: 4567890123456789 и 1111222233334444.\n\n" +
            "Некорректные значения:\n" +
            "123456789012345, 12345678901234567, A1234567890123456B.";
    }

    private void RegexButton_Click(object sender, RoutedEventArgs e) =>
        ShowResults(_service.FindByRegex(InputText.Text), "Regex");

    private void AutomatonButton_Click(object sender, RoutedEventArgs e) =>
        ShowResults(_service.FindByAutomaton(InputText.Text), "Конечный автомат");

    private void CompareButton_Click(object sender, RoutedEventArgs e)
    {
        var regex = _service.FindByRegex(InputText.Text);
        var automaton = _service.FindByAutomaton(InputText.Text);
        bool equal = regex.Select(x => (x.Value, x.Position, x.Length))
            .SequenceEqual(automaton.Select(x => (x.Value, x.Position, x.Length)));

        ShowResults(regex, "Сравнение");
        StatusText.Text = equal ? "Результаты совпадают" : "Обнаружено расхождение";
        StatusText.Foreground = equal
            ? System.Windows.Media.Brushes.ForestGreen
            : System.Windows.Media.Brushes.Firebrick;
        SummaryText.Text = $"Regex: {regex.Count}; автомат: {automaton.Count}. " +
                           (equal ? "Алгоритмы выдали одинаковые совпадения." :
                                    "Проверьте реализацию и входные данные.");
    }

    private void ShowResults(List<MatchResult> results, string mode)
    {
        ResultsGrid.ItemsSource = results;
        SummaryText.Text = $"{mode}: найдено совпадений — {results.Count}. " +
                           "Позиции отсчитываются с нуля.";
        StatusText.Text = "Поиск завершён";
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        InputText.Clear();
        ResultsGrid.ItemsSource = null;
        SummaryText.Text = "Результаты поиска появятся здесь.";
        StatusText.Text = "Очищено";
    }
}
