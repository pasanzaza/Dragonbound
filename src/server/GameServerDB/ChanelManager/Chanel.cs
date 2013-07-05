using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Web;

namespace GameServerDB.ChanelManager
{
    // test script
    public class Chanel
    {
        #region Private
        private int         _chanel_id          = 0;
        private bool        _chanel_private     = false;
        private int         _chanel_maxplayers  = 8;
        private int         _chanel_gamemode    = 0;
        private int         _chanel_status      = 0;
        private string      _chanel_name        = "";
        private string      _chanel_password    = "";
        private int         _chanel_useronli    = 0;
        private int         _chanel_look        = 0;
        private int         _chanel_map         = -1;
        private int         _chanel_poweruse    = 0;
        private bool        _chanel_avatar_on   = false;
        private int         _chanel_max_wind    = 0;
        private int         _chanel_gp_rate     = 0;
        #endregion

        #region Public
        public int chanel_id { set { _chanel_id = value; } get { return _chanel_id;} }
        public string chanel_name { set { _chanel_name = value; } get { return _chanel_name; } }
        public int chanel_useronli { set { _chanel_useronli = value; } get { return _chanel_useronli; } }
        public int chanel_maxplayers { set { _chanel_maxplayers = value; } get { return _chanel_maxplayers; } }
        public int chanel_status { set { _chanel_status = value; } get { return _chanel_status; } }
        public int chanel_gamemode { set { _chanel_gamemode = value; } get { return _chanel_gamemode; } }
        public int chanel_map { set { _chanel_map = value; } get { return _chanel_map; } }
        public int chanel_poweruse { set { _chanel_poweruse = value; } get { return _chanel_poweruse; } }
        public string chanel_password { set { _chanel_password = value; } get { return _chanel_password; } }
        #endregion

        bool[] user_slot = new bool[8] { false, false, false, false, false, false, false, false };
        List<UserManager.UserClass> UserInSala = new List<UserManager.UserClass>();

