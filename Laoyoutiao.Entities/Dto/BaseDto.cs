using Laoyoutiao.WorkFlow.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laoyoutiao.Models.Dto
{
    public class BaseDto
    {
        public  long Id { get; set; }
        public virtual string? Url { get; set; }
        public virtual long? UserId { get; set; }
        public virtual WorkFlowStatus? WorkFlowStatus { get; set; }
    }
}
