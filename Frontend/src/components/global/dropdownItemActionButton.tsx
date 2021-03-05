import React, { useState } from 'react';
import { Dropdown } from 'react-bootstrap';

export interface DropdownItemActionButtonProps {
    iconComponent: any;
    action: (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => Promise<any>
}

const DropdownItemActionButton:React.FunctionComponent<DropdownItemActionButtonProps> = ({
    iconComponent,
    children,
    action
}) => {
    const [loading, setLoading] = useState<boolean>(false);

    const onClick = (event: React.MouseEvent<HTMLButtonElement, MouseEvent>) => {
        event.preventDefault();
        setLoading(true);

        action(event)
            .finally(() => setLoading(false));
    };

    return (
        <Dropdown.Item onClick={onClick} disabled={loading}>
            {loading 
                ? <img src="images/spinner.gif" style={{height: '20px', marginLeft: '-5px'}}/> 
                : React.createElement(iconComponent)
            }  &nbsp;
            {children}
        </Dropdown.Item>
    );
};

export default DropdownItemActionButton;