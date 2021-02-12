import React from 'react';
import { getState } from '../../state';
import Edit from '../global/edit';

const column = {
    'description': 'Server description',
}

const Servers: React.FunctionComponent = () => {
    const {state: {servers}} = getState();

    return (
        <>
            <Edit data={servers} columnMapping={column}/>
        </>
    );
};

export default Servers;