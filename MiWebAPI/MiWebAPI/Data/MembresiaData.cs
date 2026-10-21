
using MiWebAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace MiWebAPI.Data
{
    public class MembresiaData
    {
        private readonly string conexion;

        public MembresiaData(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("CadenaSQL")!;
        }

        // 1. LISTAR TODOS 
        public async Task<List<Membresia>> Lista()
        {
            List<Membresia> lista = new List<Membresia>();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.Membresia_ConsultarTodos", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Membresia
                        {
                            IdMembresia = Convert.ToInt32(reader["IdMembresia"]),
                            Clave = reader["Clave"].ToString() ?? "",
                            Nombre = reader["Nombre"].ToString() ?? "",
                            Descripcion = reader["Descripcion"] as string,
                            DuracionDias = Convert.ToInt32(reader["DuracionDias"]),
                            Precio = Convert.ToDecimal(reader["Precio"]),
                            Activo = Convert.ToBoolean(reader["Activo"]),
                            FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                            FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : null
                        });
                    }
                }
            }

            return lista;
        }

        
        // 2. CONSULTAR POR ID (CORREGIDO)
        public async Task<Membresia?> ObtenerPorId(int id)
        {
            Membresia? objeto = null;

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                // Asegúrate de que llame a Membresia_ConsultarPorId
                SqlCommand cmd = new SqlCommand("dbo.Membresia_ConsultarPorId", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@IdMembresia", id); // Pasa el parámetro ID

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        objeto = new Membresia
                        {
                            IdMembresia = Convert.ToInt32(reader["IdMembresia"]),
                            Clave = reader["Clave"].ToString() ?? "",
                            Nombre = reader["Nombre"].ToString() ?? "",
                            Descripcion = reader["Descripcion"] as string,
                            DuracionDias = Convert.ToInt32(reader["DuracionDias"]),
                            Precio = Convert.ToDecimal(reader["Precio"]),
                            Activo = Convert.ToBoolean(reader["Activo"]),
                            FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                            FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(reader["FechaActualizacion"]) : null
                        };
                    }
                }
            }

            return objeto;
        }
       
        public async Task<bool> Crear(Membresia objeto)
        {
            bool respuesta = false;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.Membresia_Insertar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Clave", objeto.Clave);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)objeto.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DuracionDias", objeto.DuracionDias);
                cmd.Parameters.AddWithValue("@Precio", objeto.Precio);

                var paramId = new SqlParameter("@IdMembresia", SqlDbType.Int) { Direction = ParameterDirection.Output };
                cmd.Parameters.Add(paramId);

                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();

                    // Verificamos si la BD generó un ID de salida válido
                    if (paramId.Value != DBNull.Value && Convert.ToInt32(paramId.Value) > 0)
                    {
                        objeto.IdMembresia = Convert.ToInt32(paramId.Value);
                        respuesta = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en Crear: {ex.Message}");
                    respuesta = false;
                }
            }

            return respuesta;
        }

        // 4. ACTUALIZAR
        public async Task<bool> Editar(Membresia objeto)
        {
            bool respuesta = false;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.Membresia_Actualizar", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@IdMembresia", objeto.IdMembresia);
                cmd.Parameters.AddWithValue("@Clave", objeto.Clave);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Descripcion", (object?)objeto.Descripcion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@DuracionDias", objeto.DuracionDias);
                cmd.Parameters.AddWithValue("@Precio", objeto.Precio);

                try
                {
                    await con.OpenAsync();
                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();

                    // Si el procedimiento tiene SET NOCOUNT ON o modifica filas, consideramos éxito si no hay excepción
                    respuesta = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en Editar: {ex.Message}");
                    respuesta = false;
                }
            }

            return respuesta;
        }

        // 5. ELIMINAR (Baja lógica)
        public async Task<bool> Eliminar(int id)
        {
            bool respuesta = false;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.Membresia_Eliminar", con);
                cmd.Parameters.AddWithValue("@IdMembresia", id);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await con.OpenAsync();
                    await cmd.ExecuteNonQueryAsync();
                    respuesta = true; // Si no hay excepción, la baja se procesó
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error en Eliminar: {ex.Message}");
                    respuesta = false;
                }
            }

            return respuesta;
        }

    }
}
