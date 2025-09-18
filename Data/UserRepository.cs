using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class UserRepository
{
    private readonly SqliteConnection _conn;
    public UserRepository(SqliteConnection conn)
    {
        _conn = conn;
        _conn.Open();
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = @"CREATE TABLE IF NOT EXISTS Users (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserName TEXT, Password TEXT);
                            CREATE TABLE IF NOT EXISTS Notes (Id INTEGER PRIMARY KEY AUTOINCREMENT, UserId INTEGER, Content TEXT);";
        cmd.ExecuteNonQuery();
    }

    public void AddUser(string username, string password)
    {
        var sql = $"INSERT INTO Users (UserName, Password) VALUES ('{username}', '{password}')"; // INSECURE
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public User GetByUserName(string username)
    {
        var sql = $"SELECT Id, UserName, Password FROM Users WHERE UserName = '{username}' LIMIT 1"; // INSECURE
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = sql;
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
        var sql = $"INSERT INTO Notes (UserId, Content) VALUES ({userId}, '{content}')"; // INSECURE
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = sql;
        cmd.ExecuteNonQuery();
    }

    public IEnumerable<Note> GetNotes(int userId)
    {
        var sql = $"SELECT Id, UserId, Content FROM Notes WHERE UserId = {userId}"; // INSECURE
        using var cmd = _conn.CreateCommand();
        cmd.CommandText = sql;
        using var r = cmd.ExecuteReader();
        var list = new List<Note>();
        while (r.Read())
        {
            list.Add(new Note { Id = r.GetInt32(0), UserId = r.GetInt32(1), Content = r.GetString(2) });
        }
        return list;
    }
}
