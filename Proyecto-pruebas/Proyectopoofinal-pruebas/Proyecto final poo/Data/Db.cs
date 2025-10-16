using MySql.Data.MySqlClient;

namespace Proyecto_final_poo.Data
{
    public static class Db
    {
        private const string ConnStr = "server=localhost;port=3306;database=proyectopoo;user=root;password=hola12345;";

        public static MySqlConnection Con() => new MySqlConnection(ConnStr);

        public static void Init()
        {
            try
            {
                using (var cNoDb = new MySqlConnection("server=localhost;port=3306;user=root;password=hola12345;"))
                {
                    cNoDb.Open();
                    using var cmdDb = new MySqlCommand(
                        "CREATE DATABASE IF NOT EXISTS proyectopoo CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci;", cNoDb);
                    cmdDb.ExecuteNonQuery();
                }

                using var c = Con();
                c.Open();

                string sql = @"
CREATE TABLE IF NOT EXISTS Categorias(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Nombre VARCHAR(60) NOT NULL,
  Descripcion VARCHAR(255) NULL
);

CREATE TABLE IF NOT EXISTS Productos(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Nombre VARCHAR(100) NOT NULL,
  Precio DECIMAL(10,2) NOT NULL,
  Stock INT NOT NULL,
  CategoriaId INT NOT NULL,
  CONSTRAINT FK_Prod_Cat FOREIGN KEY (CategoriaId) REFERENCES Categorias(Id)
);

CREATE TABLE IF NOT EXISTS Clientes(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Nombre VARCHAR(120) NOT NULL,
  Telefono VARCHAR(30) NULL,
  Email VARCHAR(120) NULL
);

CREATE TABLE IF NOT EXISTS Ventas(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  ClienteId INT NOT NULL,
  Fecha DATETIME NOT NULL,
  Total DECIMAL(10,2) NULL,
  MetodoPago VARCHAR(30) NULL,
  EmpleadoId INT NULL,
  CONSTRAINT FK_Venta_Cliente FOREIGN KEY (ClienteId) REFERENCES Clientes(Id)
);

CREATE TABLE IF NOT EXISTS Detalles(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  VentaId INT NOT NULL,
  ProductoId INT NOT NULL,
  NombreProducto VARCHAR(120) NOT NULL,
  PrecioUnitario DECIMAL(10,2) NOT NULL,
  Cantidad INT NOT NULL,
  CONSTRAINT FK_Det_Venta FOREIGN KEY (VentaId) REFERENCES Ventas(Id),
  CONSTRAINT FK_Det_Prod FOREIGN KEY (ProductoId) REFERENCES Productos(Id)
);

CREATE TABLE IF NOT EXISTS Proveedores(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Nombre VARCHAR(120) NOT NULL,
  Telefono VARCHAR(30) NULL
);

CREATE TABLE IF NOT EXISTS Empleados(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Nombre VARCHAR(120) NOT NULL,
  Rol VARCHAR(60) NULL
);

CREATE TABLE IF NOT EXISTS Configuracion(
  Clave VARCHAR(50) PRIMARY KEY,
  ValorDecimal DECIMAL(10,4) NULL,
  ValorTexto VARCHAR(200) NULL
);

CREATE TABLE IF NOT EXISTS Usuarios(
  Id INT AUTO_INCREMENT PRIMARY KEY,
  Username VARCHAR(60) NOT NULL UNIQUE,
  PasswordHash CHAR(64) NOT NULL,
  Nombre VARCHAR(120) NULL,
  IsAdmin BIT NOT NULL DEFAULT 0
);";
                using var cmd = new MySqlCommand(sql, c);
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error al inicializar la base de datos: {ex.Message}", "Error de BD", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                throw;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error inesperado al inicializar: {ex.Message}", "Error", 
                    System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                throw;
            }
        }
    }
}