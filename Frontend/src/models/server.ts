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
    properties?: ServerPropertiesModel;
};

export type ServerQueryModel = {
    maxPlayers: number;
    numPlayers: number;

    responseFields: { [key: string]: string };
    players: PlayerInformationModel[];
    plugins: PluginInformationModel[];
}

export type ServerInfoModel = {
    hasMapImage: boolean;
    server: any;
    isAlive: boolean;
    properties: ServerPropertiesModel;
};

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
    max_tick_time: any;
    generator_settings: any;
    allow_nether: any;
    force_gamemode: any;
    gamemode: any;
    player_idle_timeout: any;
    difficulty: any;
    spawn_monsters: any;
    op_permission_level: any;
    announce_player_achievements: any;
    pvp: any;
    snooper_enabled: any;
    level_type: any;
    hardcore: any;
    enable_command_block: any;
    max_players: any;
    network_compression_threshold: any;
    resource_pack_sha1: any;
    max_world_size: any;
    server_ip: any;
    spawn_npcs: any;
    allow_flight: any;
    view_distance: any;
    resource_pack: any;
    spawn_animals: any;
    white_list: any;
    generate_structures: any;
    online_mode: any;
    max_build_height: any;
    level_seed: any;
    prevent_proxy_connections: any;
    motd: any;
};