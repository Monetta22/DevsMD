using MiProyecto.Database;
using MiProyecto.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MiProyecto.Services
{
    public class SaleService
    {
        private readonly DatabaseConnection _db;
        private readonly ProductService _productService;

        public SaleService()
        {
            _db = new DatabaseConnection();
            _productService = new ProductService();
        }

        public int CreateSale(Sale sale)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                MySqlTransaction? transaction = null;
                try
                {
                    conn.Open();
                    transaction = conn.BeginTransaction();

                    // Insertar la venta
                    string saleQuery = @"INSERT INTO sales (id_user, id_customer, total_sale, payment_method, status_sale) 
                                       VALUES (@userId, @customerId, @total, @paymentMethod, @status);
                                       SELECT LAST_INSERT_ID();";
                    
                    int saleId;
                    using (MySqlCommand cmd = new MySqlCommand(saleQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@userId", sale.IdUser);
                        cmd.Parameters.AddWithValue("@customerId", sale.IdCustomer.HasValue ? (object)sale.IdCustomer.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@total", sale.TotalSale);
                        cmd.Parameters.AddWithValue("@paymentMethod", sale.PaymentMethod);
                        cmd.Parameters.AddWithValue("@status", sale.StatusSale);
                        
                        saleId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Insertar los detalles de la venta
                    foreach (var detail in sale.Details)
                    {
                        string detailQuery = @"INSERT INTO sale_details (id_sale, id_product, quantity, unit_price, total_price) 
                                             VALUES (@saleId, @productId, @quantity, @unitPrice, @totalPrice)";
                        
                        using (MySqlCommand cmd = new MySqlCommand(detailQuery, conn, transaction))
                        {
                            cmd.Parameters.AddWithValue("@saleId", saleId);
                            cmd.Parameters.AddWithValue("@productId", detail.IdProduct);
                            cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                            cmd.Parameters.AddWithValue("@unitPrice", detail.UnitPrice);
                            cmd.Parameters.AddWithValue("@totalPrice", detail.TotalPrice);
                            cmd.ExecuteNonQuery();
                        }

                        // Actualizar el stock del producto
                        var product = _productService.GetProductById(detail.IdProduct);
                        if (product != null)
                        {
                            int newStock = product.StockProduct - detail.Quantity;
                            string updateStockQuery = "UPDATE product SET stock_product = @newStock WHERE id_product = @productId";
                            using (MySqlCommand cmd = new MySqlCommand(updateStockQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@newStock", newStock);
                                cmd.Parameters.AddWithValue("@productId", detail.IdProduct);
                                cmd.ExecuteNonQuery();
                            }

                            // Registrar movimiento de inventario
                            string movementQuery = @"INSERT INTO inventory_movements (id_product, movement_type, quantity, reason, id_user) 
                                                   VALUES (@productId, 'out', @quantity, 'Venta', @userId)";
                            using (MySqlCommand cmd = new MySqlCommand(movementQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@productId", detail.IdProduct);
                                cmd.Parameters.AddWithValue("@quantity", detail.Quantity);
                                cmd.Parameters.AddWithValue("@userId", sale.IdUser);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    transaction.Commit();
                    return saleId;
                }
                catch (Exception ex)
                {
                    transaction?.Rollback();
                    Console.WriteLine($"Error al crear venta: {ex.Message}");
                    return -1;
                }
            }
        }

        public List<Sale> GetSalesByDate(DateTime date)
        {
            List<Sale> sales = new List<Sale>();
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT s.*, u.name_user, u.lastName_user, c.name_customer
                                   FROM sales s
                                   LEFT JOIN users u ON s.id_user = u.id_user
                                   LEFT JOIN customers c ON s.id_customer = c.id_customer
                                   WHERE DATE(s.sale_date) = @date
                                   ORDER BY s.sale_date DESC";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var customerValue = reader["id_customer"];
                                sales.Add(new Sale
                                {
                                    IdSale = reader.GetInt32("id_sale"),
                                    IdUser = reader.GetInt32("id_user"),
                                    IdCustomer = customerValue == DBNull.Value ? null : Convert.ToInt32(customerValue),
                                    TotalSale = reader.GetDecimal("total_sale"),
                                    PaymentMethod = reader.GetString("payment_method"),
                                    SaleDate = reader.GetDateTime("sale_date"),
                                    StatusSale = reader.GetString("status_sale")
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener ventas: {ex.Message}");
                }
            }
            return sales;
        }

        public Sale? GetSaleById(int saleId)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT s.*, u.name_user, u.lastName_user, c.name_customer
                                   FROM sales s
                                   LEFT JOIN users u ON s.id_user = u.id_user
                                   LEFT JOIN customers c ON s.id_customer = c.id_customer
                                   WHERE s.id_sale = @saleId";
                    
                    Sale? sale = null;
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@saleId", saleId);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var customerValue = reader["id_customer"];
                                sale = new Sale
                                {
                                    IdSale = reader.GetInt32("id_sale"),
                                    IdUser = reader.GetInt32("id_user"),
                                    IdCustomer = customerValue == DBNull.Value ? null : Convert.ToInt32(customerValue),
                                    TotalSale = reader.GetDecimal("total_sale"),
                                    PaymentMethod = reader.GetString("payment_method"),
                                    SaleDate = reader.GetDateTime("sale_date"),
                                    StatusSale = reader.GetString("status_sale")
                                };
                            }
                        }
                    }

                    if (sale != null)
                    {
                        // Obtener detalles de la venta
                        string detailQuery = @"SELECT sd.*, p.name_product 
                                             FROM sale_details sd
                                             JOIN product p ON sd.id_product = p.id_product
                                             WHERE sd.id_sale = @saleId";
                        
                        using (MySqlCommand cmd = new MySqlCommand(detailQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@saleId", saleId);
                            
                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    sale.Details.Add(new SaleDetail
                                    {
                                        IdDetail = reader.GetInt32("id_detail"),
                                        IdSale = reader.GetInt32("id_sale"),
                                        IdProduct = reader.GetInt32("id_product"),
                                        Quantity = reader.GetInt32("quantity"),
                                        UnitPrice = reader.GetDecimal("unit_price"),
                                        TotalPrice = reader.GetDecimal("total_price"),
                                        ProductName = reader.GetString("name_product")
                                    });
                                }
                            }
                        }
                    }

                    return sale;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener venta: {ex.Message}");
                    return null;
                }
            }
        }

        public decimal GetTotalSalesAmount(DateTime date)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT COALESCE(SUM(total_sale), 0) 
                                   FROM sales 
                                   WHERE DATE(sale_date) = @date AND status_sale = 'completed'";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        return Convert.ToDecimal(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener total de ventas: {ex.Message}");
                    return 0;
                }
            }
        }

        public int GetTotalSalesCount(DateTime date)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT COUNT(*) 
                                   FROM sales 
                                   WHERE DATE(sale_date) = @date AND status_sale = 'completed'";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@date", date.Date);
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener conteo de ventas: {ex.Message}");
                    return 0;
                }
            }
        }
    }
}