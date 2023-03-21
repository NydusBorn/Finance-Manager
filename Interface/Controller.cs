using Connection_Manager;
using System.Data;

namespace Interface;

/// <summary>
/// Класс действий производимых над базой данных
/// </summary>
public class Controller
{
    private Connector _connection;
    private string _path; 
    public Controller(string path)
    {
        _connection = new Connector(path);
        _path = path;
    }

    public DataTable GetUsers() {
        return _connection.Execute_Query("SELECT * FROM Users", new string[]{"id", "name"});
    }

    /// <summary>
    /// Производит закрытие соединение
    /// </summary>
    public void Cause_Close()
    {
        _connection.Close();
    }
}