using MySql.Data.MySqlClient;
using PIA.Database;
using static PIA.EmailSender.EmailSender;

namespace PIA.UserCruds
{
    public static class UserCruds
    {

        public static void LogIn()
        {
            string email_user;
            string password_user;
            Console.Write("Correo Electronico: ");
            email_user = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            password_user = Console.ReadLine()!;

            DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT name_user, lastName_user FROM users WHERE email_user = @email_user AND password_user = @password_user";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@email_user", email_user);
                        cmd.Parameters.AddWithValue("@password_user", password_user);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine($" Bienvenido {reader["name_user"]} {reader["lastName_user"]}");
                            }
                            else
                            {
                                Console.WriteLine("Error. Usuario no encontrado. Intente de nuevo.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error,. Error al iniciar sesión: " + ex.Message);
                }
            }
        }
        public static void SignUp()
        {
            Console.Write("Nombre: ");
            string name_input = Console.ReadLine()!.ToUpper();
            Console.Write("Apellidos: ");
            string lastName_input = Console.ReadLine()!.ToUpper();
            Console.Write("Correo Electronico: ");
            string email_input = Console.ReadLine()!;
            Console.Write("Contraseña: ");
            string password_input = Console.ReadLine()!;

            // meter aqui el EmailSender para mejorar la experiencia del usuario, despues de ingresar sus datos
            SendEmail(); // le falta desarrollarse
            DatabaseConnection db = new DatabaseConnection();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO users (name_user, lastName_user, email_user, password_user) VALUES (@name_input, @lastName_input, @email_input, @password_input)";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name_input", name_input);
                        cmd.Parameters.AddWithValue("@lastName_input", lastName_input);
                        cmd.Parameters.AddWithValue("@email_input", email_input);
                        cmd.Parameters.AddWithValue("@password_input", password_input);

                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Usuario registrado exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("Error. No se pudo registrar el usuario.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error al registrar usuario: " + ex.Message);
                }
            }
        }

    }
}
