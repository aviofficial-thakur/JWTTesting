using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Xml.Linq;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JWTAPI.Models;

namespace JWTAPI.BussinessLogic
{
    public class UserLists
    {
        SqlConnection con = null;
        SqlCommand cmd = null;

        public IConfiguration Configuration {get;set;}

        private string GetConfigurationString()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            Configuration = builder.Build();
            return Configuration.GetConnectionString("AppConn");
            
        }

        public string LoginCheck (string Username , string password)
        {
            string msg = string.Empty;
            if(string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(password))
            {
                return msg = "Please provide both email and password.";
            }

            try
            {
                using(SqlConnection connection = new SqlConnection(GetConfigurationString()))
                {
                    string query = "SELECT UserID, PasswordHash, FirstName, LastName FROM Users WHERE Email = @Email AND IsActive = 1";
                    using(SqlCommand command = new SqlCommand(query,connection))
                    {
                        command.Parameters.AddWithValue("@Email", Username);
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if(reader.Read())
                        {
                            int userid = reader.GetInt32(0);
                            string storedPasswordHash = reader.GetString(1);
                            string firstName = reader.GetString(2);
                            string lastName = reader.IsDBNull(3) ? "" : reader.GetString(3); 

                            if(password == storedPasswordHash)
                            {
                            //    var claims = new[] {
                            //    new Claim(JwtRegisteredClaimNames.Sub, Configuration["Jwt:Subject"]),
                            //    new Claim("UserName", Username)
                            //    };

                            //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
                            //    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            //    var token = new JwtSecurityToken(
                            //    Configuration["Jwt:Issuer"],
                            //    Configuration["Jwt:Audience"],
                            //    claims,
                            //    expires: DateTime.UtcNow.AddMinutes(5),
                            //    signingCredentials: signIn);

                            //    var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                            //    return jwtToken;

                            var claims = new List<Claim>
                            {
                                new Claim(JwtRegisteredClaimNames.Email, Username)
                            };
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
                            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                            var tokendes = new SecurityTokenDescriptor
                            {
                                Subject = new ClaimsIdentity(claims),
                                Expires = DateTime.UtcNow.AddMinutes(5),
                                SigningCredentials = creds,
                                Issuer = Configuration["Jwt:Issuer"],
                                Audience = Configuration["Jwt:Audience"]

                            };
                            var tokenHandler = new JwtSecurityTokenHandler();

                            var token = tokenHandler.CreateToken(tokendes);
                            return tokenHandler.WriteToken(token);

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return msg = "An error occurred while processing your request.";
            }

            return msg = "Invalid email or password.";
        }

            public List<UserDetails> Get_All_User_Detail()
            {
                List<UserDetails> lst = new List<UserDetails>();
                using (con = new SqlConnection(GetConfigurationString()))
            {
                cmd = con.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Usp_AddUser";
                cmd.Parameters.AddWithValue("@Flag", 2);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
            {
                UserDetails data = new UserDetails();
                data.id = Convert.ToInt32(reader["UserID"]);
                data.name = reader["FullName"].ToString();
                lst.Add(data);
            }
                con.Close();
            }
            return lst;
    }
    }
}