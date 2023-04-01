using System;
using System.Data;
using Connection_Manager;

namespace Finance_Manager;

/// <summary>
///     Класс действий производимых над базой данных
/// </summary>
public class Controller
{
    private readonly Connector _connection;
    private string _path;
    public Connector.Database_State State;

    public DataTable Users;
    public DataTable Transactions;
    
    
    public Controller(string path)
    {
        _connection = new Connector(path);
        _path = path;
        State = _connection.State;
    }


    public void GetUsers()
    {
        Users = _connection.Execute_Query(
            "SELECT Pk_User AS \"ID пользователя\", \"User Name\" AS \"Имя пользователя\" FROM Users");
    }

    public void GetTransactions()
    {
        Transactions = _connection.Execute_Query("SELECT " +
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
        var time = Time(dt);
        var time2 = Time(dt2);
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

    public DataTable GetUser(int n)
    {
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
        var time = Time(dt);
        var time2 = Time(dt2);
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
        var dt2 = new DateTime(1970, 1, 1, 0, 0, 0);
        var time = (dt.Ticks - dt2.Ticks) / TimeSpan.TicksPerSecond;
        return time;
    }

    public void Add_User(string UserName)
    {
        int id;
        if (Users.Rows.Count != 0)
        {
            id = int.Parse((string)Users.Rows[^1][0]) + 1;
        }
        else
        {
            id = 0;
        }
        
        _connection.Execute_Action($"Insert Into 'Users' Values ({id}, '{UserName}')");
    }

    public void Add_Transaction(int user, string description, int change, long date)
    {
        int id;
        if (Transactions.Rows.Count != 0)
        {
            id = int.Parse((string)Transactions.Rows[^1][0]) + 1;
        }
        else
        {
            id = 0;
        }
        
        _connection.Execute_Action($"Insert Into 'Transactions' Values ({id}, '{user}', '{description}', '{change}', '{date}')");
    }

    /// <summary>
    ///     Производит закрытие соединение
    /// </summary>
    public void Cause_Close()
    {
        _connection.Close();
    }

    /// <summary>
    ///     Производит пересоздание базы данных
    /// </summary>
    public void Cause_Creation()
    {
        _connection.Create_Database();
    }
}