import React, { useCallback } from 'react';
import { getState } from '../../state';
import Edit from '../global/edit';

const column = {
    'displayName': { label: 'Name', required: true }
};

const Mods: React.FunctionComponent = () => {
    const { state: { mods }, dispatch } = getState();

    const onSave = useCallback((mod: any, isNew: boolean) => {
        return new Promise<void>(resolve => resolve());
    }, [dispatch]);

    return (
        <>
            <Edit data={mods} columnMapping={column} onSave={onSave} />
        </>
    );
};

export default Mods; 