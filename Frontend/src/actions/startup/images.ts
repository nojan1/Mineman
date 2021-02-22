import { Action } from "../../reducer";
import { getAuthedAxios } from '../../auth';
import { ImageModel } from "../../models/image";
import { RemoteImageModel } from "../../models/remoteImage";
import { AxiosInstance } from "axios";

const getImages = async (dispatch: React.Dispatch<Action>, axios: AxiosInstance) => {
    const images = await axios.get<ImageModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/image`);
    dispatch({ type: 'imagesLoaded', images: images.data })
}

const getRemoteImages = async (dispatch: React.Dispatch<Action>, axios: AxiosInstance) => {
    const images = await axios.get<RemoteImageModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/image/remote`);
    dispatch({ type: 'remoteImagesLoaded', remoteImages: images.data })
}

const imagesStartupActions = async (dispatch: React.Dispatch<Action>) =>
    getAuthedAxios(false).then(axios =>
        Promise.all([
            getImages(dispatch, axios),
            getRemoteImages(dispatch, axios)
        ]))
    .catch(() => { });

export default imagesStartupActions;