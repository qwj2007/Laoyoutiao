using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Common
{
    public class BaseTreeEntity<T>:BaseEntity
    {        
        [SugarColumn(IsIgnore = true)]
        public virtual List<T> Children { get; set; }
        [SugarColumn(IsNullable = false, ColumnDescription = "上级")]
        public virtual long ParentId { get; set; }
    }
}
