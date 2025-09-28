using MiProyecto.Database;
using System;

namespace DevProjects
{
    class TestConnection
    {
        static void TestDatabaseConnection()
        {
            Console.WriteLine("=== PRUEBA DE CONEXIÓN MySQL ===");
            
            DatabaseConnection db = new DatabaseConnection();
            
            Console.WriteLine("Intentando conectar a la base de datos...");
            
            if (db.TestConnection())
            {
                Console.WriteLine("? ¡Conexión exitosa!");
                Console.WriteLine("La base de datos está funcionando correctamente.");
            }
            else
            {
                Console.WriteLine("? Error de conexión.");
                Console.WriteLine("\nPosibles causas:");
                Console.WriteLine("1. MySQL no está iniciado");
                Console.WriteLine("2. La base de datos 'pos_system' no existe");
                Console.WriteLine("3. Credenciales incorrectas");
                Console.WriteLine("4. Puerto MySQL bloqueado");
                
                Console.WriteLine("\nSoluciones:");
                Console.WriteLine("• Verifica que MySQL esté running");
                Console.WriteLine("• Ejecuta el script database_script.sql");
                Console.WriteLine("• Revisa la cadena de conexión en DatabaseConnection.cs");
            }
            
            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}