using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITodaysPlus.Models
{
    public class BaseResponseModel<T>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }

    public class BaseResponseModel : BaseResponseModel<object>
    { }
}
