using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using WebSocketSharp;
//using WebSocketSharp.Net;
using WebSocketSharp.Server;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#region Using
using GameServerDB.UserManager;
using GameServerDB.ChanelManager;
#endregion


using System.Web;

namespace GameServerDB
{
    public class Serverb : WebSocketService
    {
        UserClass User = new UserClass();

        ChanelManager.Chanel _test = new ChanelManager.Chanel();

        public int _uid = 0;
        public Serverb()
        {
            
            
        }
        protected override void OnOpen()
        {
            string _hi_p = "[9,22,\"Server 2 - Pro\"]";
            Send(_hi_p);
            Console.WriteLine("id: {0}", this.ID);
            User.sep = this;
            Program.Users.Add(User);

            //test
            Random rand = new Random();
            _test.chanel_id = rand.Next(1, 6);
            _test.chanel_map = -1;
            _test.chanel_maxplayers = 8;
            _test.chanel_name = "Test";
            _test.chanel_poweruse = 0;
            _test.chanel_status = 0;
            _test.chanel_useronli = 1;
            _test.chanel_gamemode = 0;
            Program.Chanels.Add(_test);
        }
        protected override void OnClose(CloseEventArgs e)
        {
            Program.Users.Remove(User);

            //test
            Program.Chanels.Remove(_test);
        }
        
        protected override void OnMessage(MessageEventArgs e)
        {
            switch (e.Type)
            {
                case Opcode.TEXT:
                    {
                        JArray dcode = JArray.Parse(e.Data);
                        ClientOpcode opc = (ClientOpcode)int.Parse(dcode[0].ToString());
                        switch (opc)
                        {
                            case ClientOpcode.login:
                                {
                                    User.version = dcode[1].ToObject<int>();
                                    User.user_id = dcode[2].ToObject<int>();
                                    User.user_key = dcode[3].ToObject<string>();
                                    User.Login();
                                    Chat.ChatManager.Notice(User);
                                    Chat.ChatManager.UpdateBoddy(this);
                                    Chanel.SendList(this);
                                    break;
                                }
                            case ClientOpcode.chat:
                                {
                                    Broadcast(Chat.ChatManager.Msj(dcode[1].ToString(),User));
                                    break;
                                }
                            case ClientOpcode.room_create:
                                {
                                    Chanel _new_c = new Chanel();
                                    _new_c.chanel_id = new Random().Next(2, 9);
                                    _new_c.chanel_name = dcode[1].ToObject<string>();
                                    _new_c.chanel_password = dcode[2].ToObject<string>();
                                    _new_c.chanel_maxplayers = dcode[3].ToObject<int>();
                                    _new_c.chanel_gamemode = dcode[4].ToObject<int>();
                                    User.Is_Master = 1;
                                    _new_c.Join(User);
                                    Program.Chanels.Add(_new_c);
                                    Chanel.SendList(this);
                                    break;
                                }
                            case ClientOpcode.room_join:
                                {
                                    int _sala_id = dcode[1].ToObject<int>();
                                    Chanel _cha = Program.Chanels.Single(a => a.chanel_id == _sala_id);
                                    if (_cha != null)
                                    {
                                        _cha.Join(User);
                                    }
                                    break;
                                }
                            case ClientOpcode.channel_join:
                                {
                                    Chanel _chan = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_chan != null)
                                    {
                                        _chan.RemoveUser(User);
                                    }
                                    User.PlayerInfo();
                                    Chat.ChatManager.UpdateBoddy(this);
                                    Chanel.SendList(this);
                                    break;
                                }
                            case ClientOpcode.mobile:
                                {
                                    Chanel _cha = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_cha != null)
                                    {
                                        int _mobil = dcode[1].ToObject<int>();
                                        _cha.ChangeMobil(User, _mobil);
                                    }
                                    break;
                                }
                            case ClientOpcode.get_my_avatars:
                                {
                                    User.GetAvatars();
                                    break;
                                }
                            case ClientOpcode.equip:
                                {
                                    User.SendEquip(dcode[1].ToString());
                                    break;
                                }
                            case ClientOpcode.buy:
                                {
                                    User.BuyItems(dcode[1].ToObject<string>(), dcode[2].ToObject<string>(), dcode[3].ToObject<int>(), dcode[4].ToObject<int>());
                                    break;
                                }
                            case ClientOpcode.change_name:
                                {
                                    User.ChangeName(dcode[1].ToObject<string>());
                                    User.PlayerInfo();
                                    Chat.ChatManager.UpdateBoddy(this);
                                    break;
                                }
                            case ClientOpcode.room_change_ready:
                                {
                                    Chanel _chan = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_chan != null)
                                    {
                                        _chan.ChangeReady(User, dcode[1].ToObject<int>());
                                    }
                                    break;
                                }
                            case ClientOpcode.game_start:
                                {
                                    Chanel _chan = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_chan != null)
                                    {
                                        _chan.GameStart();
                                    }
                                    break;
                                }
                            case ClientOpcode.game_move:
                                {
                                    Chanel _chan = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_chan != null)
                                    {
                                        User.x = dcode[1].ToObject<int>();
                                        User.y = dcode[2].ToObject<int>();
                                        User.body_move = dcode[3].ToObject<int>();
                                        User.look = dcode[4].ToObject<int>();
                                        User.ang = dcode[5].ToObject<int>();
                                        _chan.Move(User);
                                    }
                                    break;
                                }
                            case ClientOpcode.game_shoot:
                                {
                                    Chanel _chan = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_chan != null)
                                    {
                                        _chan.GameShoot(User, dcode[1].ToObject<int>(), dcode[2].ToObject<int>(), dcode[3].ToObject<int>(), dcode[4].ToObject<int>(), dcode[5].ToObject<int>(), dcode[6].ToObject<int>(), dcode[7].ToObject<int>(), dcode[8].ToObject<int>());
                                    }
                                    break;
                                }
                            case ClientOpcode.game_pass_turn:
                                {
                                    Chanel _chan = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_chan != null)
                                    {

                                    }
                                    break;
                                }
                            case ClientOpcode.room_change_team:
                                {
                                    Chanel _chan = Program.Chanels.Single(a => a.UserIn(User) == true);
                                    if (_chan != null)
                                    {
                                        _chan.ChangeTeam(User, dcode[1].ToObject<string>());
                                    }
                                    break;
                                }
                            default:
                                {
                                    Console.WriteLine("opcode: {0}",opc);
                                    Console.WriteLine("data: {0}", e.Data);
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        Console.WriteLine("tp: {0}",e.Type);
                        break;
                    }
            }
        }
    }
}
