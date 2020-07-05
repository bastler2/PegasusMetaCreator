using System;
using System.Collections.Generic;
using System.Text;

namespace PegasusMetaCreator
{
    class CollectionModel
    {
        public string collection { get; set; }
        public List<string> files { get; set; } = new List<string>();
    }
}
