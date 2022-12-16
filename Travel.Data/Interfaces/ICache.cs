using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel.Data.Interfaces
{
    public interface ICache
    {
        T Get<T>(string key);
        bool Set<T>(T data, string key);
        bool Update<T>(T data, string key);
        void Remove(string key);
    }
}
