using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeletonLabNote.Models;
using SkeletonLabNote.Storage;

namespace SkeletonLabNote.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly FileNoteStorage _storage = new();

    public ObservableCollection<Note> Notes { get; } = new();
    public ObservableCollection<Notebook> Notebooks { get; } = new();
    
    [ObservableProperty]
    private Note? _selectedNote;
    
    [ObservableProperty]
    private Notebook? _selectedNotebook;
    
    public MainViewModel()
    {
        _ = LoadNotebooksAsync();
    }

    private async Task LoadNotebooksAsync()
    {
        var notebooks = await _storage.GetNoteBooksAsync();

        if (!notebooks.Any())
        {
            await _storage.CreateNotebookAsync(new Notebook
            {
                Name = "Default Notebook"
            });
            notebooks = await _storage.GetNoteBooksAsync();
        }
        
        foreach (var notebook in notebooks)
        {
            Notebooks.Add(notebook);
        }
        
        SelectedNotebook = Notebooks.FirstOrDefault();
    }
    
    partial void OnSelectedNotebookChanged(Notebook? value)
    {
        LoadNotes();
    }

    async void LoadNotes()
    {
        Notes.Clear();

        if (SelectedNotebook == null)
        {
            return;
        }

        var notes = await _storage.LoadNotesAsync(SelectedNotebook.Id);
        
        foreach (var note in notes)
        {
            Notes.Add(note);
        }
        
        SelectedNote = Notes.FirstOrDefault();
    }

    [RelayCommand]
    async Task NewNote()
    {
        if (SelectedNotebook is null)
        {
            return;
        }
        
        var note = new Note();
        Notes.Add(note);
        // SelectedNote = note;
        await _storage.SaveAsync(note, SelectedNotebook.Id);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedNote is not null)
        {
            await _storage.SaveAsync(SelectedNote, SelectedNotebook!.Id);
        }
        
        if (SelectedNotebook is not null)
        {
            await _storage.SaveNoteBookAsync(SelectedNotebook);
        }
    }
    
    [RelayCommand]
    public async Task NewNotebook()
    {
        var notebook = new Notebook();
        Notebooks.Add(notebook);
        SelectedNotebook = notebook;
        await _storage.CreateNotebookAsync(notebook);
    }
}