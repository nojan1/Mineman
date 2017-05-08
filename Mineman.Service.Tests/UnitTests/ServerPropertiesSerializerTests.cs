using Mineman.Common.Models;
using Mineman.Service.Helpers;
using System;
using Xunit;

namespace Mineman.Service.Tests
{
    public class ServerPropertiesSerializerTests
    {
        [Fact]
        public void ModelSerializesCorrectly()
        {
            var serverProperties = new ServerProperties();
            var serializer = new ServerPropertiesSerializer();

            var data = serializer.Serialize(serverProperties);

            Assert.DoesNotContain("_", data);
            Assert.NotEmpty(data);
        }

        [Fact]
        public void ModelDeserializesCorrectly()
        {
            var data = @"#Minecraft server properties
#(File Modification Datestamp)
max-tick-time=60000
generator-settings=
allow-nether=true
force-gamemode=false
gamemode=0
enable-query=false
player-idle-timeout=0
difficulty=1
spawn-monsters=true
op-permission-level=4
announce-player-achievements=true
pvp=true
snooper-enabled=true
level-type=DEFAULT
hardcore=false
enable-command-block=false
max-players=20
network-compression-threshold=256
resource-pack-sha1=
max-world-size=29999984
server-port=25565
server-ip=
spawn-npcs=true
allow-flight=false
level-name=world
view-distance=10
resource-pack=
spawn-animals=true
white-list=false
generate-structures=true
online-mode=true
max-build-height=256
level-seed=
prevent-proxy-connections=false
motd=A Minecraft Server
enable-rcon=false";

            var serializer = new ServerPropertiesSerializer();
            var serverProperties = serializer.Deserialize(data);

            Assert.Equal("A Minecraft Server", serverProperties.Motd);
            Assert.Equal(25565, serverProperties.Server_Port);
        }
    }
}