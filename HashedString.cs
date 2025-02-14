using System;
using UnityEngine;
using ZeroPass.Serialization;

namespace ZeroPass
{
    [Serializable]
    [SerializationConfig(MemberSerialization.OptIn)]
    public struct HashedString : IComparable<HashedString>, IEquatable<HashedString>, ISerializationCallbackReceiver
    {
        public static HashedString Invalid = default(HashedString);

        [SerializeField]
        [Serialize]
        private int hash;

        public bool IsValid => HashValue != 0;

        public int HashValue
        {
            get
            {
                return hash;
            }
            set
            {
                hash = value;
            }
        }

        public HashedString(string name)
        {
            hash = Hash.SDBMLower(name);
        }

        public HashedString(int initial_hash)
        {
            hash = initial_hash;
        }

        public static implicit operator HashedString(string s)
        {
            return new HashedString(s);
        }

        public int CompareTo(HashedString obj)
        {
            return hash - obj.hash;
        }

        public override bool Equals(object obj)
        {
            HashedString hashedString = (HashedString)obj;
            return hash == hashedString.hash;
        }

        public bool Equals(HashedString other)
        {
            return hash == other.hash;
        }

        public override int GetHashCode()
        {
            return hash;
        }

        public static bool operator ==(HashedString x, HashedString y)
        {
            return x.hash == y.hash;
        }

        public static bool operator !=(HashedString x, HashedString y)
        {
            return x.hash != y.hash;
        }


        public override string ToString()
        {
            return ((ValueType)this).ToString();
        }

        public void OnAfterDeserialize()
        {
        }

        public void OnBeforeSerialize()
        {
        }
    }
}
