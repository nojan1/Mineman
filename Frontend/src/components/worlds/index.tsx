import React, { useCallback } from 'react';
import { addWorld, deleteWorld } from '../../actions/worlds';
import { WorldModel } from '../../models/world';
import { getState } from '../../state';
import Edit from '../global/edit';

const column = {
    'displayName': { label: 'Name', required: true },
    'useCount': { label: 'Use count', hideFromEditor: true, valueFormater: (world: WorldModel) => world?.serversUsingWorld?.length.toString() ?? "0"}
};

const Worlds: React.FunctionComponent = () => {
    const { state: { worlds }, dispatch } = getState();

    const onSave = useCallback((world: any, isNew: boolean) => addWorld(dispatch, world), [dispatch]);
    const onDelete = useCallback((world: WorldModel) => deleteWorld(dispatch, world.id), [dispatch]);

    return (
        <>
            <Edit 
                data={worlds} 
                columnMapping={column} 
                onSave={onSave}
                onDelete={onDelete} 
                supportEdit={false}
                canDelete={(world: WorldModel) => !world?.serversUsingWorld?.length} />
        </>
    );
};

export default Worlds; 