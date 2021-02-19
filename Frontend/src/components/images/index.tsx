import React, { useCallback } from 'react';
import { Tab, Table, Tabs } from 'react-bootstrap';
import { MdCloudDownload } from 'react-icons/md';
import { importImage } from '../../actions/images';
import { RemoteImageModel } from '../../models/remoteImage';
import { getState } from '../../state';
import ActionButton from '../global/actionButton';
import Edit from '../global/edit';
import { FittingCell } from '../global/tableHelpers';

const column = {
    'name': { label: 'Name', required: true },
    'modDirectory': { label: 'Mod directory (leave empty to disable mod support', hideFromTable: true },
    // 'modSupported': { label: 'Mods', hideFromEditor: true, valueFormater: (value: any) => value.modDirectory ? 'Yes' : 'No'},
    'buildStatus': { label: 'Build', hideFromEditor: true, valueFormater: (value: any) => value?.buildSuccessful ? 'Yes' : 'No' }
};

const Images: React.FunctionComponent = () => {
    const { state: { images, remoteImages }, dispatch } = getState();

    const hasRemoteImage = (remoteImage: RemoteImageModel) =>
        images?.some(i => i.remoteHash == remoteImage.sHA256Hash) ?? false;

    const onSave = useCallback((image: any, isNew: boolean) => {
        return new Promise<void>(resolve => resolve());
    }, [dispatch]);

    return (
        <Tabs>
            <Tab eventKey='local' title='Installed'>
                <Edit data={images} columnMapping={column} onSave={onSave} />
            </Tab>
            <Tab eventKey='remote' title='Available'>
                <Table>
                    <thead>
                        <tr>
                            <th>Name</th>
                            <th>Supports mods</th>
                            <th>Imported</th>
                            <th />
                        </tr>
                    </thead>
                    <tbody>
                        {remoteImages.map(i =>
                            <tr key={i.sHA256Hash}>
                                <td>
                                    {i.displayName}
                                </td>
                                <td>
                                    {i.modDirectory ? 'Yes' : 'No'}
                                </td>
                                <td>
                                    {hasRemoteImage(i) ? 'Yes' : 'No'}
                                </td>
                                <FittingCell>
                                    <ActionButton
                                        action={() => importImage(dispatch, i)}
                                        disabled={hasRemoteImage(i)}
                                    >
                                        <MdCloudDownload />
                                        Import
                                    </ActionButton>
                                </FittingCell>
                            </tr>
                        )}
                    </tbody>
                </Table>
            </Tab>
        </Tabs>
    );
};

export default Images;