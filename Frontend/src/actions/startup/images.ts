import { Action } from "../../reducer";
import {getAuthedAxios} from '../../auth';
import { ImageModel } from "../../models/image";

const imagesStartupActions = async (dispatch: React.Dispatch<Action>) => {
    const axios = await getAuthedAxios(true);
    const images = await axios.get<ImageModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/image`);
    dispatch({type: 'imagesLoaded', images: images.data})
}

export default imagesStartupActions;