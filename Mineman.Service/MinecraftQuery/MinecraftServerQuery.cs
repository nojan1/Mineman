using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mineman.Common.Database.Models;
using Mineman.Common.Models;
using Mineman.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mineman.Service.MinecraftQuery
{
    public class MinecraftServerQuery : IMinecraftServerQuery
    {
        private const int STATISTIC = 0x00;
        private const int HANDSHAKE = 0x09;
        private readonly string[] knownKeys = new string[] { "hostname", "gametype", "version", "plugins", "map", "numplayers", "maxplayers", "hostport", "hostip", "game_id" };

        private string queryIp;

        public MinecraftServerQuery(IOptions<Configuration> configuration)
        {
            queryIp = configuration.Value.QueryIpAddress;
        }

        public async Task<QueryInformation> GetInfo(Server server)
        {
            var udp = new UdpClient();

            //uint IOC_IN = 0x80000000;
            //uint IOC_VENDOR = 0x18000000;
            //uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
            //udp.Client.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);

            var destination = new IPEndPoint(IPAddress.Parse(queryIp), server.QueryPort);

            var rawChallenge = await WriteData(udp, destination, HANDSHAKE);
            var challenge = ParseChallenge(rawChallenge).ToList();
            challenge.AddRange(Enumerable.Repeat<byte>(0x00, 4));

            var statisticResponse = await WriteData(udp, destination, STATISTIC, challenge);
            statisticResponse = statisticResponse.Skip(11).ToArray(); //Trim padding

            return ParseStatisticResponse(statisticResponse);
        }

        private byte[] ParseChallenge(byte[] raw)
        {
            var challengeString = Encoding.ASCII.GetString(raw);
            var challengeInt = Convert.ToInt32(challengeString);
            var bytes =  BitConverter.GetBytes(challengeInt);

            return new byte[] { bytes[3], bytes[2], bytes[1], bytes[0] };
        }

        private async Task<byte[]> WriteData(UdpClient udp, IPEndPoint destination, byte command, IEnumerable<byte> payload = null)
        {
            var data = new List<byte> (new byte[] { 0xFE, 0xFD, command, 0x01, 0x02, 0x03, 0x04 });

            if(payload != null)
            {
                data.AddRange(payload);
            }

            if(await udp.SendAsync(data.ToArray(), data.Count, destination) != data.Count)
            {
                throw new Exception("Failed to send data");
            }

            var response = await udp.ReceiveAsync().WithTimeout(TimeSpan.FromSeconds(5));
            if(response.Buffer.Length < 5 || response.Buffer[0] != data[2])
            {
                throw new Exception($"Recieved data is incorrect. Data: {BitConverter.ToString(response.Buffer)}");
            }

            return response.Buffer.Skip(5).ToArray();
        }

        private QueryInformation ParseStatisticResponse(byte[] data)
        {
            var queryInformation = new QueryInformation();

            var dataString = Encoding.ASCII.GetString(data);
            var halfSeparator = "\x00\x01player_\x00\x00";

            var messageParts = dataString.Split(new string[] { halfSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var keyValueList = messageParts[0].Split('\0');

            for (int i = 0; i < keyValueList.Length - 1; i+=2)
            {
                var key = keyValueList[i];
                var value = keyValueList[i + 1];

                if (knownKeys.Contains(key))
                {
                    queryInformation.ResponseFields[key] = value;
                }
            }

            PostProcessPlayers(queryInformation, messageParts[1]);
            PostProcessPlugins(queryInformation);
            
            return queryInformation;
        }

        private void PostProcessPlugins(QueryInformation queryInformation)
        {
            if (queryInformation.ResponseFields.ContainsKey("plugins"))
            {
                //TODO: Implement when one can figure out the format ;)
                // http://wiki.vg/Query
            }
        }

        private void PostProcessPlayers(QueryInformation queryInformation, string dataString)
        {
            queryInformation.Players = dataString.Split(new char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries)
                                                 .Select(x => new PlayerInformation { Name = x })
                                                 .ToList();
        }
    }
}
