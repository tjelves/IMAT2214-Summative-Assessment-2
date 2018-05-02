using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
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




            //Create new list to store the results in.
            List<string> Times = new List<string>();
            //clear the listbox
            listBoxTimes.Items.Clear();

            //Create a connection to the MDF file
            string connectionStringDestination = Properties.Settings.Default.DestinationDatabaseConnectionString;
            //string connectionStringDestination = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=Databases\DestinationDatabase.mdf;Integrated Security=True;Connect Timeout=30";

            using (SqlConnection myConnection = new SqlConnection(connectionStringDestination))
            {
                //
                // Open the SqlConnection.
                //
                myConnection.Open();
                //
                // The following code uses an SqlCommand based on the SqlConnection.
                //
                using (SqlCommand command = new SqlCommand("SELECT * from Time", myConnection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Console.WriteLine("Reading from destination database!");
                        foreach (string item in reader)
                        {
                            
                            Console.WriteLine(item.ToString());
                        }
                    }
                }
            }

            //Bind the listbox to the list.
            listBoxTimes.DataSource = Times;
        }
    }
}
