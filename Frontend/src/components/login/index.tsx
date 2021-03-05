import Container from 'react-bootstrap/Container';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import React, { FormEvent, useState } from 'react';
import styled from '@emotion/styled';
import { login } from '../../actions/login';
import { Alert } from 'react-bootstrap';

const LoginButton = styled(Button)`
    display: block;
    width: 100%;
    max-width: 70%;
    margin:0 auto;
`;

const Login: React.FunctionComponent = () => {

    const [username, setUsername] = useState<string>('');
    const [password, setPassword] = useState<string>('');
    const [errorMessage, setErrorMessage] = useState<string>();
    const [loading, setLoading] = useState<boolean>(false);

    const onSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);

        login(username, password).then(() => {
            window.location.href = '/';
        })
        .catch(reason => setErrorMessage(reason))
        .finally(() => setLoading(false));
    }

    return (
        <Container style={{ maxWidth: '500px' }}>
            <h4>
                Login to Mineman
            </h4>

            {errorMessage ? 
                <Alert variant='danger'>
                    {errorMessage.toString()}
                </Alert>
            :null}

            <Form onSubmit={onSubmit}>
                <Form.Group>
                    <Form.Label>Username</Form.Label>
                    <Form.Control type="text" value={username} onChange={e => setUsername(e.target.value)} />
                </Form.Group>

                <Form.Group>
                    <Form.Label>Password</Form.Label>
                    <Form.Control type="password" value={password} onChange={e => setPassword(e.target.value)}/>
                </Form.Group>

                <LoginButton variant='primary' type='submit' disabled={loading}>
                    Login
                </LoginButton>
            </Form>
        </Container>
    );
};

export default Login;