using System;
using Connection_Manager;
using System.Data;
using System.Threading.Tasks;
using Wpf.Ui.Controls;

namespace Interface;

/// <summary>
/// Класс действий производимых над базой данных
/// </summary>
public class Controller
{
    private Connector _connection;
    public Connector.Database_State State;
    private string _path; 
    public Controller(string path)
    {
        _connection = new Connector(path);
        _path = path;
        State = _connection.State;
    }

    
    
    public DataTable GetUsers() {
        return _connection.Execute_Query("SELECT Pk_User AS \"ID пользователя\", \"User Name\" AS \"Имя пользователя\" FROM Users");
    }

    public DataTable GetTransactions()
    {
        return _connection.Execute_Query("SELECT " +
                                         "\"Pk_Transaction\" AS \"ID транзакции\", " +
                                         "\"User Name\" AS \"Имя пользователя\", " +
                                         "\"Transaction Description\" AS \"Описание транзакции\", " +
                                         "\"Transaction Change\" AS \"Изменение транзакции\", " +
                                         "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
                                         "FROM Transactions " +
                                         "JOIN Users on Fk_User = Pk_User ");
    }

    public DataTable GetTransactionsPerPeriod(DateTime dt, DateTime dt2)
    {
        long time = Time(dt);
        long time2 = Time(dt2);
        return _connection.Execute_Query("SELECT " +
            "\"Pk_Transaction\" AS \"ID транзакции\", " +
            "\"User Name\" AS \"Имя пользователя\", " +
            "\"Transaction Description\" AS \"Описание транзакции\", " +
            "\"Transaction Change\" AS \"Изменение транзакции\", " +
            "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
            "FROM Transactions " +
            "JOIN Users on Fk_User = Pk_User " +
            $"WHERE \"Transaction Date\" > {time} AND \"Transaction Date\" < {time2}");
    }

    public DataTable GetUser(int n) {
        return _connection.Execute_Query("SELECT " +
            "\"Pk_Transaction\" AS \"ID транзакции\", " +
            "\"User Name\" AS \"Имя пользователя\", " +
            "\"Transaction Description\" AS \"Описание транзакции\", " +
            "\"Transaction Change\" AS \"Изменение транзакции\", " +
            "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
            "FROM Transactions " +
            "JOIN Users on Fk_User = Pk_User " +
            $"WHERE Users.Pk_User = {n}");
    }

    public DataTable GetTransactionsPerPeriodAndUser(DateTime dt, DateTime dt2, int n)
    {
        long time = Time(dt);
        long time2 = Time(dt2);
        return _connection.Execute_Query("SELECT " +
            "\"Pk_Transaction\" AS \"ID транзакции\", " +
            "\"User Name\" AS \"Имя пользователя\", " +
            "\"Transaction Description\" AS \"Описание транзакции\", " +
            "\"Transaction Change\" AS \"Изменение транзакции\", " +
            "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
            "FROM Transactions " +
            "JOIN Users on Fk_User = Pk_User " +
            $"WHERE \"Transaction Date\" > {time} AND \"Transaction Date\" < {time2} AND Users.Pk_User = {n}");
    }

    public long Time(DateTime dt)
    {
        DateTime dt2 = new DateTime(1970, 1, 1, 0, 0, 0);
        long time = (dt.Ticks - dt2.Ticks)/TimeSpan.TicksPerSecond;
        return time;
    }
    /// <summary>
    /// Производит закрытие соединение
    /// </summary>
    public void Cause_Close()
    {
        _connection.Close();
    }

    /// <summary>
    /// Производит пересоздание базы данных
    /// </summary>
    public void Cause_Creation()
    {
        _connection.Create_Database();
    }
}