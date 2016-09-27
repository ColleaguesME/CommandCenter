using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace CommandCenter
{
    [DataContract]
    class OrderChange
    {
        static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(OrderChange));
        [DataMember]
        int index;
        [DataMember]
        string type;
        [DataMember]
        List<Order> changedOrders;
        public OrderChange(int index, string type, List<Order> changedOrders)
        {
            this.index = index;
            this.type = type;
            this.changedOrders = changedOrders;
        }
        public void WriteToStream(NetworkStream networkStream)
        {
            serializer.WriteObject(networkStream, this);

        }
    }
}
