using System.Data;
using Microsoft.Data.Sqlite;

namespace Connection_Manager;

/// <summary>
///     Класс гарантирующий наличие базы данных в каталоге программы и её соответствие формату
/// </summary>
public class Connector
{
    public enum Database_State
    {
        Correct,
        Incorrect,
        Missing
    }

    private SqliteConnection _connection;
    private readonly string _path;
    public Database_State State;

    public Connector(string path)
    {
        _path = path;
        if (!File.Exists(_path))
        {
            _connection = new SqliteConnection($"Data Source={_path}");
            _connection.Open();
            Create_Database();
            State = Database_State.Correct;
            return;
        }
        try
        {
            _connection = new SqliteConnection($"Data Source={_path}");
            _connection.Open();
            if (!Check_Conforming()) State = Database_State.Incorrect;
            if (State == Database_State.Incorrect)
            {
                return;
            }
            State = Database_State.Correct;
        }
        catch (Exception e)
        {
            State = Database_State.Missing;
        }
    }

    /// <summary>
    ///     Обнуляет базу данных на подключении и создаёт новую, подходящую для приложения
    /// </summary>
    public void Create_Database()
    {
        var creator = new SqliteCommand();
        _connection.Close();
        _connection.Dispose();
        _connection = new SqliteConnection($"Data Source={_path}");
        SqliteConnection.ClearAllPools();
        File.Delete(_path);
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
                              "'Transaction Change' int not null," +
                              "'Transaction Date' int not null," +
                              "foreign key('Fk_User') references 'Users'('Pk_User')" +
                              ");";
        creator.ExecuteNonQuery();
    }

    /// <summary>
    ///     Проверяет базу данных на соответствие требованиям
    /// </summary>
    /// <returns>true если соответствует, иначе false</returns>
    public bool Check_Conforming()
    {
        SqliteCommand checker = new();
        checker.Connection = _connection;
        checker.CommandText = "select * from sqlite_master where type is 'table';";
        var res = checker.ExecuteReader();
        bool has_users = false, has_transactions = false;
        if (!res.HasRows) return false;
        while (res.Read())
        {
            var resb = (string)res["name"];
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
            if ((string)res["name"] != "Pk_User" || (string)res["type"] != "INT" || (long)res["notnull"] != 1 ||
                (long)res["pk"] != 1 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else
        {
            return false;
        }

        if (res.Read())
        {
            if ((string)res["name"] != "User Name" || (string)res["type"] != "varchar(255)" ||
                (long)res["notnull"] != 1 || (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else
        {
            return false;
        }

        if (res.Read()) return false;
        res.Close();

        checker.CommandText = "PRAGMA table_info ('Transactions');";
        res = checker.ExecuteReader();
        if (res.Read())
        {
            if ((string)res["name"] != "Pk_Transaction" || (string)res["type"] != "INT" || (long)res["notnull"] != 1 ||
                (long)res["pk"] != 1 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else
        {
            return false;
        }

        if (res.Read())
        {
            if ((string)res["name"] != "Fk_User" || (string)res["type"] != "INT" || (long)res["notnull"] != 1 ||
                (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else
        {
            return false;
        }

        if (res.Read())
            if ((string)res["name"] != "Transaction Description" || (string)res["type"] != "varchar(255)" ||
                (long)res["notnull"] != 1 || (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        if (res.Read())
            if ((string)res["name"] != "Transaction Change" || (string)res["type"] != "INT" ||
                (long)res["notnull"] != 1 || (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        if (res.Read())
        {
            if ((string)res["name"] != "Transaction Date" || (string)res["type"] != "INT" ||
                (long)res["notnull"] != 1 || (long)res["pk"] != 0 ||
                res["dflt_value"].GetType().Name != "DBNull")
                return false;
        }
        else
        {
            return false;
        }

        if (res.Read()) return false;

        return true;
    }

    /// <summary>
    ///     Закрывает соединение
    /// </summary>
    public void Close()
    {
        _connection.Close();
    }

    /// <summary>
    ///     Выполняет запрос
    /// </summary>
    /// <param name="Query">Запрос</param>
    /// <returns>Результат запроса в виде таблицы</returns>
    public DataTable Execute_Query(string Query)
    {
        var table = new DataTable();
        var command = new SqliteCommand(Query, _connection);
        var f = true;
        using (var reader = command.ExecuteReader())
        {
            if (reader.HasRows)
                while (reader.Read())
                {
                    if (f)
                    {
                        for (var i = 0; i <= reader.FieldCount - 1; i++)
                            table.Columns.Add(reader.GetName(i));
                        f = false;
                    }

                    var row = table.NewRow();
                    for (var i = 0; i < reader.FieldCount; i++) row[i] = reader.GetValue(i);
                    table.Rows.Add(row);
                }
        }

        return table;
    }

    /// <summary>
    ///     Выполняет запрос
    /// </summary>
    /// <param name="Query">Запрос</param>
    public void Execute_Action(string Query)
    {
        var command = new SqliteCommand(Query, _connection);
        command.ExecuteNonQuery();
    }
}