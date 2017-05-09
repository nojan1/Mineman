using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;

namespace Mineman.Common.Models
{
    public class NonUserChangableProperty : Attribute { }

    public class ServerProperties
    {
        [NonUserChangableProperty]
        public bool Enable_Query { get; set; } = true;
        [NonUserChangableProperty]
        public int Server_Port { get; set; } = 25565;
        [NonUserChangableProperty]
        public string Level_Name { get; set; } = "world";
        [NonUserChangableProperty]
        public bool Enable_Rcon { get; set; } = true;
        [NonUserChangableProperty]
        public int Query__Port { get; set; } = 26565;
        [NonUserChangableProperty]
        public int Rcon__Port { get; set; } = 27565;
        [NonUserChangableProperty]
        public string Rcon__Password { get; set; }

        public int Max_Tick_Time { get; set; } = 60000;
        public string Generator_Settings { get; set; }
        public bool Allow_Nether { get; set; } = true;
        public bool Force_Gamemode { get; set; } = false;
        public int Gamemode { get; set; } = 0;
        public int Player_Idle_Timeout { get; set; } = 0;
        public int Difficulty { get; set; } = 1;
        public bool Spawn_Monsters { get; set; } = true;
        public int Op_Permission_Level { get; set; } = 4;
        public bool Announce_Player_Achievements { get; set; } = true;
        public bool Pvp { get; set; } = true;
        public bool Snooper_Enabled { get; set; } = true;
        public string Level_Type { get; set; } = "DEFAULT";
        public bool Hardcore { get; set; } = false;
        public bool Enable_Command_Block { get; set; } = false;
        public int Max_Players { get; set; } = 20;
        public int Network_Compression_Threshold { get; set; } = 256;
        public string Resource_Pack_Sha1 { get; set; } = "";
        public int Max_World_Size { get; set; } = 29999984;
        public string Server_Ip { get; set; } = "";
        public bool Spawn_Npcs { get; set; } = true;
        public bool Allow_Flight { get; set; } = false;
        public int View_Distance { get; set; } = 10;
        public string Resource_Pack { get; set; } = "";
        public bool Spawn_Animals { get; set; } = true;
        public bool White_List { get; set; } = false;
        public bool Generate_Structures { get; set; } = true;
        public bool Online_Mode { get; set; } = true;
        public int Max_Build_Height { get; set; } = 256;
        public string Level_Seed { get; set; } = "";
        public bool Prevent_Proxy_Connections { get; set; } = false;
        public string Motd { get; set; } = "";

        public ServerProperties()
        {
            Rcon__Password = Guid.NewGuid().ToString("N");
        }
    }
}
