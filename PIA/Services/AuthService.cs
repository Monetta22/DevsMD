using MiProyecto.Database;
using MiProyecto.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MiProyecto.Services
{
    public class AuthService
    {
        private readonly DatabaseConnection _db;

        public AuthService()
        {
            _db = new DatabaseConnection();
        }

        public User? Login(string email, string password)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT id_user, name_user, lastName_user, email_user, user_type 
                                   FROM users 
                                   WHERE email_user = @email AND password_user = @password";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new User
                                {
                                    IdUser = reader.GetInt32("id_user"),
                                    NameUser = reader.GetString("name_user"),
                                    LastNameUser = reader.GetString("lastName_user"),
                                    EmailUser = reader.GetString("email_user"),
                                    UserType = reader.GetString("user_type")
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en login: {ex.Message}");
                }
            }
            return null;
        }

        public bool Register(string name, string lastName, string email, string password, string userType = "employee")
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    
                    // Verificar si el email ya existe
                    string checkQuery = "SELECT COUNT(*) FROM users WHERE email_user = @email";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@email", email);
                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            Console.WriteLine("Error: El email ya está registrado.");
                            return false;
                        }
                    }

                    string query = @"INSERT INTO users (name_user, lastName_user, email_user, password_user, user_type) 
                                   VALUES (@name, @lastName, @email, @password, @userType)";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", name);
                        cmd.Parameters.AddWithValue("@lastName", lastName);
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", password);
                        cmd.Parameters.AddWithValue("@userType", userType);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en registro: {ex.Message}");
                    return false;
                }
            }
        }
    }
}