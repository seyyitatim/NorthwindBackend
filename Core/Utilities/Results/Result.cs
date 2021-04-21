using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Utilities.Results
{
    public class Result : IResult
    {
        public Result(bool success,string message):this(success)//burada diğer contructer'ı tetikliyor.
        {
            Message = message;
        }
        public Result(bool success)
        {
            Success = success;
        }
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
