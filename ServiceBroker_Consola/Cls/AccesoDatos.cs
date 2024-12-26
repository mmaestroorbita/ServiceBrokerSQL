using System.Data;
using System.Data.SqlClient;

namespace ServiceBroker_Consola.Cls
{
  internal static class AccesoDatos
  {
    // Cadena de conexión a la BBDD SQL
    public static string cadenaConexion = @"Data Source=localhost;Initial Catalog=prueba;User ID=sa;Password=NuevaContraseñaSegura123!;MultipleActiveResultSets=True;";

    /// <summary>
    /// Ejecuta una consulta SQL en la base de datos, y devuelve los resultados obtenidos en un objeto DataTable.
    /// </summary>
    /// <param name="SQL"></param>
    /// <param name="parametros">Array de string con formato: nombre:valor</param>
    /// <returns>Devuelve un objeto DataTable con los resultados obtenidos tras ejecución de la consulta.</returns>
    public static DataTable GetDataTable(string SQL, string[] parametros)
    {
      try
      {
        SqlConnection conexion = new SqlConnection(cadenaConexion);
        SqlCommand comando = new SqlCommand(SQL, conexion);
        for (int i = 0; i < parametros.Length; i++)
          comando.Parameters.Add(new SqlParameter(parametros[i].Split(':')[0], parametros[i].Split(':')[1]));
        SqlDataAdapter da = new SqlDataAdapter(comando);
        DataSet ds = new DataSet();
        da.Fill(ds);
        conexion.Close();
        da.Dispose();
        conexion.Dispose();
        return ds.Tables[0];
      }
      catch (Exception)
      {
        throw;
      }
    }


        /// <summary>
        /// Ejecuta una consulta SQL en la base de datos, y devuelve los resultados obtenidos en un objeto DataTable.
        /// A diferencia de GetDataTable, este método es vulnerable a la inyección de dependencias, por lo que se recomienda usar sólo para procesos temporales internos.
        /// </summary>
        /// <param name="SQL"></param>
        /// <returns>Devuelve un DataTable con los resultados de la ejecución de la consulta.</returns>
        public static DataTable GetTmpDataTable(string SQL)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    // Verificar que la conexión esté abierta
                    if (conexion.State != ConnectionState.Open)
                        conexion.Open();

                    using (SqlDataAdapter comando = new SqlDataAdapter(SQL, conexion))
                    {
                        if (comando == null)
                            throw new Exception("No se pudo crear el SqlDataAdapter");

                        DataSet ds = new DataSet();

                        // Verificar que ds no sea null
                        if (ds == null)
                            throw new Exception("No se pudo crear el DataSet");

                        // Intentar llenar el DataSet
                        comando.Fill(ds);

                        // Verificar que tengamos tablas y filas
                        if (ds.Tables.Count > 0)
                        {
                            if (ds.Tables[0].Rows.Count > 0)
                                return ds.Tables[0];
                            else
                                throw new Exception("La consulta no devolvió filas");
                        }
                        else
                            throw new Exception("La consulta no devolvió ninguna tabla");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Manejar específicamente errores de SQL
                throw new Exception($"Error de SQL: {sqlEx.Message}", sqlEx);
            }
            catch (Exception ex)
            {
                // Manejar otros errores
                throw new Exception($"Error general: {ex.Message}", ex);
            }
        }



        public static void ExecuteNonQuery(string SQL, params SqlParameter[] parametros)
        {
            try
            {
                using (SqlConnection conexion = new SqlConnection(cadenaConexion))
                {
                    using (SqlCommand comando = new SqlCommand(SQL, conexion))
                    {
                        comando.Parameters.AddRange(parametros);
                        conexion.Open();
                        comando.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
