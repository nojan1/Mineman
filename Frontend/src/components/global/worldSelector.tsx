import React from 'react';
import { Form } from 'react-bootstrap';
import { WorldModel } from '../../models/world';
import { getState } from '../../state';

export interface WorldSelectorProps {
    label: string;
    value?: WorldModel;
    required?: boolean;
    onChange: (event: any) => void;
}

const WorldSelector:React.FunctionComponent<WorldSelectorProps> = ({
    label,
    value,
    required,
    onChange
}) => {
    const { state: { worlds } } = getState();

    const onDropdownChange = (e: React.ChangeEvent<any>) => {
        const world = worlds.find(w => w.id == e.target.value);
        onChange({target: {value: world}});
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
                {worlds.map(i =>
                    <option key={i.id} value={i.id}>
                        {i.displayName}
                    </option>
                )}
            </Form.Control>
        </Form.Group>
    );
};

export default WorldSelector;