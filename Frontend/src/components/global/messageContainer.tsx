import styled from '@emotion/styled';
import React from 'react';
import { Alert } from 'react-bootstrap';
import { clearMessage } from '../../actions/messages';
import { getState } from '../../state';

const OuterMessageContainer = styled.div`
    position: absolute;
    bottom: 0;
    left: 5px;
    max-width: 500px;
`;

const MessageContainer:React.FunctionComponent = () => {
    const { state: {messages}, dispatch} = getState();

    return (
        <OuterMessageContainer>
            {messages.map((message,i) => 
                <Alert variant={message.type} key={i} dismissible onClose={() => clearMessage(dispatch, message)}>
                    <Alert.Heading>{message.title}</Alert.Heading>
                    {message.content ? <p>{message.content}</p> : null}
                </Alert>
            )}
        </OuterMessageContainer>
    );
};


export default MessageContainer;