import { ImageModel } from "../models/image";
import { ModsModel } from "../models/mods";
import { ServerQueryModel, ServerModel } from "../models/server";
import { User } from "../models/user";
import { WorldModel } from "../models/world";
import { ApplicationState } from "../state/initial";

export type Action =
    { type: 'serversLoaded', servers: ServerModel[] } |
    { type: 'serverQueryLoaded', id: string, query: ServerQueryModel } |
    { type: 'imagesLoaded', images: ImageModel[] } |
    { type: 'worldsLoaded', worlds: WorldModel[] } |
    { type: 'modsLoaded', mods: ModsModel[] } |
    { type: 'userLoaded', user: User }

export const mainReducer = (prevState: ApplicationState, action: Action): ApplicationState => {
    switch (action.type) {
        case 'serversLoaded':
            return { ...prevState, servers: action.servers };
        case 'serverQueryLoaded':
            const newServers = prevState.servers
                .map(x => x.id === action.id ? { ...x, query: action.query } : x);

            return { ...prevState, servers: newServers };
        case 'userLoaded':
            return { ...prevState, user: action.user };
        default:
            return prevState;
    }
};