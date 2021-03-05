import React from 'react';
import { Dropdown, Form } from 'react-bootstrap';
import { ImageModel } from '../../models/image';
import { getState } from '../../state';

export interface ImageSelectorProps {
    label: string;
    value?: ImageModel;
    required?: boolean;
    onChange: (event: any) => void;
}

const ImageSelector: React.FunctionComponent<ImageSelectorProps> = ({
    label,
    value,
    onChange,
    required
}) => {
    const { state: { images } } = getState();

    const onDropdownChange = (e: React.ChangeEvent<any>) => {
        const image = images.find(w => w.id == e.target.value);
        onChange({target: {value: image}});
    }

    return (
        <Form.Group>
            <Form.Label>{label}</Form.Label>
            <Form.Control 
                as='select' 
                defaultValue={value?.id ?? ''} 
                required={required}
                onChange={onDropdownChange}
            >
                <option key=''></option>
                {images.map(i =>
                    <option key={i.id} value={i.id}>
                        {i.name}
                    </option>
                )}
            </Form.Control>
        </Form.Group>
    );
};

export default ImageSelector;