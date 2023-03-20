using System.Data;
using Microsoft.Data.Sqlite;

namespace Connection_Manager;

/*
todo: Параметризовать функции Create_Database и Check_Conforming, Например Create_Database(string path)
todo: Реализовать Execute_Query и Execute_Action
 */



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
        _connection.Close();
        _connection.Dispose();
        _connection = new("Data Source=Data.db");
        SqliteConnection.ClearAllPools();
        File.Delete("Data.db");
        _connection.Open();
        creator.Connection = _connection;
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
        DataTable table = new DataTable();
        SqliteCommand command = new SqliteCommand(Query, _connection);
        bool f = true;
        using (SqliteDataReader reader = command.ExecuteReader())
        {
            if (reader.HasRows) 
            {
                while (reader.Read())  
                {
                    if (f)
                    {
                        for (int i = 0; i <= reader.FieldCount; i++)
                            table.Columns.Add(i.ToString(), typeof(object));
                        f = false;
                    }

                    DataRow row = table.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                        row[i] = reader.GetValue(i);
                    table.Rows.Add(row);    
                }
            }
        }
        return table;
    }

    /// <summary>
    /// Выполняет запрос
    /// </summary>
    /// <param name="Query">Запрос</param>
    public void Execute_Action(string Query)
    {
        SqliteCommand command = new SqliteCommand(Query, _connection);
        command.ExecuteNonQuery();
    }
}