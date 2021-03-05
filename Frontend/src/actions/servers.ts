import { getAuthedAxios } from "../auth";
import { ServerAddModel, ServerConfigurationModel, ServerModel } from "../models/server";
import { Action } from "../reducer";
import { handleAxiosError } from "./error";

export const createServer = async (dispatch: React.Dispatch<Action>, server: ServerAddModel) => {
    try {
        const axios = await getAuthedAxios(false);
        const response = await axios.post<ServerModel>(`${process.env.REACT_APP_BACKEND_URL}/api/server`, server);

        dispatch({type: 'serverAdded', server: response.data});
        return response.data;
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}

export const updateServer = async (dispatch: React.Dispatch<Action>, serverId: number, server: ServerConfigurationModel) => {
    try {
        const axios = await getAuthedAxios(false);
        const response = await axios.post<ServerModel>(`${process.env.REACT_APP_BACKEND_URL}/api/server/${serverId}`, server);

        dispatch({type: 'serverUpdated', server: response.data});
        return response.data;
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}

export const deleteServer = async (dispatch: React.Dispatch<Action>, serverId: number) => {
    try {
        const axios = await getAuthedAxios(false);
        await axios.delete(`${process.env.REACT_APP_BACKEND_URL}/api/server/${serverId}`);
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}

export const startServer = async (dispatch: React.Dispatch<Action>, serverId: number) => {
    try {
        const axios = await getAuthedAxios(false);
        await axios.post(`${process.env.REACT_APP_BACKEND_URL}/api/server/start/${serverId}`);
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}

export const stopServer = async (dispatch: React.Dispatch<Action>, serverId: number) => {
    try {
        const axios = await getAuthedAxios(false);
        await axios.post(`${process.env.REACT_APP_BACKEND_URL}/api/server/stop/${serverId}`);
    } catch (error) {
        handleAxiosError(dispatch)(error);
    }
}