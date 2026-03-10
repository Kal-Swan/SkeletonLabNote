using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SkeletonLabNote.Models;

namespace SkeletonLabNote.Storage;

public class FileNoteStorage
{
    private readonly string _root;

    public FileNoteStorage()
    {
        var baseFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "SkeletonLabNotes");
        
        _root = Path.Combine(baseFolder, "notebooks");

        Directory.CreateDirectory(_root);
    }

    public async Task SaveAsync(Note note, string notebookId)
    {
        var notebookPath = Path.Combine(_root, notebookId);
        Directory.CreateDirectory(notebookPath);
        
        var path = Path.Combine(notebookPath, $"{note.Id}.json");
        var json = JsonSerializer.Serialize(note, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        await File.WriteAllTextAsync(path, json);
    }
    
    public async Task SaveNoteBookAsync(Notebook notebook)
    {
        var path = Path.Combine(_root, notebook.Id);
        var metadataFile = Directory.GetFiles(path, "*-notebook.json").FirstOrDefault();

        if (metadataFile is null)
        {
            throw new FileNotFoundException("Notebook metadata not found.");
        }
        
        var currentJson = await File.ReadAllTextAsync(metadataFile);
        var notebookToUpdate = JsonSerializer.Deserialize<Notebook>(currentJson);

        notebookToUpdate.Name = notebook.Name;
        
        var updatedJson = JsonSerializer.Serialize(notebookToUpdate, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        await File.WriteAllTextAsync($"{path}/{notebookToUpdate.Id}.json",updatedJson);
    }

    public async Task<List<Note>> LoadNotesAsync(string notebookId)
    {
        var notes = new List<Note>();

        var notebookPath = Path.Combine(_root, notebookId);

        if (!Directory.Exists(notebookPath))
        {
            return notes;
        }

        foreach (var file in Directory.GetFiles(notebookPath, "*-note.json"))
        {
            var json = await File.ReadAllTextAsync(file);
            var note = JsonSerializer.Deserialize<Note>(json);

            if (note != null)
            {
                notes.Add(note);
            }
        }

        return notes.OrderBy(note => note.Created).ToList();
    }

    public async Task<List<Notebook>> GetNoteBooksAsync()
    {
        var notebooks = new List<Notebook>();
        var directories = Directory.GetDirectories(_root);

        foreach (var directory in directories)
        {
            foreach (var file in Directory.GetFiles(directory, "*-notebook.json"))
            {
                var json = await File.ReadAllTextAsync(file);
                var notebook = JsonSerializer.Deserialize<Notebook>(json);
                notebooks.Add(notebook);
            }
        }

        return notebooks;
    }

    public async Task CreateNotebookAsync(Notebook noteBook)
    {
        var path = Path.Combine(_root, noteBook.Id);
        
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        var json = JsonSerializer.Serialize(noteBook, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        await File.WriteAllTextAsync($"{path}/{noteBook.Id}.json",json);
    }
}