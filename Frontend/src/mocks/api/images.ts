import MockAdapter from "axios-mock-adapter/types";
import { ImageModel } from "../../models/image";

const setupImagesApiMock = (mock: MockAdapter) => {
    mock.onGet(`${process.env.REACT_APP_BACKEND_URL}/api/image`)
        .reply(200, [
            {
                id: 1,
                name: 'Vanilla 13.1',
                serversUsingImage: [],
                buildStatus: {
                    buildSucceeded: true,
                    id: 1,
                    log: ''
                }
            }
        ] as ImageModel[]);
}

export default setupImagesApiMock;