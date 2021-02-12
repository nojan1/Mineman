import { createStyles, makeStyles, Theme, Fab, Table, TableHead, TableBody, TableRow, TableCell, IconButton } from '@material-ui/core';
import AddIcon from '@material-ui/icons/Add';
import EditIcon from '@material-ui/icons/Edit';
import React, { PropsWithChildren } from 'react';

const useStyles = makeStyles((theme: Theme) =>
    createStyles({
        fab: {
            position: 'absolute',
            bottom: theme.spacing(2),
            right: theme.spacing(2),
        }
    }),
);

export interface EditProps<T> {
    columnMapping: { [key: string]: string }
    data?: T[];
}

const Edit = <T,>({
    data,
    columnMapping
}: PropsWithChildren<EditProps<T>>) => {
    const classes = useStyles();

    return (
        <>
            <Table>
                <TableHead>
                    <TableRow>
                        {Object.entries(columnMapping).map(([prop, name]) =>
                            <TableCell key={prop}>{name}</TableCell>
                        )}
                        <TableCell />
                    </TableRow>
                </TableHead>
                <TableBody>
                    {data?.map((row:any) => 
                        <TableRow key={row.id}>
                            {Object.keys(columnMapping).map(key => 
                                <TableCell key={key}>
                                    {row[key]}
                                </TableCell>    
                            )}

                            <TableCell>
                                <IconButton>
                                    <EditIcon />
                                </IconButton>
                            </TableCell>
                        </TableRow>
                    )}
                </TableBody>
            </Table>

            <Fab color="secondary" className={classes.fab}>
                <AddIcon />
            </Fab>
        </>
    );
};

export default Edit;