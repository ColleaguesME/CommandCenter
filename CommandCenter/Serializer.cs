using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace CommandCenter
{
    public static class Serializer
    {
        static JavaScriptSerializer serializer = new JavaScriptSerializer();
        public static JavaScriptSerializer Get()
        {
            return serializer;
        }
    }
}
