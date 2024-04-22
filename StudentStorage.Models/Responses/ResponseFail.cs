using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentStorage.Models.Responses
{
    public class ResponseFail
    {
        public string Status => "fail";
        public object? Data { get; set; }
    }
}
