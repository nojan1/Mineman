import { Action } from "../../reducer";
import { getAuthedAxios } from '../../auth';
import { WorldModel } from "../../models/world";

const worldsStartupActions = async (dispatch: React.Dispatch<Action>) => {
    try {
        const axios = await getAuthedAxios(false);
        const worlds = await axios.get<WorldModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/world`);
        dispatch({ type: 'worldsLoaded', worlds: worlds.data })
    } catch { }
}

export default worldsStartupActions;