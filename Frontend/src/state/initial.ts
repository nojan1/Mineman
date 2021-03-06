import { ImageModel } from '../models/image';
import { Message } from '../models/message';
import { ModsModel } from '../models/mods';
import { RemoteImageModel } from '../models/remoteImage';
import {ServerModel} from '../models/server';
import {User} from '../models/user';
import { WorldModel } from '../models/world';

export interface ApplicationState {
    servers: ServerModel[];
    images: ImageModel[];
    remoteImages: RemoteImageModel[];
    worlds: WorldModel[];
    mods: ModsModel[];
    user?: User;
    messages: Message[];
}


export const getInitialState = () => {
    return {
       servers: [],
       images: [],
       remoteImages: [],
       worlds: [],
       mods: [],
       messages: []
    };
}