using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace SaveSystem
{

    public enum SaveCollectionBehavior : byte
    {
        Auto, //Only Listeners
        AppendListeners, //Append existing listeners to specific data
        ReplaceListeners //Replace existing listeners with a new data
    }

}


