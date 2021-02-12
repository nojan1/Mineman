import React from 'react';
import Card from 'react-bootstrap/Card';
import Dropdown from 'react-bootstrap/Dropdown';
import { MdSettings } from 'react-icons/md';
import { ServerModel } from '../../models/server';
import { getState } from '../../state';

const getImageOrDefault = (server: ServerModel) =>
    server.hasMap ? '' : 'images/map-default.png';

export interface ServerCardProps {
    server: ServerModel;
}

const ServerCard: React.FunctionComponent<ServerCardProps> = ({
    server
}) => {
    const { state: { user } } = getState();

    return (
        <Card style={{ width: '350px' }}>
            <Card.Img variant="top" src={getImageOrDefault(server)} />
            <Card.Body>
                <Card.Title>{server.description}</Card.Title>
                <Card.Text>
                    {!server.isAlive ?
                        <h5>Offline</h5>
                        : null}

                    {server.motd &&
                        <p>
                            {server.motd}
                        </p>
                    }

                    {server.query &&
                        <>
                            <p>
                                <b>Players:</b> {server.query.numPlayers} / {server.query.maxPlayers} <br />
                                <b>Software:</b> {server.query.responseFields['game_id']} {server.query.responseFields['version']}
                            </p>
                        </>
                    }
                </Card.Text>
            </Card.Body>
            {user ?
                <Card.Footer>
                    <Dropdown style={{float: 'right'}}>
                        <Dropdown.Toggle variant='default' id="dropdown-basic">
                            <MdSettings />
                        </Dropdown.Toggle>

                        <Dropdown.Menu>
                            <Dropdown.Item href="#/">{server.isAlive ? 'Stop' : 'Start'}</Dropdown.Item>
                            <Dropdown.Item href="#/">Configure</Dropdown.Item>
                            <Dropdown.Item href="#/">Logs</Dropdown.Item>
                            <Dropdown.Item href="#/">Commands</Dropdown.Item>
                        </Dropdown.Menu>
                    </Dropdown>
                </Card.Footer>
                : null}
        </Card>
    );
};

export default ServerCard;