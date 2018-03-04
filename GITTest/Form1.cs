using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GITTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void splitDates(string date)
        {
            //Split the date down and assign it to variables for later use.
            string[] arrayDate = date.Split('/');
            int year = Convert.ToInt32(arrayDate[2]);
            int month = Convert.ToInt32(arrayDate[1]);
            int day = Convert.ToInt32(arrayDate[0]);

            DateTime dateTime = new DateTime(year, month, day);

            string dayOfWeek = dateTime.DayOfWeek.ToString();
            int dayOfYear = dateTime.DayOfYear;
            string monthName = dateTime.ToString("MMMM");
            int weekNumber = dayOfYear / 7;
            bool weekend = false;
            if (dayOfWeek == "Saturday" || dayOfWeek == "Sunday") weekend = true;
            string dbDate = dateTime.ToString("M/dd/yyyy");

            insertTimeDimension(dbDate, dayOfWeek, day, monthName, month, weekNumber, year, weekend, dayOfYear);
        }

        private void insertTimeDimension(string date, string dayName, int dayNumber, string monthName, int monthNumber, int weekNumber, int year, bool weekend, int dayOfYear)
        {
            //Create a connection to the MDF file
            string connectionStringDestination = Properties.Settings.Default.DestinationDatabaseConnectionString;

            using (SqlConnection myConnection = new SqlConnection(connectionStringDestination))
            {

                // Open the SqlConnection.
                myConnection.Open();
                // The following code uses an SqlCommand based on the SqlConnection.
                SqlCommand command = new SqlCommand("SELECT id FROM Time WHERE date = @date", myConnection);
                command.Parameters.Add(new SqlParameter("date", date));

                //Create a variable and assign it to false by default.
                bool exists = false;

                //Run the command & read the results
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    //If there are rows, it means the date exsists so change the exsists variable. 
                    if (reader.HasRows)
                    {
                        exists = true;
                        Console.WriteLine("Data exsists!");
                    }
                }

                if (exists == false)
                {
                    SqlCommand insertCommand = new SqlCommand(
                        "INSERT INTO Time (dayName, dayNumber, monthName, monthNumber, weekNumber, year, weekend, date, dayOfYear) " +
                    " VALUES (@dayName, @dayNumber, @monthName, @monthNumber, @weekNumber, @year, @weekend, @date, @dayOfYear) ", myConnection);
                    insertCommand.Parameters.Add(new SqlParameter("dayName", dayName));
                    insertCommand.Parameters.Add(new SqlParameter("dayNumber", dayNumber));
                    insertCommand.Parameters.Add(new SqlParameter("monthName", monthName));
                    insertCommand.Parameters.Add(new SqlParameter("monthNumber", monthNumber));
                    insertCommand.Parameters.Add(new SqlParameter("weekNumber", weekNumber));
                    insertCommand.Parameters.Add(new SqlParameter("year", year));
                    insertCommand.Parameters.Add(new SqlParameter("weekend", weekend));
                    insertCommand.Parameters.Add(new SqlParameter("date", date));
                    insertCommand.Parameters.Add(new SqlParameter("dayOfYear", dayOfYear));


                    try
                    {
                        //insert the line
                        int recordsAffected = insertCommand.ExecuteNonQuery();
                        Console.WriteLine("Records affected: " + recordsAffected);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine(dayName + " " + dayNumber + " " + monthName + " " + monthNumber + " " + weekNumber + " " + year + " " + weekend + " " + date + " " + dayOfYear);
                        throw;
                    }
                }
            }
        }


        private void GetDates_Click(object sender, EventArgs e)
        {
            //Create new list to store the results in.
            List<string> Dates = new List<string>();
            //clear the listbox
            listBoxDates.Items.Clear();

            //Create the database string
            string connectionString = Properties.Settings.Default.Data_set_1ConnectionString;

            using (OleDbConnection connection = new OleDbConnection(connectionString))
            {
                connection.Open();
                OleDbDataReader reader = null;
                OleDbCommand getDates = new OleDbCommand("SELECT [Order Date], [Ship Date] from Sheet1'", connection);

                reader = getDates.ExecuteReader();
                while (reader.Read())
                {
                    Dates.Add(reader[0].ToString());
                    Dates.Add(reader[1].ToString());
                }
            }

            //Create a new list for the formatted data.
            List<string> DatesFormatted = new List<string>();

            foreach (string date in Dates)
            {
                //Split the string on whitespace and remove anything thats blank.
                var dates = date.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
                //Grab the first item (we know this is the date) and add it to our new list.
                DatesFormatted.Add(dates[0]);
            }

            //Bind the listbox to the list.
            listBoxDates.DataSource = DatesFormatted;


            //split the dates and insert every date in the list!
            foreach(string date in DatesFormatted)
            {
                splitDates(date);
            }
            
             
            
            
           
        }
    }
}
