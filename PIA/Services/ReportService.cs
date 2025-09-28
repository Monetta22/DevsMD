using MiProyecto.Database;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MiProyecto.Services
{
    public class ReportService
    {
        private readonly DatabaseConnection _db;

        public ReportService()
        {
            _db = new DatabaseConnection();
        }

        public void ShowProductsReport()
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT id_product, name_product, categoria_product, 
                                   precioCompra_product, precioVenta_product, stock_product,
                                   (precioVenta_product - precioCompra_product) as ganancia_unitaria
                                   FROM product 
                                   WHERE status_product = 'active' 
                                   ORDER BY categoria_product, name_product";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n================ REPORTE DE PRODUCTOS ================");
                        Console.WriteLine($"{"ID",-5} {"Nombre",-20} {"Categoría",-15} {"P.Compra",-10} {"P.Venta",-10} {"Stock",-8} {"Ganancia",-10}");
                        Console.WriteLine(new string('=', 85));

                        string currentCategory = "";
                        decimal totalValue = 0;
                        int totalProducts = 0;

                        while (reader.Read())
                        {
                            string category = reader.GetString("categoria_product");
                            if (category != currentCategory)
                            {
                                if (!string.IsNullOrEmpty(currentCategory))
                                {
                                    Console.WriteLine(new string('-', 85));
                                }
                                Console.WriteLine($"\n--- {category.ToUpper()} ---");
                                currentCategory = category;
                            }

                            int id = reader.GetInt32("id_product");
                            string name = reader.GetString("name_product");
                            decimal purchasePrice = reader.GetDecimal("precioCompra_product");
                            decimal salePrice = reader.GetDecimal("precioVenta_product");
                            int stock = reader.GetInt32("stock_product");
                            decimal unitProfit = reader.GetDecimal("ganancia_unitaria");

                            Console.WriteLine($"{id,-5} {name,-20} {category,-15} ${purchasePrice,-9:F2} ${salePrice,-9:F2} {stock,-8} ${unitProfit,-9:F2}");

                            totalValue += salePrice * stock;
                            totalProducts++;
                        }

                        Console.WriteLine(new string('=', 85));
                        Console.WriteLine($"Total de productos: {totalProducts}");
                        Console.WriteLine($"Valor total del inventario: ${totalValue:F2}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al generar reporte de productos: {ex.Message}");
                }
            }
        }

        public void ShowLowStockReport()
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT id_product, name_product, categoria_product, stock_product, stock_min
                                   FROM product 
                                   WHERE status_product = 'active' AND stock_product <= stock_min
                                   ORDER BY stock_product ASC";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n================ PRODUCTOS CON STOCK BAJO ================");
                        Console.WriteLine($"{"ID",-5} {"Nombre",-25} {"Categoría",-15} {"Stock Actual",-12} {"Stock Mín",-10}");
                        Console.WriteLine(new string('=', 70));

                        int lowStockCount = 0;
                        while (reader.Read())
                        {
                            int id = reader.GetInt32("id_product");
                            string name = reader.GetString("name_product");
                            string category = reader.GetString("categoria_product");
                            int currentStock = reader.GetInt32("stock_product");
                            int minStock = reader.GetInt32("stock_min");

                            string stockStatus = currentStock == 0 ? "SIN STOCK" : "BAJO";
                            Console.WriteLine($"{id,-5} {name,-25} {category,-15} {currentStock,-12} {minStock,-10} [{stockStatus}]");
                            lowStockCount++;
                        }

                        if (lowStockCount == 0)
                        {
                            Console.WriteLine("No hay productos con stock bajo.");
                        }
                        else
                        {
                            Console.WriteLine($"\nTotal de productos con stock bajo: {lowStockCount}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al generar reporte de stock bajo: {ex.Message}");
                }
            }
        }

        public void ShowSalesReport(DateTime date)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    
                    // Resumen de ventas del día
                    string summaryQuery = @"SELECT 
                                          COUNT(*) as total_ventas,
                                          SUM(total_sale) as total_ingresos,
                                          AVG(total_sale) as venta_promedio
                                          FROM sales 
                                          WHERE DATE(sale_date) = @date AND status_sale = 'completed'";
                    
                    using (MySqlCommand cmd = new MySqlCommand(summaryQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Console.WriteLine($"\n================ REPORTE DE VENTAS - {date:dd/MM/yyyy} ================");
                                Console.WriteLine($"Total de ventas: {reader.GetInt32("total_ventas")}");
                                Console.WriteLine($"Ingresos totales: ${reader.GetDecimal("total_ingresos"):F2}");
                                Console.WriteLine($"Venta promedio: ${reader.GetDecimal("venta_promedio"):F2}");
                            }
                        }
                    }

                    // Detalle de ventas
                    string detailQuery = @"SELECT s.id_sale, s.total_sale, s.payment_method, s.sale_date,
                                         u.name_user, u.lastName_user
                                         FROM sales s
                                         JOIN users u ON s.id_user = u.id_user
                                         WHERE DATE(s.sale_date) = @date AND s.status_sale = 'completed'
                                         ORDER BY s.sale_date DESC";
                    
                    using (MySqlCommand cmd = new MySqlCommand(detailQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            Console.WriteLine("\n--- DETALLE DE VENTAS ---");
                            Console.WriteLine($"{"ID Venta",-10} {"Total",-12} {"Método Pago",-12} {"Hora",-8} {"Vendedor",-20}");
                            Console.WriteLine(new string('-', 70));

                            while (reader.Read())
                            {
                                int saleId = reader.GetInt32("id_sale");
                                decimal total = reader.GetDecimal("total_sale");
                                string paymentMethod = reader.GetString("payment_method");
                                DateTime saleDate = reader.GetDateTime("sale_date");
                                string seller = $"{reader.GetString("name_user")} {reader.GetString("lastName_user")}";

                                Console.WriteLine($"{saleId,-10} ${total,-11:F2} {paymentMethod,-12} {saleDate:HH:mm} {seller,-20}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al generar reporte de ventas: {ex.Message}");
                }
            }
        }

        public void ShowTopProductsReport()
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT p.name_product, p.categoria_product, 
                                   SUM(sd.quantity) as total_vendido,
                                   SUM(sd.total_price) as total_ingresos
                                   FROM sale_details sd
                                   JOIN product p ON sd.id_product = p.id_product
                                   JOIN sales s ON sd.id_sale = s.id_sale
                                   WHERE s.status_sale = 'completed'
                                   GROUP BY p.id_product, p.name_product, p.categoria_product
                                   ORDER BY total_vendido DESC
                                   LIMIT 10";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        Console.WriteLine("\n================ TOP 10 PRODUCTOS MÁS VENDIDOS ================");
                        Console.WriteLine($"{"Producto",-25} {"Categoría",-15} {"Cantidad",-10} {"Ingresos",-12}");
                        Console.WriteLine(new string('=', 65));

                        int position = 1;
                        while (reader.Read())
                        {
                            string productName = reader.GetString("name_product");
                            string category = reader.GetString("categoria_product");
                            int totalSold = reader.GetInt32("total_vendido");
                            decimal totalRevenue = reader.GetDecimal("total_ingresos");

                            Console.WriteLine($"{position}. {productName,-22} {category,-15} {totalSold,-10} ${totalRevenue,-11:F2}");
                            position++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al generar reporte de productos más vendidos: {ex.Message}");
                }
            }
        }
    }
}