        public Chanel()
        {
        }
        public void Join(UserManager.UserClass _user)
        {
            _user.sep.Send("[" + (int)ServerOpcode.enter_room + "]");

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.room_state);
                writer.WriteStartArray();
                writer.WriteValue(_chanel_id);
                writer.WriteValue(_chanel_name);
                writer.WriteValue(_chanel_password);
                writer.WriteValue(_chanel_maxplayers);
                writer.WriteValue(_chanel_gamemode);
                writer.WriteValue(_chanel_map);
                writer.WriteValue(_chanel_avatar_on);
                writer.WriteValue(_chanel_max_wind);
                writer.WriteValue(_chanel_gp_rate);
                writer.WriteEndArray();
                writer.WriteEndArray();
            }
            _user.sep.Send(sb.ToString());
            RoomPlayer(_user);
        }
        public void RoomPlayer(UserManager.UserClass user_send)
        {
            for (int x = 0; x < 8; x++)
            {
                if (user_slot[x] == false)
                {
                    user_send.Position = x;
                    user_slot[x] = true;
                    break;
                }
            }
            UserInSala.Add(user_send);
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.room_players);
                writer.WriteStartArray();
                foreach (UserManager.UserClass _user in UserInSala)
                {
                    writer.WriteValue(5);
                    writer.WriteValue(0);
                    writer.WriteValue(_user.Position);
                    writer.WriteValue(_user.user_id);
                    writer.WriteValue(_user.Name);
                    writer.WriteValue(_user.rank);
                    writer.WriteValue(0);
                    writer.WriteValue(1);
                    writer.WriteValue(1);
                    writer.WriteValue(_user.gender);
                    writer.WriteValue(_user.mobil);
                    writer.WriteStartArray();
                    writer.WriteValue(_user.head);
                    writer.WriteValue(_user.body);
                    writer.WriteValue(_user.eyes);
                    writer.WriteValue(_user.flag);
                    writer.WriteValue(_user.foreground);
                    writer.WriteValue(_user.background);
                    writer.WriteEndArray();
                }
                writer.WriteValue(0);
                writer.WriteValue(0);
                writer.WriteEndArray();
                writer.WriteEndArray();
            }
            user_send.sep.Send(sb.ToString());
        }
        public static void SendList(Serverb _serv)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.rooms_list);
                writer.WriteStartArray();
                foreach (Chanel _cha in Program.Chanels)
                {
                    writer.WriteStartArray();
                    writer.WriteValue(_cha._chanel_id);
                    writer.WriteValue(_cha._chanel_name);
                    writer.WriteValue(_cha._chanel_useronli);
                    writer.WriteValue(_cha._chanel_maxplayers);
                    writer.WriteValue(_cha._chanel_status);
                    writer.WriteValue(_cha._chanel_gamemode);
                    writer.WriteValue(_cha._chanel_look);
                    writer.WriteValue(_cha._chanel_map);
                    writer.WriteValue(_cha._chanel_poweruse);
                    writer.WriteEndArray();
                }
                writer.WriteEndArray();
                writer.WriteValue(0);
                writer.WriteValue(0);
                writer.WriteEndArray();
            }
            _serv.Send(sb.ToString());
        }
        public bool UserIn(UserManager.UserClass _user)
        {
            bool vr = false;
            if (UserInSala.Count > 0)
            {
                UserManager.UserClass rt = UserInSala.Single(a => a.user_id == _user.user_id);
                if (rt != null)
                    vr = true;
            }
            return vr;
        }
        public void ChangeMobil(UserManager.UserClass _user, int _mobil)
        {
            _user.mobil = _mobil;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.changed_mobile);
                writer.WriteValue(_user.user_id);
                writer.WriteValue(_mobil);
                writer.WriteEndArray();
            }
            foreach (UserManager.UserClass _usp in UserInSala)
            {
                _usp.sep.Send(sb.ToString());
            }
        }
        public void GameStart()
        {
            int[][] posx = new int[8][];
            posx[0] = new int[3];
            posx[1] = new int[3];
            posx[2] = new int[3];
            posx[3] = new int[3];
            posx[4] = new int[3];
            posx[5] = new int[3];
            posx[6] = new int[3];
            posx[7] = new int[3];

            posx[0][0] = 168;
            posx[0][1] = 151;
            posx[1][0] = 268;
            posx[1][1] = 101;
            posx[2][0] = 368;
            posx[2][1] = 151;
            posx[3][0] = 468;
            posx[3][1] = 151;
            posx[4][0] = 568;
            posx[4][1] = 151;
            posx[5][0] = 668;
            posx[5][1] = 151;
            posx[6][0] = 668;
            posx[6][1] = 151;
            posx[7][0] = 868;
            posx[7][1] = 151;
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.game_start);
                writer.WriteStartArray();
                writer.WriteStartArray();
                int c = 0;
                foreach (UserManager.UserClass usp in UserInSala)
                {
                    usp.x = posx[c][0];
                    usp.y = posx[c][1];
                    writer.WriteStartArray();
                    writer.WriteValue(usp.Position);
                    writer.WriteValue(usp.user_id);
                    writer.WriteValue(usp.Name);
                    string dsd = null;
                    writer.WriteValue(dsd);
                    writer.WriteValue(usp.rank);
                    writer.WriteValue(usp.x);
                    writer.WriteValue(usp.y);
                    writer.WriteValue(1000);
                    writer.WriteValue(0);
                    writer.WriteValue(0);
                    writer.WriteValue(10);
                    writer.WriteValue(55);
                    writer.WriteValue(-8);
                    writer.WriteValue(usp.mobil);
                    writer.WriteStartArray();
                    writer.WriteValue(usp.head);
                    writer.WriteValue(usp.body);
                    writer.WriteValue(usp.eyes);
                    writer.WriteValue(usp.flag);
                    writer.WriteValue(usp.foreground);
                    writer.WriteValue(usp.background);
                    writer.WriteEndArray();
                    writer.WriteValue(52);
                    writer.WriteValue(33);
                    writer.WriteValue(52);
                    writer.WriteValue(33);
                    writer.WriteValue(52);
                    writer.WriteValue(33);
                    writer.WriteEndArray();
                    c++;
                }
                writer.WriteEndArray();
                writer.WriteValue(0);
                writer.WriteValue(761);
                writer.WriteValue(-488);
                writer.WriteValue(0);
                writer.WriteValue(0);
                writer.WriteStartArray();
                writer.WriteValue(0);
                writer.WriteValue(0);
                writer.WriteValue(1);
                writer.WriteValue(3);
                writer.WriteValue(2);
                writer.WriteEndArray();
                writer.WriteValue(0);
                writer.WriteValue(144);
                writer.WriteValue(3);
                writer.WriteEndArray();
                writer.WriteEndArray();
                foreach (UserManager.UserClass usp in UserInSala)
                {
                    usp.sep.Send(sb.ToString());
                }
            }
        }
        public void ChangeReady(UserManager.UserClass _user, int tp)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.changed_ready);
                writer.WriteValue(_user.user_id);
                writer.WriteValue(tp);
                writer.WriteEndArray();
                foreach (UserManager.UserClass _usp in UserInSala)
                {
                    _usp.sep.Send(sb.ToString());
                }
            }
        }
        public void RemoveUser(UserManager.UserClass _user)
        {
            /*if (_user.Is_Master == 1)
            {
                Program.Chanels.Remove(this);
            }*/
            UserInSala.Remove(_user);
            if (UserInSala.Count() > 0)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.None;
                    writer.WriteStartArray();
                    writer.WriteValue((int)ServerOpcode.player_left);
                    writer.WriteValue(5);
                    writer.WriteValue(0);
                    writer.WriteValue(_user.user_id);
                    writer.WriteEndArray();
                }
                foreach (UserManager.UserClass usp in UserInSala)
                {
                    usp.sep.Send(sb.ToString());
                }
                switch (_user.Position)
                {
                    case 1:
                        {
                            UserManager.UserClass _usr = UserInSala.Single(a => a.Position == 3);
                            if (_usr != null)
                            {
                                user_slot[1] = true;
                                user_slot[3] = false;
                                _usr.Position = 1;
                            }
                            break;
                        }
                    case 2:
                        {
                            UserManager.UserClass _usr = UserInSala.Single(a => a.Position == 4);
                            if (_usr != null)
                            {
                                user_slot[2] = true;
                                user_slot[4] = false;
                                _usr.Position = 2;
                            }
                            break;
                        }
                    case 3:
                        {
                            UserManager.UserClass _usr = UserInSala.Single(a => a.Position == 5);
                            if (_usr != null)
                            {
                                user_slot[3] = true;
                                user_slot[5] = false;
                                _usr.Position = 3;
                            }
                            break;
                        }
                    case 4:
                        {
                            UserManager.UserClass _usr = UserInSala.Single(a => a.Position == 6);
                            if (_usr != null)
                            {
                                user_slot[4] = true;
                                user_slot[6] = false;
                                _usr.Position = 4;
                            }
                            break;
                        }
                    case 5:
                        {
                            UserManager.UserClass _usr = UserInSala.Single(a => a.Position == 7);
                            if (_usr != null)
                            {
                                user_slot[5] = true;
                                user_slot[7] = false;
                                _usr.Position = 5;
                            }
                            break;
                        }
                    case 6:
                        {
                            user_slot[6] = false;
                            break;
                        }
                    case 7:
                        {
                            user_slot[7] = false;
                            break;
                        }
                }
            }
        }
        public void GameShoot(UserManager.UserClass _user, int _x, int _y, int _body, int _look, int _angle, int _a, int _b, int _d)
        {
            /*
            0.74,// Armor
            0.78,// Mage
            0.99,
            0.87,
            0.74,
            1.395,// Boomer
            0.827,
            0.72,
            0.625,
            0.765,
            0.625,
            0.74,
            0.65,
            0.695,
            0.67,
            0.905,
            0.0,// Slots
            0.0	// Aid*/

            int x = _x;
            int y = _y;
            //int xb = _xb;
            int look = _look;
            int angle = _angle;
            int body = _body;
            int a = (look == 0 ? 180 - angle : angle) - body;
            if (90 < a)
                a = 180 - a;
            double xn = (look == 0 ? x - 22 : x + 22);
            double yn = -Math.Sin((angle * Math.PI / 180));
            int delay = 1;
            int next_turn_of_player = 0;

            double DEGTORAD = 0.0174532925199433;
            double[] WindEffect = { 0.74 };
            double[] Gravity = { 73.5 };

            double x_f = 0.5 * Math.Cos(a) * 0.5 + _user.x;
            double y_f = -1 * ((0.5 * Math.Sin(a)) * 0.5 - ((0.5) * 73.5 * ((int)0.5 ^ 2)) - _user.y);

            double digg = Math.PI / 180;
            int d=0;
            int c=0;
            double x_o = (Math.Cos(d * digg) * c) * 0.74;
            double y_o = (Math.Sin(d * digg) * c) * 0.74 - 73.5;


            LogConsole.Show("Send: x: {0} - y: {1}", _x, _y);
            LogConsole.Show("Player: x: {0} - y: {1}", _user.x, _user.y);
            LogConsole.Show("Fin: x: {0} - y: {1}", x_f, y_f);

            int fin_p = 0;
            if (_a == 1)
                fin_p = 2;
            else if ((_a + _a) + 1 <= 10)
                fin_p = (_a + _a) + 1;
            else if ((_a + _a) + 2 <= 20)
                fin_p = (_a + _a) + 2;
            else if ((_a + _a) + 3 <= 30)
                fin_p = (_a + _a) + 3;
            else if ((_a + _a) + 4 <= 40)
                fin_p = (_a + _a) + 4;
            else if ((_a + _a) + 5 <= 50)
                fin_p = (_a + _a) + 5;
            else if ((_a + _a) + 6 <= 60)
                fin_p = (_a + _a) + 6;
            else if ((_a + _a) + 7 <= 70)
                fin_p = (_a + _a) + 7;
            else if ((_a + _a) + 8 <= 80)
                fin_p = (_a + _a) + 8;
            else if ((_a + _a) + 9 <= 90)
                fin_p = (_a + _a) + 9;
            else if ((_a + _a) + 10 <= 100)
                fin_p = (_a + _a) + 10 + 11;
            else if ((_a + _a) + 20 <= 200)
                fin_p = (_a + _a) + 20;
            else if ((_a + _a) + 30 <= 300)
                fin_p = (_a + _a) + 30;
            else
                fin_p = (_a + _a) + 40;

            LogConsole.Show("a: {1} Fin_p: {0}", fin_p, _a);

            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                //gp(x,             y,       a, b, c, d,           e,                f)
                //gp(f.pos.x, f.pos.y, f.angle, 1, 0, 0, d.oponent.x - 15, d.oponent.y - 15);
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.play);
                writer.WriteStartArray();
                writer.WriteValue(0);
                writer.WriteValue(_user.Position);
                writer.WriteValue(x);
                writer.WriteValue(y);
                writer.WriteValue(look);
                writer.WriteValue(delay);
                writer.WriteValue(next_turn_of_player);
                writer.WriteStartArray();
                writer.WriteEndArray();
                writer.WriteValue(899);
                writer.WriteValue(-342);
                writer.WriteValue(249);
                writer.WriteValue(0);
                writer.WriteValue(1);
                writer.WriteValue(0);
                writer.WriteValue(213);

                writer.WriteStartArray();

                writer.WriteStartObject();
                writer.WritePropertyName("start");

                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(x);//x
                writer.WritePropertyName("y");
                writer.WriteValue(y);//y
                writer.WritePropertyName("ang");
                writer.WriteValue(a);
                writer.WritePropertyName("power");
                writer.WriteValue(fin_p);//575
                writer.WritePropertyName("ax");
                writer.WriteValue(x_o);
                writer.WritePropertyName("ay");
                writer.WriteValue(y_o);
                writer.WriteEndObject();

                writer.WritePropertyName("exp");
                writer.WriteValue(3);
                writer.WritePropertyName("img");
                writer.WriteValue(4);
                writer.WritePropertyName("time");
                writer.WriteValue(2880);
                writer.WritePropertyName("hole");

                writer.WriteStartArray();
                writer.WriteValue(360);
                writer.WriteValue(40);
                writer.WriteValue(55);
                writer.WriteValue(42);
                writer.WriteEndArray();

                writer.WritePropertyName("damages");
                writer.WriteStartArray();
                writer.WriteEndArray();
                writer.WriteEndObject();

                writer.WriteEndArray();
                writer.WriteValue(0);
                writer.WriteEndArray();
                writer.WriteEndArray();
            }
            SendAll(sb.ToString());
        }
        public void PassTurn()
        {

        }
        public void ChangeTeam(UserManager.UserClass _user, string team)
        {
            //[8,"B"]
            //[33,0,5,146634,"B"]
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.changed_team);
                writer.WriteValue(0);
                writer.WriteValue(5);
                writer.WriteValue(_user.user_id);
                writer.WriteValue(team);
                writer.WriteEndArray();
            }
            _user.sep.Send(sb.ToString());
        }
        public void Move(UserManager.UserClass _user)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.update);
                writer.WriteStartArray();
                writer.WriteValue(_user.Position);
                writer.WriteValue(_user.x);
                writer.WriteValue(_user.y);
                writer.WriteValue(_user.look);
                writer.WriteEndArray();
                writer.WriteEndArray();
            }
            SendAll(sb.ToString());
        }
        public void SendAll(string sb)
        {
            foreach (UserManager.UserClass _usp in UserInSala)
            {
                _usp.sep.Send(sb);
            }
        }
    }
}
