import MockAdapter from "axios-mock-adapter/types";
import { ServerModel, ServerQueryModel } from '../../models/server';

const setupServerApiMock = (mock: MockAdapter) => {
    mock.onGet(`${process.env.REACT_APP_BACKEND_URL}/api/server`)
        .reply(200, [
            {
                id: '123',
                description: 'This is a mocked test server',
                hasMap: false,
                isAlive: false,
                mainPort: 23
            },
            {
                id: '124',
                description: 'This is another mocked test server',
                hasMap: false,
                isAlive: true,
                mainPort: 24
            }
        ] as ServerModel[]);

    mock.onGet(`${process.env.REACT_APP_BACKEND_URL}/api/server/query/124`)
        .reply(200,
            {
                maxPlayers: 8,
                numPlayers: 2,
                players: [
                    { name: 'bill' }, { name: 'bob' }
                ],
                plugins: [],
                responseFields: {
                    'game_id': 'Minecraft',
                    'version': '13.2'
                }
            } as ServerQueryModel);
}

export default setupServerApiMock;