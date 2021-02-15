import Axios from "axios";
import jwt_decode from "jwt-decode";
import { User } from "../models/user";
import { clear, load, store } from "../actions/persist";

const TOKEN_STORAGE_KEY = "token";

export const getToken = async () => {
    const token = load<string>(TOKEN_STORAGE_KEY);

    if (!token || token.toLowerCase().indexOf("html") !== -1)
        return null;

    const decodedToken = jwt_decode<User>(token);
    if (hasExpired(decodedToken)) {
        //TODO: Get refresh token here

        return null;
    }

    return { decodedToken, token };
}

export const saveToken = async (token: string) => {
    store(TOKEN_STORAGE_KEY, token);

    return {
        decodedToken: jwt_decode<User>(token),
        token
    };
}

export const removedStoredToken = () => {
    clear(TOKEN_STORAGE_KEY);
}

export const hasExpired = (profile: User) =>
    Date.now() >= profile.exp * 1000;
