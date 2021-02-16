import MockAdapter from "axios-mock-adapter/types";
import { ModsModel } from "../../models/mods";

const setupModsApiMock = (mock: MockAdapter) => {
    mock.onGet(`${process.env.REACT_APP_BACKEND_URL}/api/mod`)
        .reply(200, [

        ] as ModsModel[]);
}

export default setupModsApiMock;