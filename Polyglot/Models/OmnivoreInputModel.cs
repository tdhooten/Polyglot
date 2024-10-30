namespace Polyglot.Models;

public class OmnivoreInputModel
{
    public List<string> labels { get; set; } = new();
    public DateTime savedAt { get; set; }
    public string state { get; set; } = String.Empty;
    public string title { get; set; } = String.Empty;
    public string url { get; set; } = String.Empty;
}
