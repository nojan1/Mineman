import React from 'react';
import { WorldModel } from '../../models/world';

export interface WorldSelectorProps {
    label: string;
    value?: WorldModel;
    onChange: (image: WorldModel) => void
}

const WorldSelector:React.FunctionComponent<WorldSelectorProps> = ({

}) => {

    return (
        <></>
    );
};

export default WorldSelector;