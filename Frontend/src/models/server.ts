export type ServerModel = {
    id: number;
    description: string;
    mainPort: number;
    isAlive: boolean;
    hasMap: boolean;
    motd?:string;
    query?: ServerQueryModel;
};

// export type ServerInfoModel = {
//     motd: string;
//     query: ServerQueryModel;
// };

export type ServerQueryModel = {
    maxPlayers: number;
    numPlayers: number;

    responseFields: {[key:string]: string};
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
    worldID: number;
    imageID: number;
    memoryAllocationMB: number;
    modIds: number[];
    properties: any;
};