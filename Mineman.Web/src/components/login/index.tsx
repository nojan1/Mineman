import { Button, Container, FormControl, TextField, Typography } from '@material-ui/core';
import React from 'react';

const Login: React.FunctionComponent = () => {

    return (
        <Container maxWidth="xs" title='Login'>
            <Typography variant='h4' align='center'>
                Login to Mineman
            </Typography>

            <form onSubmit={() => {window.location.href = '/'}}>
                <TextField label='Username' required fullWidth autoFocus margin="normal"/>
                <TextField label='Password' type='password' required fullWidth autoFocus margin="normal"/>

                <Button color='primary' variant='contained' type='submit' fullWidth>
                    Login
                </Button>
            </form>
        </Container>
    );
};

export default Login;