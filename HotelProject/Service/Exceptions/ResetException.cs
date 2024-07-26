using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Exceptions
{
    public class ResetException
    {
        public class RestException : Exception
        {
            public RestException()
            {
            }
            public RestException(int code, string message)
            {
                this.Code = code;
                this.Message = message;
            }

            public RestException(int code, string errorKey, string errorMessage, string? message = null)
            {
                this.Code = code;
                this.Message = message;
                this.Errors = new List<RestExceptionError> { new RestExceptionError(errorKey, errorMessage) };
            }

            public int Code { get; set; }
            public string? Message { get; set; }
            public List<RestExceptionError> Errors { get; set; } = new List<RestExceptionError>();
        }

        public class RestExceptionError
        {
            public RestExceptionError()
            {

            }
            public RestExceptionError(string key, string message)
            {
                this.Key = key;
                this.Message = message;
            }
            public string Key { get; set; }
            public string Message { get; set; }
        }
    }
}
