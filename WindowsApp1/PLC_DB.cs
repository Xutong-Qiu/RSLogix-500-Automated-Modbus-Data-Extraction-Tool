using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic.CompilerServices;

namespace WindowsApp1
{
    /// <summary>
/// PLC Database
/// An instance of this class stores all data of a RSS file. One database is binded to one file. To load a new file,
/// a new instance needs to be created.
/// </summary>
    public class PLC_DB
    {
        private Dictionary<string, DataEntry> addrDic;
        private Dictionary<string, string> tag_ref_list;
        private object programs;
        private string coil_start_addr = "";
        private List<string> modbusList;
        /// <summary>
    /// The constructor of the PLC database
    /// </summary>
    /// <param name="proj">a logixProject object</param>
        public PLC_DB(object proj)
        {
            if (proj is null)
            {
                throw new ArgumentException("The project instance is NULL.");
            }
            addrDic = new Dictionary<string, DataEntry>();
            tag_ref_list = new Dictionary<string, string>();
            var data_collection = proj.AddrSymRecords;
            this.LoadDataEntry(data_collection);
            programs = proj.ProgramFiles;
            modbusList = new List<string>();
        }
        /// <summary>
    /// This function checks whether the current database is empty
    /// </summary>
    /// <returns>A boolean that tells whether the current database is empty.</returns>
        public bool Empty()
        {
            return addrDic.Count == 0;
        }
        /// <summary>
    /// This function converts the current database into a list of string arrays that can be easily
    /// displayed on a data grid.
    /// </summary>
    /// <returns>A list of string array. Each array represents a data entry and has three
    /// elements: the entry's address, tag name, and description. Index is in order.</returns>
        public List<string[]> DBtoList()
        {
            var content = new List<string[]>();
            foreach (var addr in addrDic.Keys)
                content.Add(new[] { addr, addrDic[addr].TagName, addrDic[addr].Description });
            content.Sort(new WindowsApp1.DataEntryComparer());
            return content;
        }

        private void LoadDataEntry(object data_collection)
        {
            var numOfRec = data_collection.Count;
            object record;
            for (int i = 0, loopTo = Conversions.ToInteger(Operators.SubtractObject(numOfRec, 1)); i <= loopTo; i++)
            {
                record = data_collection.GetRecordViaIndex(i);
                var desp = record.Description;
                desp = desp.Replace(Environment.NewLine, " ");
                if (Conversions.ToBoolean(Operators.AndObject(record.Address is not null, Operators.ConditionalCompareObjectNotEqual(record.Address, "", false))))
                {
                    Add(Conversions.ToString(record.Address), Conversions.ToString(record.Symbol), Conversions.ToString(desp));
                }
            }
        }

