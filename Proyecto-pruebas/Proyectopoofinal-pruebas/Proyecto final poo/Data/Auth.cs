using MySql.Data.MySqlClient;
using Proyecto_final_poo.Data;
using Proyecto_final_poo.Domain;

namespace Proyecto_final_poo.Security
{
    public static class Auth
    {
        // metodo para intentar iniciar sesion, retorna el usuario si es correcto o null si falla
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
                System.Windows.Forms.MessageBox.Show($"error de base de datos: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"error al intentar iniciar sesion: {ex.Message}");
                return null;
            }
        }

        // metodo para cambiar la contrasena de un usuario, retorna true si la cambia y false si falla
        public static bool CambiarPassword(int usuarioId, string actual, string nueva, out string mensaje)
        {
            mensaje = "";
            using var c = Db.Con(); c.Open();
            // verifica contrasena actual
            using (var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM Usuarios WHERE Id=@id AND PasswordHash=SHA2(@p,256);", c))
            {
                cmd.Parameters.AddWithValue("@id", usuarioId);
                cmd.Parameters.AddWithValue("@p", actual);
                var ok = Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                if (!ok)
                {
                    mensaje = "contrasena actual incorrecta.";
                    return false;
                }
            }
            // cambia la contrasena
            using (var cmd2 = new MySqlCommand(
                "UPDATE Usuarios SET PasswordHash=SHA2(@n,256) WHERE Id=@id;", c))
            {
                cmd2.Parameters.AddWithValue("@n", nueva);
                cmd2.Parameters.AddWithValue("@id", usuarioId);
                cmd2.ExecuteNonQuery();
            }
            mensaje = "contrasena actualizada.";
            return true;
        }
    }
}