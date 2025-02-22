﻿using System.Collections.Generic;

namespace ZeroPass
{
    public struct TagBits
    {
        private static Dictionary<Tag, int> tagTable = new Dictionary<Tag, int>();

        private static List<Tag> inverseTagTable = new List<Tag>();

        private const int Capacity = 384;

        private ulong bits0;

        private ulong bits1;

        private ulong bits2;

        private ulong bits3;

        private ulong bits4;

        private ulong bits5;

        public static TagBits None = default(TagBits);

        public TagBits(ref TagBits other)
        {
            bits0 = other.bits0;
            bits1 = other.bits1;
            bits2 = other.bits2;
            bits3 = other.bits3;
            bits4 = other.bits4;
            bits5 = other.bits5;
        }

        public TagBits(Tag tag)
        {
            bits0 = 0uL;
            bits1 = 0uL;
            bits2 = 0uL;
            bits3 = 0uL;
            bits4 = 0uL;
            bits5 = 0uL;
            SetTag(tag);
        }

        public TagBits(Tag[] tags)
        {
            bits0 = 0uL;
            bits1 = 0uL;
            bits2 = 0uL;
            bits3 = 0uL;
            bits4 = 0uL;
            bits5 = 0uL;
            if (tags != null)
            {
                foreach (Tag tag in tags)
                {
                    SetTag(tag);
                }
            }
        }

        public List<Tag> GetTagsVerySlow()
        {
            List<Tag> list = new List<Tag>();
            GetTagsVerySlow(0, bits0, list);
            GetTagsVerySlow(1, bits1, list);
            GetTagsVerySlow(2, bits2, list);
            GetTagsVerySlow(3, bits3, list);
            GetTagsVerySlow(4, bits4, list);
            GetTagsVerySlow(5, bits5, list);
            return list;
        }

        private void GetTagsVerySlow(int bits_idx, ulong bits, List<Tag> tags)
        {
            for (int i = 0; i < 64; i++)
            {
                if (((long)bits & (1L << i)) != 0)
                {
                    int index = 64 * bits_idx + i;
                    tags.Add(inverseTagTable[index]);
                }
            }
        }

        private static int ManifestFlagIndex(Tag tag)
        {
            if (tagTable.TryGetValue(tag, out int value))
            {
                return value;
            }
            value = tagTable.Count;
            tagTable.Add(tag, value);
            inverseTagTable.Add(tag);
            DebugUtil.Assert(inverseTagTable.Count == value + 1);
            if (tagTable.Count >= 384)
            {
                string text = "Out of tag bits:\n";
                int num = 0;
                foreach (KeyValuePair<Tag, int> item in tagTable)
                {
                    text = text + item.Key.ToString() + ", ";
                    num++;
                    if (num % 64 == 0)
                    {
                        text += "\n";
                    }
                }
                Debug.LogError(text);
            }
            return value;
        }

        public void SetTag(Tag tag)
        {
            int num = ManifestFlagIndex(tag);
            if (num < 64)
            {
                bits0 |= (ulong)(1L << num);
            }
            else if (num < 128)
            {
                bits1 |= (ulong)(1L << num);
            }
            else if (num < 192)
            {
                bits2 |= (ulong)(1L << num);
            }
            else if (num < 256)
            {
                bits3 |= (ulong)(1L << num);
            }
            else if (num < 320)
            {
                bits4 |= (ulong)(1L << num);
            }
            else if (num < 384)
            {
                bits5 |= (ulong)(1L << num);
            }
            else
            {
                Debug.LogError("Out of bits!");
            }
        }

        public void Clear(Tag tag)
        {
            int num = ManifestFlagIndex(tag);
            if (num < 64)
            {
                bits0 &= (ulong)(~(1L << num));
            }
            else if (num < 128)
            {
                bits1 &= (ulong)(~(1L << num));
            }
            else if (num < 192)
            {
                bits2 &= (ulong)(~(1L << num));
            }
            else if (num < 256)
            {
                bits3 &= (ulong)(~(1L << num));
            }
            else if (num < 320)
            {
                bits4 &= (ulong)(~(1L << num));
            }
            else if (num < 384)
            {
                bits5 &= (ulong)(~(1L << num));
            }
            else
            {
                Debug.LogError("Out of bits!");
            }
        }

        public void ClearAll()
        {
            bits0 = 0uL;
            bits1 = 0uL;
            bits2 = 0uL;
            bits3 = 0uL;
            bits4 = 0uL;
            bits5 = 0uL;
        }

        public bool HasAll(ref TagBits tag_bits)
        {
            return (bits0 & tag_bits.bits0) == tag_bits.bits0 && (bits1 & tag_bits.bits1) == tag_bits.bits1 && (bits2 & tag_bits.bits2) == tag_bits.bits2 && (bits3 & tag_bits.bits3) == tag_bits.bits3 && (bits4 & tag_bits.bits4) == tag_bits.bits4 && (bits5 & tag_bits.bits5) == tag_bits.bits5;
        }

        public bool HasAny(ref TagBits tag_bits)
        {
            return ((bits0 & tag_bits.bits0) | (bits1 & tag_bits.bits1) | (bits2 & tag_bits.bits2) | (bits3 & tag_bits.bits3) | (bits4 & tag_bits.bits4) | (bits5 & tag_bits.bits5)) != 0;
        }

        public bool AreEqual(ref TagBits tag_bits)
        {
            return tag_bits.bits0 == bits0 && tag_bits.bits1 == bits1 && tag_bits.bits2 == bits2 && tag_bits.bits3 == bits3 && tag_bits.bits4 == bits4 && tag_bits.bits5 == bits5;
        }

        public void And(ref TagBits rhs)
        {
            bits0 &= rhs.bits0;
            bits1 &= rhs.bits1;
            bits2 &= rhs.bits2;
            bits3 &= rhs.bits3;
            bits4 &= rhs.bits4;
            bits5 &= rhs.bits5;
        }

        public void Or(ref TagBits rhs)
        {
            bits0 |= rhs.bits0;
            bits1 |= rhs.bits1;
            bits2 |= rhs.bits2;
            bits3 |= rhs.bits3;
            bits4 |= rhs.bits4;
            bits5 |= rhs.bits5;
        }

        public void Xor(ref TagBits rhs)
        {
            bits0 ^= rhs.bits0;
            bits1 ^= rhs.bits1;
            bits2 ^= rhs.bits2;
            bits3 ^= rhs.bits3;
            bits4 ^= rhs.bits4;
            bits5 ^= rhs.bits5;
        }

        public void Complement()
        {
            bits0 = ~bits0;
            bits1 = ~bits1;
            bits2 = ~bits2;
            bits3 = ~bits3;
            bits4 = ~bits4;
            bits5 = ~bits5;
        }

        public static TagBits MakeComplement(ref TagBits rhs)
        {
            TagBits result = new TagBits(ref rhs);
            result.Complement();
            return result;
        }
    }
}
