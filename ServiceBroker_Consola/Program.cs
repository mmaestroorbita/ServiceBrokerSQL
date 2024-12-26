using Newtonsoft.Json;
using ServiceBroker_Consola.Cls;
using ServiceBroker_Consola.Cls.Payloads;
using System.Data;
using System.Data.SqlClient;
using System.Text;

// Definición de la clase principal
public class Program
{
    private static int? ultimoNumeroEventos = null;

    public static void Main(string[] args)
    {
       
        AccesoDatos.cadenaConexion = @"Data Source=localhost;Initial Catalog=prueba;User ID=sa;Password=NuevaContraseñaSegura123!;MultipleActiveResultSets=True;";

        var sb = new ServiceBrokerSQL(
            AccesoDatos.cadenaConexion,
            @"DECLARE @conversationHandle UNIQUEIDENTIFIER;
            DECLARE @message_body VARBINARY(MAX);
            RECEIVE TOP(1)
                @conversationHandle = conversation_handle,
                @message_body = message_body
            FROM EventosActivosQueue;
            SELECT 
                CAST(@message_body AS NVARCHAR(MAX)) AS mensaje,
                @conversationHandle AS conversation_handle;",
            "ESCUCHAR_EVENTOS_ACTIVOS"
        );

        sb.OnMensajeRecibido += new ServiceBrokerSQL.MensajeRecibido(sbColaTareas_InformacionRecibida);
        sb.IniciarEscucha();

        Console.Clear();
        Console.WriteLine("Esperando mensajes de Service Broker...");
        Thread.Sleep(Timeout.Infinite);

        sb.DetenerEscucha();
    }

    private static void sbColaTareas_InformacionRecibida(object sender, string nombreMensaje)
    {
        try
        {
            string receiveQuery = @"
                DECLARE @conversationHandle UNIQUEIDENTIFIER;
                DECLARE @message_body VARBINARY(MAX);
                
                RECEIVE TOP(1)
                    @conversationHandle = conversation_handle,
                    @message_body = message_body
                FROM EventosActivosQueue;
                
                SELECT 
                    CAST(@message_body AS NVARCHAR(MAX)) as mensaje,
                    @conversationHandle as conversation_handle;";


            DataTable dt = AccesoDatos.GetTmpDataTable(receiveQuery);

            if (dt.Rows.Count > 0 && dt.Rows[0]["mensaje"] != DBNull.Value)
            {
                string mensaje = dt.Rows[0]["mensaje"].ToString();
                Guid? conversationHandle = null;

                if (dt.Rows[0]["conversation_handle"] != DBNull.Value)
                {
                    conversationHandle = (Guid)dt.Rows[0]["conversation_handle"];
                }

                if (!string.IsNullOrEmpty(mensaje))
                {
                    var payload = JsonConvert.DeserializeObject<List<EventDto>>(mensaje).FirstOrDefault();
                    
                    NotifyService.NotifyServiceAsync(payload).Wait();
                    Console.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} Mensaje recibido y procesado: {mensaje}");
                }

                if (conversationHandle.HasValue)
                {
                    string endConversationQuery = "END CONVERSATION @conversationHandle;";
                    SqlParameter parameter = new SqlParameter("@conversationHandle", SqlDbType.UniqueIdentifier)
                    {
                        Value = conversationHandle.Value
                    };
                    AccesoDatos.ExecuteNonQuery(endConversationQuery, parameter);
                }
            }

            var sb = (ServiceBrokerSQL)sender;
            sb.IniciarEscucha();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
