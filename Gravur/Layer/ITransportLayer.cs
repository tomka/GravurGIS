using System;
using System.Collections.Generic;
using System.Text;

namespace GravurGIS.Layers
{
    interface ITransportLayer
    {
        int BbHeightMargin
        {
            get;
            set;
        }
        int BbWidthMargin
        {
            get;
            set;
        }
        int Count
        {
            get;
        }
    }
}
