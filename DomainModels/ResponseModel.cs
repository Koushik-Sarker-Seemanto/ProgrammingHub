using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModels
{
    public class ResponseModel
    {
        public Object Data { get; set; }
        public string Response { get; set; }
        public ResponseEnum ResponseStatus { get; set; }
    }
}
