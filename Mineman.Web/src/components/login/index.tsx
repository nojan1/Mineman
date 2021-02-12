import Container from 'react-bootstrap/Container';
import Form from 'react-bootstrap/Form';
import Button from 'react-bootstrap/Button';
import React from 'react';

const Login: React.FunctionComponent = () => {

    return (
        <Container>
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

                <Button variant='primary' type='submit'>
                    Login
                </Button>
            </Form>
        </Container>
    );
};

export default Login;