using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace GameServerDB.UserManager
{
    public class UserClass
    {
        #region Public
        public string Name = "test";
        public int rank = 0;
        public int user_id = 1;
        public int version = 0;
        public string user_key = "xxxx";
        public string user_fb = "13131";
        public string gender = "m";
        public Int64 foto = 0;

        public int gold = 0;
        public int cash = 0;
        public int gp = 0;

        //
        public int head = 1;
        public int body = 2;
        public int eyes = 0;
        public int flag = 0;

        public int background = 0;
        public int foreground = -1;

        public int room_number = 0;
        public int location_type = 1;
        public int unlock = 1;

        public int event1 = -1;
        public int event2 = -1;

        public int guild = 0;
        public int guild_job = 0;
        public int name_changes = 1;
        public int power_user = 0;

        //Sala
        public int Position = 0;
        public int Is_Master = 0;
        public int Is_Ready = 0;
        public int Is_Bot = 0;

        // game_move guardar a donde **** se mueve el user
        public int x;
        public int y;
        public int body_move;
        public int look;
        public int ang;

        public int mobil = 0;

        public string unk0=null;

        public Serverb sep;
        #endregion
        public void Login()
        {
            sep.Send("[" + (int)ServerOpcode.login_profile + "]");
            SQLResult result = Program._SQL.Select("SELECT * FROM users WHERE id=?", user_id);
            if (result.Count != 0)
            {
                user_id = result.Read<int>(0, "id");
                Name = result.Read<string>(0, "Name");
                rank = result.Read<int>(0, "rank");
                user_fb = result.Read<string>(0, "fbid");
                gold = result.Read<int>(0, "gold");
                cash = result.Read<int>(0, "cash");
                gp = result.Read<int>(0, "gp");
                gender = result.Read<string>(0, "gender");
                user_key = result.Read<string>(0, "Key");

                if (!(result.Read<int>(0, "foto") == 0))
                    foto = Int64.Parse(user_fb);
            }

            sep.Send("[" + (int)ServerOpcode.login_avatars + "]");
            SQLResult result2 = Program._SQL.Select("SELECT a.avatar,i.genero,i.parte FROM user_items a JOIN items i ON a.avatar=i.codigo AND a.puesto=1 AND a.id_user=?", user_id);
            if (result2.Count != 0)
            {
                for (int p = 0; p < result2.Count; p++)
                {
                    int _avatar = result2.Read<int>(p, "avatar");
                    string _parte = result2.Read<string>(p, "parte");
                    switch (_parte)
                    {
                        case "h": { head = _avatar; break; }
                        case "e": { eyes = _avatar; break; }
                        case "b": { body = _avatar; break; }
                        case "f": { flag = _avatar; break; }
                        case "1": { background = _avatar; break; }
                        case "2": { foreground = _avatar; break; }
                    }
                }
            }
            PlayerInfo();
        }
        public void UpdateInfo()
        {
            SQLResult result2 = Program._SQL.Select("SELECT a.avatar,i.genero,i.parte FROM user_items a JOIN items i ON a.avatar=i.codigo AND a.puesto=1 AND a.id_user=?", user_id);
            if (result2.Count != 0)
            {
                for (int p = 0; p < result2.Count; p++)
                {
                    int _avatar = result2.Read<int>(p, "avatar");
                    string _parte = result2.Read<string>(p, "parte");
                    switch (_parte)
                    {
                        case "h": { head = _avatar; break; }
                        case "e": { eyes = _avatar; break; }
                        case "b": { body = _avatar; break; }
                        case "f": { flag = _avatar; break; }
                        case "1": { background = _avatar; break; }
                        case "2": { foreground = _avatar; break; }
                    }
                }
            }
        }
        public void PlayerInfo()
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.my_player_info);
                writer.WriteStartArray();
                writer.WriteValue(user_id);
                writer.WriteValue(location_type);
                writer.WriteValue(room_number);
                writer.WriteValue(Name);
                writer.WriteValue(rank);
                writer.WriteValue(gp);
                writer.WriteValue(gold);
                writer.WriteValue(cash);
                writer.WriteValue(gender);
                writer.WriteValue(unlock);
                writer.WriteValue(head);
                writer.WriteValue(body);
                writer.WriteValue(eyes);
                writer.WriteValue(flag);
                writer.WriteValue(foreground);
                writer.WriteValue(background);
                writer.WriteValue(event1);
                writer.WriteValue(event2);
                writer.WriteValue(foto);
                writer.WriteValue(guild);
                writer.WriteValue(guild_job);
                writer.WriteValue(name_changes);
                writer.WriteValue(power_user);
                writer.WriteEndArray();
                writer.WriteEndArray();
            }
            sep.Send(sb.ToString());
        }
        public void GetAvatars()
        {
            SQLResult result = Program._SQL.Select("SELECT no,avatar,puesto FROM user_items WHERE id_user=?", user_id);
            if (result.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.None;
                    writer.WriteStartArray();
                    writer.WriteValue((int)ServerOpcode.my_avatars);
                    writer.WriteStartArray();
                    writer.WriteStartArray();
                    for (int i = 0; i < result.Count; i++)
                    {
                        writer.WriteStartArray();
                        writer.WriteValue(result.Read<int>(i, "no"));
                        writer.WriteValue(result.Read<int>(i, "avatar"));
                        writer.WriteValue(result.Read<int>(i, "puesto"));
                        string dsdsd = null;
                        writer.WriteValue(dsdsd);
                        writer.WriteValue(0);
                        writer.WriteValue(0);
                        writer.WriteEndArray();
                    }
                    writer.WriteEndArray();
                    writer.WriteValue(gold);
                    writer.WriteValue(cash);
                    writer.WriteEndArray();
                    writer.WriteEndArray();
                    sep.Send(sb.ToString());
                }
            }
        }
        public void SendEquip(string _equipx)
        {
            JArray _equipd = JArray.Parse(_equipx);

            string query = "";
            SQLResult result = Program._SQL.Select("SELECT avatar FROM user_items WHERE id_user=?", user_id);
            if (result.Count != 0)
            {
                for (int i = 0; i < _equipd.Count(); i++)
                {
                    query = query + " AND no!='" + _equipd[i].ToObject<int>() + "'";
                }
                for (int i = 0; i < _equipd.Count(); i++)
                {
                    bool actu1 = Program._SQL.Execute("UPDATE user_items SET puesto=1 WHERE no='" + _equipd[i].ToObject<int>() + "' AND id_user='" + user_id + "' ");
                    bool actu2 = Program._SQL.Execute("UPDATE user_items SET puesto=0 WHERE no!='" + _equipd[i].ToObject<int>() + "' " + query + " AND id_user='" + user_id + "' ");
                }
            }
            UpdateInfo();
            PlayerInfo();
        }
        public void BuyItems(string _item, string _tipo_precio, int _periodo, int _precio)
        {
            string avatar = _item.Replace("\"", "");
            string tipo_precio = _tipo_precio;// true | false
            int periodo = _periodo;   // 0=WEEK   1=MONTH     2=total
            int precio = _precio;
            string campo = "", campot = "";
            if (tipo_precio == "true") { campo = "cash"; campot = "c"; } else { campo = "gold"; campot = "g"; }
            SQLResult result = Program._SQL.Select("SELECT " + campo + " FROM users WHERE id=?", user_id);
            if (result.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.None;
                    writer.WriteStartArray();
                    writer.WriteValue((int)ServerOpcode.alert);
                    if (result.Read<int>(0, campo) > precio)
                    {
                        SQLResult result2 = Program._SQL.Select("SELECT avatar FROM user_items WHERE id_user=" + user_id + " AND avatar=" + avatar);
                        if (result2.Count == 0)
                        {
                            bool compra = Program._SQL.Execute("INSERT INTO user_items SET avatar='" + avatar + "',puesto=0,tipo_precio='" + campot + "',periodo='" + periodo + "',expira='',id_user='" + user_id + "' ");
                            if (compra)
                            {
                                bool actumoneda = Program._SQL.Execute("UPDATE users SET " + campo + "=" + campo + "-" + precio + " WHERE id=" + user_id);
                                writer.WriteValue("Bien G");
                                writer.WriteValue("Acabas de comprar el avatar.");
                            }
                        }
                        else
                        {
                            writer.WriteValue("Sorry");
                            writer.WriteValue("Ya habias comprado este avatar.");
                        }
                    }
                    else
                    {
                        writer.WriteValue("Sorry");
                        writer.WriteValue("Not enough Cash. You can get more by playing or charging.");
                    }
                    writer.WriteEndArray();
                }
                sep.Send(sb.ToString());
                GetAvatars();
            }
        }
        public void ChangeName(string _name)
        {
            SQLResult result = Program._SQL.Select("SELECT gold FROM users WHERE id=?", user_id);
            if (result.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                StringWriter sw = new StringWriter(sb);
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    writer.Formatting = Formatting.None;
                    writer.WriteStartArray();
                    writer.WriteValue((int)ServerOpcode.alert);
                    if (result.Read<int>(0, "gold") > 4000)
                    {
                        Name = _name;
                        gold = gold - 4000;
                        Program._SQL.Execute("UPDATE users SET Name='" + Name + "',gold=gold-4000 WHERE id=" + user_id);
                        writer.WriteValue("ChangeName");
                        writer.WriteValue("Tu Nombre Fue Cambiado.");
                    }
                    else
                    {
                        writer.WriteValue("Sorry");
                        writer.WriteValue("Not enough cash.<p>Name change costs 4,000 cash.");
                    }
                    writer.WriteEndArray();
                }
                sep.Send(sb.ToString());
            }
        }
    }
}
