using MySql.Data.MySqlClient;

namespace PIA.Database
{
    public class DatabaseConnection
    {
        private string connectionString = "server=localhost;user=root;password=GearsofWar_3;database=metodologias;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}