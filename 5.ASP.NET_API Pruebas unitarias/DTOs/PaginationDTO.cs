using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASP.NET_API.DTOs
{
    /// <summary>
    /// Modelo para paginacion
    /// </summary>
    public class PaginationDTO
    {
        public int Page { get; set; }
        private int recordsPerPage = 10;
        private readonly int maxRecordsPerPage = 50;

        public int RecordsPerPage
        {
            get
            {
                return recordsPerPage;
            }

            set
            {
                recordsPerPage = (value > maxRecordsPerPage) ? maxRecordsPerPage : value;
                //evalua si el valor ingresado es mayor que el numero maximo de pagina
            }
        }
        
    }
}
