export type ServerModel = {
    id: number;
    description: string;
    mainPort: number;
    isAlive: boolean;
    hasMap: boolean;
    motd?: string;
    query?: ServerQueryModel;
    imageId?: number;
    worldId?: number;
};

export type ServerQueryModel = {
    maxPlayers: number;
    numPlayers: number;

    responseFields: { [key: string]: string };
    players: PlayerInformationModel[];
    plugins: PluginInformationModel[];
}

export type PlayerInformationModel = {
    name: string;
};

export type PluginInformationModel = {
    name: string;
};

export type ServerAddModel = {
    description: string;
    worldId: number;
    imageId: number;
    serverPort: number;
    memoryAllocationMB: number;
    modIds?: number[];
};

export type ServerConfigurationModel = {
    description: string;
    serverPort: number;
    worldId: number;
    imageId: number;
    memoryAllocationMB: number;
    modIds: number[];
    properties: ServerPropertiesModel;
};

export type ServerPropertiesModel = {
    max_Tick_Time: any;
    generator_Settings: any;
    allow_Nether: any;
    force_Gamemode: any;
    gamemode: any;
    player_Idle_Timeout: any;
    difficulty: any;
    spawn_Monsters: any;
    op_Permission_Level: any;
    announce_Player_Achievements: any;
    pvp: any;
    snooper_Enabled: any;
    level_Type: any;
    hardcore: any;
    enable_Command_Block: any;
    max_Players: any;
    network_Compression_Threshold: any;
    resource_Pack_Sha1: any;
    max_World_Size: any;
    server_Ip: any;
    spawn_Npcs: any;
    allow_Flight: any;
    view_Distance: any;
    resource_Pack: any;
    spawn_Animals: any;
    white_List: any;
    generate_Structures: any;
    online_Mode: any;
    max_Build_Height: any;
    level_Seed: any;
    prevent_Proxy_Connections: any;
    motd: any;
};