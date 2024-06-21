namespace Polyglot.Models;

public class GoodLinksModel
{
    public float addedAt { get; set; }
    public float readAt { get; set; }
    public bool starred { get; set; }
    public string summary { get; set; } = String.Empty;
    public string[] tags { get; set; } = [];
    public string title { get; set; } = String.Empty;
    public string url { get; set; } = String.Empty;
}
