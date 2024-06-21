namespace Polyglot.Models;

public class MasterModel
{
    public bool Archived { get; set; }
    public DateTime DatePublished { get; set; }
    public DateTime DateSaved { get; set; }
    public bool Favorite { get; set; }
    public string Notes { get; set; } = String.Empty;
    public List<string> Tags { get; set; } = [];
    public string Title { get; set; } = String.Empty;
    public string Url { get; set; } = String.Empty;
}
