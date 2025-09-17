using MySql.Data.MySqlClient;
using PIA.Database;

namespace PIA.ProductCruds
{
    public static class ProductCruds
    {
        public static void SelectProduct()
        {

        }
        public static void CreateProduct()
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

        public static void UpdateProduct()
        {

        }

        public static void DeleteProduct()
        {

        }
    }

}
