using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class UserData
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public UserData() { }

        public UserData(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
