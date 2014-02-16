using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace Tesla
{
    class DatabaseQuery
    {
        private MySqlConnection con;
        private MySqlDataAdapter da;
        private MySqlCommand cmd;

        public DatabaseQuery()  // constructor
        {
            da = new MySqlDataAdapter();
            cmd = new MySqlCommand();
        }

        public void dbfetch(string Query,DataSet ds)
        {
            try
            {
                con = new MySqlConnection(GetConnectionString());
                con.Open();
                da = new MySqlDataAdapter(Query, con);
                da.Fill(ds);
                con.Close();
            }
            catch (MySqlException m)
            {
                Console.WriteLine(m);
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
        }

        public string GetValue(string Query)
        {
            string result;
            con = new MySqlConnection(GetConnectionString());
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = Query;
            result =(string) cmd.ExecuteScalar();
            con.Close();
            return (result);
        }

        public void UpdateValue(string Query)
        {
            con = new MySqlConnection(GetConnectionString());
            con.Open();
            cmd.Connection = con;
            cmd.CommandText = Query;
            cmd.ExecuteNonQuery();
            con.Close();
        }
       
        public void UpdateData(DataSet ds)
        {
            try
            {
                MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
                con = new MySqlConnection(GetConnectionString());
                con.Open();
                da.UpdateBatchSize = 1000;
                da.Update(ds);
                con.Close();
            }
            catch (MySqlException m)
            {
                Console.WriteLine(m);
            }
            catch (Exception e1)
            {
                Console.WriteLine(e1);
            }
        }

        public User Password_verify(string username, string password)
        {
            User u = new User();
            
            String Stored_Procedure = "password_verify";
            con = new MySqlConnection(GetConnectionString());
            cmd = new MySqlCommand(Stored_Procedure, con);
            cmd.CommandType = CommandType.StoredProcedure;
            
            MySqlParameter user = new MySqlParameter("@username", MySqlDbType.VarChar);
            user.Direction = ParameterDirection.Input;
            user.Value = username;

            MySqlParameter pass = new MySqlParameter("@password", MySqlDbType.VarChar);
            pass.Direction = ParameterDirection.Input;
            pass.Value = password;

            MySqlParameter u_name = new MySqlParameter("@name", MySqlDbType.VarChar);
            u_name.Direction = ParameterDirection.Output;

            MySqlParameter u_role = new MySqlParameter("@role", MySqlDbType.Int32);
            u_role.Direction = ParameterDirection.Output;

            MySqlParameter u_userid = new MySqlParameter("@userid", MySqlDbType.Int32);
            u_userid.Direction = ParameterDirection.Output;

            MySqlParameter u_eventname = new MySqlParameter("@eventname", MySqlDbType.VarChar);
            u_eventname.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(user);
            cmd.Parameters.Add(pass);
            cmd.Parameters.Add(u_name);
            cmd.Parameters.Add(u_role);
            cmd.Parameters.Add(u_userid);
            cmd.Parameters.Add(u_eventname);

            try
            {
                con.Open();
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Exception Occured:"+ e.Message);
            }
            con.Close();
            u.name = u_name.Value.ToString(); ;
            u.role = Convert.ToInt32(u_role.Value);
            u.UserId = Convert.ToInt32(u_userid.Value);
            u.eventname = u_eventname.Value.ToString();

            return (u);

        }

        public Object[] CreateUser(string Fname, string Lname, string mobile,string emailid, int role)
        {
            object[] data = new object[3];
            String Stored_Procedure = "CreateUser";
            
            con = new MySqlConnection(GetConnectionString());
            cmd = new MySqlCommand(Stored_Procedure, con);
            cmd.CommandType = CommandType.StoredProcedure;

            MySqlParameter fname = new MySqlParameter("@firstname", MySqlDbType.VarChar);
            fname.Direction = ParameterDirection.Input;
            fname.Value = Fname;

            MySqlParameter lname = new MySqlParameter("@lastname", MySqlDbType.VarChar);
            lname.Direction = ParameterDirection.Input;
            lname.Value = Lname;

            MySqlParameter mob = new MySqlParameter("@mobile", MySqlDbType.VarChar);
            mob.Direction = ParameterDirection.Input;
            mob.Value = mobile;

            MySqlParameter email = new MySqlParameter("@emailid", MySqlDbType.VarChar);
            email.Direction = ParameterDirection.Input;
            email.Value = emailid;

            MySqlParameter u_role = new MySqlParameter("@role", MySqlDbType.Int32);
            u_role.Direction = ParameterDirection.Input;
            u_role.Value = role;


            MySqlParameter status = new MySqlParameter("@status", MySqlDbType.Bit);
            status.Direction = ParameterDirection.Output;

            MySqlParameter username = new MySqlParameter("@username", MySqlDbType.VarChar);
            username.Direction = ParameterDirection.Output;


            MySqlParameter password = new MySqlParameter("@pass", MySqlDbType.VarChar);
            password.Direction = ParameterDirection.Output;

            cmd.Parameters.Add(fname);
            cmd.Parameters.Add(lname);
            cmd.Parameters.Add(mob);
            cmd.Parameters.Add(email);
            cmd.Parameters.Add(u_role);
            cmd.Parameters.Add(status);
            cmd.Parameters.Add(username);
            cmd.Parameters.Add(password);

            con.Open();
            cmd.ExecuteNonQuery();

            con.Close();
            data[0] = status.Value;
            data[1] = username.Value;
            data[2] = password.Value;

            return (data);

        }
        
        private string GetConnectionString()
        {
            try
            {
                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings["MysqlConnect"];
                return (setting.ConnectionString);
            }
            catch (ConfigurationErrorsException e)
            {
                Console.Write(e);
            }
            return (null);
        }

    }
}
