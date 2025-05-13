using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Text;

namespace SmsWsdlSrv
{
    /// <summary>
    /// Summary description for SmsService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SmsService : System.Web.Services.WebService
    {

        private bool IsAuthenticated()
        {
            // استخراج هدر Authorization
            var authHeader = HttpContext.Current.Request.Headers["Authorization"];
            if (authHeader == null || !authHeader.StartsWith("Basic "))
                return false;

            // رمزگشایی هدر Authorization برای دریافت نام کاربری و رمز عبور
            var base64Credentials = authHeader.Substring(6); // حذف "Basic "
            var credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(':');
            var username = credentials[0];
            var password = credentials[1];

            // مقایسه با اطلاعات احراز هویت ثابت
            return username == "bhUser" && password == "bhpass123";
        }

        [WebMethod]
        public string InsSmsData(int SourceNumber, string MessageText, string MobileNumber, string CreateDate)
        {
            // ابتدا بررسی می‌کنیم که آیا کاربر احراز هویت شده است یا خیر
            if (!IsAuthenticated())
            {
                return "خطا: احراز هویت ناموفق";
            }

            string result = "1";
            try
            {
                string connectionString = "Data Source=.;Initial Catalog=SmsDb;Persist Security Info=True;User ID=SmsUser; Password=sa%sa22 ;Integrated Security =true";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand cmd = new SqlCommand("SP_InsSmsData", conn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@SourceNumber", SourceNumber);
                    cmd.Parameters.AddWithValue("@MessageText", MessageText);
                    cmd.Parameters.AddWithValue("@MobileNumber", MobileNumber);
                    cmd.Parameters.AddWithValue("@CreateDate", CreateDate);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    conn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            { 
                result = ex.Message.ToString();
            }
            return result;
        }
    }
}