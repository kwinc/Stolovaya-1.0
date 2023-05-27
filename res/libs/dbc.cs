using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows;

namespace Stolovaya_1._0.res.libs
{
    static internal class dbc
    {
        public static string connectionString = "";
        public static DataTable Select(string selectSQL) // функция подключения к базе данных и обработка запросов
        {
            DataTable dataTable = new DataTable("dataBase");                // создаём таблицу в приложении
            SqlConnection sqlConnection;                                       // подключаемся к базе данных
            sqlConnection = new SqlConnection(connectionString);
            SqlCommand sqlCommand = sqlConnection.CreateCommand();          // создаём команду
            sqlCommand.CommandText = selectSQL;                             // присваиваем команде текст
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand); // создаём обработчик
            sqlDataAdapter.Fill(dataTable);                                 // возращаем таблицу с результатом
            //sqlConnection.Close();                                          // антилаг
            return dataTable;
        }
        public static bool isAgeAllowed(int minAge, DateTime birthDate)
        {
            double age = Math.Round(System.DateTime.Now.Subtract(birthDate).TotalDays / 365.25, 2);
            return (age > minAge);
        }

        

    }
}
