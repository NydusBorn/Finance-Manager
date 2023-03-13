using Connection_Manager;

namespace Interface;

/// <summary>
/// Класс действий производимых над базой данных
/// </summary>
public class Controller
{
    private Connector _connection;

    public Controller()
    {
        _connection = new Connector();
    }
}