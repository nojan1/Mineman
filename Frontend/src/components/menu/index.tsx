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

        // <Drawer
        //     className={classes.drawer}
        //     variant="permanent"
        //     classes={{
        //         paper: classes.drawerPaper,
        //     }}
        //     anchor="left"
        // >
        //     <List>
        //         <ListItemLink to='/'>
        //             <ListItemIcon>
        //                 <DashboardIcon />
        //             </ListItemIcon>
        //             <ListItemText primary='Dashboard' />
        //         </ListItemLink>
        //         {user ?
        //             <>
        //                 <ListItemLink to='/servers'>
        //                     <ListItemIcon>
        //                         <StorageIcon />
        //                     </ListItemIcon>
        //                     <ListItemText primary='Servers' />
        //                 </ListItemLink>
        //                 <ListItemLink to='/images'>
        //                     <ListItemIcon>
        //                         <WebAssetIcon />
        //                     </ListItemIcon>
        //                     <ListItemText primary='Images' />
        //                 </ListItemLink>
        //                 <ListItemLink to='/worlds'>
        //                     <ListItemIcon>
        //                         <PublicIcon />
        //                     </ListItemIcon>
        //                     <ListItemText primary='Worlds' />
        //                 </ListItemLink>
        //                 <ListItemLink to='/mods'>
        //                     <ListItemIcon>
        //                         <LibraryAddIcon />
        //                     </ListItemIcon>
        //                     <ListItemText primary='Mods' />
        //                 </ListItemLink>
        //             </>
        //             : null}



        //     </List>
        // </Drawer>
    );
};

export default Menu;