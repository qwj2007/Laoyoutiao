using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelImageImportToDB
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Nickname { get; set; }
        public string AvatarPath { get; set; }   // 头像文件相对路径
        public string Address { get; set; }
    }
}
