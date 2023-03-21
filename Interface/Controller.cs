using Connection_Manager;

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

    /// <summary>
    /// Производит закрытие соединение
    /// </summary>
    public void Cause_Close()
    {
        _connection.Close();
    }
}