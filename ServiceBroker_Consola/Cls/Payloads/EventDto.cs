using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceBroker_Consola.Cls.Payloads
{
    public class EventDto
    {
        public int ID_EVENTO_ACTIVO { get; set; }
        public int ID_EVENTO { get; set; }
        public string MATRICULA { get; set; }
        public int ID_NAVE { get; set; }
        public int ID_LINEA { get; set; }
        public int ID_CIRCUITO { get; set; }
        public int ID_SECTOR { get; set; }
        public DateTime FECHA { get; set; }
    }
}
