using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringOperation
{
    public interface ICondition
    {

    }

    public interface IOperation
    {

        void Exec(ICondition condition);
    }
}
