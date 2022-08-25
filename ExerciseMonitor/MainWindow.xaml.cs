using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data.SqlClient; //local DB
using System.Data; //Connection state, data table
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ExerciseMonitor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>       
    public partial class MainWindow : Window
    {
        String Id = "";

        public MainWindow()
        {
            InitializeComponent();
            SetDateTextbox();
            SetNumberTextbox();
            LoadTable();
            
        }

        protected override void OnClosed(EventArgs e)
        {

            DBClass.Close_DB_Connection();
            base.OnClosed(e);
        }

        //Insert data to the DB
        private void insert_button_Click(object sender, RoutedEventArgs e)
        {
            //String A = num_textbox.Text;
            //https://stackoverflow.com/questions/21835891/process-starturl-fails
            //Process.Start(new ProcessStartInfo(A) {UseShellExecute = true });

            Input_class input = new Input_class();

            input.day_number = Int32.Parse(num_textbox.Text);
            input.Date = datepicker.SelectedDate.Value.ToShortDateString();
            input.Distance = float.Parse(distance_textbox.Text);
            input.Calories = float.Parse(calories_textbox.Text);

            if (Data_check(input) == false)
            {
                MessageBox.Show("Please fill all the fields");
            }

            else
            {
                DBClass.Get_DB_Connection();

                DBClass.Add_info(input);
                
            }
            //DBClass.Close_DB_Connection();
            //DBClass.Get_DB_Connection();

            
        }

        //Refreshes the datagrid with DB info
        private void Refresh_button_click(object sender, RoutedEventArgs e)
        {
            /*List<Datagrid_test> people = new List<Datagrid_test>();
            people.Add(new Datagrid_test() { id = 1, name = "Pepe", distance = 20.15 });
            people.Add(new Datagrid_test() { id = 2, name = "Natasha", distance = 20.15 });
            people.Add(new Datagrid_test() { id = 3, name = "Rin", distance = 20.15 });
            people.Add(new Datagrid_test() { id = 4, name = "Kama", distance = 20.15 });
            people.Add(new Datagrid_test() { id = 5, name = "Kiara", distance = 20.15 });
            LoadTable();
            Exs_DataGrid.ItemsSource = people;*/
            LoadTable();
        }

        //
        private void Erase_row(object sender, RoutedEventArgs e)
        {
            Select_data();
        }

        private void Erase(object sender, RoutedEventArgs e)
        {

            MessageBoxResult result = MessageBox.Show("Erase selected rows", "Warning", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            switch (result)
            {
                case MessageBoxResult.Yes:
                    Debug.WriteLine(DBClass.Erase_Row(Id));
                break;

                case MessageBoxResult.No:

                break;
            }
            
        }

        //Gets data when a row is selected in the datagrid
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = (DataGrid)sender;
            DataRowView rowView = grid.SelectedItem as DataRowView;

            if (rowView != null)
            {
                Debug.WriteLine(rowView["Day_Num"].ToString());
                Id = rowView["Day_Num"].ToString();
            }
        }

        //----------------------------------------------------------------------//

        //Checks that all textbox are filled before inserting data into database
        private bool Data_check(Input_class input_check)
        {
            bool state = true;
            
            if(input_check.day_number == 0 || input_check.Date == "" || datepicker.SelectedDate.Value.ToShortDateString() =="" ||
                input_check.Distance == 0 || input_check.Calories == 0)
            {
                state = false;
            }



            Debug.WriteLine("------" + "Date: " + input_check.day_number + "---------");
            Debug.WriteLine("------" + "Date: " + input_check.Date + "---------");
            Debug.WriteLine("------" + "Distance: "  + input_check.Distance + "---------");
            Debug.WriteLine("------" + "Calories: " + input_check.Calories + "---------");

            Debug.WriteLine("------" + state + "---------");
            
            return state;
        }

        private void Dissconnect(object sender, RoutedEventArgs e)
        {
            //DBClass.Close_DB_Connection();
            //DBClass.Get_day();

            Debug.WriteLine(datepicker.SelectedDate.Value.ToShortDateString());
        }

        //Acces Db and sets the current sesion of excercise
        private void SetNumberTextbox()
        {
            num_textbox.Text = (DBClass.Get_day() + 1).ToString();
        }

        //Loads current date as soon as the application starts
        private void SetDateTextbox()
        {                        
            DateTime dateTime = DateTime.Now;
            DateTime date = dateTime.Date;
            date_textbox.Text = date.ToString("d");
            datepicker.SelectedDate = DateTime.Today;
        }

        //Loads data into Datagrid
        private void LoadTable()
        {
            
            SqlConnection sqlConnection = DBClass.Get_DB_Connection();
            String Query = "SELECT Day_Num, Date, Distance_Km, Time, Calories FROM Exs_Table";
            SqlCommand sqlCommand = new SqlCommand(Query, sqlConnection);
            

            try
            {
                sqlCommand.ExecuteNonQuery();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);                
                DataTable dataTable = new DataTable("Exs_Table");

                dataAdapter.Fill(dataTable);                
                Exs_DataGrid.ItemsSource = dataTable.DefaultView;
                dataAdapter.Update(dataTable);
                Debug.WriteLine("-----Updated datagrid-----");
            }

            catch (SqlException)
            {
                Debug.WriteLine("-----Error loading data table-----");
            }
            
            Debug.WriteLine("-----Load table method-----");
        }

        //Gets data from selected row in DataGrid
        private void Select_data()
        {
            Input_class input = new Input_class();


            /*foreach(DataRowView row in Exs_DataGrid.Items)
            {
                Debug.WriteLine(row.Row.ItemArray[1].ToString());
            }*/

            

            var currentIndex = Exs_DataGrid.Items.IndexOf(Exs_DataGrid.SelectedItem);
            Debug.WriteLine("DATA_LOADED: " + currentIndex);

            if (Exs_DataGrid.SelectedItem != null)
            {

                
                var input_2 = Exs_DataGrid.SelectedItems as Input_class;
                Debug.WriteLine("DATA_LOADED");
                //Debug.WriteLine("DATA RETRIEVED: " + input_2.Calories);

            }

            else
            {
                Debug.WriteLine("NULL DATA");
            }

            

        }


    }
}
