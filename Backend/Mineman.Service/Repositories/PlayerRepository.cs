using Mineman.Common.Database;
using Mineman.Common.Database.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.Repositories
{
    public class Property
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class ApiProfile
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Property> properties { get; set; }
    }

    public class TextureInfo
    {
        public long timestamp { get; set; }
        public string profileId { get; set; }
        public string profileName { get; set; }
        public JObject textures { get; set; }
    }

    public class PlayerRepository : IPlayerRepository
    {
        private const string SESSION_SERVER_URL = "https://sessionserver.mojang.com/session/minecraft/profile/";

        private static readonly TimeSpan RETENTION_TIME = TimeSpan.FromHours(5);
        private static readonly HttpClient client = new HttpClient();

        private readonly DatabaseContext _context;

        public PlayerRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<PlayerProfile> Get(string uuid)
        {
            uuid = uuid.Replace("-", "").ToLower();

            var profile = _context.PlayerProfiles.FirstOrDefault(p => p.UUID == uuid);
            if(profile == null)
            {
                profile = await FetchFromAPI(uuid);
                if (profile == null)
                    return null;

                profile.LastFetched = DateTimeOffset.Now;

                _context.Add(profile);
                await _context.SaveChangesAsync();

            }else if(profile.LastFetched + RETENTION_TIME < DateTimeOffset.Now)
            {
                var newProfileData = await FetchFromAPI(uuid);
                if (newProfileData == null)
                    return null;

                profile.LastFetched = DateTimeOffset.Now;
                profile.Name = newProfileData.Name;
                profile.SkinURL = newProfileData.SkinURL;

                _context.Update(profile);
                await _context.SaveChangesAsync();
            }

            return profile;
        }

        private async Task<PlayerProfile> FetchFromAPI(string uuid)
        {
            var data = await client.GetStringAsync($"{SESSION_SERVER_URL}{uuid}");
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            var profileObject = JsonConvert.DeserializeObject<ApiProfile>(data);
            string skinUrl = "";

            var textureBase64 = profileObject.properties.FirstOrDefault(p => p.name == "textures")?.value;
            if(textureBase64 != null)
            {
                var textInfoJson = Encoding.ASCII.GetString(Convert.FromBase64String(textureBase64));
                var textInfoObject = JsonConvert.DeserializeObject<TextureInfo>(textInfoJson);

                if(textInfoObject.textures.TryGetValue("SKIN", StringComparison.CurrentCultureIgnoreCase, out JToken skinValue))
                {
                    skinUrl = skinValue.Value<string>("url");
                }
            }

            return new PlayerProfile
            {
                Name = profileObject.name,
                SkinURL = skinUrl,
                UUID = uuid
            };
        }
    }
}
