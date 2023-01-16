using System;
using System.Collections.Generic;
using System.Text;

namespace DocsPlugin.ResponseModels
{
    public class BaseResponseModel<T>
    {
        public int Code { get; set; }

        public string Message { get; set; }

        public T Data { get; set; }
    }
}
