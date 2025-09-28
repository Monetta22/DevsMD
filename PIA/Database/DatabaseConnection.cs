using MySql.Data.MySqlClient;

namespace MiProyecto.Database
{
    public class DatabaseConnection
    {
        private readonly string connectionString;

        public DatabaseConnection()
        {
            // CONFIGURACI�N PARA XAMPP (sin contrase�a):
            //connectionString = "Server=localhost;Database=pos_system;Uid=root;Pwd=;";
            
            //SI TU MYSQL TIENE CONTRASE�A, descomenta y usa esta l�nea:
            connectionString = "Server=localhost;Database=pos_system;Uid=root;Pwd=GearsofWar_3;";
            
            // PARA OTROS SERVIDORES MySQL, ajusta seg�n necesites:
            // connectionString = "Server=tu_servidor;Database=pos_system;Uid=tu_usuario;Pwd=tu_contrase�a;";
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (var connection = GetConnection())
                {
                    connection.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Para debugging, puedes descomentar esta l�nea para ver el error espec�fico:
                // Console.WriteLine($"Error de conexi�n: {ex.Message}");
                return false;
            }
        }
    }
}