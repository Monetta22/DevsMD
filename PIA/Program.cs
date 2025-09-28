using MiProyecto.Database;
using MiProyecto.Models;
using MiProyecto.Services;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DevProjects
{
    class Program
    {
        private static User? currentUser = null;
        private static AuthService authService = new AuthService();
        private static ProductService productService = new ProductService();
        private static SaleService saleService = new SaleService();
        private static ReportService reportService = new ReportService();

        static void Main(string[] args)
        {
            Console.WriteLine("=== SISTEMA DE PUNTO DE VENTA ===");
            
            // Verificar conexión a la base de datos
            DatabaseConnection db = new DatabaseConnection();
            if (!db.TestConnection())
            {
                Console.WriteLine("Error: No se pudo conectar a la base de datos.");
                Console.WriteLine("Presione cualquier tecla para salir...");
                Console.ReadKey();
                return;
            }

            // Proceso de login
            if (!Login())
            {
                Console.WriteLine("No se pudo iniciar sesión. Cerrando aplicación...");
                return;
            }

            // Menú principal
            MainMenu();
        }

        static bool Login()
        {
            int attempts = 0;
            const int maxAttempts = 3;

            while (attempts < maxAttempts)
            {
                Console.Clear();
                Console.WriteLine("=== BIENVENIDO AL SISTEMA DE PUNTO DE VENTA ===");
                Console.WriteLine("1. Iniciar Sesión");
                Console.WriteLine("2. Registrarse");
                Console.WriteLine("0. Salir");
                
                Console.Write("\nSeleccione una opción: ");
                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1:
                        if (LoginUser())
                            return true;
                        attempts++;
                        break;
                    case 2:
                        RegisterUser();
                        break;
                    case 0:
                        return false;
                    default:
                        Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                        Console.ReadKey();
                        break;
                }
            }

            Console.WriteLine($"Se han agotado los {maxAttempts} intentos de login.");
            return false;
        }

        static bool LoginUser()
        {
            Console.Clear();
            Console.WriteLine("=== INICIAR SESIÓN ===");
            
            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";
            
            Console.Write("Contraseña: ");
            string password = Console.ReadLine() ?? "";

            currentUser = authService.Login(email, password);
            
            if (currentUser != null)
            {
                Console.WriteLine($"\n¡Bienvenido {currentUser.NameUser} {currentUser.LastNameUser}!");
                Console.WriteLine($"Tipo de usuario: {currentUser.UserType}");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.WriteLine("Email o contraseña incorrectos.");
                Console.WriteLine("Presione cualquier tecla para continuar...");
                Console.ReadKey();
                return false;
            }
        }

        static void RegisterUser()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRO DE USUARIO ===");
            
            Console.Write("Nombre: ");
            string name = Console.ReadLine()?.ToUpper() ?? "";
            
            Console.Write("Apellido: ");
            string lastName = Console.ReadLine()?.ToUpper() ?? "";
            
            Console.Write("Email: ");
            string email = Console.ReadLine() ?? "";
            
            Console.Write("Contraseña: ");
            string password = Console.ReadLine() ?? "";

            string userType = "employee";
            if (currentUser == null) // Si no hay usuario logueado, el primero será admin
            {
                Console.WriteLine("¿Es administrador? (s/n): ");
                string isAdmin = Console.ReadLine()?.ToLower() ?? "";
                if (isAdmin == "s" || isAdmin == "si")
                {
                    userType = "admin";
                }
            }

            if (authService.Register(name, lastName, email, password, userType))
            {
                Console.WriteLine("Usuario registrado exitosamente.");
            }
            else
            {
                Console.WriteLine("Error al registrar usuario.");
            }
            
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void MainMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== PUNTO DE VENTA - {currentUser?.NameUser} {currentUser?.LastNameUser} ===");
                Console.WriteLine("1. Realizar Venta");
                Console.WriteLine("2. Gestión de Productos");
                Console.WriteLine("3. Reportes");
                Console.WriteLine("4. Configuración");
                Console.WriteLine("0. Cerrar Sesión");
                
                Console.Write("\nSeleccione una opción: ");
                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1:
                        SalesMenu();
                        break;
                    case 2:
                        ProductsMenu();
                        break;
                    case 3:
                        ReportsMenu();
                        break;
                    case 4:
                        if (currentUser?.UserType == "admin")
                            ConfigurationMenu();
                        else
                            Console.WriteLine("No tiene permisos para acceder a configuración.");
                        Console.ReadKey();
                        break;
                    case 0:
                        currentUser = null;
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void SalesMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MÓDULO DE VENTAS ===");
                Console.WriteLine("1. Nueva Venta");
                Console.WriteLine("2. Ver Ventas del Día");
                Console.WriteLine("3. Buscar Venta por ID");
                Console.WriteLine("0. Volver");
                
                Console.Write("\nSeleccione una opción: ");
                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1:
                        CreateNewSale();
                        break;
                    case 2:
                        ViewTodaySales();
                        break;
                    case 3:
                        SearchSaleById();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateNewSale()
        {
            Console.Clear();
            Console.WriteLine("=== NUEVA VENTA ===");
            
            var sale = new Sale
            {
                IdUser = currentUser!.IdUser,
                IdCustomer = 1, // Cliente general por defecto
                SaleDate = DateTime.Now,
                StatusSale = "completed"
            };

            var cart = new List<SaleDetail>();
            decimal total = 0;

            while (true)
            {
                Console.WriteLine($"\nTotal actual: ${total:F2}");
                Console.WriteLine("\n1. Agregar producto");
                Console.WriteLine("2. Ver carrito");
                Console.WriteLine("3. Eliminar producto del carrito");
                Console.WriteLine("4. Finalizar venta");
                Console.WriteLine("0. Cancelar venta");
                
                Console.Write("Opción: ");
                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Opción inválida.");
                    continue;
                }

                switch (option)
                {
                    case 1:
                        AddProductToCart(cart, ref total);
                        break;
                    case 2:
                        ShowCart(cart);
                        break;
                    case 3:
                        RemoveProductFromCart(cart, ref total);
                        break;
                    case 4:
                        if (cart.Count > 0)
                        {
                            if (FinalizeSale(sale, cart, total))
                                return;
                        }
                        else
                        {
                            Console.WriteLine("El carrito está vacío.");
                        }
                        break;
                    case 0:
                        Console.WriteLine("Venta cancelada.");
                        Console.ReadKey();
                        return;
                    default:
                        Console.WriteLine("Opción inválida.");
                        break;
                }
            }
        }

        static void AddProductToCart(List<SaleDetail> cart, ref decimal total)
        {
            Console.Write("ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int productId))
            {
                Console.WriteLine("ID inválido.");
                return;
            }

            var product = productService.GetProductById(productId);
            if (product == null)
            {
                Console.WriteLine("Producto no encontrado.");
                return;
            }

            Console.WriteLine($"Producto: {product.NameProduct}");
            Console.WriteLine($"Precio: ${product.PrecioVentaProduct:F2}");
            Console.WriteLine($"Stock disponible: {product.StockProduct}");
            
            Console.Write("Cantidad: ");
            if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0)
            {
                Console.WriteLine("Cantidad inválida.");
                return;
            }

            if (quantity > product.StockProduct)
            {
                Console.WriteLine("Stock insuficiente.");
                return;
            }

            var existingItem = cart.FirstOrDefault(c => c.IdProduct == productId);
            if (existingItem != null)
            {
                if (existingItem.Quantity + quantity > product.StockProduct)
                {
                    Console.WriteLine("Stock insuficiente para la cantidad total.");
                    return;
                }
                
                total -= existingItem.TotalPrice;
                existingItem.Quantity += quantity;
                existingItem.TotalPrice = existingItem.Quantity * existingItem.UnitPrice;
                total += existingItem.TotalPrice;
            }
            else
            {
                var detail = new SaleDetail
                {
                    IdProduct = productId,
                    Quantity = quantity,
                    UnitPrice = product.PrecioVentaProduct,
                    TotalPrice = quantity * product.PrecioVentaProduct,
                    ProductName = product.NameProduct
                };
                cart.Add(detail);
                total += detail.TotalPrice;
            }

            Console.WriteLine("Producto agregado al carrito.");
        }

        static void ShowCart(List<SaleDetail> cart)
        {
            Console.Clear();
            Console.WriteLine("=== CARRITO DE COMPRAS ===");
            
            if (cart.Count == 0)
            {
                Console.WriteLine("El carrito está vacío.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"{"Producto",-20} {"Cantidad",-10} {"Precio Unit",-12} {"Subtotal",-10}");
            Console.WriteLine(new string('-', 55));

            decimal total = 0;
            foreach (var item in cart)
            {
                Console.WriteLine($"{item.ProductName,-20} {item.Quantity,-10} ${item.UnitPrice,-11:F2} ${item.TotalPrice,-9:F2}");
                total += item.TotalPrice;
            }

            Console.WriteLine(new string('-', 55));
            Console.WriteLine($"{"TOTAL:",-42} ${total,-9:F2}");
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void RemoveProductFromCart(List<SaleDetail> cart, ref decimal total)
        {
            if (cart.Count == 0)
            {
                Console.WriteLine("El carrito está vacío.");
                return;
            }

            Console.WriteLine("Productos en el carrito:");
            for (int i = 0; i < cart.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {cart[i].ProductName} (Cantidad: {cart[i].Quantity})");
            }

            Console.Write("Seleccione el número del producto a eliminar: ");
            if (int.TryParse(Console.ReadLine(), out int index) && index > 0 && index <= cart.Count)
            {
                var item = cart[index - 1];
                total -= item.TotalPrice;
                cart.RemoveAt(index - 1);
                Console.WriteLine("Producto eliminado del carrito.");
            }
            else
            {
                Console.WriteLine("Selección inválida.");
            }
        }

        static bool FinalizeSale(Sale sale, List<SaleDetail> cart, decimal total)
        {
            Console.WriteLine("\n=== FINALIZAR VENTA ===");
            Console.WriteLine($"Total a pagar: ${total:F2}");
            Console.WriteLine("Métodos de pago:");
            Console.WriteLine("1. Efectivo");
            Console.WriteLine("2. Tarjeta");
            Console.WriteLine("3. Transferencia");
            
            Console.Write("Seleccione método de pago: ");
            if (!int.TryParse(Console.ReadLine(), out int paymentOption))
            {
                Console.WriteLine("Opción inválida.");
                return false;
            }

            string paymentMethod = paymentOption switch
            {
                1 => "cash",
                2 => "card",
                3 => "transfer",
                _ => "cash"
            };

            if (paymentMethod == "cash")
            {
                Console.Write($"Cantidad recibida: $");
                if (decimal.TryParse(Console.ReadLine(), out decimal amountReceived))
                {
                    if (amountReceived < total)
                    {
                        Console.WriteLine("Cantidad insuficiente.");
                        return false;
                    }
                    
                    decimal change = amountReceived - total;
                    if (change > 0)
                    {
                        Console.WriteLine($"Cambio: ${change:F2}");
                    }
                }
            }

            sale.TotalSale = total;
            sale.PaymentMethod = paymentMethod;
            sale.Details = cart;

            int saleId = saleService.CreateSale(sale);
            
            if (saleId > 0)
            {
                Console.WriteLine($"\n¡Venta realizada exitosamente!");
                Console.WriteLine($"ID de venta: {saleId}");
                PrintReceipt(sale, saleId);
                Console.WriteLine("\nPresione cualquier tecla para continuar...");
                Console.ReadKey();
                return true;
            }
            else
            {
                Console.WriteLine("Error al procesar la venta.");
                Console.ReadKey();
                return false;
            }
        }

        static void PrintReceipt(Sale sale, int saleId)
        {
            Console.WriteLine("\n" + new string('=', 40));
            Console.WriteLine("         TICKET DE VENTA");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine($"Venta #: {saleId}");
            Console.WriteLine($"Fecha: {sale.SaleDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Vendedor: {currentUser?.NameUser} {currentUser?.LastNameUser}");
            Console.WriteLine($"Método de pago: {sale.PaymentMethod.ToUpper()}");
            Console.WriteLine(new string('-', 40));
            
            foreach (var item in sale.Details)
            {
                Console.WriteLine($"{item.ProductName}");
                Console.WriteLine($"  {item.Quantity} x ${item.UnitPrice:F2} = ${item.TotalPrice:F2}");
            }
            
            Console.WriteLine(new string('-', 40));
            Console.WriteLine($"TOTAL: ${sale.TotalSale:F2}");
            Console.WriteLine(new string('=', 40));
            Console.WriteLine("    ¡Gracias por su compra!");
        }

        static void ViewTodaySales()
        {
            var sales = saleService.GetSalesByDate(DateTime.Today);
            decimal totalAmount = saleService.GetTotalSalesAmount(DateTime.Today);
            int totalCount = saleService.GetTotalSalesCount(DateTime.Today);

            Console.Clear();
            Console.WriteLine($"=== VENTAS DEL DÍA - {DateTime.Today:dd/MM/yyyy} ===");
            Console.WriteLine($"Total de ventas: {totalCount}");
            Console.WriteLine($"Ingresos del día: ${totalAmount:F2}");
            Console.WriteLine(new string('-', 60));

            if (sales.Count == 0)
            {
                Console.WriteLine("No hay ventas registradas para hoy.");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Hora",-8} {"Total",-12} {"Método Pago",-15}");
                Console.WriteLine(new string('-', 45));

                foreach (var sale in sales)
                {
                    Console.WriteLine($"{sale.IdSale,-5} {sale.SaleDate:HH:mm} ${sale.TotalSale,-11:F2} {sale.PaymentMethod,-15}");
                }
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void SearchSaleById()
        {
            Console.Write("ID de la venta: ");
            if (!int.TryParse(Console.ReadLine(), out int saleId))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }

            var sale = saleService.GetSaleById(saleId);
            if (sale == null)
            {
                Console.WriteLine("Venta no encontrada.");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("=== DETALLE DE VENTA ===");
            Console.WriteLine($"ID: {sale.IdSale}");
            Console.WriteLine($"Fecha: {sale.SaleDate:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total: ${sale.TotalSale:F2}");
            Console.WriteLine($"Método de pago: {sale.PaymentMethod}");
            Console.WriteLine($"Estado: {sale.StatusSale}");
            Console.WriteLine("\nProductos:");
            Console.WriteLine(new string('-', 50));

            foreach (var detail in sale.Details)
            {
                Console.WriteLine($"{detail.ProductName}");
                Console.WriteLine($"  Cantidad: {detail.Quantity} x ${detail.UnitPrice:F2} = ${detail.TotalPrice:F2}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void ProductsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== GESTIÓN DE PRODUCTOS ===");
                Console.WriteLine("1. Registrar Producto");
                Console.WriteLine("2. Ver Todos los Productos");
                Console.WriteLine("3. Buscar Producto");
                Console.WriteLine("4. Actualizar Producto");
                Console.WriteLine("5. Eliminar Producto");
                Console.WriteLine("6. Actualizar Stock");
                Console.WriteLine("0. Volver");
                
                Console.Write("\nSeleccione una opción: ");
                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1:
                        CreateProduct();
                        break;
                    case 2:
                        ViewAllProducts();
                        break;
                    case 3:
                        SearchProduct();
                        break;
                    case 4:
                        UpdateProduct();
                        break;
                    case 5:
                        DeleteProduct();
                        break;
                    case 6:
                        UpdateStock();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void CreateProduct()
        {
            Console.Clear();
            Console.WriteLine("=== REGISTRAR PRODUCTO ===");
            
            var product = new Product();

            // ID del producto con validación
            while (true)
            {
                Console.Write("ID del Producto: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    if (!productService.ProductExists(id))
                    {
                        product.IdProduct = id;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: El producto ya existe. Intente con otro ID.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Ingrese un ID válido.");
                }
            }

            // Nombre del producto
            do
            {
                Console.Write("Nombre del Producto: ");
                product.NameProduct = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(product.NameProduct))
                {
                    Console.WriteLine("Error: El nombre no puede estar vacío.");
                    product.NameProduct = "";
                }
            } while (string.IsNullOrEmpty(product.NameProduct));

            // Categoría
            do
            {
                Console.Write("Categoría: ");
                product.CategoriaProduct = Console.ReadLine()?.Trim() ?? "";
                if (string.IsNullOrWhiteSpace(product.CategoriaProduct))
                {
                    Console.WriteLine("Error: La categoría no puede estar vacía.");
                    product.CategoriaProduct = "";
                }
            } while (string.IsNullOrEmpty(product.CategoriaProduct));

            // Precio de compra
            while (true)
            {
                Console.Write("Precio de Compra: $");
                if (decimal.TryParse(Console.ReadLine(), out decimal purchasePrice) && purchasePrice > 0)
                {
                    product.PrecioCompraProduct = purchasePrice;
                    break;
                }
                Console.WriteLine("Error: Ingrese un precio válido mayor a 0.");
            }

            // Precio de venta
            while (true)
            {
                Console.Write("Precio de Venta: $");
                if (decimal.TryParse(Console.ReadLine(), out decimal salePrice) && salePrice > 0)
                {
                    if (salePrice >= product.PrecioCompraProduct)
                    {
                        product.PrecioVentaProduct = salePrice;
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Error: El precio de venta debe ser mayor o igual al precio de compra.");
                    }
                }
                else
                {
                    Console.WriteLine("Error: Ingrese un precio válido mayor a 0.");
                }
            }

            // Stock inicial
            while (true)
            {
                Console.Write("Stock inicial: ");
                if (int.TryParse(Console.ReadLine(), out int stock) && stock >= 0)
                {
                    product.StockProduct = stock;
                    break;
                }
                Console.WriteLine("Error: Ingrese un stock válido (0 o mayor).");
            }

            // Descripción (opcional)
            Console.Write("Descripción (opcional): ");
            product.DescriptionProduct = Console.ReadLine()?.Trim() ?? "";

            if (productService.CreateProduct(product))
            {
                Console.WriteLine("¡Producto registrado exitosamente!");
            }
            else
            {
                Console.WriteLine("Error al registrar el producto.");
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void ViewAllProducts()
        {
            var products = productService.GetAllProducts();
            
            Console.Clear();
            Console.WriteLine("=== LISTA DE PRODUCTOS ===");
            
            if (products.Count == 0)
            {
                Console.WriteLine("No hay productos registrados.");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Nombre",-20} {"Categoría",-15} {"Precio",-10} {"Stock",-8}");
                Console.WriteLine(new string('-', 60));

                foreach (var product in products)
                {
                    string stockStatus = product.StockProduct == 0 ? " (SIN STOCK)" : 
                                       product.StockProduct <= product.StockMin ? " (BAJO)" : "";
                    
                    Console.WriteLine($"{product.IdProduct,-5} {product.NameProduct,-20} {product.CategoriaProduct,-15} " +
                                    $"${product.PrecioVentaProduct,-9:F2} {product.StockProduct,-8}{stockStatus}");
                }
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void SearchProduct()
        {
            Console.Write("ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }

            var product = productService.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine("Producto no encontrado.");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("=== INFORMACIÓN DEL PRODUCTO ===");
            Console.WriteLine($"ID: {product.IdProduct}");
            Console.WriteLine($"Nombre: {product.NameProduct}");
            Console.WriteLine($"Categoría: {product.CategoriaProduct}");
            Console.WriteLine($"Precio de Compra: ${product.PrecioCompraProduct:F2}");
            Console.WriteLine($"Precio de Venta: ${product.PrecioVentaProduct:F2}");
            Console.WriteLine($"Stock Actual: {product.StockProduct}");
            Console.WriteLine($"Ganancia Unitaria: ${product.PrecioVentaProduct - product.PrecioCompraProduct:F2}");
            
            if (!string.IsNullOrEmpty(product.DescriptionProduct))
            {
                Console.WriteLine($"Descripción: {product.DescriptionProduct}");
            }

            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void UpdateProduct()
        {
            Console.Write("ID del producto a actualizar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }

            var product = productService.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine("Producto no encontrado.");
                Console.ReadKey();
                return;
            }

            Console.Clear();
            Console.WriteLine("=== ACTUALIZAR PRODUCTO ===");
            Console.WriteLine("Presione Enter para mantener el valor actual");
            
            Console.Write($"Nombre actual: {product.NameProduct}\nNuevo nombre: ");
            string newName = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newName))
                product.NameProduct = newName;

            Console.Write($"Categoría actual: {product.CategoriaProduct}\nNueva categoría: ");
            string newCategory = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newCategory))
                product.CategoriaProduct = newCategory;

            Console.Write($"Precio de compra actual: ${product.PrecioCompraProduct:F2}\nNuevo precio de compra: $");
            if (decimal.TryParse(Console.ReadLine(), out decimal newPurchasePrice) && newPurchasePrice > 0)
                product.PrecioCompraProduct = newPurchasePrice;

            Console.Write($"Precio de venta actual: ${product.PrecioVentaProduct:F2}\nNuevo precio de venta: $");
            if (decimal.TryParse(Console.ReadLine(), out decimal newSalePrice) && newSalePrice >= product.PrecioCompraProduct)
                product.PrecioVentaProduct = newSalePrice;

            Console.Write($"Descripción actual: {product.DescriptionProduct}\nNueva descripción: ");
            string newDescription = Console.ReadLine()?.Trim() ?? "";
            if (!string.IsNullOrEmpty(newDescription))
                product.DescriptionProduct = newDescription;

            if (productService.UpdateProduct(product))
            {
                Console.WriteLine("Producto actualizado exitosamente.");
            }
            else
            {
                Console.WriteLine("Error al actualizar el producto.");
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void DeleteProduct()
        {
            Console.Write("ID del producto a eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }

            var product = productService.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine("Producto no encontrado.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"¿Está seguro que desea eliminar el producto '{product.NameProduct}'? (s/n): ");
            string confirm = Console.ReadLine()?.ToLower() ?? "";
            
            if (confirm == "s" || confirm == "si")
            {
                if (productService.DeleteProduct(id))
                {
                    Console.WriteLine("Producto eliminado exitosamente.");
                }
                else
                {
                    Console.WriteLine("Error al eliminar el producto.");
                }
            }
            else
            {
                Console.WriteLine("Operación cancelada.");
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void UpdateStock()
        {
            Console.Write("ID del producto: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("ID inválido.");
                Console.ReadKey();
                return;
            }

            var product = productService.GetProductById(id);
            if (product == null)
            {
                Console.WriteLine("Producto no encontrado.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine($"Producto: {product.NameProduct}");
            Console.WriteLine($"Stock actual: {product.StockProduct}");
            Console.Write("Nuevo stock: ");
            
            if (int.TryParse(Console.ReadLine(), out int newStock) && newStock >= 0)
            {
                if (productService.UpdateStock(id, newStock))
                {
                    Console.WriteLine("Stock actualizado exitosamente.");
                }
                else
                {
                    Console.WriteLine("Error al actualizar el stock.");
                }
            }
            else
            {
                Console.WriteLine("Stock inválido.");
            }

            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
        }

        static void ReportsMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MÓDULO DE REPORTES ===");
                Console.WriteLine("1. Reporte de Productos");
                Console.WriteLine("2. Productos con Stock Bajo");
                Console.WriteLine("3. Reporte de Ventas del Día");
                Console.WriteLine("4. Top Productos Más Vendidos");
                Console.WriteLine("5. Reporte de Ventas por Fecha");
                Console.WriteLine("0. Volver");
                
                Console.Write("\nSeleccione una opción: ");
                if (!int.TryParse(Console.ReadLine(), out int option))
                {
                    Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                    Console.ReadKey();
                    continue;
                }

                switch (option)
                {
                    case 1:
                        reportService.ShowProductsReport();
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                    case 2:
                        reportService.ShowLowStockReport();
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                    case 3:
                        reportService.ShowSalesReport(DateTime.Today);
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                    case 4:
                        reportService.ShowTopProductsReport();
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                    case 5:
                        Console.Write("Fecha (dd/mm/yyyy): ");
                        if (DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", null, 
                            System.Globalization.DateTimeStyles.None, out DateTime date))
                        {
                            reportService.ShowSalesReport(date);
                        }
                        else
                        {
                            Console.WriteLine("Formato de fecha inválido.");
                        }
                        Console.WriteLine("\nPresione cualquier tecla para continuar...");
                        Console.ReadKey();
                        break;
                    case 0:
                        return;
                    default:
                        Console.WriteLine("Opción inválida. Presione cualquier tecla...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void ConfigurationMenu()
        {
            Console.WriteLine("=== CONFIGURACIÓN ===");
            Console.WriteLine("1. Registrar nuevo usuario");
            Console.WriteLine("2. Ver estadísticas del sistema");
            Console.WriteLine("0. Volver");
            
            Console.Write("Opción: ");
            if (int.TryParse(Console.ReadLine(), out int option))
            {
                switch (option)
                {
                    case 1:
                        RegisterUser();
                        break;
                    case 2:
                        ShowSystemStats();
                        break;
                }
            }
        }

        static void ShowSystemStats()
        {
            Console.WriteLine("\n=== ESTADÍSTICAS DEL SISTEMA ===");
            // Aquí puedes agregar más estadísticas según necesites
            var products = productService.GetAllProducts();
            var todaySales = saleService.GetSalesByDate(DateTime.Today);
            
            Console.WriteLine($"Total de productos activos: {products.Count}");
            Console.WriteLine($"Ventas del día: {todaySales.Count}");
            Console.WriteLine($"Ingresos del día: ${saleService.GetTotalSalesAmount(DateTime.Today):F2}");
            
            Console.WriteLine("\nPresione cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}