        private string ChangeName(string Name, string extension)
        {
            string[] words = Name.Split('_');
            string changedName = "";
            foreach (var word in words)
            {
                if (tag_ref_list.ContainsKey(word))
                {
                    changedName += tag_ref_list[word] + "_";
                }
                else
                {
                    changedName += word + "_";
                }
            }
            if (!string.IsNullOrEmpty(extension))
            {
                changedName += extension;
            }
            return changedName;
        }
        /// <summary>
    /// This function loads all the modbus mapping information into the current database.
    /// </summary>
        public void LoadMapping()
        {
            coil_start = false;
            modbusList = new List<string>();
            try
            {
                LoadDefaultTagNameRef();
            }
            catch (FileNotFoundException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            var numOfProg = programs.Count();
            object modbus_file;
            for (int i = 0, loopTo = Conversions.ToInteger(Operators.SubtractObject(numOfProg, 1)); i <= loopTo; i++)
            {
                if (programs.Item(i) is not null && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(programs.Item(i).Name, "MODBUS", false))) // retrieve modbus program file
                {
                    modbus_file = programs.Item(i);
                    var numOfRung = modbus_file.NumberOfRungs;
                    for (int j = 0, loopTo1 = Conversions.ToInteger(Operators.SubtractObject(numOfRung, 1)); j <= loopTo1; j++) // iterate through rungs in the modbus file
                    {
                        var mappings = ExtractMapping(Conversions.ToString(modbus_file.GetRungAsAscii(j))); // for each rung, extract mappings embedded in it
                        foreach (var pair in mappings) // for each mapping pair
                        {
                            string extension = WindowsApp1.AddressSolver.GetExtension(pair.Item1);
                            string srcInfo = pair.Item1;
                            string src_addr = WindowsApp1.AddressSolver.Tune(pair.Item1);
                            string des_addr = pair.Item2;
                            if (ContainEntry(src_addr)) // if db contain src
                            {
                                string src_name = GetTagName(src_addr);
                                if (src_name is null || src_name == "ALWAYS_OFF") // skip always_off
                                {
                                    continue;
                                }
                                modbusList.Add(des_addr);
                                if (!ContainEntry(des_addr)) // if no mapping target, add mapping target
                                {
                                    Add(des_addr);
                                }
                                addrDic[src_addr].AddMappingTo(des_addr);
                                addrDic[des_addr].AddMappedTo(pair.Item1);
                                if (GetMappingSrc(des_addr).Count == 1)
                                {
                                    string target_name = ChangeName(src_name, extension);
                                    if (target_name.Length > 20)
                                    {
                                        MessageBox.Show("Tag Name exceeds 20 characters: " + target_name);
                                    }
                                    else
                                    {
                                        UpdateTagName(des_addr, target_name);
                                    }
                                    UpdateDescription(des_addr, addrDic[src_addr].Description);
                                }
                                else
                                {
                                    UpdateTagName(des_addr, "");
                                    UpdateDescription(des_addr, "");
                                }
                            }
                            else // handles exception
                            {
                                if (!ContainEntry(des_addr)) // if no mapping target, add mapping target
                                {
                                    Add(des_addr);
                                }
                                modbusList.Add(des_addr);
                                addrDic[des_addr].AddMappedTo(srcInfo);
                            }
                        }
                    }
                    break;
                }
            }
            modbusList = modbusList.Distinct().ToList();
        }

        public List<string[]> GetModbusData()
        {
            if (modbusList.Count == 0)
            {
                FindModbusData();
            }
            var content = new List<string[]>();
            var comp = new WindowsApp1.AddrComparer();
            modbusList.Sort(comp);
            int coil_start = Conversions.ToInteger(WindowsApp1.AddressSolver.ConvertCoilAddr(coil_start_addr));
            foreach (var addr in modbusList)
            {
                if (comp.Compare(addr, coil_start_addr.ToString()) > 0)
                {
                    content.Add(new[] { WindowsApp1.AddressSolver.ConvertCoilAddr(addr).ToString(), GetTagName(addr), GetDescription(addr) });
                }
                else
                {
                    content.Add(new[] { WindowsApp1.AddressSolver.ConvertRegAddr(addr).ToString(), GetTagName(addr), GetDescription(addr) });
                }
            }
            return content;
        }


        public void FindModbusData()
        {
            coil_start = false;
            var numOfProg = programs.Count();
            object modbus_file;
            for (int i = 0, loopTo = Conversions.ToInteger(Operators.SubtractObject(numOfProg, 1)); i <= loopTo; i++)
            {
                if (programs.Item(i) is not null && Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(programs.Item(i).Name, "MODBUS", false))) // retrieve modbus program file
                {
                    modbus_file = programs.Item(i);
                    var numOfRung = modbus_file.NumberOfRungs;
                    for (int j = 0, loopTo1 = Conversions.ToInteger(Operators.SubtractObject(numOfRung, 1)); j <= loopTo1; j++) // iterate through rungs in the modbus file
                    {
                        var mappings = ExtractMapping(Conversions.ToString(modbus_file.GetRungAsAscii(j))); // for each rung, extract mappings embedded in it
                        foreach (var pair in mappings) // for each mapping pair
                        {
                            string extension = WindowsApp1.AddressSolver.GetExtension(pair.Item1);
                            string src_addr = WindowsApp1.AddressSolver.Tune(pair.Item1);
                            string des_addr = pair.Item2;
                            if (ContainEntry(src_addr) && ContainEntry(des_addr)) // if db contain src
                            {
                                string src_name = GetTagName(src_addr);
                                if (src_name is null || src_name == "ALWAYS_OFF") // skip always_off
                                {
                                    continue;
                                }
                                modbusList.Add(des_addr);
                            }
                        }
                    }
                    break;
                }
            }
            modbusList = modbusList.Distinct().ToList();
        }

