using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAdsAuth.Data
{
    public class SimpleAdsRepository
    {
        private string _connectionString;
        public SimpleAdsRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void NewAd(Ad ad)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Ads (Title,Description,Date,PhoneNumber,UserId) " +
                                    "VALUES (@title, @description, @date,@phoneNumber, @userId)";
            command.Parameters.AddWithValue("@title", ad.Title);
            command.Parameters.AddWithValue("@description", ad.Description);
            command.Parameters.AddWithValue("@date", ad.Date);
            command.Parameters.AddWithValue("@phoneNumber", ad.PhoneNumber);
            command.Parameters.AddWithValue("@userId", ad.UserId);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public List<Ad> GetAllAds()
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT a.*, u.Name FROM Ads a " +
                                    "JOIN Users u " +
                                    "ON a.UserId = u.Id " +
                                    "ORDER BY a.Date DESC";
            connection.Open();

            var ads = new List<Ad>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    UserId = (int)reader["UserId"],
                    UserName = (string)reader["Name"]
                });
            }
            return ads;
        }
        public void AddUser(User user, string password)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Name, Email, PasswordHash)" +
                                    "VALUES (@name, @email, @hash)";
            
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            command.Parameters.AddWithValue("@name", user.Name);
            command.Parameters.AddWithValue("@email", user.Email);
            command.Parameters.AddWithValue("@hash", passwordHash);
            connection.Open();
            command.ExecuteNonQuery();
        }
        public User Login(string email, string password)
        {
            var user = GetUserByEmail(email);
            if(user == null)
            {
                return null;
            }

            var isValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isValid)
            {
                return null;
            }
            return user;
        }
        public User GetUserByEmail(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Email = @email";
            command.Parameters.AddWithValue("@email", email);
            connection.Open();
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return null;
            }
            return new User
            {
                Id = (int)reader["Id"],
                Name = (string)reader["Name"],
                Email = (string)reader["Email"],
                PasswordHash = (string)reader["PasswordHash"]
            };
        }
        public List<Ad> GetAdsForUser(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT a.*, u.Name FROM Ads a " +
                                    "JOIN Users u " +
                                    "ON u.Id = a.UserId " +
                                    "WHERE a.UserId = @id";
            command.Parameters.AddWithValue("@id", id);
            connection.Open();
            var ads = new List<Ad>();
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                ads.Add(new()
                {
                    Id = (int)reader["Id"],
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    Date = (DateTime)reader["Date"],
                    PhoneNumber = (string)reader["PhoneNumber"],
                    UserId = (int)reader["UserId"],
                    UserName = (string)reader["Name"]
                });
            }
            return ads;
        }
    }
}
