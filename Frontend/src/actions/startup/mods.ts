import { Action } from "../../reducer";
import {getAuthedAxios} from '../../auth';
import { ModsModel } from "../../models/mods";

const modsStartupActions = async (dispatch: React.Dispatch<Action>) => {
    const axios = await getAuthedAxios(true);
    const mods = await axios.get<ModsModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/mod`);
    dispatch({type: 'modsLoaded', mods: mods.data})
}

export default modsStartupActions;