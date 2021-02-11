import React, { useState, useEffect } from 'react';
import { getState } from '../../state';
import userStartupActions from '../../actions/user';
import serverStartupActions from '../../actions/servers';
import { makeStyles } from '@material-ui/core';

const useStyles = makeStyles({
    container: {
        display: 'flex',
        width: '100vw',
        height: '100vh',
        alignItems: 'center',
        justifyContent: 'center'
    },
    loading: {
        maxWidth: '550px'
    },
    innerContainer: {
        textAlign: 'center'
    }
});

export const AppLoaderContent = () => {
    const classes = useStyles();

    return (
        <div className={classes.container}>
            <div className={classes.innerContainer}>
                <img src="/images/spinner.gif" className={classes.loading}/>
                <h1>Mineman starting...</h1>
            </div>
        </div>
    );
}

const AppLoader: React.FunctionComponent = ({
    children
}) => {
    const { dispatch } = getState();
    const [isCompleted, setIsCompleted] = useState(false);

    useEffect(() => {
        userStartupActions(dispatch)
            .then(() =>
                Promise.all([
                    serverStartupActions(dispatch)
                ])
            )
            .then(() => setIsCompleted(true));
    }, [dispatch]);

    return isCompleted ?
        <>{children}</> :
        <AppLoaderContent />;
}

export default AppLoader;