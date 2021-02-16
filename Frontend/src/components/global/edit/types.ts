export type ColumnMappingSettings = {
    label: string;
    type?: string;
    hideFromTable?: boolean;
    required?: boolean;
    default?: any;
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