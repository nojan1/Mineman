export type ImageModel = {
    id: number;
    name: string;
    modDirectory?: string;
    remoteHash?: string;
    buildStatus?: ImageBuildStatus;
    serversUsingImage: number[]
};

export type ImageBuildStatus = {
    id: number;
    buildSucceeded: boolean;
    log: string;
};

export type ImageAddModel = {
    displayName: string;
    imageContents: File;
    modDirectory?: string;
};