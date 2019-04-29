using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace TADE.Models
{
    public class TADESql
    {
        public string ConnectionString;
        public string TADESQLConn()
        {
            // Set up connection string
            //ConnectionString = ConfigurationManager.ConnectionStrings("DEIMOSConnectionString").ConnectionString;

            ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["TADEDBConnection"].ConnectionString;

            return ConnectionString;
        }
        public void MakeBulkCopyTable(string destinationTableName,DataTable sourceTable)
        {
            // Open a connection to the AdventureWorks database.
            using (SqlConnection connection =
                       new SqlConnection(TADESQLConn()))
            {
                connection.Open();

                // Perform an initial count on the destination table.
                SqlCommand commandRowCount = new SqlCommand(
                    "SELECT COUNT(*) FROM " +
                    destinationTableName,
                    connection);
                long countStart = System.Convert.ToInt32(
                    commandRowCount.ExecuteScalar());
                Console.WriteLine("Starting row count = {0}", countStart);

                // Create a table with some rows. 
               // DataTable newProducts = SelectAvailableDates();

                // Create the SqlBulkCopy object. 
                // Note that the column positions in the source DataTable 
                // match the column positions in the destination table so 
                // there is no need to map columns. 
                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName =
                        destinationTableName;

                    try
                    {
                        // Write from the source to the destination.
                        bulkCopy.WriteToServer(sourceTable);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                
                // Perform a final count on the destination 
                // table to see how many rows were added.
                long countEnd = System.Convert.ToInt32(
                    commandRowCount.ExecuteScalar());
               
            }
        }
        public DataTable SelectAvailableDates()
        {
            SqlDataAdapter mySqlDataAdapter;
            DataTable myDataTable;
            myDataTable = new DataTable();
            string strQuery = String.Empty;
            using (SqlConnection mySqlConnection = new SqlConnection(TADESQLConn()))
            {
                strQuery = "SELECT * FROM [AvailableDates] where AvailableDate < @availableDate";
                using (SqlCommand objCmd = new SqlCommand(strQuery, mySqlConnection))
                {
                    objCmd.Parameters.Add(new SqlParameter("@availableDate", SqlDbType.Date)).Value = Convert.ToDateTime(DateTime.Today.ToShortDateString());

                    try
                    {
                        mySqlConnection.Open();
                        objCmd.CommandText = strQuery;
                        objCmd.CommandType = CommandType.Text;
                        objCmd.Connection = mySqlConnection;
                        mySqlDataAdapter = new SqlDataAdapter();
                        mySqlDataAdapter.SelectCommand = objCmd;
                        mySqlDataAdapter.Fill(myDataTable);
                        mySqlDataAdapter.Dispose();
                        objCmd.Dispose();
                        mySqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }



            return myDataTable;
        }
        public bool DeleteAvailableDates()
        {
            bool retresult = false;

            string strSMSQuery = string.Empty;
            using (SqlConnection avadts = new SqlConnection(TADESQLConn()))
            {

                strSMSQuery = "DELETE FROM  [AvailableDates] where AvailableDate < @availableDate";

                using (SqlCommand AVDTCmd = new SqlCommand(strSMSQuery, avadts))
                {

                    var _with1 = AVDTCmd;

                    _with1.Parameters.Add(new SqlParameter("@availableDate", SqlDbType.Date)).Value = Convert.ToDateTime(DateTime.Today.ToShortDateString()); 



                    try
                    {
                        avadts.Open();

                        AVDTCmd.ExecuteNonQuery();
                        avadts.Close();
                        retresult = true;

                    }
                    catch (Exception ex)
                    {
                        retresult = false;
                    }

                }

            }


            return retresult;
        }

        public bool DeleteAvailableSeatsForSlotsByDate(int? availableDatesId)
        {
            bool retresult = false;

            string strSMSQuery = string.Empty;
            using (SqlConnection avadts = new SqlConnection(TADESQLConn()))
            {

                strSMSQuery = "DELETE FROM  AvailableSeatsForSlots where AvailableDatesId < @availableDatesId";

                using (SqlCommand AVDTCmd = new SqlCommand(strSMSQuery, avadts))
                {

                    var _with1 = AVDTCmd;

                    _with1.Parameters.Add(new SqlParameter("@availableDatesId", SqlDbType.Int)).Value = availableDatesId;



                    try
                    {
                        avadts.Open();

                        AVDTCmd.ExecuteNonQuery();
                        avadts.Close();
                        retresult = true;

                    }
                    catch (Exception ex)
                    {
                        retresult = false;
                    }

                }

            }


            return retresult;
        }
        public DataTable MakeTableAvailableSeatsForSlots(List<AvailableSeatsForSlot> availableSeatsForSlot)
        // Create a new DataTable named tblAvailableSeatsForSlots. 
        {
            DataTable tblAvailableSeatsForSlots = new DataTable("tblAvailableSeatsForSlots");
         

            DataColumn AvailableSeatsForSlotsId = new DataColumn();
            AvailableSeatsForSlotsId.DataType = System.Type.GetType("System.Int32");
            AvailableSeatsForSlotsId.ColumnName = "AvailableSeatsForSlotsId";
            AvailableSeatsForSlotsId.AutoIncrement = true;
            tblAvailableSeatsForSlots.Columns.Add(AvailableSeatsForSlotsId);

            DataColumn AvailableDatesId = new DataColumn();
            AvailableDatesId.DataType = System.Type.GetType("System.Int32");
            AvailableDatesId.ColumnName = "AvailableDatesId";
            tblAvailableSeatsForSlots.Columns.Add(AvailableDatesId);

            DataColumn SlotId = new DataColumn();
            SlotId.DataType = System.Type.GetType("System.Int32");
            SlotId.ColumnName = "SlotId";
            tblAvailableSeatsForSlots.Columns.Add(SlotId);


            DataColumn TotalSeats = new DataColumn();
            TotalSeats.DataType = System.Type.GetType("System.Int32");
            TotalSeats.ColumnName = "TotalSeats";
            tblAvailableSeatsForSlots.Columns.Add(TotalSeats);

            DataColumn RemainingSeats = new DataColumn();
            RemainingSeats.DataType = System.Type.GetType("System.Int32");
            RemainingSeats.ColumnName = "RemainingSeats";
            tblAvailableSeatsForSlots.Columns.Add(RemainingSeats);
            
            DataColumn Status = new DataColumn();
            Status.DataType = System.Type.GetType("System.Boolean");
            Status.ColumnName = "Status";
            tblAvailableSeatsForSlots.Columns.Add(Status);
            // Create an array for DataColumn objects.
            DataColumn[] keys = new DataColumn[1];
            keys[0] = AvailableSeatsForSlotsId;
            tblAvailableSeatsForSlots.PrimaryKey = keys;

            // Add some new rows to the collection. 
            DataRow row;
            foreach (var availableSeat in availableSeatsForSlot)
            {
                row= tblAvailableSeatsForSlots.NewRow();
                row["SlotId"] = availableSeat.SlotId;
                row["AvailableDatesId"] = availableSeat.AvailableDatesId;
                row["TotalSeats"] = availableSeat.TotalSeats;
                row["RemainingSeats"] = availableSeat.TotalSeats;
                row["Status"] = true;
                tblAvailableSeatsForSlots.Rows.Add(row);
                
            }
           
            tblAvailableSeatsForSlots.AcceptChanges();

            // Return the new DataTable. 
            return tblAvailableSeatsForSlots;
        }

        public string UpdateAvailableSeatsForSlot(int availableSeatsForSlotsId,int? totalSeats, int? remainingSeats)
        {

            string retresult = null;

            string strQuery = string.Empty;
            using (SqlConnection objConn = new SqlConnection(TADESQLConn()))
            {

                strQuery = "UPDATE [dbo].[AvailableSeatsForSlots] SET [TotalSeats] = @totalSeats,[RemainingSeats] = @remainingSeats WHERE AvailableSeatsForSlotsId = @availableSeatsForSlotsId";


                using (SqlCommand objCmd = new SqlCommand(strQuery, objConn))
                {

                    var _with1 = objCmd;
                    _with1.Parameters.Add(new SqlParameter("@availableSeatsForSlotsId", SqlDbType.Int)).Value = Convert.ToInt32(availableSeatsForSlotsId);
                    _with1.Parameters.Add(new SqlParameter("@totalSeats", SqlDbType.Int)).Value = Convert.ToInt32(totalSeats);

                    _with1.Parameters.Add(new SqlParameter("@remainingSeats", SqlDbType.Int)).Value = Convert.ToInt32(remainingSeats);
                   

                    try
                    {
                        objConn.Open();

                        objCmd.ExecuteNonQuery();
                        objConn.Close();
                        retresult = "True";

                    }
                    catch (Exception ex)
                    {
                        retresult = ex.ToString();
                    }

                }

            }


            return retresult;
        }

        public string SelectExaminerByID(int? ExaminerId)
        {
            SqlDataAdapter mySqlDataAdapter;
            DataTable myDataTable;
            myDataTable = new DataTable();
            string strQuery = String.Empty;
            using (SqlConnection mySqlConnection = new SqlConnection(TADESQLConn()))
            {
                strQuery = "SELECT AspNetUsers.UserName FROM AspNetUsers INNER JOIN ExaminerDetails ON AspNetUsers.Id = ExaminerDetails.UserId WHERE (ExaminerDetails.ExaminerId = @ExaminerId)";
                using (SqlCommand objCmd = new SqlCommand(strQuery, mySqlConnection))
                {
                    objCmd.Parameters.Add(new SqlParameter("@availableDate", SqlDbType.Int)).Value = Convert.ToInt32(ExaminerId);

                    try
                    {
                        mySqlConnection.Open();
                        objCmd.CommandText = strQuery;
                        objCmd.CommandType = CommandType.Text;
                        objCmd.Connection = mySqlConnection;
                        mySqlDataAdapter = new SqlDataAdapter();
                        mySqlDataAdapter.SelectCommand = objCmd;
                        mySqlDataAdapter.Fill(myDataTable);
                        mySqlDataAdapter.Dispose();
                        objCmd.Dispose();
                        mySqlConnection.Close();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }


            return myDataTable.Rows[0][0].ToString();
            //return myDataTable;
        }

    }
}