using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; 
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using SIGNAL_DESK.Models;

namespace SIGNAL_DESK.ViewModels
{
    public class MainViewModel : BindableObject
    {
        private readonly string _jsonFilePath = Path.Combine(FileSystem.AppDataDirectory, "tickets_data.json");

        public ObservableCollection<Ticket> AllTickets { get; set; } = new();
        public ObservableCollection<Ticket> FilteredTickets { get; set; } = new();
        public List<string> PriorityOptions { get; } = new() { "P1", "P2", "P3", "P4" };
        public List<string> StatusFilterOptions { get; } = new() { "All", "New", "InProgress", "Resolved" };

        private string _newClient = string.Empty;
        public string NewClient
        {
            get => _newClient;
            set { _newClient = value; OnPropertyChanged(); }
        }

        private string _newSummary = string.Empty;
        public string NewSummary
        {
            get => _newSummary;
            set { _newSummary = value; OnPropertyChanged(); }
        }

        private string _selectedPriorityString = "P3";
        public string SelectedPriorityString
        {
            get => _selectedPriorityString;
            set { _selectedPriorityString = value; OnPropertyChanged(); }
        }

        private Ticket? _selectedTicket;
        public Ticket? SelectedTicket
        {
            get => _selectedTicket;
            set { _selectedTicket = value; OnPropertyChanged(); }
        }

        private string _resolutionInput = string.Empty;
        public string ResolutionInput
        {
            get => _resolutionInput;
            set { _resolutionInput = value; OnPropertyChanged(); }
        }

        private string _selectedStatusFilter = "All";
        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set
            {
                _selectedStatusFilter = value;
                OnPropertyChanged();
                ApplyFilters();
            }
        }

        public int CountP1 => AllTickets.Count(t => t.Priority == TicketPriority.P1 && t.Status != TicketStatus.Resolved);
        public int CountP2 => AllTickets.Count(t => t.Priority == TicketPriority.P2 && t.Status != TicketStatus.Resolved);
        public int CountP3 => AllTickets.Count(t => t.Priority == TicketPriority.P3 && t.Status != TicketStatus.Resolved);
        public int CountP4 => AllTickets.Count(t => t.Priority == TicketPriority.P4 && t.Status != TicketStatus.Resolved);

        public ICommand CreateTicketCommand { get; }
        public ICommand StartTicketCommand { get; }
        public ICommand ResolveTicketCommand { get; }
        public ICommand SaveToJsonCommand { get; }

        public MainViewModel()
        {
            CreateTicketCommand = new Command(CreateTicket);
            StartTicketCommand = new Command(StartTicket);
            ResolveTicketCommand = new Command(ResolveTicket);
            SaveToJsonCommand = new Command(SaveTicketsToJson);

            LoadTicketsFromJson();
            ApplyFilters();
        }

        private void CreateTicket()
        {
            if (string.IsNullOrWhiteSpace(NewClient) || string.IsNullOrWhiteSpace(NewSummary)) return;

            Enum.TryParse<TicketPriority>(SelectedPriorityString, out var priority);

            int hoursToDue = priority switch
            {
                TicketPriority.P1 => 1,
                TicketPriority.P2 => 4,
                TicketPriority.P3 => 8,
                TicketPriority.P4 => 24,
                _ => 8
            };

            var ticket = new Ticket
            {
                Id = $"TCK-{DateTime.Now:HHmmss}",
                Client = NewClient,
                Summary = NewSummary,
                Priority = priority,
                Status = TicketStatus.New,
                CreatedAt = DateTime.Now,
                ResponseDueAt = DateTime.Now.AddHours(hoursToDue)
            };

            AllTickets.Add(ticket);
            NewClient = string.Empty;
            NewSummary = string.Empty;

            UpdateCountersAndFilters();
            SaveTicketsToJson();
        }

        private void StartTicket()
        {
            if (SelectedTicket != null && SelectedTicket.Status == TicketStatus.New)
            {
                SelectedTicket.Status = TicketStatus.InProgress;
                RefreshSelectedTicket();
                UpdateCountersAndFilters();
                SaveTicketsToJson();
            }
        }

        private void ResolveTicket()
        {
            if (SelectedTicket != null && SelectedTicket.Status != TicketStatus.Resolved)
            {
                SelectedTicket.Status = TicketStatus.Resolved;
                SelectedTicket.ResolvedAt = DateTime.Now;
                SelectedTicket.ResolutionNotes = $"[{DateTime.Now:yyyy-MM-dd HH:mm}] {ResolutionInput}";
                ResolutionInput = string.Empty;

                RefreshSelectedTicket();
                UpdateCountersAndFilters();
                SaveTicketsToJson();
            }
        }

        private void ApplyFilters()
        {
            FilteredTickets.Clear();
            var query = AllTickets.AsEnumerable();

            if (SelectedStatusFilter != "All" && Enum.TryParse<TicketStatus>(SelectedStatusFilter, out var status))
            {
                query = query.Where(t => t.Status == status);
            }

            foreach (var ticket in query.OrderByDescending(t => t.CreatedAt))
            {
                FilteredTickets.Add(ticket);
            }
        }

        private void UpdateCountersAndFilters()
        {
            OnPropertyChanged(nameof(CountP1));
            OnPropertyChanged(nameof(CountP2));
            OnPropertyChanged(nameof(CountP3));
            OnPropertyChanged(nameof(CountP4));
            ApplyFilters();
        }

        private void RefreshSelectedTicket()
        {
            var temp = SelectedTicket;
            SelectedTicket = null;
            SelectedTicket = temp;
        }

        public void SaveTicketsToJson()
        {
            try
            {
                var json = JsonSerializer.Serialize(AllTickets, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_jsonFilePath, json);
            }
            catch { }
        }

        private void LoadTicketsFromJson()
        {
            if (File.Exists(_jsonFilePath))
            {
                try
                {
                    var json = File.ReadAllText(_jsonFilePath);
                    var items = JsonSerializer.Deserialize<ObservableCollection<Ticket>>(json);
                    if (items != null) AllTickets = items;
                }
                catch { }
            }
        }
    }
}