using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;

namespace Proyecto_final_poo.Security
{
    public static class Auth
    {
        public static Usuario? Login(string username, string password)
        {
            try
            {
                using var c = Db.Con();
                c.Open();

                const string sql = @"
                SELECT Id, Username, Nombre, IsAdmin
                FROM Usuarios
                WHERE Username = @u AND PasswordHash = SHA2(@p, 256);";

                using var cmd = new MySqlCommand(sql, c);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                using var rd = cmd.ExecuteReader();
                if (!rd.Read()) return null;

                int ordId = rd.GetOrdinal("Id");
                int ordUser = rd.GetOrdinal("Username");
                int ordNombre = rd.GetOrdinal("Nombre");
                int ordIsAdmin = rd.GetOrdinal("IsAdmin");

                var usuario = new Usuario
                {
                    Id = rd.GetInt32(ordId),
                    Username = rd.IsDBNull(ordUser) ? "" : rd.GetString(ordUser),
                    Nombre = rd.IsDBNull(ordNombre) ? "" : rd.GetString(ordNombre),
                    IsAdmin = rd.GetBoolean(ordIsAdmin)
                };

                return usuario;
            }
            catch (MySqlException ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error de base de datos: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error al intentar iniciar sesión: {ex.Message}");
                return null;
            }
        }
    }
}
