import MockAdapter from "axios-mock-adapter/types";
import { ImageModel } from "../../models/image";
import { RemoteImageModel } from "../../models/remoteImage";

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

        mock.onGet(`${process.env.REACT_APP_BACKEND_URL}/api/image/remote`)
        .reply(200, [
            {
                displayName: 'Some remote image',
                shA256Hash: '1234ldjsfhsdf12312312323'
            }
        ] as RemoteImageModel[]);

        mock.onPost(`${process.env.REACT_APP_BACKEND_URL}/api/image/remote/1234ldjsfhsdf12312312323`)
        .replyOnce(200, {
            id: new Date().getSeconds(),
            name: 'Some remote image',
            modDirectory: '',
            remoteHash: '1234ldjsfhsdf12312312323',
            buildStatus: undefined,
            serversUsingImage: []
        });
}

export default setupImagesApiMock;