using MySql.Data.MySqlClient;

namespace MiProyecto.Database
{
    public class DatabaseConnection
    {
        private readonly string connectionString;

        public DatabaseConnection()
        {
            // CONFIGURACIÓN PARA XAMPP (sin contraseña):
            //connectionString = "Server=localhost;Database=pos_system;Uid=root;Pwd=;";
            
            //SI TU MYSQL TIENE CONTRASEÑA, descomenta y usa esta línea:
            connectionString = "Server=localhost;Database=pos_system;Uid=root;Pwd=GearsofWar_3;";
            
            // PARA OTROS SERVIDORES MySQL, ajusta según necesites:
            // connectionString = "Server=tu_servidor;Database=pos_system;Uid=tu_usuario;Pwd=tu_contraseña;";
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
                // Para debugging, puedes descomentar esta línea para ver el error específico:
                // Console.WriteLine($"Error de conexión: {ex.Message}");
                return false;
            }
        }
    }
}