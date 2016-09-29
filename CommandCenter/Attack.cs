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
    class Attack
    {
        static DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(Attack));

        [DataMember]
        double time;
        [DataMember]
        string type;

        [DataMember]
        public int x, y, spearman, swordsman, archer, heavyCavalry, axeFighter, lightCavalry, mountedArcher, ram, catapult;

        public void WriteToStream(NetworkStream networkStream)
        {
            serializer.WriteObject(networkStream, this); 

        }


    }
}
