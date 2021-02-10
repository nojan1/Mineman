export type ServerModel = {
    id: string;
    description: string;
    mainPort: number;
    isAlive: boolean;
    hasMap: boolean;
    info?: ServerInfoModel;
};

export type ServerInfoModel = {
    motd: string;
    query: ServerQueryModel;
};

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