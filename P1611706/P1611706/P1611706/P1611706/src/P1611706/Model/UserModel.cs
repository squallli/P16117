using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace P1611706.Model
{
    public class UserModel
    {

        public string UserName { get; set; }
        public string EmployeeNo { get; set; }
        public Dictionary<string, Program> ProgramDictionary { get; set; }
        
    }
    public class Program
    {
        public List<string> ProgID { get; set; }
        public List<string> ProgURL { get; set; }
        public List<string> ProgName { get; set; }
    }
}