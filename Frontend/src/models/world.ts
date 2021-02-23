export type WorldModel = {
    id: number;
    displayName: string;
    serversUsingWorld: number[];
};

export type WorldAddModel = {
    displayName: string;
    worldFile?: File
};