import {ServerModel} from '../models/server';
import {User} from '../models/user';

export interface ApplicationState {
    servers: ServerModel[];
    user?: User
}


export const getInitialState = () => {
    return {
       servers: [],
    };
}