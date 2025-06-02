using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Workflow.BasicActions
{
    public abstract class BaseAction
    {
        public Guid Id { get; set; }
        public string ActionName { get; set; }
        public string ActionType { get; set; }

    }
}
