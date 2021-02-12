import axios from 'axios';
import { getToken } from './token';

import configureApiMock from '../mocks/api';

export const getAuthedAxios = async (anonymousOkey: boolean = false) => {
    const instance = axios.create();

    if(process.env.REACT_APP_USED_MOCKED_BACKEND === 'true'){
        configureApiMock(instance);
        return instance;
    }

    const result = await getToken();
    if(!result && !anonymousOkey)
        throw new Error('No token available and anonymous was not okey');

    instance.defaults.headers.authorization = `Bearer ${result!.token}`;
    return instance;
}