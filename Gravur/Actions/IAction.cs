using System;
using System.Collections.Generic;
using System.Text;

namespace GravurGIS.Actions
{
    public interface IAction
    {
       /// <summary>
       /// performs Actions and sets state parameters to undo it
       /// </summary>
       /// <returns>whether Action could be performed or not</returns>
       bool Execute();

       void UnExecute();
       void Dispose();
    }
}
