import React, { PropsWithChildren, useState } from 'react';
import { MdAdd } from 'react-icons/md';
import Table from 'react-bootstrap/Table';
import Button from 'react-bootstrap/Button';
import { AiFillEdit } from 'react-icons/all';
import styled from '@emotion/styled';
import { ColumnMapping, ColumnMappingSettings, IsNewExtension, TabPageSettings } from './types';
import EditModal from './editModal';

const FloatingButton = styled(Button)`
    position: fixed;
    bottom: 20px;
    right: 20px;
`;

const FittingCell = styled.td`
    width:1%;
    white-space:nowrap;
`;

const flattenColumns = (columnMapping: ColumnMapping): { [key: string]: ColumnMappingSettings } => {
    if(Array.isArray(columnMapping))
        return (columnMapping as TabPageSettings[])
            .reduce((a,b) => ({...a, ...b.columns}), {});

    return columnMapping as { [key: string]: ColumnMappingSettings };
}

export interface EditProps<T> {
    columnMapping: ColumnMapping
    data?: T[];
    onSave: (item: T, isNew: boolean) => Promise<any>;
    onDelete?: (item: T) => Promise<any>;
}

const Edit = <T,>({
    data,
    columnMapping,
    onSave,
    onDelete
}: PropsWithChildren<EditProps<T>>) => {
    const [currentItem, setCurrentItem] = useState<(T | IsNewExtension)>();

    const flattenedColumns = flattenColumns(columnMapping);

    return (
        <>
            <Table hover>
                <thead>
                    <tr>
                        {Object.entries(flattenedColumns).filter(([_, setting]) => !setting.hideFromTable).map(([prop, setting]) =>
                            <th key={prop}>{setting.label}</th>
                        )}
                        <th></th>
                    </tr>
                </thead>
                <tbody>
                    {data?.map((row: any) =>
                        <tr key={row.id}>
                            {Object.entries(flattenedColumns).filter(([_, setting]) => !setting.hideFromTable).map(([prop, _]) =>
                                <td key={prop}>
                                    {row[prop]}
                                </td>
                            )}

                            <FittingCell>
                                <Button size='sm' onClick={() => setCurrentItem({...row})}>
                                    <AiFillEdit />
                                </Button>
                            </FittingCell>
                        </tr>
                    )}
                </tbody>
            </Table>

            <EditModal 
                columnMapping={columnMapping} 
                onSave={onSave} 
                onDelete={onDelete} 
                currentItem={currentItem} 
                unsetItem={() => setCurrentItem(undefined)}
                onUpdate={(key: string, value: any) => setCurrentItem(item => ({...item, [key]: value}))} />

            <FloatingButton onClick={() => setCurrentItem({ isNew: true })}>
                <MdAdd />
            </FloatingButton>
        </>
    );
};

export default Edit;