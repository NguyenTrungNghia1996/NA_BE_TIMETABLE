using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NA_Entities.Entities.APIResponse
{
    public class ApiResponse<T>
    {
        public T Data { get; set; } = default!;
        public string Message { get; set; } = "";
        public string Status { get; set; } = "";
    }
}