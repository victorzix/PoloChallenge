using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PoloChallenge.Models;
using PoloChallenge.Services;

namespace PoloChallenge.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IApiService _expectativaService;
    private int _currentPage;
    private const int PageSize = 10;
    public ObservableCollection<ExpectativaMercadoMensal> Expectativas { get; }

    public MainViewModel(IApiService expectativaService)
    {
        _expectativaService = expectativaService;
        Expectativas = new ObservableCollection<ExpectativaMercadoMensal>();
        _currentPage = 1;
    }

    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            _currentPage = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadExpectativasAsync(int skip = 0)
    {
        var expectativas = await _expectativaService.GetExpectativasAsync(skip.ToString());
        Expectativas.Clear();
        foreach (var expectativa in expectativas)
        {
            Expectativas.Add(expectativa);
        }
    }

    public async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadExpectativasAsync((CurrentPage - 1) * PageSize);
    }

    public async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadExpectativasAsync((CurrentPage - 1) * PageSize);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}