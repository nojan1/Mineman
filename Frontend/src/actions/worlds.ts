import { Dispatch } from "react";
import { getAuthedAxios } from "../auth";
import { WorldAddModel, WorldModel } from "../models/world";
import { Action } from "../reducer";
import { handleAxiosError } from "./error";

export const addWorld = async (dispatch: Dispatch<Action>, world: WorldAddModel) => {
    try {
        const axios = await getAuthedAxios(false);

        var postData = new FormData();
        postData.append("displayName", world.displayName);
        
        if(world.worldFile)
            postData.append("worldFile", world.worldFile);

        const response = await axios.post<WorldModel>(`${process.env.REACT_APP_BACKEND_URL}/api/world`, postData);

        dispatch({type: 'worldAdded', world: response.data});
        return response.data;
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}

export const deleteWorld = async (dispatch: Dispatch<Action>, worldId: number) => {
    try {
        const axios = await getAuthedAxios(false);
        await axios.delete(`${process.env.REACT_APP_BACKEND_URL}/api/world/${worldId}`);

        dispatch({type: 'worldDeleted', worldId: worldId});
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}