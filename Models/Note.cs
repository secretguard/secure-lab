public class Note
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Content { get; set; } // INSECURE: stored plaintext
}
