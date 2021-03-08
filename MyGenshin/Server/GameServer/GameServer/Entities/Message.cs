using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Entities
{
    class Message
    {
        public enum MessangeType
        {
            Mail,
            Friend
        }

        public int FromID;
        public string FromName;
          


    }
}
