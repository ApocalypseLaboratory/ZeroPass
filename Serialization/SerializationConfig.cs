using System;

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
