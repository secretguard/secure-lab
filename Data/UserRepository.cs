using Microsoft.Data.Sqlite;

public class UserRepository
{
    private readonly SqliteConnection _conn;
    public UserRepository(SqliteConnection conn)
    {
        _conn = conn;
        if (_conn.State != System.Data.ConnectionState.Open)
        {
            _conn.Open();
        }
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserName TEXT, Password TEXT);
                            CREATE TABLE IF NOT EXISTS Notes (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER, Content TEXT);";
        cmd.ExecuteNonQuery();
    }

    public void AddUser(string username, string password)
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Users (UserName, Password) VALUES (@userName, @password)"; // secure parameterized
        cmd.Parameters.AddWithValue("@userName", username);
        cmd.Parameters.AddWithValue("@password", password);
        cmd.ExecuteNonQuery();
    }

    public User GetByUserName(string username)
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "SELECT Id, UserName, Password FROM Users WHERE UserName = @userName LIMIT 1"; // secure parameterized
        cmd.Parameters.AddWithValue("@userName", username);
        using var r = cmd.ExecuteReader();
        if (r.Read())
        {
            return new User
            {
                Id = r.GetInt32(0),
                UserName = r.GetString(1),
                Password = r.GetString(2)
            };
        }
        return null;
    }

    public void AddNote(int userId, string content)
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "INSERT INTO Notes (UserId, Content) VALUES (@userId, @content)"; // secure parameterized
        cmd.Parameters.AddWithValue("@userId", userId);
        cmd.Parameters.AddWithValue("@content", content);
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<Note> GetNotes(int userId)
    {
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = "SELECT Id, UserId, Content FROM Notes WHERE UserId = @userId"; // secure parameterized
        cmd.Parameters.AddWithValue("@userId", userId);
        using var r = cmd.ExecuteReader();
        var list = new List<Note>();
        while (r.Read())
        {
            list.Add(new Note { Id = r.GetInt32(0), UserId = r.GetInt32(1), Content = r.GetString(2) });
        }
        return list;
    }
}
