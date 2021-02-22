import styled from '@emotion/styled';
import React, { useState } from 'react';
import { Button, ButtonProps } from 'react-bootstrap';

const ButtonLoadingOverlay = styled.div<{ isLoading: boolean }>`
    position: absolute;
    width: 100%;
    height: 100%;
    z-index: 1;
    display: ${props => props.isLoading ? 'flex' : 'none'};
    align-items: center;
    justify-content: center;
    background: rgba(0,0,0,0.4) !important;
`;

interface ActionButtonProps extends ButtonProps {
    action: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => Promise<any>
}

const ActionButton: React.FunctionComponent<ActionButtonProps> = (props) => {
    const [loading, setLoading] = useState<boolean>(false);

    const onClick = (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        event.preventDefault();

        setLoading(true);

        props.action(event)
            .finally(() => setLoading(false));
    };

    const buttonProps = Object.fromEntries(
        Object.entries(props)
            .filter(([key]) => key !== 'action')
    );

    return (
        <Button
            onClick={onClick}
            {...buttonProps}
            disabled={loading || buttonProps.disabled}
        >
            <ButtonLoadingOverlay isLoading={loading}>
                <img src="images/spinner.gif" style={{ maxHeight: '30px' }} />
            </ButtonLoadingOverlay>

            {props.children}
        </Button>
    );
}

export default ActionButton;