using MiWebAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace MiWebAPI.Data
{
    public class ServicioData
    {
        private readonly string _conexion;

        public ServicioData(IConfiguration configuration)
        {
            _conexion = configuration.GetConnectionString("CadenaSQL")!;
        }

        // 1. Obtener todos los registros sin requerir ID
        public async Task<List<Servicio>> Lista()
        {
            var lista = new List<Servicio>();

            using (var con = new SqlConnection(_conexion))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("Servicio_ConsultarTodos", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lista.Add(new Servicio
                            {
                                IdServicio = Convert.ToInt32(reader["IdServicio"]),
                                Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : null,
                                Precio = reader["Precio"] != DBNull.Value ? Convert.ToDecimal(reader["Precio"]) : 0,
                                Activo = reader["Activo"] != DBNull.Value && Convert.ToBoolean(reader["Activo"])
                                // Ajusta las propiedades de acuerdo a tu modelo Servicio
                            });
                        }
                    }
                }
            }
            return lista;
        }

        // 2. Obtener solo un registro por ID
        public async Task<Servicio?> ObtenerPorId(int id)
        {
            Servicio? objeto = null;

            using (var con = new SqlConnection(_conexion))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("Servicio_ConsultarPorId", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdServicio", id);

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            objeto = new Servicio
                            {
                                IdServicio = Convert.ToInt32(reader["IdServicio"]),
                                Nombre = reader["Nombre"] != DBNull.Value ? reader["Nombre"].ToString() : null,
                                Precio = reader["Precio"] != DBNull.Value ? Convert.ToDecimal(reader["Precio"]) : 0,
                                Activo = reader["Activo"] != DBNull.Value && Convert.ToBoolean(reader["Activo"])
                            };
                        }
                    }
                }
            }
            return objeto;
        }

        // 3. Crear nuevo registro
        public async Task<bool> Crear(Servicio objeto)
        {
            using (var con = new SqlConnection(_conexion))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("Servicio_Insertar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Parámetros requeridos exactamente por el Stored Procedure
                    cmd.Parameters.AddWithValue("@Clave", objeto.Clave ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Descripcion", objeto.Descripcion ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DuracionMinutos", objeto.DuracionMinutos);
                    cmd.Parameters.AddWithValue("@Precio", objeto.Precio);

                    // En mi sp usamos el scopeidentity asi que por ende aqui vamos a utilizar ExecuteScalarAsync ya que captamos el valor de retorno que despues consultaremos
                    var idGenerado = await cmd.ExecuteScalarAsync();

                    return idGenerado != null && idGenerado != DBNull.Value;
                }

            }
        }

        // 4. Actualizar registro existente
        public async Task<bool> Editar(Servicio objeto)
        {
            using (var con = new SqlConnection(_conexion))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("Servicio_Actualizar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdServicio", objeto.IdServicio);
                    cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Precio", objeto.Precio);

                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    return filasAfectadas > 0;
                }
            }
        }

        // 5. Eliminar registro
        public async Task<bool> Eliminar(int id)
        {
            using (var con = new SqlConnection(_conexion))
            {
                await con.OpenAsync();
                using (var cmd = new SqlCommand("Servicio_Eliminar", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@IdServicio", id);

                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    return filasAfectadas > 0;
                }
            }
        }
    }
}