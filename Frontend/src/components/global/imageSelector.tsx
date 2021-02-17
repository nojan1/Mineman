import React from 'react';
import { Dropdown, Form } from 'react-bootstrap';
import { ImageModel } from '../../models/image';
import { getState } from '../../state';

export interface ImageSelectorProps {
    label: string;
    value?: ImageModel;
    onChange: (image: ImageModel) => void
}

const ImageSelector: React.FunctionComponent<ImageSelectorProps> = ({
    label,
    value,
    onChange
}) => {
    const { state: { images } } = getState();

    return (
        <Form.Group>
            <Form.Label>{label}</Form.Label>
            <Form.Control as='select'>
                {images.map(i =>
                    <option key={i.id} value={i.id} selected={value?.id === i.id}>
                        {i.name}
                    </option>
                )}
            </Form.Control>
        </Form.Group>
    );
};

export default ImageSelector;