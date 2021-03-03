using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

using System.Threading;

using Network;
using GameServer.Managers;
using GameServer.Services;

namespace GameServer
{
    class GameServer
    {
        Thread thread;
        bool running = false;
        NetService network;

        public bool Init()
        {
            network = new NetService();
            network.Init(Properties.Settings.Default.ServerIP,Properties.Settings.Default.ServerPort);
            DBService.Instance.Init();
            UserService.Instance.Init();
            ItemService.Instance.Init();
            QuestService.Instance.Init();
            DataManager.Instance.Load();
            MapService.Instance.Init();
            EntityManager.Instance.Init();
            CharacterManager.Instance.Init();
        

            thread = new Thread(new ThreadStart(this.Update));

            return true;
        }

        public void Start()
        {
            network.Start();

            running = true;
            thread.Start();
        }


        public void Stop()
        {
            running = false;
            thread.Join();
            network.Stop();
           
        }

        public void Update()
        {
            while (running)
            {
                Time.Tick();
                Thread.Sleep(100);
                //Console.WriteLine("{0} {1} {2} {3} {4}", Time.deltaTime, Time.frameCount, Time.ticks, Time.time, Time.realtimeSinceStartup);
            }
        }
    }
}
