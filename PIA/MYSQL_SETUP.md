# Guía Rápida de Configuración MySQL

## Para XAMPP (Recomendado para principiantes)

1. **Descargar e instalar XAMPP**
   - Ir a https://www.apachefriends.org/
   - Descargar la versión para tu sistema operativo
   - Instalar con configuración por defecto

2. **Iniciar servicios**
   - Abrir el panel de control de XAMPP
   - Hacer clic en "Start" para Apache y MySQL
   - Los indicadores deben estar en verde

3. **Configurar la base de datos**
   - Abrir phpMyAdmin: http://localhost/phpmyadmin
   - Crear nueva base de datos llamada: `pos_system`
   - Importar o pegar el contenido del archivo `database_script.sql`

4. **Configurar conexión en el proyecto**
   - La configuración por defecto ya está lista:
   ```csharp
   connectionString = "Server=localhost;Database=pos_system;Uid=root;Pwd=;";
   ```

## Para MySQL Server independiente

1. **Instalar MySQL Server**
   - Descargar desde: https://dev.mysql.com/downloads/mysql/
   - Durante la instalación, establecer contraseña para root

2. **Instalar MySQL Workbench (opcional)**
   - Descargar desde: https://dev.mysql.com/downloads/workbench/
   - Interfaz gráfica para administrar MySQL

3. **Configurar la base de datos**
   - Conectar a MySQL con usuario root
   - Ejecutar el script `database_script.sql`

4. **Actualizar conexión en el código**
   ```csharp
   connectionString = "Server=localhost;Database=pos_system;Uid=root;Pwd=tu_contraseña;";
   ```

## Verificación de la instalación

Una vez configurado MySQL y ejecutada la base de datos:

1. **Ejecutar el proyecto**
   ```bash
   dotnet run
   ```

2. **Login de prueba**
   - Email: `admin@pos.com`
   - Contraseña: `admin123`

3. **Si funciona correctamente, verás:**
   - El menú principal del sistema
   - Puedes navegar por productos, ventas y reportes

## Problemas comunes

### Error de conexión
- ? Verificar que MySQL esté ejecutándose
- ? Comprobar usuario y contraseña en DatabaseConnection.cs
- ? Asegurarse de que la base de datos `pos_system` existe

### Error "Table doesn't exist"
- ? Ejecutar completamente el script database_script.sql
- ? Verificar que todas las tablas se crearon correctamente

### Error de permisos
- ? Usar el usuario root para desarrollo
- ? Si usas otro usuario, asignar permisos completos a la base de datos

## Estructura de la base de datos creada

Después de ejecutar el script tendrás:

- ? 6 tablas principales
- ? 1 usuario administrador
- ? 1 cliente general
- ? 5 productos de ejemplo
- ? Triggers para fechas de actualización

¡El sistema estará listo para usar! ??