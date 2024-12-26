using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBroker_Consola.Cls
{
    public class NotifyService
    {
        public static async Task NotifyServiceAsync(object payload)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44348/api/Notificaciones");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                var jsonContent = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                try
                {
                    HttpResponseMessage response = await client.PostAsync("notificaciones", jsonContent);

                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Notificación enviada exitosamente al servicio intermedio.");
                    }
                    else
                    {
                        Console.WriteLine($"Error al enviar la notificación. Código de estado: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al conectar con el servicio intermedio: {ex.Message}");
                }
            }
        }
    }
}
