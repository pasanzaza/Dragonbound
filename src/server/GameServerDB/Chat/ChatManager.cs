using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using System.Web;

namespace GameServerDB.Chat
{
    public class ChatManager
    {
        public static string Msj(string _msjx, UserManager.UserClass _user)
        {
            string msj_f = HttpUtility.HtmlEncode(_msjx.Replace("\\\"", "\""));
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.chat);
                writer.WriteValue(_msjx);
                writer.WriteValue(_user.Name);
                writer.WriteValue(0); //type
                writer.WriteEndArray();
            }

            return sb.ToString();
        }
        public static void Notice(UserManager.UserClass _user)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.room_state);

                writer.WriteStartArray();

                writer.WriteStartArray();
                writer.WriteValue("GameServerDB");
                writer.WriteValue("");
                writer.WriteValue(9);
                writer.WriteEndArray();

                writer.WriteStartArray();
                writer.WriteValue("Bienvenido!");
                writer.WriteValue("");
                writer.WriteValue(9);
                writer.WriteEndArray();

                writer.WriteEndArray();

                writer.WriteEndArray();
            }
            _user.sep.Send(sb.ToString());
        }

        public static void UpdateBoddy(Serverb _serv)
        {
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.None;
                writer.WriteStartArray();
                writer.WriteValue((int)ServerOpcode.channel_players);
                writer.WriteStartArray();

                foreach (UserManager.UserClass _user in Program.Users)
                {
                    writer.WriteValue(_user.user_id);
                    writer.WriteValue(_user.Name);
                    writer.WriteValue(_user.rank);
                    writer.WriteValue(_user.unk0);
                }

                writer.WriteEndArray();
                writer.WriteEndArray();
            }
            _serv.Broadcast(sb.ToString());
        }
    }
}
