import { Action } from "../../reducer";
import { getAuthedAxios } from '../../auth';
import { ModsModel } from "../../models/mods";
import { handleAxiosError } from "../error";

const modsStartupActions = async (dispatch: React.Dispatch<Action>) => {
    try {
        const axios = await getAuthedAxios(false);
        const mods = await axios.get<ModsModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/mod`)
            .catch(handleAxiosError(dispatch));

        if(mods)
            dispatch({ type: 'modsLoaded', mods: mods.data })
    } catch { }
}

export default modsStartupActions;