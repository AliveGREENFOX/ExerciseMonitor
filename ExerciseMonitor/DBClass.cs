using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Data.SqlClient; //local DB
using System.Data; //Connection state, data table

namespace ExerciseMonitor
{
   public static class DBClass
    {
        //https://codedocu.com/Net-Framework/WPF/Code-Samples/Add-WPF-Local-SQL-Database-to-Application-and-Connect-Data?2140

        //-----------------Connects to the DB
        public static SqlConnection Get_DB_Connection()
        {
            String cn_String = Properties.Settings.Default.Connection_String;
            SqlConnection cn_connection = new SqlConnection(cn_String);

            if(cn_connection.State != ConnectionState.Open)
            {
                try
                {
                    cn_connection.Open();
                    Debug.WriteLine("-----Connection complete-----");
                }

                catch (SqlException)
                {
                    Debug.WriteLine("-----Connection failed-----");
                }                
            }

            Debug.WriteLine("-----Connection state is: " + cn_connection.State + "-----");
            return cn_connection;
        }

        //-----------------Get DB Table
        public static DataTable Get_DataTable(String SQL_Text)
        {
            SqlConnection connection = Get_DB_Connection();
            DataTable table = new DataTable();

            SqlDataAdapter adapter = new SqlDataAdapter(SQL_Text, connection);
            adapter.Fill(table);

            return table;
        }

        //-----------------Get Day from DB

        public static int Get_day()
        {
            int day_num = 0;
            //var data_read;
            SqlConnection connection = Get_DB_Connection();
            String query = "SELECT TOP 1 Day_Num FROM Exs_Table ORDER BY Day_Num DESC"; //ORDER BY Day_Num DESC LIMIT 1 ";

            SqlCommand sqlCommand = new SqlCommand(query, connection);
            

            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    day_num = reader.GetInt32(0);
                    Debug.WriteLine(reader["Day_Num"]);
                    Debug.WriteLine("RETRIEVED DAY DATA");
                }
            }

                /*try
                {
                    while (reader.Read())
                    {
                        Debug.WriteLine("RETRIEVED DAY DATA");
                        Debug.WriteLine(reader.GetString(1));
                    }

                }

                catch (SqlException)
                {
                    Debug.WriteLine("Error trying to quary");
                }*/


                return day_num;
        }

        //-----------------Execute a SQL
        public static void Execute_SQL(String SQL_Text)
        {
            SqlConnection cn_connection = Get_DB_Connection();

            SqlCommand cmd_command = new SqlCommand(SQL_Text, cn_connection);
            cmd_command.ExecuteNonQuery();
        }

        //-----------------Erase Row from DB--------------------------
        public static int Erase_Row(String ID)
        {

            int DayNum = 0;
            SqlConnection sqlConnection = Get_DB_Connection();
            //String query = "DELETE from Exs_Table WHERE ";
            String query = "DELETE FROM Exs_Table WHERE Day_Num = " + ID;
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

            try
            {
                sqlCommand.ExecuteNonQuery();
                Debug.WriteLine("Row deleted");
            }

            catch (SqlException)
            {
                Debug.WriteLine("Error trying to delete row");
            }

            /*using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    //DayNum = reader.GetInt32(0);                    
                    Debug.WriteLine("-----Erase_Row Function: " + reader["Distance_Km"]);

                }
            }*/

            return DayNum;
        }

        ///--------------------------------------------------------
        public static void Add_info(Input_class input)
        {
            SqlConnection cn_connection = Get_DB_Connection();
            String data = "INSERT INTO Exs_Table VALUES(@Day_Num, @Date, @Distance_Mi, @Distance_Km, @Time, @Calories)";
            SqlCommand command = new SqlCommand(data, cn_connection);

            command.Parameters.Add("@Day_Num", SqlDbType.Int).Value = input.day_number;
            command.Parameters.Add("@Date", SqlDbType.Date).Value = input.Date;
            command.Parameters.Add("@Distance_Mi", SqlDbType.Float).Value = input.Distance;
            command.Parameters.Add("@Distance_Km", SqlDbType.Float).Value = input.Distance * 1.6;
            command.Parameters.Add("@Time", SqlDbType.Float).Value = input.Time;
            command.Parameters.Add("@Calories", SqlDbType.Float).Value = input.Calories;

            try
            {
                command.ExecuteNonQuery();
                Debug.WriteLine("-----Succesful query-----");
            }

            catch (SqlException)
            {
                Debug.WriteLine("Error trying to quary");
            }

            Close_DB_Connection();
        }

        public static DataTable LoadTable()
        {
            SqlConnection sqlConnection = Get_DB_Connection();
            String Query = "SELECT Day_Num, Date, Distance_Km, Time, Calories FROM Exs_Table";
            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            DataTable dataTable = new DataTable("Exs_Table");

            try
            {
                sqlCommand.BeginExecuteNonQuery();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);                
                dataAdapter.Fill(dataTable);
                               
            }

            catch (SqlException)
            {
                Debug.WriteLine("-----Error loading data table-----");
            }

            return dataTable;
        }

        //Closes DB connection
        public static void Close_DB_Connection()
        {
            Debug.WriteLine("-----CLOSE_DB_Connection entered-----");
            String cn_String = Properties.Settings.Default.Connection_String;
            SqlConnection cn_connection = new SqlConnection(cn_String);

            if(cn_connection.State != ConnectionState.Closed)
            {
                try
                {
                    cn_connection.Close();
                    Debug.WriteLine("-----DB Disconnected-----");
                }

                catch (SqlException)
                {
                    Debug.WriteLine("-----Error while dissconnecting DB-----");
                }
                
            }

            else
            {
                Debug.WriteLine("-----Connection is alraedy closed-----");
            }

            Debug.WriteLine("-----Connection state is: " + cn_connection.State + "-----");
        }

    }
}
