using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WatTvdb.V1
{
    public class TvdbAsyncResult<T>
    {
        public T Data { get; set; }
        public object UserState { get; set; }
    }
}
