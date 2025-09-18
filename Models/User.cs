public class User
{
    public int Id { get; set; }
    public string UserName { get; set; }      // insecure: no constraints
    public string Password { get; set; }      // INSECURE: plaintext password
}
