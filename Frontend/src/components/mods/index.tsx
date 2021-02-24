import React, { useCallback } from 'react';
import { addMod, deleteMod } from '../../actions/mods';
import { ModsModel } from '../../models/mods';
import { getState } from '../../state';
import { ColumnType } from '../global/edit/types';
import Edit from '../global/edit';

const column = {
    'displayName': { label: 'Name', required: true },
    'modFile': { label: 'Mod file', hideFromTable: true, type: ColumnType.file}
};

const Mods: React.FunctionComponent = () => {
    const { state: { mods }, dispatch } = getState();

    const onSave = useCallback((mod: any, _: boolean) => addMod(dispatch, mod), [dispatch]);
    const onDelete = useCallback((mod: ModsModel) => deleteMod(dispatch, mod.id), [dispatch]);

    return (
        <>
            <Edit 
                data={mods} 
                columnMapping={column} 
                onSave={onSave} 
                onDelete={onDelete} 
                supportEdit={false}
                canDelete={(mod: ModsModel) => !mod?.serversUsingMod?.length}
            />
        </>
    );
};

export default Mods; 