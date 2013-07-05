using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;
using GameServerDB.UserManager;
using GameServerDB.ChanelManager;

namespace GameServerDB
{
    class Program
    {
        public static List<UserClass> Users = new List<UserClass>();
        public static List<Chanel> Chanels = new List<Chanel>();
        public static MySqlBase _SQL = new MySqlBase();
        static void Main(string[] args)
        {
            LogConsole._Load();
            LogDebug.debug = true;
            _SQL.Init("localhost", "root", "13456", "dbbase", 3306);
            var wssv = new WebSocketServiceHost<Serverb>("ws://192.168.1.5:8085");
            
            wssv.OnError += (sender, e) =>
                {
                    Console.WriteLine("[WS] error", "WS: Error: " + e.Message, "notification-message-im");
                };

            wssv.Start();
            
            Console.WriteLine("WebSocket Server listening on port: {0}", wssv.Port);
            while (true)
            {
                Thread.Sleep(1000);

                string _comm = Console.ReadLine();
                switch (_comm)
                {
                    case "count":
                        Console.WriteLine("Users Online: {0}",Users.Count());
                        break;
                    default:
                        break;
                }
            }
            //Console.ReadKey();
        }
    }
}
