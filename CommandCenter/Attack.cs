using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        string type;

        [DataMember]
        public int x, y, spearman, swordsman, archer, heavyCavalry, axeFighter, lightCavalry, mountedArcher, ram, catapult;

        public byte[] GetBytes()
        {
            MemoryStream stream = new MemoryStream();
            serializer.WriteObject(stream, this);
            return stream.ToArray();

        }


    }
}