        /// <summary>
    /// This function checks whether a entry with the given address is present in the data base.
    /// This function must be called before any attempt to access an entry in this database to 
    /// avoid a possible exception.
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A boolean that indicates whether the entry is present or not.</returns>
        public bool ContainEntry(string addr)
        {
            return addrDic.ContainsKey(addr);
        }
        /// <summary>
    /// This function returns the tag name with the given address.
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A string that is the tag name with the given address.</returns>
        public string GetTagName(string addr)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            return addrDic[addr].TagName;
        }
        /// <summary>
    /// This function returns the description with the given address.
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A string that is the description with the given address.</returns>
        public string GetDescription(string addr)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            return addrDic[addr].Description;
        }
        /// <summary>
    /// This function returns the mapping target of the data entry with the given address. 
    /// i.e. the addresses that this data entry maps to. 
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A list of string that is the addresses of the mapping targets.</returns>
        public List<string> GetMappingTarget(string addr)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            return addrDic[addr].MappingTo;
        }
        /// <summary>
    /// This function returns the mapping source of the data entry with the given address. 
    /// i.e. the addresses that this data entry is mapped to. 
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A list of string that is the addresses of the mapping sources.</returns>
        public List<string> GetMappingSrc(string addr)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            return addrDic[addr].MappedTo;
        }
        /// <summary>
    /// This function adds a new entry to the database. It has a overload version that
    /// only takes in the address.
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <param name="name">The tag nane of the entry</param>
    /// <param name="desp">The description of the entry</param>
        public void Add(string addr, string name, string desp)
        {
            if (addrDic.ContainsKey(addr))
            {
                throw new ArgumentException("This data entry has presented in the database: " + addr);
            }
            addrDic.Add(addr, new DataEntry(addr, name, desp));
        }

        /// <summary>
    /// This function adds a new entry to the database. It has a overload version that
    /// takes in the address, tag name, and the description.
    /// </summary>
    /// <param name="addr">The address of the entry</param>
        public void Add(string addr)
        {
            if (addrDic.ContainsKey(addr))
            {
                throw new ArgumentException("This data entry has presented in the database: " + addr);
            }
            addrDic.Add(addr, new DataEntry(addr));
        }
        /// <summary>
    /// This function sets an entry with new tag name.
    /// </summary>
    /// <param name="addr">The address of the entry</param>
        public void UpdateTagName(string addr, string name)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            addrDic[addr].TagName = name;
        }
        /// <summary>
    /// This function sets an entry with a new description.
    /// </summary>
    /// <param name="addr">The address of the entry</param>
        public void UpdateDescription(string addr, string desp)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            addrDic[addr].Description = desp;
        }

        public void SetMappingLogic(string addr, Node logic)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            addrDic[addr].MappingLogic = logic;
        }
        /// <summary>
    /// This function returns the mapping logic the involves the data entry with
    /// the given address. It will be used in dealing with many to one mapping
    /// </summary>
    /// <param name="addr">The address of the entry</param>
    /// <returns>A node that represents the logic.</returns>
        public Node GetMappingLogic(string addr)
        {
            if (!addrDic.ContainsKey(addr))
            {
                throw new KeyNotFoundException("The data entry is not presented in the database: " + addr);
            }
            return addrDic[addr].MappingLogic;
        }
        /// <summary>
    /// This function finds all modified data entries after perform modbus mapping.
    /// </summary>
    /// <returns>A list of addresses that represents the modifed data entries.</returns>
        public List<string> GetModbusList()
        {
            return modbusList;
        }

        private object coil_start = false;
        // loadMapping() calls this function. It takes in a rung in text form and returns
        // all the mappings it finds as a list of tuples.
        private List<Tuple<string, string>> ExtractMapping(string str)
        {
            string[] words = str.Split(' ');
            var results = new List<Tuple<string, string>>();
            for (int i = 0, loopTo = words.Length - 3; i <= loopTo; i++)
            {
                // Check if the current rung is register mapping
                if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(coil_start, false, false)) && words[i] == "MOV")
                {
                    if (ContainEntry(words[i + 2]) && GetTagName(words[i + 2]) == "COIL_START")
                    {
                        coil_start = true;
                        coil_start_addr = words[i + 2];
                    }
                    else
                    {
                        Node logic = Parser.Parse(new LinkedList<string>(words));
                        WindowsApp1.RegLogicAnalyzer.FindRegMapping(logic, results);
                        return results;
                    }
                }
                // Check if the current rung is coil mapping
                if (Conversions.ToBoolean(coil_start && (object)(words[i] == "OR" || words[i] == "SWP")))
                {
                    // MessageBox.Show(str)
                    Node logic = Parser.Parse(new LinkedList<string>(words));
                    var cur = logic;
                    CoilLogicAnalyzer.FindCoilMapping(this, logic, results);
                    return results;
                }
            }
            return results;
        }

        public List<string[]> GetInvalidMapping()
        {
            var content = new List<string[]>();
            foreach (var addr in addrDic.Keys)
            {
                if (addrDic[addr].MappingTo.Count <= 1 && addrDic[addr].MappedTo.Count <= 1)
                {
                    continue;
                }
                string des = "";
                if (addrDic[addr].MappingTo.Count > 1)
                {
                    string target1 = GetMappingTarget(addr)[0];
                    string target2 = GetMappingTarget(addr)[1];
                    if ((GetTagName(target1) ?? "") == (GetTagName(target2) ?? ""))
                    {
                        foreach (var d in addrDic[addr].MappingTo)
                            des = Conversions.ToString(des + Operators.AddObject(d, " "));
                    }
                    else
                    {
                        continue;
                    }
                }
                string src = "";
                if (addrDic[addr].MappedTo.Count > 1)
                {
                    foreach (var target in addrDic[addr].MappedTo)
                        src = Conversions.ToString(src + Operators.AddObject(target, " "));
                }
                content.Add(new[] { addr, des, src });
            }
            content.Sort(new WindowsApp1.DataEntryComparer());
            return content;
        }

        public List<string[]> GetExceptionMapping()
        {
            var content = new List<string[]>();
            foreach (var addr in modbusList)
            {
                string src = "";
                foreach (var target in GetMappingSrc(addr))
                    src += target + " ";
                if (string.IsNullOrEmpty(GetTagName(addr)) || string.IsNullOrEmpty(GetDescription(addr)))
                {
                    content.Add(new[] { addr, GetTagName(addr), src, GetDescription(addr) });
                }
            }
            content.Sort(new WindowsApp1.DataEntryComparer());
            return content;
        }

        public void LoadDefaultTagNameRef()
        {
            string exePath = Application.StartupPath;
            string filePath = Path.Combine(exePath, "ref.xlsx");
            // Check if the file exists
            if (File.Exists(filePath))
            {
                tag_ref_list = WindowsApp1.IOHandler.LoadExcel(filePath);
            }
            else
            {
                throw new FileNotFoundException("The default translation table " + filePath + " Not Found!");
            }
        }
        public Dictionary<string, string> TagAbbrDictionary
        {
            get
            {
                return tag_ref_list;
            }
            set
            {
                tag_ref_list = value;
            }
        }

        public bool IsX()
        {
            FindModbusData();
            foreach (var addr in modbusList)
            {
                if (GetTagName(addr) is null | string.IsNullOrEmpty(GetTagName(addr)))
                {
                    return false;
                }
            }
            return true;
        }
    }
}