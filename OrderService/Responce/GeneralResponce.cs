using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace OrderService.Request_Responce
{
    public class GeneralResponse
    {
        public int _status { get; set; }
        public string _message { get; set; }
        public object? _objectType { get; set; }

        public GeneralResponse(int status, string message)
        {
            _status = status;
            _message = message;
        }

        public GeneralResponse(int status, string message, object objectType)
        {
            _status = status;
            _message = message;
            _objectType = objectType;
        }
    }
}