import { ImageModel } from "../models/image";
import { Message } from "../models/message";
import { ModsModel } from "../models/mods";
import { RemoteImageModel } from "../models/remoteImage";
import { ServerQueryModel, ServerModel } from "../models/server";
import { User } from "../models/user";
import { WorldModel } from "../models/world";
import { ApplicationState } from "../state/initial";

export type Action =
    { type: 'serversLoaded', servers: ServerModel[] } |
    { type: 'serverQueryLoaded', id: string, query: ServerQueryModel } |
    { type: 'imagesLoaded', images: ImageModel[] } |
    { type: 'imageAdded', image: ImageModel } |
    { type: 'remoteImagesLoaded', remoteImages: RemoteImageModel[] } |

    { type: 'worldsLoaded', worlds: WorldModel[] } |
    { type: 'worldAdded', world: WorldModel } |
    { type: 'worldUpdated', world: WorldModel } |
    { type: 'worldDeleted', worldId: number } |

    { type: 'modsLoaded', mods: ModsModel[] } |
    { type: 'userLoaded', user: User } |
    { type: 'messageAdded', message: Message } |
    { type: 'messageCleared', message: Message }

export const mainReducer = (prevState: ApplicationState, action: Action): ApplicationState => {
    switch (action.type) {
        case 'serversLoaded':
            return { ...prevState, servers: action.servers };
        case 'serverQueryLoaded':
            const newServers = prevState.servers
                .map(x => x.id === action.id ? { ...x, query: action.query } : x);

            return { ...prevState, servers: newServers };
        case 'imagesLoaded':
            return { ...prevState, images: action.images };
        case 'imageAdded':
            return { ...prevState, images: [...prevState.images, action.image] }
        case 'remoteImagesLoaded':
            return { ...prevState, remoteImages: action.remoteImages };

        case 'worldsLoaded':
            return { ...prevState, worlds: action.worlds };
        case 'worldAdded':
            return { ...prevState, worlds: [...prevState.worlds, action.world] };
        case 'worldDeleted':
            return { ...prevState, worlds: prevState.worlds.filter(w => w.id !== action.worldId) };

        case 'modsLoaded':
            return { ...prevState, mods: action.mods };
        case 'userLoaded':
            return { ...prevState, user: action.user };
        case 'messageAdded':
            return { ...prevState, messages: [action.message, ...prevState.messages] };
        case 'messageCleared':
            return { ...prevState, messages: prevState.messages.filter(m => m !== action.message) };
        default:
            return prevState;
    }
};