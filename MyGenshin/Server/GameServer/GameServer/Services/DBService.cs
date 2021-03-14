﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Common;

namespace GameServer.Services
{
    class DBService : Singleton<DBService>
    {
        ExtremeWorldEntities entities;

        public ExtremeWorldEntities Entities
        {
            get { return this.entities; }
        }

        public void Init()
        {
            entities = new ExtremeWorldEntities();
        }

        public async void Save(bool async=true)
        {
            if (async)
            {
               await  this.Entities.SaveChangesAsync();
            }
            else
            {
                this.Entities.SaveChanges();
            }
            
        }

    }
}
