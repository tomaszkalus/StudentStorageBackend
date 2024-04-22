using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models.Responses
{
    public class ResponseError
    {
        public string Status => "error";
        public string Message { get; set; }
        public int? Code { get; set; }
        public object? Data { get; set; }
    }
}
