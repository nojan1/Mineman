import { ImageModel } from '../models/image';
import { ModsModel } from '../models/mods';
import {ServerModel} from '../models/server';
import {User} from '../models/user';
import { WorldModel } from '../models/world';

export interface ApplicationState {
    servers: ServerModel[];
    images: ImageModel[];
    worlds: WorldModel[];
    mods: ModsModel[];
    user?: User;
}


export const getInitialState = () => {
    return {
       servers: [],
       images: [],
       worlds: [],
       mods: []
    };
}