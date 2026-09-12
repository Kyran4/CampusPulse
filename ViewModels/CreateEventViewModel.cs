using CampusPulse.Helpers;
using CampusPulse.Models;
using CampusPulse.Services;
using MvvmHelpers;
using System.Windows.Input;

namespace CampusPulse.ViewModels;

// Admin-only screen (the API rejects this from a non-Admin regardless, but
// the route is also only reachable from the Admin-visible EventsPage button).
public class CreateEventViewModel : BaseViewModel
{
    private readonly EventService _events;
    private readonly CategoryService _categories;
    private readonly NavigationService _nav;
    private readonly DialogService _dialog;

    public CreateEventViewModel(EventService events, CategoryService categories, NavigationService nav, DialogService dialog)
    {
        _events = events;
        _categories = categories;
        _nav = nav;
        _dialog = dialog;

        CreateCommand = new Command(async () => await CreateAsync());
        PickImageCommand = new Command(async () => await PickImageAsync());
        RemoveImageCommand = new Command(() =>
        {
            ImageBase64 = string.Empty;
            OnPropertyChanged(nameof(ImageBase64));
            OnPropertyChanged(nameof(PickImageButtonText));
        });
        Date = DateTime.Today.AddDays(7);
        Time = new TimeSpan(18, 0, 0);
    }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string ImageBase64 { get; set; } = string.Empty;

    public string PickImageButtonText => string.IsNullOrEmpty(ImageBase64) ? "Add Image" : "Change Image";

    public DateTime Date { get; set; }
    public TimeSpan Time { get; set; }

    // Leave blank for unlimited capacity.
    public string CapacityText { get; set; } = string.Empty;

    private List<Category> _categoriesList = new();
    public List<Category> Categories
    {
        get => _categoriesList;
        set => SetProperty(ref _categoriesList, value);
    }

    public Category SelectedCategory { get; set; }

    public ICommand CreateCommand { get; }
    public ICommand PickImageCommand { get; }
    public ICommand RemoveImageCommand { get; }

    private async Task PickImageAsync()
    {
        var base64 = await ImagePickerHelper.PickImageAsBase64Async();
        if (base64 == null) return;

        ImageBase64 = base64;
        OnPropertyChanged(nameof(ImageBase64));
        OnPropertyChanged(nameof(PickImageButtonText));
    }

    public async Task LoadCategoriesAsync()
    {
        var list = await _categories.GetCategoriesAsync();
        Categories = list ?? new List<Category>();
    }

    private async Task CreateAsync()
    {
        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(Location) || SelectedCategory == null)
        {
            await _dialog.ShowAlert("Error", "Title, description, location and category are all required.");
            return;
        }

        var eventDateTime = Date.Date + Time;
        if (eventDateTime <= DateTime.Now)
        {
            await _dialog.ShowAlert("Error", "Event date/time must be in the future.");
            return;
        }

        int? capacity = null;
        if (!string.IsNullOrWhiteSpace(CapacityText))
        {
            if (!int.TryParse(CapacityText, out var parsed) || parsed <= 0)
            {
                await _dialog.ShowAlert("Error", "Capacity must be a positive whole number, or left blank for unlimited.");
                return;
            }
            capacity = parsed;
        }

        var dto = new EventCreateDto
        {
            Title = Title,
            Description = Description,
            Location = Location,
            ImageBase64 = ImageBase64,
            Date = eventDateTime.ToUniversalTime(),
            CategoryId = SelectedCategory.CategoryId,
            Capacity = capacity
        };

        var result = await _events.CreateEventAsync(dto);

        if (result == null)
        {
            await _dialog.ShowAlert("Error", "Failed to create event.");
            return;
        }

        await _dialog.ShowAlert("Success", "Event created!");
        await _nav.GoBackAsync();
    }
}
