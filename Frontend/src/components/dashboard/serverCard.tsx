import React from 'react';
import Card from 'react-bootstrap/Card';
import Dropdown from 'react-bootstrap/Dropdown';
import Alert from 'react-bootstrap/Alert';
import { MdSettings, MdPlayArrow, MdStop, MdSpaceBar, MdPages } from 'react-icons/md';
import { ServerModel } from '../../models/server';
import { getState } from '../../state';
import DropdownItemActionButton from '../global/dropdownItemActionButton';
import { startServer, stopServer } from '../../actions/servers';

const getImageOrDefault = (server: ServerModel) =>
    server.hasMap ? '' : 'images/map-default.png';

export interface ServerCardProps {
    server: ServerModel;
}

const ServerCard: React.FunctionComponent<ServerCardProps> = ({
    server
}) => {
    const { state: { user }, dispatch } = getState();

    return (
        <Card style={{ width: '350px' }}>
            <Card.Img variant="top" src={getImageOrDefault(server)} />
            <Card.Body>
                <Card.Title>{server.description}</Card.Title>
                <Card.Text>
                    {!server.isAlive ?
                        <Alert variant='warning'>
                            <Alert.Heading>Offline</Alert.Heading>
                            This is server is currently not running
                        </Alert>
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
                            <DropdownItemActionButton
                                iconComponent={server.isAlive ? MdStop: MdPlayArrow}
                                action={() => server.isAlive ? stopServer(dispatch, server.id) : startServer(dispatch, server.id)}
                            >
                                {server.isAlive ? 'Stop' : 'Start'}
                            </DropdownItemActionButton>
                            <Dropdown.Item href="#/">
                                <MdPages /> &nbsp;
                                Logs
                            </Dropdown.Item>
                            <Dropdown.Item href="#/">
                                <MdSpaceBar /> &nbsp;
                                Commands    
                            </Dropdown.Item>
                        </Dropdown.Menu>
                    </Dropdown>
                </Card.Footer>
                : null}
        </Card>
    );
};

export default ServerCard;