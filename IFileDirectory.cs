using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZeroPass
{
    public interface IFileDirectory
    {
        string GetRoot();

        byte[] ReadBytes(string filename);

        void GetFiles(Regex re, string path, ICollection<string> result);

        string GetID();

        bool FileExists(string path);
    }
}
