import React from 'react';
import { Button, Card, CardActionArea, CardActions, CardContent, CardMedia, Typography } from '@material-ui/core';
import { ServerModel } from '../../models/server';

const getImageOrDefault = (server: ServerModel) =>
    server.hasMap ? '' : 'images/map-default.png';

export interface ServerCardProps {
    server: ServerModel;
}

const ServerCard:React.FunctionComponent<ServerCardProps> = ({
    server
}) => {

    return (
        <Card>
            <CardActionArea>
                <CardMedia image={getImageOrDefault(server)}/>
            </CardActionArea>
            <CardContent>
                <Typography>
                    {server.description}
                </Typography>
                {server.info && 
                    <Typography>
                        {server.info!.motd}
                    </Typography>
                }
            </CardContent>
            <CardActions>
                
            </CardActions>
        </Card>
    );
};

export default ServerCard;