import { RemoteImageModel } from "../models/remoteImage";
import { Action } from "../reducer";

export const importImage = (dispatch: React.Dispatch<Action>, remoteImage: RemoteImageModel) =>
    new Promise<void>(resolve => setTimeout(resolve, 2000));