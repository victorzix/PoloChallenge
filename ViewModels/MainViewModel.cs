﻿using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using PoloChallenge.Models;
using PoloChallenge.Services;

namespace PoloChallenge.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly IApiService _expectativaService;
    private int _currentPage;
    private const int PageSize = 100;
    public string _filter = "";
    public string _startDate = "";
    public string _endDate = "";
    private bool _isBusy;
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

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }
    
    private bool _hasData;

    public bool HasData
    {
        get => _hasData;
        set
        {
            _hasData = value;
            OnPropertyChanged();
        }
    }

    public async Task LoadExpectativasAsync(string filter = "", string startDate = null, string endDate = null, int skip = 0)
    {
        try
        {
            IsBusy = true;
            var expectativas = await _expectativaService.GetExpectativasAsync(filter, startDate, endDate, skip.ToString());
            Expectativas.Clear();
            foreach (var expectativa in expectativas)
            {
                Expectativas.Add(expectativa);
            }
            HasData = Expectativas.Any();
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void ResetPage()
    {
        CurrentPage = 1;
    }

    public async Task NextPageAsync()
    {
        CurrentPage++;
        await LoadExpectativasAsync(_filter, _startDate, _endDate, (CurrentPage - 1) * PageSize);
    }

    public async Task PreviousPageAsync()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadExpectativasAsync(_filter, _startDate, _endDate, (CurrentPage - 1) * PageSize);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}