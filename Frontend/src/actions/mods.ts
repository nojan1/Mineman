import { Dispatch } from "react";
import { getAuthedAxios } from "../auth";
import { ModsModel } from "../models/mods";
import { Action } from "../reducer";
import { handleAxiosError } from "./error";

export const addMod = async (dispatch: Dispatch<Action>, mod: any) => {
    try {
        const axios = await getAuthedAxios(false);

        var postData = new FormData();
        postData.append("displayName", mod.displayName);
        
        if(mod.modFile)
            postData.append("modFile", mod.modFile);

        const response = await axios.post<ModsModel>(`${process.env.REACT_APP_BACKEND_URL}/api/mod`, postData);

        dispatch({type: 'modAdded', mod: response.data});
        return response.data;
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}

export const deleteMod = async (dispatch: Dispatch<Action>, modId: number) => {
    try {
        const axios = await getAuthedAxios(false);
        await axios.delete(`${process.env.REACT_APP_BACKEND_URL}/api/mod/${modId}`);

        dispatch({type: 'modDeleted', modId: modId});
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}