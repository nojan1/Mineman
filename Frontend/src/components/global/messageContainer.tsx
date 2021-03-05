import styled from '@emotion/styled';
import React from 'react';
import { Toast } from 'react-bootstrap';
import { clearMessage } from '../../actions/messages';
import { Message } from '../../models/message';
import { getState } from '../../state';

const OuterMessageContainer = styled.div`
    position: absolute;
    bottom: 10px;
    left: 10px;
    width: 100%;
    max-width: 350px;
`;


const getStyle = (message: Message): any => {
    const baseStyle= {
        padding: '5px',
    }

    if (message.type == 'danger') {
        return {
            ...baseStyle,
            backgroundColor: '#d4441a',
            color: 'white'
        }
    } else {
        return { baseStyle };
    }
}

const MessageContainer: React.FunctionComponent = () => {
    const { state: { messages }, dispatch } = getState();

    return (
        <OuterMessageContainer>
            {messages.map((message, i) =>
                <Toast
                    key={i}
                    onClose={() => clearMessage(dispatch, message)}
                    show={true}
                    delay={5000}
                    // autohide
                    style={getStyle(message)}
                >
                    <Toast.Header>
                        <strong className="mr-auto">{message.title}</strong>
                    </Toast.Header>
                    <Toast.Body>{message.content}</Toast.Body>
                </Toast>
            )}
        </OuterMessageContainer>
    );
};


export default MessageContainer;