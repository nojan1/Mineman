import React from 'react';
import { Drawer, List, ListItem, ListItemIcon, ListItemText, makeStyles, Theme } from '@material-ui/core';

import DashboardIcon from '@material-ui/icons/Dashboard';
import PublicIcon from '@material-ui/icons/Public';
import WebAssetIcon from '@material-ui/icons/WebAsset';
import LibraryAddIcon from '@material-ui/icons/LibraryAdd';
import LockOpenIcon from '@material-ui/icons/LockOpen';

import { Link } from 'react-router-dom';

const drawerWidth = 240;

const useStyles = makeStyles((theme: Theme) => ({
  root: {
    display: 'flex',
  },
  appBar: {
    width: `calc(100% - ${drawerWidth}px)`,
    marginLeft: drawerWidth,
  },
  drawer: {
    width: drawerWidth,
    flexShrink: 0,
  },
  drawerPaper: {
    width: drawerWidth,
  },
  // necessary for content to be below app bar
  toolbar: theme.mixins.toolbar,
  content: {
    flexGrow: 1,
    backgroundColor: theme.palette.background.default,
    padding: theme.spacing(3),
  },
}));

const ListItemLink = (props: any) =>
    <ListItem button component={Link} {...props} />

const Menu: React.FunctionComponent = () => {
    const classes = useStyles();

    return (
        <Drawer
            className={classes.drawer}
            variant="permanent"
            classes={{
                paper: classes.drawerPaper,
            }}
            anchor="left"
        >
            <List>
                <ListItemLink to='/'>
                    <ListItemIcon>
                        <DashboardIcon />
                    </ListItemIcon>
                    <ListItemText primary='Dashboard' />
                </ListItemLink>
                <ListItemLink to='/servers'>
                    <ListItemIcon>
                        <PublicIcon /> 
                    </ListItemIcon>
                    <ListItemText primary='Servers' />
                </ListItemLink>
                <ListItemLink to='/images'>
                    <ListItemIcon>
                        <WebAssetIcon />
                    </ListItemIcon>
                    <ListItemText primary='Images'/>
                </ListItemLink>
                <ListItemLink to='/mods'>
                    <ListItemIcon>
                        <LibraryAddIcon />
                    </ListItemIcon>
                    <ListItemText primary='Mods'/>
                </ListItemLink>

                <ListItemLink to='/login'>
                    <ListItemIcon>
                        <LockOpenIcon />
                    </ListItemIcon>
                    <ListItemText primary='Log in'/>
                </ListItemLink>
            </List>
        </Drawer>
    );
};

export default Menu;