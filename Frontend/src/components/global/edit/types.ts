export enum ColumnType {
    text,
    number,
    bool
}

export type ColumnMappingSettings = {
    label: string;
    type?: ColumnType;
    hideFromTable?: boolean;
    hideFromEditor?: boolean;
    required?: boolean;
    default?: any;
    valueFormater?: (value: any) => string;
    component?: React.Component<{value: any, onChange: (value: any) => void}>
};

export type TabPageSettings = {
    title: string;
    columns: { [key: string]: ColumnMappingSettings };
};

export type ColumnMapping = 
    { [key: string]: ColumnMappingSettings } |
    TabPageSettings[];

export type IsNewExtension = {
    isNew?: boolean
};