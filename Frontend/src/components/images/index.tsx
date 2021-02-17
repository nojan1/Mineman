import React, { useCallback } from 'react';
import { getState } from '../../state';
import Edit from '../global/edit';

const column = {
    'name': { label: 'Name', required: true },
    'modDirectory': { label: 'Mod directory (leave empty to disable mod support', hideFromTable: true },
    // 'modSupported': { label: 'Mods', hideFromEditor: true, valueFormater: (value: any) => value.modDirectory ? 'Yes' : 'No'},
    'buildStatus': { label: 'Build', hideFromEditor: true, valueFormater: (value: any) => value?.buildSuccessful ? 'Yes' : 'No'}
};

const Images: React.FunctionComponent = () => {
    const { state: { images }, dispatch } = getState();

    const onSave = useCallback((image: any, isNew: boolean) => {
        return new Promise<void>(resolve => resolve()); 
    }, [dispatch]);

    return (
        <>
            <Edit data={images} columnMapping={column} onSave={onSave} />
        </>
    );
};

export default Images;