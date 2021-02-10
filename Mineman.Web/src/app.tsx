import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import Menu from './components/menu';
import Dashboard from './components/dashboard';
import { makeStyles, Theme } from '@material-ui/core';

const useStyles = makeStyles((theme: Theme) => ({
    appContainer: {
        display: 'flex'
    }
}));

const App: React.FunctionComponent = () => {
    const styles = useStyles();

    return (
        <Router>
            <Switch>
                <Route path='/login' exact>
                    <h1>Log me in</h1>
                </Route>
                <Route path=''>
                    <div className={styles.appContainer}>
                        <Menu />
                        <main>
                            <Switch>
                                <Route path='/'>
                                    <Dashboard />
                                </Route>
                            </Switch>
                        </main>
                    </div>
                </Route>
            </Switch>
        </Router>
    );
};

export default App;