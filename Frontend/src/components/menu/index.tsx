import React from 'react';
import Nav from 'react-bootstrap/Nav';
import { MdDashboard, MdLibraryAdd, MdLock, MdLockOpen, MdStorage } from 'react-icons/md';
import { FaServer } from 'react-icons/fa';
import { BiWorld } from 'react-icons/bi';
import { Link } from 'react-router-dom';
import { getState } from '../../state';
import { removedStoredToken } from '../../auth/token';

const Menu: React.FunctionComponent = () => {
    const { state: { user } } = getState();

    const doLogout = () => {
        removedStoredToken();
        window.location.href = '/';
    };

    return (
        <Nav defaultActiveKey="/home" className="flex-column">
            <Nav.Link as={Link} to="/">
                <MdDashboard />
                Dashboard
            </Nav.Link>
            {user ? <>
                <Nav.Link as={Link} to="/servers">
                    <FaServer />
                Servers
            </Nav.Link>
                <Nav.Link as={Link} to="/images">
                    <MdStorage />
                Images
            </Nav.Link>
                <Nav.Link as={Link} to="/worlds">
                    <BiWorld />
                Worlds
            </Nav.Link>
                <Nav.Link as={Link} to="/mods">
                    <MdLibraryAdd />
                Mods
            </Nav.Link>
            </> : null}

            {user ?
                <Nav.Link onClick={doLogout}>
                    <MdLock />
                    Log out
                </Nav.Link>
                :
                <Nav.Link as={Link} to="/login">
                    <MdLockOpen />
                    Log in
                </Nav.Link>
            }
        </Nav>
    );
};

export default Menu;