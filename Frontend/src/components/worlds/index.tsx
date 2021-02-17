import React, { useCallback } from 'react';
import { getState } from '../../state';
import Edit from '../global/edit';

const column = {
    'displayName': { label: 'Name', required: true }
};

const Worlds: React.FunctionComponent = () => {
    const { state: { worlds }, dispatch } = getState();

    const onSave = useCallback((world: any, isNew: boolean) => {
        return new Promise<void>(resolve => resolve());
    }, [dispatch]);

    return (
        <>
            <Edit data={worlds} columnMapping={column} onSave={onSave} />
        </>
    );
};

export default Worlds; 