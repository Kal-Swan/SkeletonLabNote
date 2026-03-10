using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SkeletonLabNote.Models;

public class Note
{
    public string Id { get; set; } = Guid.NewGuid() + "-note";
    public string Title { get; set; } = "New Note";
    public string Content { get; set; } = "";
    public DateTime Created { get; set; } = DateTime.UtcNow;
}