using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Configuration;

namespace DBHomework_fruits_vegetables
{

    /*Задание1:
     -создать однотабличную БД фрукты и овощи (название, тип, цвет, калорийность)

     Задание2:
     -подключаться и отключаться от БД

    Задание3:
    -добавить функционал:
    1.отображение всей инфы из таблицы +
    2.отобр всех названий +
    3.отобр всех цветов +
    4.показ макс калорийность +
    5.показ мин калорийность +
    6.показ среднюю калорийность +
    7.показ колво овощей +
    8.показ колво фруктов +
    9.показ колво овощей и фруктов заданного цвета+
    10.показ колво овощей-фруктов каждого цвета +
    11.показать овощи-фрукты с калорийностью ниже указанной +
    12.показвать овощи-фрукты с калорийностью выше указанной +
    13.показать овощи-фрукты с калорийностью в указанном диапазоне 
    14.показать все овощи-фрукты у которых цвет желтый или красный +
     */
    internal class Program
    {
        static SqlConnection OpenDbConnection()
        {
            // обработка исключений будет выполняться выше по стеку
            string connectionString = ConfigurationManager.ConnectionStrings["LocalConnection"].ConnectionString;
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
        static void ReadQueryResult(SqlDataReader queryResult)
        {
            // 1. вывести названия столбцов результирующей таблицы (представления)
            for (int i = 0; i < queryResult.FieldCount - 1; i++)
            {
                Console.Write($"{queryResult.GetName(i)} - ");
            }
            Console.WriteLine(queryResult.GetName(queryResult.FieldCount - 1));
            // 2. вывести значения построчно
            bool noRows = true;
            while (queryResult.Read())
            {
                noRows = false;
                for (int i = 0; i < queryResult.FieldCount - 1; i++)
                {
                    Console.Write($"{queryResult[i]} - ");
                }
                Console.WriteLine(queryResult[queryResult.FieldCount - 1]);
            }
            if (noRows)
            {
                Console.WriteLine("No rows in result");
            }
        }
        static void QueryHandler(string query_string)
        {
            /*
             функция для запросов без входных параметров,
            чтобы не копировать одинаковый код
             */
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand(query_string, connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void SelectAllRows()//вывод всей информации из таблицы
        {
            QueryHandler("SELECT * FROM fruits_vegetables_t");
        }
        static void SelectAllNames()//вывод всех названий
        {
            QueryHandler("SELECT name_f FROM fruits_vegetables_t;");
        }
        static void SelectAllColors()//вывод всех цветов
        {
            QueryHandler("SELECT DISTINCT color_f FROM fruits_vegetables_t;");
        }
        static void SelectMaxCal()//показать макс калорийность
        {
            QueryHandler("SELECT MAX(cal_f) AS 'Max cal:' FROM fruits_vegetables_t;");
        }
        static void SelectMinCal()//показать мин калорийность
        {
            QueryHandler("SELECT MIN(cal_f) AS 'Min cal:' FROM fruits_vegetables_t;");
        }
        static void SelectAvgCal()//показать среднюю калорийность
        {
            QueryHandler("SELECT AVG(cal_f) AS 'AVG cal:' FROM fruits_vegetables_t;");
        }
        static void SelectCountOfVegetables()//показать колво овощей
        {
            QueryHandler("SELECT COUNT(type_f) AS 'Count of vegetables:'\r\nFROM fruits_vegetables_t\r\nWHERE type_f='овощ';");
        }
        static void SelectCountOfFruits()//показать колво фруктов
        {
            QueryHandler("SELECT COUNT(type_f) AS 'Count of fruits:'\r\nFROM fruits_vegetables_t\r\nWHERE type_f='фрукт';");
        }
        static void SelectColorsCount()//показать колво фруктов и овощей всех цветов
        {
            QueryHandler("SELECT color_f AS 'Color', COUNT(color_f) AS 'Count'\r\nFROM fruits_vegetables_t\r\nGROUP BY color_f;");
        }
        static void SelectYellowFetus()//показать овощи-фрукты желтого цвета
        {
            QueryHandler("SELECT name_f AS 'Fetus:'\r\nFROM fruits_vegetables_t\r\nWHERE color_f='желтый';");
        }
        //--------------функции с входными параметрами----------------------
        static void SelectFetusByColor(string color)//вывод колва фруктов-овощей по параметру цвет
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT COUNT(color_f) AS 'Count of fetus by color:'\r\nFROM fruits_vegetables_t\r\nWHERE color_f='{color}';", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void SelectMinCalFetus(int cal)//вывод фруктов-овощей с калорийностью ниже указанной
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT name_f, cal_f\r\nFROM fruits_vegetables_t\r\nWHERE cal_f<{cal};", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void SelectMaxCalFetus(int cal)//вывод ф-о с калорийностью выше указанной
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT name_f, cal_f\r\nFROM fruits_vegetables_t\r\nWHERE cal_f>{cal};", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void SelectRangeCalFetus(int min, int max)
        {
            SqlConnection connection = null;
            SqlDataReader queryResult = null;
            try
            {
                // 1. открыть соединение
                connection = OpenDbConnection();
                // 2. подготовить запрос
                SqlCommand query = new SqlCommand($"SELECT name_f, cal_f\r\nFROM fruits_vegetables_t\r\nWHERE cal_f>{min} AND cal_f<{max};", connection);//clients_t orders_t
                // 3. выполнить запрос с табличным результом
                queryResult = query.ExecuteReader();
                // 4. считать запрос (универсальный способ)
                ReadQueryResult(queryResult);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Something wrong: {ex.Message}");
            }
            finally
            {
                connection?.Close();    // закрыть соединение (если != null)
                queryResult?.Close();
            }
        }
        static void Main(string[] args)
        {
            //OpenDbConnection().Close();
            //SelectAllRows();
            //SelectAllNames();
            //SelectAllColors();
            //SelectMaxCal();
            //SelectMinCal();
            //SelectAvgCal();
            //SelectCountOfVegetables();
            // SelectCountOfFruits();
            //SelectColorsCount();
            //SelectYellowFetus();
            //SelectFetusByColor("красный");
            //SelectMinCalFetus(20);
            //SelectMaxCalFetus(40);
            //SelectRangeCalFetus(10, 20);
        }
    }
}
