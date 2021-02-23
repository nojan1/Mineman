import styled from '@emotion/styled';
import React, { FormEvent, useState } from 'react';
import { Button, Form, Modal, Tab, Tabs } from 'react-bootstrap';
import { ColumnMapping, ColumnMappingSettings, ColumnType, TabPageSettings } from './types';

const LoadingOverlay = styled.div<{ isLoading: boolean }>`
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 1;
    display: ${props => props.isLoading ? 'flex' : 'none'};
    align-items: center;
    justify-content: center;
    background: rgba(0,0,0,0.4) !important;

    img{
        max-width: 60%;
    }
`;


export interface EditModalProps {
    currentItem?: any;
    columnMapping: ColumnMapping;
    unsetItem: () => void;
    onSave: (item: any, isNew: boolean) => Promise<any>;
    onUpdate: (key: string, value: any) => void;
}

const EditModal: React.FunctionComponent<EditModalProps> = ({
    currentItem,
    columnMapping,
    unsetItem,
    onSave,
    onUpdate,
}) => {
    const [loading, setLoading] = useState<boolean>(false);

    const onDoSave = (e: FormEvent<HTMLFormElement>) => {
        e.preventDefault();
        setLoading(true);

        onSave(currentItem, currentItem.isNew)
            .then(unsetItem)
            .finally(() => setLoading(false));
    }

    const renderControl = (prop: string, settings: ColumnMappingSettings) => {
        const value = (currentItem as any)?.[prop] ?? settings.default ?? '';

        if (settings.component)
            return React.createElement((settings.component as any), { value: value, onChange: () => { } });
        else if (settings.type === ColumnType.bool)
            return (
                <Form.Check

                />
            );
        else
            return (
                <Form.Control
                    type={settings.type === ColumnType.number ? 'number' : 'text'}
                    value={value}
                    required={settings.required}
                    onChange={(e) => onUpdate(prop, e.target.value)} />
            );
    }

    const renderControls = (columns: { [key: string]: ColumnMappingSettings }) =>
        Object.entries(columns).filter(([_, settings]) => !settings.hideFromEditor).map(([prop, settings]) =>
            <Form.Group key={prop}>
                <Form.Label>{settings.label}</Form.Label>
                {renderControl(prop, settings)}
            </Form.Group>
        )

    const renderTabs = (tabs: TabPageSettings[]) =>
        <Tabs>
            {tabs.map((t, i) =>
                <Tab key={i} eventKey={t.title} title={t.title}>
                    {renderControls(t.columns)}
                </Tab>
            )}
        </Tabs>

    return (
        <Modal centered
            show={currentItem !== undefined}
            onHide={unsetItem}>
            <LoadingOverlay isLoading={loading}>
                <img src="images/spinner.gif" />
            </LoadingOverlay>

            <Form onSubmit={onDoSave}>
                <Modal.Header closeButton>
                    <Modal.Title>
                        {(currentItem as any)?.isNew ? 'Create' : 'Edit'}
                    </Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    {Array.isArray(columnMapping)
                        ? renderTabs(columnMapping as TabPageSettings[])
                        : renderControls(columnMapping as { [key: string]: ColumnMappingSettings })
                    }
                </Modal.Body>
                <Modal.Footer>
                    <Button variant="primary" type="submit" disabled={loading}>Save changes</Button>
                </Modal.Footer>
            </Form>
        </Modal>
    );
};

export default EditModal;