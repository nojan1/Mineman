import axios from 'axios';
import { getToken } from './token';

export type User = {
    sub: string;
    exp: number;
    role: string[];
}

export const getAuthedAxios = async () => {
    const {token} = await getToken();

    const instance = axios.create();
    instance.defaults.headers.authorization = `Bearer ${token}`;
    return instance;
}