import React, { useCallback } from 'react';
import { Tab, Table, Tabs } from 'react-bootstrap';
import { MdCloudDownload } from 'react-icons/md';
import { addImage, deleteImage, importImage } from '../../actions/images';
import { ImageModel } from '../../models/image';
import { RemoteImageModel } from '../../models/remoteImage';
import { getState } from '../../state';
import ActionButton from '../global/actionButton';
import Edit from '../global/edit';
import { ColumnType } from '../global/edit/types';
import { FittingCell } from '../global/tableHelpers';

const column = {
    'name': { label: 'Name', required: true },
    'modDirectory': { label: 'Mod directory (leave empty to disable mod support', hideFromTable: true },
    // 'modSupported': { label: 'Mods', hideFromEditor: true, valueFormater: (value: any) => value.modDirectory ? 'Yes' : 'No'},
    'buildStatus': { label: 'Build', hideFromEditor: true, valueFormater: (value: any) => value?.buildSuccessful ? 'Yes' : 'No' },
    'useCount': { label: 'Use count', hideFromEditor: true, valueFormater: (value: ImageModel) => value?.serversUsingImage?.length.toString() ?? "0" },
    'imageContents': { label: 'Image content (zip)', hideFromTable: true, type: ColumnType.file }
};

const Images: React.FunctionComponent = () => {
    const { state: { images, remoteImages }, dispatch } = getState();

    const hasRemoteImage = (remoteImage: RemoteImageModel) =>
        images?.some(i => i.remoteHash == remoteImage.shA256Hash) ?? false;

    const onSave = useCallback((image: any, _: boolean) => addImage(dispatch, image), [dispatch]);
    const onDelete = useCallback((image: ImageModel) => deleteImage(dispatch, image.id), [dispatch]);

    return (
        <Tabs>
            <Tab eventKey='local' title='Installed'>
                <Edit
                    data={images}
                    columnMapping={column}
                    onSave={onSave}
                    onDelete={onDelete}
                    canDelete={(image: ImageModel) => !image?.serversUsingImage?.length}
                    supportEdit={false}
                />
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
                            <tr key={i.shA256Hash}>
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
                                        <MdCloudDownload /> &nbsp;
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