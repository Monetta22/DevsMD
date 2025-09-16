using MiProyecto.Database;
using MySql.Data.MySqlClient;

namespace DevProjects
{
    class Program
    {
        static void Main(string[] args)
        {
            CRUD();
            // login(); 
        }
        static void login()
        {
            int option_user;
            string email_user;
            string password_user;

            Console.WriteLine("------ BIENVENIDO ------");
            Console.WriteLine("\n---- 1. INICIA SESION ----");
            Console.WriteLine("\n---- 2. REGISTRATE ----");

            while (true)
            {
                Console.Write("Seleccione una opción: ");
                if (int.TryParse(Console.ReadLine(), out option_user))
                {
                    if (option_user == 1 || option_user == 2)
                    {
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error. Ingrese una opcion valida. Intente de nuevo.");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Error. Debes ingresar un dato numerico. Intenta de nuevo");
                }
            }

            if (option_user == 1) // INICIO DE SESIÓN
            {
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
            else if (option_user == 2) // REGISTRO
            {
                Console.Write("Nombre: ");
                string name_input = Console.ReadLine()!.ToUpper();
                Console.Write("Apellidos: ");
                string lastName_input = Console.ReadLine()!.ToUpper();
                Console.Write("Correo Electronico: ");
                string email_input = Console.ReadLine()!;
                Console.Write("Contraseña: ");
                string password_input = Console.ReadLine()!;

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
        static void CRUD()
        {
            int option_user;
            Console.WriteLine("------ MENU PRODUCTO ------");
            Console.WriteLine("1. Registrar Producto");
            Console.WriteLine("2. Seleccionar Producto");
            Console.WriteLine("3. Actualizar Producto");
            Console.WriteLine("4. Eliminar Producto");
            Console.WriteLine("0. Salir");
            while (true)
            {
                Console.Write("\nSeleccione una opcion: ");
                if (int.TryParse(Console.ReadLine(), out option_user))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error. Ingrese un dato numerico. Intente de nuevo");
                }
            }
            if (option_user == 1)
            {
                CreateProduct();
            }
        }
        static void CreateProduct()
        {
            int id_product;
            string name_product;
            string category_product;
            int purchasePrice_product;
            int salePrice_product;
            Console.WriteLine("------- Producto ------");
            while (true) // logica del ID con validaciones
            {
                Console.Write("Ingrese el ID del Producto: ");
                if (int.TryParse(Console.ReadLine(), out id_product))
                {
                    DatabaseConnection database = new DatabaseConnection();
                    using (MySqlConnection conn = database.GetConnection())
                    {
                        try
                        {
                            conn.Open();
                            string query = "SELECT id_product FROM product WHERE id_product = @id_product";
                            using (MySqlCommand cmd = new MySqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@id_product", id_product);
                                using (MySqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        Console.WriteLine($"Error. El producto ya existe. Intente de nuevo");
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error. Ingrese un dato numerico. Intente de nuevo.");
                }
            }
            do // Loigca del Nombre
            {
                Console.Write("Ingrese el Nombre del Producto: ");
                name_product = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(name_product) || !name_product.Replace(" ", "").All(char.IsLetter))
                {
                    Console.WriteLine("Error. El campo solo acepta letras. Intenta de nuevo.");
                    name_product = null!;
                }
            } while (name_product == null);
            do // Loigca de la Categoria
            {
                Console.Write("Ingrese la Categoria del Producto: ");
                category_product = Console.ReadLine()!;
                if (string.IsNullOrWhiteSpace(category_product) || !category_product.Replace(" ", "").All(char.IsLetter))
                {
                    Console.WriteLine("Error. El campo solo acepta letras. Intenta de nuevo.");
                    category_product = null!;
                }
            } while (category_product == null);
            while (true) // logica del precio de compra
            {
                Console.Write("Ingrese el Precio de Compra del Producto: ");
                if (int.TryParse(Console.ReadLine(), out purchasePrice_product))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error. Ingrese un dato numerico. Intente de nuevo.");
                }
            }
            while (true) //logica del precio de venta
            {
                Console.Write("Ingrese el Precio de Venta del Producto: ");
                if (int.TryParse(Console.ReadLine(), out salePrice_product))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Error. Ingrese un dato numerico. Intente de nuevo.");
                }
            }
            DatabaseConnection db = new DatabaseConnection(); //INSERT PRODUCT
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO product (id_product, name_product, categoria_product, precioCompra_product, precioVenta_product) VALUES (@id_product, @name_product, @category_product, @purchasePrice_product, @salePrice_product);";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id_product", id_product);
                        cmd.Parameters.AddWithValue("@name_product", name_product);
                        cmd.Parameters.AddWithValue("@category_product", category_product);
                        cmd.Parameters.AddWithValue("@purchasePrice_product", purchasePrice_product);
                        cmd.Parameters.AddWithValue("@salePrice_product", salePrice_product);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            Console.WriteLine("Producto registrado exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("Error. No se pudo registrar el producto.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error. {ex}");
                }
            }
        }
    }
}