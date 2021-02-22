import { getAuthedAxios } from "../auth";
import { ImageModel } from "../models/image";
import { RemoteImageModel } from "../models/remoteImage";
import { Action } from "../reducer";

export const importImage = async (dispatch: React.Dispatch<Action>, remoteImage: RemoteImageModel) => {
    const axios = await getAuthedAxios(false);
    const response = await axios.post<ImageModel>(`${process.env.REACT_APP_BACKEND_URL}/api/image/remote/${remoteImage.shA256Hash}`);
    
    dispatch({type: 'imageAdded', image: response.data});
    return response.data;
}