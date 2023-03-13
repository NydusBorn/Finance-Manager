using System.Data;
using Microsoft.Data.Sqlite;

namespace Connection_Manager;

/// <summary>
/// Класс гарантирующий наличие базы данных в каталоге программы и её соответствие формату
/// </summary>
public class Connector
{
    private SqliteConnection _connection;
    
    public Connector()
    {
        _connection = new SqliteConnection("Data Source=Data.db");
        _connection.Open();
    }

    /// <summary>
    /// Обнуляет базу данных на подключении и создаёт новую, подходящую для приложения
    /// </summary>
    public void Create_Database()
    {
        SqliteCommand creator = new SqliteCommand();
        creator.Connection = _connection;
        creator.CommandText = "PRAGMA writable_schema = 1;" + 
                              "delete from sqlite_master where type in ('table', 'index', 'trigger');" +
                              "PRAGMA writable_schema = 0;";
        creator.ExecuteNonQuery();
        creator.CommandText = "Create table 'Users'(" +
                              "'Pk_User' int not null primary key," +
                              "'User Name' varchar(255) not null" +
                              ");";
        creator.ExecuteNonQuery();
        creator.CommandText = "CREATE table 'Transactions'(" +
                              "'Pk_Transaction' int not null primary key," +
                              "'Fk_User' int not null," +
                              "'Transaction Description' varchar(255) not null," +
                              "foreign key('Fk_User') references 'Users'('Pk_User')" +
                              ");";
        creator.ExecuteNonQuery();
    }

    /// <summary>
    /// Проверяет базу данных на соответствие требованиям
    /// </summary>
    /// <returns>true если соответствует, иначе false</returns>
    public bool Check_Conforming()
    {
        SqliteCommand checker = new();
        checker.Connection = _connection;
        checker.CommandText = "select * from sqlite_master where type is 'table';";
        var res = checker.ExecuteReader();
        bool has_users = false, has_transactions = false;
        while (res.Read())
        {
            string resb = (string)res["name"];
            if (!has_users && resb == "Users") has_users = true;
            else if (has_users && resb == "Users") return false;
            else if (!has_transactions && resb == "Transactions") has_transactions = true;
            else if (has_transactions && resb == "Transactions") return false;
            else return false;
        }
        res.Close();
        
        checker.CommandText = "PRAGMA table_info ('Users');";
        res = checker.ExecuteReader();
        if (res.Read())
        {
            if ((string)res["name"] != "Pk_User" || (string)res["type"] != "INT" || (long)res["notnull"] != 1 || (long)res["pk"] != 1 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else return false;
        if (res.Read())
        {
            if ((string)res["name"] != "User Name" || (string)res["type"] != "varchar(255)" || (long)res["notnull"] != 1 || (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else return false;
        if (res.Read()) return false;
        res.Close();
        
        checker.CommandText = "PRAGMA table_info ('Transactions');";
        res = checker.ExecuteReader();
        if (res.Read())
        {
            if ((string)res["name"] != "Pk_Transaction" || (string)res["type"] != "INT" || (long)res["notnull"] != 1 || (long)res["pk"] != 1 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else return false;
        if (res.Read())
        {
            if ((string)res["name"] != "Fk_User" || (string)res["type"] != "INT" || (long)res["notnull"] != 1 || (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else return false;
        if (res.Read())
        {
            if ((string)res["name"] != "Transaction Description" || (string)res["type"] != "varchar(255)" || (long)res["notnull"] != 1 || (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else return false;
        if (res.Read()) return false;

        return true;
    }

    /// <summary>
    /// Закрывает соединение
    /// </summary>
    public void Close()
    {
        _connection.Close();
    }

    /// <summary>
    /// Выполняет запрос
    /// </summary>
    /// <param name="Query">Запрос</param>
    /// <returns>Результат запроса в виде таблицы</returns>
    public DataTable Execute_Query(string Query)
    {
        throw new NotImplementedException();
    }
}