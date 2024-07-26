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
using System.Security.Cryptography;

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

        public UserDetails LoginCheck(string Username, string password)
{
    try
    {
        using (SqlConnection connection = new SqlConnection(GetConfigurationString()))
        {
            string query = "SELECT UserID, PasswordHash, FirstName, LastName FROM Users WHERE Email = @Email AND IsActive = 1";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@Email", Username);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    int userid = reader.GetInt32(0);
                    string storedPasswordHash = reader.GetString(1);
                    string firstName = reader.GetString(2);
                    string lastName = reader.IsDBNull(3) ? "" : reader.GetString(3);

                    if (password == storedPasswordHash)
                    {
                        var accessToken = GenerateAccessToken(Username);
                        var refreshToken = GenerateRefreshToken();

                        return new UserDetails
                        {
                            id = userid,
                            name = $"{firstName} {lastName}",
                            AccessToken = accessToken,
                            RefreshToken = refreshToken
                        };
                    }
                }
            }
        }
        // If we get here, either the user wasn't found or the password was incorrect
        return null;
    }
    catch (Exception ex)
    {
        // Log the exception here
        Console.WriteLine($"An error occurred: {ex.Message}");
        return null;
    }
}

        private string GenerateAccessToken(string email)
        {
            var claims = new List<Claim>
            {
             new Claim(JwtRegisteredClaimNames.Email, email)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokendes = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15), // Short-lived access token
            SigningCredentials = creds,
            Issuer = Configuration["Jwt:Issuer"],
            Audience = Configuration["Jwt:Audience"]
        };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokendes);
            return tokenHandler.WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
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