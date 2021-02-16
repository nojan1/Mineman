export type ImageModel = {
    id: number;
    name: string;
    modDirectory?: string;
    buildStatus?: ImageBuildStatus;
    serversUsingImage: number[]
};

export type ImageBuildStatus = {
    id: number;
    buildSucceeded: boolean;
    log: string;
};