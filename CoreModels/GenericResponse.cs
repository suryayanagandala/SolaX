using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModels
{
    public class GenericResponse<T>
    {
        public bool Status {  get; set; }
        public object? Data {  get; set; }
        public string? Error { get; set; }

        public string? Message { get; set; }
    }


    public class OutputResponse
    {
        public bool? Status { get; set; }

        public object? Data {  get; set; }

        public string? Error { get; set; }
    }


    public class OutPutResponse<T>
    {
        public bool? Status {  set; get; }

        public T? Data { get; set; }

        public string? Error { get; set; }
    }
}
