using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WindowsApp1
{
    /// <summary>
/// PLC Database
/// An instance of this class stores all data of a RSS file. One database is binded to one file. To load a new file,
/// a new instance needs to be created.
/// </summary>
    public class DataEntry
    {
        private string addr;
        private string name;
        private string desp;
        private List<string> des;
        private List<string> src;
        private string ext;
        private Node logic;
        private Regex addr_format = new Regex(@"^(?:([A-Z]{1,3})(\d{1,3}):(\d{1,3})|(?:(I|O|S|U):(\d{1,3}(?:\.\d{1,3})*)))(?:\/(\d{1,2}))*(.*)$");
        public DataEntry(string address, string name, string description)
        {
            var match = addr_format.Match(address);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid address format: " + address);
            }
            addr = address;
            this.name = name;
            desp = description;
            des = new List<string>();
            src = new List<string>();
            ext = "";
        }

        public DataEntry(string address)
        {
            var match = addr_format.Match(address);
            if (!match.Success)
            {
                throw new ArgumentException("Invalid address format: " + address);
            }
            addr = address;
            desp = "";
            name = "";
            des = new List<string>();
            src = new List<string>();
            ext = "";
        }

        public void CopyNameAndDesp(DataEntry other)
        {
            name = other.TagName;
            desp = other.Description;
        }
        public string Address
        {
            get
            {
                return addr;
            }
            set
            {
                addr = value;
            }
        }

        public string TagName
        {
            get
            {
                return name;
            }
            set
            {
                if ((name ?? "") != (value ?? ""))
                {
                }
                name = value;
            }
        }

        public string Description
        {
            get
            {
                return desp;
            }
            set
            {
                if ((desp ?? "") != (value ?? ""))
                {
                }
                desp = value;
            }
        }

        public List<string> MappingTo
        {
            get
            {
                return des;
            }
            set
            {
                des = value;
            }
        }

        public Node MappingLogic
        {
            get
            {
                return logic;
            }
            set
            {
                logic = value;
            }
        }
        public void AddMappingTo(string addr)
        {
            if (!MappingTo.Contains(addr))
            {
                des.Add(addr);
            }
        }
        public void AddMappedTo(string addr)
        {
            if (!MappedTo.Contains(addr))
            {
                src.Add(addr);
            }
        }
        public List<string> MappedTo
        {
            get
            {
                return src;
            }
            set
            {
                src = value;
            }
        }
        public string Extension
        {
            get
            {
                return ext;
            }
            set
            {
                ext = value;
            }
        }
    }
}