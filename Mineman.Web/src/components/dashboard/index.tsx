import React from 'react';
import ServerCard from './serverCard';
import { ServerModel } from '../../models/server';

const Dashboard:React.FunctionComponent = () => {

    const servers: ServerModel[] = [
        {
            id: '1',
            description: 'Test server',
            hasMap: false,
            isAlive: false,
            mainPort: 56232
        }
    ];

    return (
        <>
            {servers.map(s => <ServerCard server={s} key={s.id} />)}
        </>
    );
};

export default Dashboard;