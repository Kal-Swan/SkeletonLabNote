using System;

namespace SkeletonLabNote.Models;

public class Notebook
{
    public string Id { get; set; } = Guid.NewGuid() + "-notebook";
    public string Name { get; set; } = "New Notebook";
}