import React, { PropsWithChildren } from 'react';
import Table from 'react-bootstrap/Table';
import Button from 'react-bootstrap/Button';
import {AiFillEdit} from 'react-icons/all';

export interface EditProps<T> {
    columnMapping: { [key: string]: string }
    data?: T[];
}

const Edit = <T,>({
    data,
    columnMapping
}: PropsWithChildren<EditProps<T>>) => {
    return (
        <>
            <Table>
                <thead>
                    <tr>
                        {Object.entries(columnMapping).map(([prop, name]) =>
                            <td key={prop}>{name}</td>
                        )}
                        <td />
                    </tr>
                </thead>
                <tbody>
                    {data?.map((row:any) => 
                        <tr key={row.id}>
                            {Object.keys(columnMapping).map(key => 
                                <td key={key}>
                                    {row[key]}
                                </td>    
                            )}

                            <td>
                                <Button>
                                    <AiFillEdit />
                                </Button>
                            </td>
                        </tr>
                    )}
                </tbody>
            </Table>

            {/* <Fab color="secondary" className={classes.fab}>
                <AddIcon />
            </Fab> */}
        </>
    );
};

export default Edit;