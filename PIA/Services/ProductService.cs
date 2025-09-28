using MiProyecto.Database;
using MiProyecto.Models;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace MiProyecto.Services
{
    public class ProductService
    {
        private readonly DatabaseConnection _db;

        public ProductService()
        {
            _db = new DatabaseConnection();
        }

        public bool CreateProduct(Product product)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"INSERT INTO product (id_product, name_product, categoria_product, 
                                   precioCompra_product, precioVenta_product, stock_product, description_product) 
                                   VALUES (@id, @name, @category, @purchasePrice, @salePrice, @stock, @description)";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", product.IdProduct);
                        cmd.Parameters.AddWithValue("@name", product.NameProduct);
                        cmd.Parameters.AddWithValue("@category", product.CategoriaProduct);
                        cmd.Parameters.AddWithValue("@purchasePrice", product.PrecioCompraProduct);
                        cmd.Parameters.AddWithValue("@salePrice", product.PrecioVentaProduct);
                        cmd.Parameters.AddWithValue("@stock", product.StockProduct);
                        cmd.Parameters.AddWithValue("@description", product.DescriptionProduct);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear producto: {ex.Message}");
                    return false;
                }
            }
        }

        public List<Product> GetAllProducts()
        {
            List<Product> products = new List<Product>();
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM product WHERE status_product = 'active' ORDER BY name_product";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var descriptionValue = reader["description_product"];
                            products.Add(new Product
                            {
                                IdProduct = reader.GetInt32("id_product"),
                                NameProduct = reader.GetString("name_product"),
                                CategoriaProduct = reader.GetString("categoria_product"),
                                PrecioCompraProduct = reader.GetDecimal("precioCompra_product"),
                                PrecioVentaProduct = reader.GetDecimal("precioVenta_product"),
                                StockProduct = reader.GetInt32("stock_product"),
                                DescriptionProduct = descriptionValue == DBNull.Value ? "" : descriptionValue.ToString() ?? ""
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener productos: {ex.Message}");
                }
            }
            return products;
        }

        public Product? GetProductById(int id)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM product WHERE id_product = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                var descriptionValue = reader["description_product"];
                                return new Product
                                {
                                    IdProduct = reader.GetInt32("id_product"),
                                    NameProduct = reader.GetString("name_product"),
                                    CategoriaProduct = reader.GetString("categoria_product"),
                                    PrecioCompraProduct = reader.GetDecimal("precioCompra_product"),
                                    PrecioVentaProduct = reader.GetDecimal("precioVenta_product"),
                                    StockProduct = reader.GetInt32("stock_product"),
                                    DescriptionProduct = descriptionValue == DBNull.Value ? "" : descriptionValue.ToString() ?? ""
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener producto: {ex.Message}");
                }
            }
            return null;
        }

        public bool UpdateProduct(Product product)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"UPDATE product SET name_product = @name, categoria_product = @category,
                                   precioCompra_product = @purchasePrice, precioVenta_product = @salePrice,
                                   stock_product = @stock, description_product = @description
                                   WHERE id_product = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", product.IdProduct);
                        cmd.Parameters.AddWithValue("@name", product.NameProduct);
                        cmd.Parameters.AddWithValue("@category", product.CategoriaProduct);
                        cmd.Parameters.AddWithValue("@purchasePrice", product.PrecioCompraProduct);
                        cmd.Parameters.AddWithValue("@salePrice", product.PrecioVentaProduct);
                        cmd.Parameters.AddWithValue("@stock", product.StockProduct);
                        cmd.Parameters.AddWithValue("@description", product.DescriptionProduct);

                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar producto: {ex.Message}");
                    return false;
                }
            }
        }

        public bool DeleteProduct(int id)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE product SET status_product = 'inactive' WHERE id_product = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar producto: {ex.Message}");
                    return false;
                }
            }
        }

        public bool ProductExists(int id)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM product WHERE id_product = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al verificar producto: {ex.Message}");
                    return false;
                }
            }
        }

        public bool UpdateStock(int productId, int newStock)
        {
            using (MySqlConnection conn = _db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE product SET stock_product = @stock WHERE id_product = @id";
                    
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", productId);
                        cmd.Parameters.AddWithValue("@stock", newStock);
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar stock: {ex.Message}");
                    return false;
                }
            }
        }
    }
}