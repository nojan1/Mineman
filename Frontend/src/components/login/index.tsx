import Container from 'react-bootstrap/Container';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import React, { FormEvent } from 'react';
import styled from '@emotion/styled';
import { login } from '../../actions/login';

const LoginButton = styled(Button)`
    display: block;
    width: 100%;
    max-width: 70%;
    margin:0 auto;
`;

const Login: React.FunctionComponent = () => {

    const onSubmit = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        login("", "").then(() => {
            window.location.href = '/';
        });
    }

    return (
        <Container style={{ maxWidth: '500px' }}>
            <h4>
                Login to Mineman
            </h4>

            <Form onSubmit={onSubmit}>
                <Form.Group>
                    <Form.Label>Username</Form.Label>
                    <Form.Control type="text" />
                </Form.Group>

                <Form.Group>
                    <Form.Label>Password</Form.Label>
                    <Form.Control type="password" />
                </Form.Group>

                <LoginButton variant='primary' type='submit'>
                    Login
                </LoginButton>
            </Form>
        </Container>
    );
};

export default Login;