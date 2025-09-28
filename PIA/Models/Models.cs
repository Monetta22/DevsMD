using System;

namespace MiProyecto.Models
{
    public class User
    {
        public int IdUser { get; set; }
        public string NameUser { get; set; } = string.Empty;
        public string LastNameUser { get; set; } = string.Empty;
        public string EmailUser { get; set; } = string.Empty;
        public string PasswordUser { get; set; } = string.Empty;
        public string UserType { get; set; } = "employee";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Product
    {
        public int IdProduct { get; set; }
        public string NameProduct { get; set; } = string.Empty;
        public string CategoriaProduct { get; set; } = string.Empty;
        public decimal PrecioCompraProduct { get; set; }
        public decimal PrecioVentaProduct { get; set; }
        public int StockProduct { get; set; }
        public int StockMin { get; set; } = 5;
        public string DescriptionProduct { get; set; } = string.Empty;
        public string StatusProduct { get; set; } = "active";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class Customer
    {
        public int IdCustomer { get; set; }
        public string NameCustomer { get; set; } = string.Empty;
        public string PhoneCustomer { get; set; } = string.Empty;
        public string EmailCustomer { get; set; } = string.Empty;
        public string AddressCustomer { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class Sale
    {
        public int IdSale { get; set; }
        public int IdUser { get; set; }
        public int? IdCustomer { get; set; }
        public decimal TotalSale { get; set; }
        public string PaymentMethod { get; set; } = "cash";
        public DateTime SaleDate { get; set; }
        public string StatusSale { get; set; } = "completed";
        public List<SaleDetail> Details { get; set; } = new List<SaleDetail>();
    }

    public class SaleDetail
    {
        public int IdDetail { get; set; }
        public int IdSale { get; set; }
        public int IdProduct { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string ProductName { get; set; } = string.Empty;
    }

    public class InventoryMovement
    {
        public int IdMovement { get; set; }
        public int IdProduct { get; set; }
        public string MovementType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int IdUser { get; set; }
        public DateTime MovementDate { get; set; }
    }
}