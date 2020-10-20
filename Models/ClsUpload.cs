using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Test.Models
{
    public class ClsUpload
    {
        public string ID { get; set; }

        public string CONTENT { get; set; }

        public IQueryable<TBL_M_CONTENT> showAll (string param)
        {
            LtsTestDataContext dataTest = new LtsTestDataContext();
            if(param == null || param == "")
            {
                return dataTest.TBL_M_CONTENTs;
            }
            else
            {
                return dataTest.TBL_M_CONTENTs.Where(i => i.CONTENT.Equals(param));
            }
        }

        public DataTable ProcessCSV(string fileName, string sSessUpload)
        {
            //Set up our variables
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray;
            List<string> strList;

            DataTable dt = new DataTable();
            DataRow row;
            //Regex r = new Regex(";(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            Regex r = new Regex("|");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName);

            //Read the first line and split the string at , with our regular expression in to an array
            line = sr.ReadLine();
            //strList = r.Split(line).ToList();
            strList = line.Split(';').ToList();
            strArray = r.Split(line);
            //strArray = strList.ToArray();

            //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
            Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));

            //Read each line in the CVS file until it’s empty
            while ((line = sr.ReadLine()) != null)
            {
                row = dt.NewRow();

                //add our current value to our data row
                line = string.Format("{0}|{1}", line);
                row.ItemArray = r.Split(line);
                dt.Rows.Add(row);
            }

            //Tidy Streameader up
            sr.Dispose();
            //return a the new DataTable
            return dt;
        }

        public String ProcessBulkCopy(DataTable dt, string sConnetionString, string sTblTempName)
        {
            string Feedback = string.Empty;
            string connString = ConfigurationManager.ConnectionStrings[sConnetionString].ConnectionString;

            //make our connection and dispose at the end
            using (SqlConnection conn = new SqlConnection(connString))
            {
                //make our command and dispose at the end
                using (var copy = new SqlBulkCopy(conn))
                {
                    //Open our connection
                    conn.Open();
                    ///Set target table and tell the number of rows
                    copy.DestinationTableName = sTblTempName;
                    copy.BatchSize = dt.Rows.Count;
                    try
                    {
                        //Send it to the server
                        copy.WriteToServer(dt);
                        Feedback = "Complete";
                    }
                    catch (Exception ex)
                    {
                        Feedback = ex.Message;
                    }
                }
            }
            return Feedback;
        }

    }
}