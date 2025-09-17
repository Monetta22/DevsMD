using static PIA.ProductCruds.ProductCruds;
using static PIA.UserCruds.UserCruds;


namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            //CRUD();
            UserAccess();
        }
        static void UserAccess()
        {
            int option_user;
            bool isRunning = true;
            do
            {
                Console.WriteLine("------ BIENVENIDO ------");
                Console.WriteLine("\n---- 1. INICIA SESION ----");
                Console.WriteLine("\n---- 2. REGISTRATE ----");
                Console.WriteLine("\n---- 3. SALIR ----");
                Console.WriteLine("Seleccione una opción: ");
                option_user = Convert.ToInt32(Console.ReadLine());

                if (option_user == 1) LogIn();
                else if (option_user == 2) SignUp();
                else if (option_user == 3) isRunning = false;
                else Console.WriteLine("Invalid option\n");
            }
            while (isRunning);
        }
        static void CRUD()
        {
            int option_user;
            bool isRunning = true;
            do
            {
                Console.WriteLine("------ MENU PRODUCTO ------");
                Console.WriteLine("1. Registrar Producto");
                Console.WriteLine("2. Seleccionar Producto");
                Console.WriteLine("3. Actualizar Producto");
                Console.WriteLine("4. Eliminar Producto");
                Console.WriteLine("5. Salir");
                Console.WriteLine("\nSeleccione una opcion: ");
                option_user = Convert.ToInt32(Console.ReadLine()); // meter validacion opcional si no mete un numero

                if (option_user == 1) CreateProduct();
                else if (option_user == 2) SelectProduct();
                else if (option_user == 3) UpdateProduct();
                else if (option_user == 4) DeleteProduct();
                else if (option_user == 5) isRunning = false;
                else Console.WriteLine("Invalid option\n");
            }
            while (isRunning);
        }
    }
}