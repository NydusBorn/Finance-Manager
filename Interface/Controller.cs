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
        return _connection.Execute_Query("SELECT Pk_User AS \"ID пользователя\", \"User Name\" AS \"Имя пользователя\" FROM Users");
    }

    public DataTable GetTransactions() {
        return _connection.Execute_Query("SELECT " +
            "\"Pk_Transaction\" AS \"ID транзакции\", " +
            "\"User Name\" AS \"Имя пользователя\", " +
            "\"Transaction Description\" AS \"Описание транзакции\", " +
            "\"Transaction Change\" AS \"Изменение транзакции\", " +
            "DATETIME(\"Transaction Date\") AS \"Дата транзакции\" " +
            "FROM Transactions " +
            "JOIN Users on Fk_User = Pk_User");
    }

    /// <summary>
    /// Производит закрытие соединение
    /// </summary>
    public void Cause_Close()
    {
        _connection.Close();
    }
}