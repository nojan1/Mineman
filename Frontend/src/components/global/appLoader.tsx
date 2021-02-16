import React, { useState, useEffect } from 'react';
import { getState } from '../../state';
import userStartupActions from '../../actions/startup/user';
import serverStartupActions from '../../actions/startup/servers';
import styled from '@emotion/styled';

const OuterContainer = styled.div`
    display: flex;
    width: 100vw,;
    height: 100vh;
    align-items: center;
    justify-content: center;
    text-align: center;
`;

const LoadingImage = styled.img`
    max-width: 550px;
`;

export const AppLoaderContent = () => {
    return (
        <OuterContainer>
            <div>
                <LoadingImage src="/images/spinner.gif" />
                <h1>Mineman starting...</h1>
            </div>
        </OuterContainer>
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