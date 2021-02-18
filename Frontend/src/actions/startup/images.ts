import { Action } from "../../reducer";
import {getAuthedAxios} from '../../auth';
import { ImageModel } from "../../models/image";
import { RemoteImageModel } from "../../models/remoteImage";

const getImages = async (dispatch: React.Dispatch<Action>) => {
    const axios = await getAuthedAxios(true);
    const images = await axios.get<ImageModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/image`);
    dispatch({type: 'imagesLoaded', images: images.data})
}

const getRemoteImages = async (dispatch: React.Dispatch<Action>) => {
    const axios = await getAuthedAxios(true);
    const images = await axios.get<RemoteImageModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/image/remote`);
    dispatch({type: 'remoteImagesLoaded', remoteImages: images.data})
}

const imagesStartupActions = (dispatch: React.Dispatch<Action>) => Promise.all([
    getImages(dispatch),
    getRemoteImages(dispatch)
])

export default imagesStartupActions;