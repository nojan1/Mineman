import { Dispatch } from "react";
import { getAuthedAxios } from "../auth";
import { ImageAddModel, ImageModel } from "../models/image";
import { RemoteImageModel } from "../models/remoteImage";
import { Action } from "../reducer";
import { handleAxiosError } from "./error";

export const importImage = async (dispatch: React.Dispatch<Action>, remoteImage: RemoteImageModel) => {
    const axios = await getAuthedAxios(false);
    const response = await axios.post<ImageModel>(`${process.env.REACT_APP_BACKEND_URL}/api/image/remote/${remoteImage.shA256Hash}`);
    
    dispatch({type: 'imageAdded', image: response.data});
    return response.data;
}

export const addImage = async (dispatch: Dispatch<Action>, image: ImageAddModel) => {
    try {
        const axios = await getAuthedAxios(false);

        var postData = new FormData();
        postData.append("displayName", image.displayName);
        postData.append("modDir", image.modDirectory ?? '');
        postData.append("imageContents", image.imageContents);

        const response = await axios.post<ImageModel>(`${process.env.REACT_APP_BACKEND_URL}/api/iikamge`, postData);

        dispatch({type: 'imageAdded', image: response.data});
        return response.data;
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}

export const deleteImage = async (dispatch: Dispatch<Action>, imageId: number) => {
    try {
        const axios = await getAuthedAxios(false);
        await axios.delete(`${process.env.REACT_APP_BACKEND_URL}/api/image/${imageId}`);

        dispatch({type: 'imageDeleted', imageId: imageId});
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}