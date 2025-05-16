using JobPortal.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace JobPortal.Repository
{
    public class EmployerRepository
    {
        private SqlConnection con;

        
        private void connection()
        {
            string conn = ConfigurationManager.ConnectionStrings["mycon"].ToString();
            con = new SqlConnection(conn);

        }
        
        public bool EmployerRegister(EmployerModel emp, HttpPostedFileBase logoUpload)
        {
            try
            {
                connection();
                using (BinaryReader binaryReader = new BinaryReader(logoUpload.InputStream))
                {
                    emp.CompanyLogo = binaryReader.ReadBytes(logoUpload.ContentLength);
                }
                emp.SetPassword(emp.Password);
                SqlCommand com = new SqlCommand("SP_CreateEmployer", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@CompanyName", emp.CompanyName);
                com.Parameters.AddWithValue("@OfficialEmail", emp.OfficialEmail);
                com.Parameters.AddWithValue("@Email", emp.Email);
                com.Parameters.AddWithValue("@ContactPhone", emp.ContactPhone);
                com.Parameters.AddWithValue("@Website", emp.Website);
                com.Parameters.AddWithValue("@Name", emp.Name);
                com.Parameters.AddWithValue("@Username", emp.Username);
                com.Parameters.AddWithValue("@Designation", emp.Designation);
                com.Parameters.AddWithValue("@CompanyLogo", emp.CompanyLogo);
                com.Parameters.AddWithValue("@Password", emp.Password);
                con.Open();
                int i = com.ExecuteNonQuery();
                return i > 0;
            }
            finally { con.Close(); }
        }
        
        public bool UpdateEmployer(EmployerModel emp, HttpPostedFileBase uploadedLogo,int employerId)
        {
            try {
                connection();
                if (uploadedLogo != null)
                {
                    using (BinaryReader binaryReader = new BinaryReader(uploadedLogo.InputStream))
                    {
                        emp.CompanyLogo = binaryReader.ReadBytes(uploadedLogo.ContentLength);
                    }
                }
                else
                {
                    emp.CompanyLogo = null;
                }

                SqlCommand com = new SqlCommand("SP_UpdateEmployer", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@EmployerID", employerId);
                com.Parameters.AddWithValue("@CompanyName", emp.CompanyName);
                com.Parameters.AddWithValue("@OfficialEmail", emp.OfficialEmail);
                com.Parameters.AddWithValue("@Email", emp.Email);
                com.Parameters.AddWithValue("@ContactPhone", emp.ContactPhone);
                com.Parameters.AddWithValue("@Website", emp.Website);
                com.Parameters.AddWithValue("@Name", emp.Name);
                com.Parameters.AddWithValue("@Designation", emp.Designation);
                com.Parameters.AddWithValue("@CompanyLogo", emp.CompanyLogo);
                con.Open();
                int i = com.ExecuteNonQuery();
                return i > 0;
            }
            finally { con.Close(); }

        }
        
        public List<EmployerModel> Employers()
        {
            try
            {
                connection();
                SqlCommand com = new SqlCommand("SP_ReadEmployer", con);
                List<EmployerModel> employers = new List<EmployerModel>();
                com.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    employers.Add(new EmployerModel()
                    {
                        EmployerID = Convert.ToInt32(dr["EmployerID"]),
                        CompanyName = Convert.ToString(dr["CompanyName"]),
                        OfficialEmail = Convert.ToString(dr["OfficialEmail"]),
                        Email = Convert.ToString(dr["Email"]),
                        ContactPhone = Convert.ToString(dr["ContactPhone"]),
                        Website = Convert.ToString(dr["Website"]),
                        Name = Convert.ToString(dr["Name"]),
                        Designation = Convert.ToString(dr["Designation"]),
                        CompanyLogo = (byte[])dr["CompanyLogo"],
                        Status = Convert.ToString(dr["Status"]),
                        Username = Convert.ToString(dr["Username"])
                    });
                }

                return employers;
            }
            finally
            {
                con.Close();
            }
        }

       
        public bool AddJobVacancy(JobVacancy obj, int employerId)
        {
            try
            {
                connection();
                SqlCommand com = new SqlCommand("SP_CreateJobVacancy", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@EmployerID", employerId);
                com.Parameters.AddWithValue("@JobTitle", obj.JobTitle);
                com.Parameters.AddWithValue("@Description", obj.Description);
                com.Parameters.AddWithValue("@CategoryID", obj.CategoryID);
                com.Parameters.AddWithValue("@Location", obj.Location);
                com.Parameters.AddWithValue("@Salary", obj.Salary);
                com.Parameters.AddWithValue("@EmploymentType", obj.EmploymentType);
                com.Parameters.AddWithValue("@ApplicationDeadline", obj.ApplicationDeadline);
                con.Open();
                int i = com.ExecuteNonQuery();
                return i > 0;
            }
            finally { con.Close(); }
        }
        
        public bool UpdateJobVacancy(JobVacancy obj)
        {
            try
            {
                connection();
                SqlCommand com = new SqlCommand("SP_UpdateJobVacancy", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@VacancyID", obj.VacancyID);
                com.Parameters.AddWithValue("@JobTitle", obj.JobTitle);
                com.Parameters.AddWithValue("@Description", obj.Description);
                com.Parameters.AddWithValue("@CategoryID", obj.CategoryID);
                com.Parameters.AddWithValue("@Location", obj.Location);
                com.Parameters.AddWithValue("@Salary", obj.Salary);
                com.Parameters.AddWithValue("@EmploymentType", obj.EmploymentType);
                com.Parameters.AddWithValue("@ApplicationDeadline", obj.ApplicationDeadline);
                com.Parameters.AddWithValue("@IsPublished", obj.IsPublished);
                con.Open();
                int i = com.ExecuteNonQuery();
                return i > 0;
            }
            finally { con.Close(); }
        }
        
        public bool JobApplicationApprove(int id)
        {
            try
            {
                connection();
                SqlCommand com = new SqlCommand("SP_JobApplicationApprove", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ApplicationId", id);

                con.Open();
                int i = com.ExecuteNonQuery();
                return i >= 0;
            }
            finally { con.Close(); }
        }
       
        public bool JobApplicationReject(int id)
        {
            try
            {
                connection();
                SqlCommand com = new SqlCommand("SP_JobApplicationReject", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ApplicationId", id);
                con.Open();
                int i = com.ExecuteNonQuery();
                return i >= 0;
            }
            finally { con.Close(); }
        }
        
        public bool JobApplicationRead(int id)
        {
            try { 
                connection();
                SqlCommand com = new SqlCommand("SP_JobApplicationRead", con);
                com.CommandType =CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@ApplicationId", id);
                con.Open();
                int i = com.ExecuteNonQuery();
                return i >= 0;
            }
            finally { con.Close(); }    
        }
       
        public List<JobApplication> GetJobApplications(int JobId)
        {
            try
            {
                connection();
                SqlCommand com = new SqlCommand("SP_ReadJobApplication", con);
                List<JobApplication> jobApplications = new List<JobApplication>();
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@JobId", JobId);
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                con.Open();
                da.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    jobApplications.Add(new JobApplication()
                    {
                        JobApplicationID = Convert.ToInt32(dr["ApplicationID"]),
                        JobId = Convert.ToInt32(dr["JobId"]),
                        SeekerId = Convert.ToInt32(dr["SeekerID"]),
                        ApplicationDate = Convert.ToDateTime(dr["ApplicationDate"]),
                        Status = Convert.ToString(dr["Status"]),
                        SeekerName = Convert.ToString(dr["FirstName"])
                    });
                }
                return jobApplications;
            }finally { con.Close(); }
        }

       
        public bool ChangePassword(string oldPassword, string newPassword, int employerId)
        {
            try
            {
                connection();
                SqlCommand com = new SqlCommand("SP_ReadEmployerPassword", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@EmployerId", employerId);
                con.Open();
                string password = Convert.ToString(com.ExecuteScalar());
                if (BCrypt.Net.BCrypt.Verify(oldPassword, password))
                {
                    com = new SqlCommand("SP_ChangeEmployerPassword", con);
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@EmployerId",employerId);
                    newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    com.Parameters.AddWithValue("@NewPassword", newPassword);
                    int i = com.ExecuteNonQuery();
                    return i > 0;
                }
                else
                    return false;
            }
            finally
            {
                con.Close();
            }
        }
       
        public List<ChatList> ChatList(int employerId)
        {
            try
            {
                connection();
                List<ChatList> chats = new List<ChatList>();
                SqlCommand com = new SqlCommand("SP_ChatListEmployer", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("@EmployerID", employerId);
                con.Open();
                SqlDataAdapter da = new SqlDataAdapter(com);
                DataTable dt = new DataTable();
                da.Fill(dt);
                foreach (DataRow dr in dt.Rows)
                {
                    chats.Add(new ChatList
                    {
                        SeekerID = Convert.ToInt32(dr["SeekerID"]),
                        EmployerID = Convert.ToInt32(dr["EmployerID"]),
                        ChatID = Convert.ToInt32(dr["ChatID"]),
                        SeekerName = dr["SeekerName"].ToString(),
                        CompanyName = dr["CompanyName"].ToString(),
                    });
                }
                return chats;
            }
            finally { con.Close(); }
        }
    }
}