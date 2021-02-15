import Container from 'react-bootstrap/Container';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import React from 'react';
import styled from '@emotion/styled';

const LoginButton = styled(Button)`
    display: block;
    width: 100%;
    max-width: 70%;
    margin:0 auto;
`;

const Login: React.FunctionComponent = () => {

    return (
        <Container style={{maxWidth: '500px'}}>
            <h4>
                Login to Mineman
            </h4>

            <Form>
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