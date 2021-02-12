import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import AppLoader from './components/global/appLoader';
import Menu from './components/menu';
import Login from './components/login';
import Dashboard from './components/dashboard';
import Servers from './components/servers';
import Images from './components/images';
import Worlds from './components/worlds';
import Mods from './components/mods';
import styled from '@emotion/styled';

const AppContainer = styled.div`
    display: flex;
`;

const App: React.FunctionComponent = () => {
    return (
        <Router>
            <Switch>
                <Route path='/login' exact>
                    <Login />
                </Route>
                <Route path=''>
                    <AppLoader>
                        <AppContainer>
                            <Menu />
                            <main>
                                <Switch>
                                    <Route path='/servers'>
                                        <Servers />
                                    </Route>
                                    <Route path='/images'>
                                        <Images />
                                    </Route>
                                    <Route path='/worlds'>
                                        <Worlds />
                                    </Route>
                                    <Route path='/mods'>
                                        <Mods />
                                    </Route>
                                    <Route path='/'>
                                        <Dashboard />
                                    </Route>
                                </Switch>
                            </main>
                        </AppContainer>
                    </AppLoader>
                </Route>
            </Switch>
        </Router>
    );
};

export default App;