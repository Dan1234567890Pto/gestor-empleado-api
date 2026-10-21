using MiWebAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace MiWebAPI.Data
{
    public class ProveedorData
    {
        private readonly string conexion;

        public ProveedorData(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("CadenaSQL")!;
        }

        public async Task<List<Proveedor>> Lista()
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_LISTAR", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(new Proveedor
                        {
                            Id = Convert.ToInt64(reader["Id"]),
                            Nombre = reader["Nombre"].ToString() ?? "",
                            ApellidoPaterno = reader["ApellidoPaterno"].ToString() ?? "",
                            ApellidoMaterno = reader["ApellidoMaterno"].ToString() ?? "",
                            Rfc = reader["Rfc"].ToString() ?? "",
                            Correo = reader["Correo"].ToString() ?? "",
                            Credito = Convert.ToDecimal(reader["Credito"]),
                            NombreBanco = reader["NombreBanco"].ToString() ?? ""
                        });
                    }
                }
            }

            return lista;
        }

        public async Task<bool> Crear(Proveedor objeto)
        {
            bool respuesta = false;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_AGREGAR", con);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@ApellidoPaterno", objeto.ApellidoPaterno);
                cmd.Parameters.AddWithValue("@ApellidoMaterno", objeto.ApellidoMaterno);
                cmd.Parameters.AddWithValue("@Rfc", objeto.Rfc);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                cmd.Parameters.AddWithValue("@Credito", objeto.Credito);
                cmd.Parameters.AddWithValue("@NombreBanco", objeto.NombreBanco);
                cmd.CommandType = CommandType.StoredProcedure;

                try
                {
                    await con.OpenAsync();
                    int filasAfectadas = await cmd.ExecuteNonQueryAsync();
                    respuesta = filasAfectadas > 0;
                }
                catch (Exception)
                {
                    respuesta = false;
                }
            }

            return respuesta;
        }
    }
}