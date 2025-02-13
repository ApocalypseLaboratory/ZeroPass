using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeroPass.Serialization
{
    public sealed class SerializationConfig : Attribute
    {
        public MemberSerialization MemberSerialization
        {
            get;
            set;
        }

        public SerializationConfig(MemberSerialization memberSerialization)
        {
            MemberSerialization = memberSerialization;
        }
    }
}
