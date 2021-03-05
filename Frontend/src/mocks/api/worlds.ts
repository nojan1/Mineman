import MockAdapter from "axios-mock-adapter/types";
import { WorldModel } from "../../models/world";

const setupWorldsApiMock = (mock: MockAdapter) => {
    mock.onGet(`${process.env.REACT_APP_BACKEND_URL}/api/world`)
        .reply(200, [
            {
                id: 1,
                displayName: 'Home MP',
                serversUsingWorld: []
            }
        ] as WorldModel[]);
}

export default setupWorldsApiMock;