import { Action } from "../../reducer";
import { getAuthedAxios } from '../../auth';
import { ServerModel, ServerQueryModel } from "../../models/server";
import { handleAxiosError } from "../error";

const serverStartupActions = async (dispatch: React.Dispatch<Action>) => {
    const axios = await getAuthedAxios(true);
    const servers = await axios.get<ServerModel[]>(`${process.env.REACT_APP_BACKEND_URL}/api/server`)
        .catch(handleAxiosError(dispatch));
        
    if (!servers)
        return;

    dispatch({ type: 'serversLoaded', servers: servers.data })

    await Promise.all(servers.data
        .filter(s => s.isAlive)
        .map(s =>
            axios.get<ServerQueryModel>(`${process.env.REACT_APP_BACKEND_URL}/api/server/query/${s.id}`)
                .then(x => dispatch({ type: 'serverQueryLoaded', id: s.id, query: x.data }))
        )
    )
}

export default serverStartupActions;