import { ImageModel } from "../models/image";
import { RemoteImageModel } from "../models/remoteImage";
import { Action } from "../reducer";

export const importImage = (dispatch: React.Dispatch<Action>, remoteImage: RemoteImageModel) =>
    new Promise<ImageModel>(resolve => {
        setTimeout(() => {
            const newImage: ImageModel = {
                id: new Date().getSeconds(),
                name: remoteImage.displayName,
                modDirectory: remoteImage.modDirectory,
                remoteHash: remoteImage.sHA256Hash,
                buildStatus: undefined,
                serversUsingImage: []
            };

            dispatch({type: 'imageAdded', image: newImage});
            resolve(newImage);
        }, 2000)
    });