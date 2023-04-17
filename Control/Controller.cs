using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Connection_Manager;

namespace Finance_Manager;

/// <summary>
///     Класс действий производимых над базой данных
/// </summary>
public class Controller
{
    private readonly Connector _connection;
    private string _path;
    public DataTable Categories;
    public Connector.Database_State State;
    public DataTable Transactions;

    public DataTable Users;


    public Controller(string path)
    {
        _connection = new Connector(path);
        _path = path;
        State = _connection.State;
    }


    public void GetUsers()
    {
        Users = _connection.Execute_Query(
            "SELECT Pk_User AS \"ID пользователя\", \"User Name\" AS \"Имя пользователя\", SUM(\"Transaction Change\") As \"Текущий Баланс\" FROM Users left JOIN Transactions ON Fk_User = Pk_User GROUP BY Pk_User");
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

    public void GetCategories()
    {
        Categories = _connection.Execute_Query("SELECT * from 'Categories'");
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

    public DataTable GetTransactionPerCategory(int categoryId) {
        return _connection.Execute_Query("SELECT " +
                                        "\"Pk_Transaction\" AS \"ID транзакции\", " +
                                        "\"User Name\" AS \"Имя пользователя\", " +
                                        "\"Category Name\" AS \"Категория транзакции\", " +
                                        "\"Transaction Description\" AS \"Описание транзакции\", " +
                                        "\"Transaction Change\" AS \"Изменение транзакции\", " +
                                        "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
                                        "FROM Transactions " +
                                        "JOIN Users on Fk_User = Pk_User " +
                                        "JOIN Categories on Fk_Category = Pk_Category " +
                                         $"WHERE Categories.Pk_Category = {categoryId}");
    }

    public DataTable GetTransactionGroupByCategory() {
        return _connection.Execute_Query("SELECT " +
                                        "\"Category Name\" AS \"Категория транзакции\", " +
                                        "SUM(\"Transaction Change\") AS \"Сумма всех транзакций по данной категории\" " +
                                        "FROM Transactions " +
                                        "JOIN Users on Fk_User = Pk_User " +
                                        "JOIN Categories on Fk_Category = Pk_Category " +
                                        "GROUP BY Categories.\"Category Name\"");
    }

    public DataTable GetTransactionPerCategoryAndDate(DateTime dt, DateTime dt2, int categoryId) {
        var time = Time(dt);
        var time2 = Time(dt2);
        return _connection.Execute_Query("SELECT " +
                                        "\"Pk_Transaction\" AS \"ID транзакции\", " +
                                        "\"User Name\" AS \"Имя пользователя\", " +
                                        "\"Category Name\" AS \"Категория транзакции\", " +
                                        "\"Transaction Description\" AS \"Описание транзакции\", " +
                                        "\"Transaction Change\" AS \"Изменение транзакции\", " +
                                        "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
                                        "FROM Transactions " +
                                        "JOIN Users on Fk_User = Pk_User " +
                                        "JOIN Categories on Fk_Category = Pk_Category " +
                                         $"WHERE Categories.Pk_Category = {categoryId} AND \"Transaction Date\" > {time} AND \"Transaction Date\" < {time2}");
    }

    public DataTable GetTransactionPerCategoryAndUser(int userId, int categoryId) {
        return _connection.Execute_Query("SELECT " +
                                        "\"Pk_Transaction\" AS \"ID транзакции\", " +
                                        "\"User Name\" AS \"Имя пользователя\", " +
                                        "\"Category Name\" AS \"Категория транзакции\", " +
                                        "\"Transaction Description\" AS \"Описание транзакции\", " +
                                        "\"Transaction Change\" AS \"Изменение транзакции\", " +
                                        "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
                                        "FROM Transactions " +
                                        "JOIN Users on Fk_User = Pk_User " +
                                        "JOIN Categories on Fk_Category = Pk_Category " +
                                         $"WHERE Categories.Pk_Category = {categoryId} AND Users.Pk_User = {userId}");
    }

    public DataTable GetTransactionPerCategoryAndDateAndUser(DateTime dt, DateTime dt2, int categoryId, int userId) {
        var time = Time(dt);
        var time2 = Time(dt2);
        return _connection.Execute_Query("SELECT " +
                                        "\"Pk_Transaction\" AS \"ID транзакции\", " +
                                        "\"User Name\" AS \"Имя пользователя\", " +
                                        "\"Category Name\" AS \"Категория транзакции\", " +
                                        "\"Transaction Description\" AS \"Описание транзакции\", " +
                                        "\"Transaction Change\" AS \"Изменение транзакции\", " +
                                        "strftime('%d-%m-%Y %H:%M', \"Transaction Date\", 'unixepoch') AS \"Дата транзакции\" " +
                                        "FROM Transactions " +
                                        "JOIN Users on Fk_User = Pk_User " +
                                        "JOIN Categories on Fk_Category = Pk_Category " +
                                         $"WHERE Categories.Pk_Category = {categoryId} AND \"Transaction Date\" > {time} AND \"Transaction Date\" < {time2} AND Users.Pk_User = {userId}");
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
            id = int.Parse((string)Users.Rows[^1][0]) + 1;
        else
            id = 0;

        _connection.Execute_Action($"Insert Into 'Users' Values ({id}, '{UserName}')");
    }

    public void Add_Transaction(int user, int category, string description, int change, long date)
    {
        int id;
        if (Transactions.Rows.Count != 0)
            id = int.Parse((string)Transactions.Rows[^1][0]) + 1;
        else
            id = 0;

        _connection.Execute_Action(
            $"Insert Into 'Transactions' Values ({id}, {user}, {category}, '{description}', {change}, {date})");
    }

    public void Add_Category(string name)
    {
        int id;
        if (Categories.Rows.Count != 0)
            id = int.Parse((string)Categories.Rows[^1][0]) + 1;
        else
            id = 0;

        _connection.Execute_Action($"Insert Into 'Categories' Values ({id}, '{name}')");
    }

    public void Remove_Users(List<int> users)
    {
        var sb = new StringBuilder();
        foreach (var id in users) sb.Append($"{id}, ");

        sb.Remove(sb.Length - 2, 2);
        _connection.Execute_Action($"Delete From 'Transactions' Where Fk_User In ({sb})");
        _connection.Execute_Action($"Delete From 'Users' Where Pk_User In ({sb})");
    }

    public void Remove_Transactions(List<int> transactions)
    {
        var sb = new StringBuilder();
        foreach (var id in transactions) sb.Append($"{id}, ");

        sb.Remove(sb.Length - 2, 2);
        _connection.Execute_Action($"Delete From 'Transactions' Where Pk_Transaction In ({sb})");
    }

    public void Remove_Category(string category)
    {
        _connection.Execute_Action(
            $"Delete From 'Transactions' Where Fk_Category In (Select Pk_Category from 'Categories' where [Category Name] = '{category}')");
        _connection.Execute_Action($"Delete From 'Categories' Where \"Category Name\" = '{category}'");
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