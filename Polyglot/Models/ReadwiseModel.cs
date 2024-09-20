namespace Polyglot.Models;

public class ReadwiseModel
{
    public string URL { get; set; } = String.Empty;
    public string Title { get; set; } = String.Empty;
    public string Selection { get; set; } = String.Empty;
    public string Folder { get; set; } = "Unread";
    public string Timestamp { get; set; } = String.Empty;
}
