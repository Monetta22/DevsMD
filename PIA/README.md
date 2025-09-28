# Sistema de Punto de Venta (POS) - MySQL

Este es un sistema completo de punto de venta desarrollado en C# con **MySQL** como base de datos.

## Características

### ?? Autenticación
- Sistema de login y registro de usuarios
- Roles de usuario (Administrador y Empleado)
- Validación de credenciales

### ?? Módulo de Ventas
- Creación de ventas con carrito de compras
- Múltiples métodos de pago (Efectivo, Tarjeta, Transferencia)
- Cálculo automático de cambio
- Impresión de tickets
- Búsqueda de ventas por ID
- Visualización de ventas del día

### ?? Gestión de Productos
- CRUD completo de productos
- Control de inventario y stock
- Categorización de productos
- Alertas de stock bajo
- Actualización masiva de precios

### ?? Reportes
- Reporte completo de productos
- Productos con stock bajo
- Reporte de ventas diarias
- Top productos más vendidos
- Análisis de ingresos

### ?? Configuración
- Gestión de usuarios (solo administradores)
- Estadísticas del sistema

## Instalación

### Prerrequisitos
- .NET 9.0 o superior
- **MySQL 8.0 o superior**
- Visual Studio 2022 o VS Code

### Configuración de Base de Datos MySQL

1. **Instalar MySQL**: Asegúrate de tener MySQL instalado y funcionando
2. **Crear la base de datos**: Ejecuta el script `database_script.sql` en tu servidor MySQL
3. **Configurar conexión**: Modifica la cadena de conexión en `Database/DatabaseConnection.cs`:
   ```csharp
   connectionString = "Server=localhost;Database=pos_system;Uid=tu_usuario;Pwd=tu_contraseña;";
   ```

   **Configuraciones comunes:**
   - Para XAMPP: `"Server=localhost;Database=pos_system;Uid=root;Pwd=;"`
   - Para MySQL local: `"Server=localhost;Database=pos_system;Uid=root;Pwd=tu_contraseña;"`
   - Para servidor remoto: `"Server=tu_servidor;Database=pos_system;Uid=tu_usuario;Pwd=tu_contraseña;"`

### Instalación del Proyecto

1. Clona o descarga el proyecto
2. Abre la terminal en la carpeta del proyecto
3. Restaurar paquetes NuGet:
   ```bash
   dotnet restore
   ```
4. Compilar el proyecto:
   ```bash
   dotnet build
   ```
5. Ejecutar:
   ```bash
   dotnet run
   ```

## Uso del Sistema

### Primer Inicio
- Usuario administrador por defecto:
  - **Email**: admin@pos.com
  - **Contraseña**: admin123

### Flujo de Trabajo Típico

1. **Iniciar sesión** con las credenciales
2. **Registrar productos** en el módulo de productos
3. **Realizar ventas** desde el módulo de ventas
4. **Consultar reportes** para análisis de negocio

## Estructura del Proyecto

```
PIA/
??? Database/
?   ??? DatabaseConnection.cs    # Conexión a MySQL
??? Models/
?   ??? Models.cs               # Modelos de datos
??? Services/
?   ??? AuthService.cs          # Servicio de autenticación
?   ??? ProductService.cs       # Servicio de productos
?   ??? SaleService.cs          # Servicio de ventas
?   ??? ReportService.cs        # Servicio de reportes
??? Program.cs                  # Aplicación principal
??? database_script.sql         # Script de MySQL
??? README.md                   # Este archivo
```

## Tablas de la Base de Datos MySQL

### users
- Información de usuarios del sistema
- Campos: id_user, name_user, lastName_user, email_user, password_user, user_type

### product
- Catálogo de productos
- Campos: id_product, name_product, categoria_product, precioCompra_product, precioVenta_product, stock_product

### customers
- Información de clientes
- Campos: id_customer, name_customer, phone_customer, email_customer, address_customer

### sales
- Registro de ventas
- Campos: id_sale, id_user, id_customer, total_sale, payment_method, sale_date, status_sale

### sale_details
- Detalle de productos vendidos
- Campos: id_detail, id_sale, id_product, quantity, unit_price, total_price

### inventory_movements
- Historial de movimientos de inventario
- Campos: id_movement, id_product, movement_type, quantity, reason, id_user

## Funcionalidades por Módulo

### Módulo de Ventas
- ? Crear nueva venta
- ? Agregar/quitar productos del carrito
- ? Cálculo automático de totales
- ? Manejo de diferentes métodos de pago
- ? Actualización automática de inventario
- ? Generación de tickets
- ? Consulta de ventas

### Módulo de Productos
- ? Crear productos con validaciones
- ? Listar todos los productos
- ? Buscar producto por ID
- ? Actualizar información de productos
- ? Eliminación lógica de productos
- ? Actualización de stock

### Módulo de Reportes
- ? Reporte completo de inventario
- ? Productos con stock crítico
- ? Reportes de ventas por fecha
- ? Ranking de productos más vendidos
- ? Análisis de ingresos

## Datos de Prueba

El sistema incluye datos de ejemplo:
- Usuario administrador
- Cliente general para ventas
- 5 productos de ejemplo en diferentes categorías

## Configuración de MySQL

### Usando XAMPP
1. Inicia Apache y MySQL desde el panel de control de XAMPP
2. Abre phpMyAdmin (http://localhost/phpmyadmin)
3. Importa o ejecuta el script `database_script.sql`
4. Usa la cadena de conexión: `"Server=localhost;Database=pos_system;Uid=root;Pwd=;"`

### Usando MySQL Workbench
1. Abre MySQL Workbench
2. Conecta a tu servidor MySQL
3. Ejecuta el script `database_script.sql`
4. Configura la cadena de conexión según tu configuración

### Solución de Problemas Comunes

**Error de conexión:**
- Verifica que MySQL esté ejecutándose
- Confirma el usuario y contraseña
- Asegúrate de que la base de datos `pos_system` exista

**Error de permisos:**
- Verifica que el usuario tenga permisos de lectura/escritura
- Para desarrollo local, el usuario `root` funciona bien

## Soporte y Contribuciones

Para reportar errores o sugerir mejoras, puedes:
1. Crear un issue en el repositorio
2. Enviar un pull request con mejoras
3. Contactar al desarrollador

## Dependencias

- **MySql.Data**: Cliente oficial de MySQL para .NET
- **.NET 9.0**: Framework de desarrollo

## Licencia

Este proyecto está bajo la licencia MIT. Puedes usarlo libremente para fines educativos y comerciales.

---

**¡Gracias por usar nuestro Sistema de Punto de Venta con MySQL!** ??