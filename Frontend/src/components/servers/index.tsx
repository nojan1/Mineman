import React, { useCallback } from 'react';
import { create, update } from '../../actions/servers';
import { getState } from '../../state';
import Edit from '../global/edit';
import { ColumnType, TabPageSettings } from '../global/edit/types';
import ImageSelector from '../global/imageSelector';
import WorldSelector from '../global/worldSelector';

const column = [
    {
        title: 'Standard',
        columns:
        {
            'description': { label: 'Description', required: true },
            'serverPort': { label: 'Port', type: ColumnType.number, required: true, default: 25565 },
            'memoryAllocationMB': {
                label: 'Memory allocation in MB',
                hideFromTable: true,
                type: ColumnType.number,
                required: true,
                default: 1024
            },
            'image': {
                label: 'Server image',
                valueFormater: image => image?.name,
                component: ImageSelector
            },
            'world': {
                label: 'World',
                valueFormater: world => world?.name,
                component: WorldSelector
            }
        }
    },
    {
        title: 'Server properties',
        columns:
        {
            'max_Tick_Time': { label: 'Max Tick Time', hideFromTable: true, default: 60000 },
            'generator_Settings': { label: 'Generator Settings', hideFromTable: true },
            'allow_Nether': { label: 'Allow Nether', hideFromTable: true, default: true },
            'force_Gamemode': { label: 'Force Gamemode', hideFromTable: true, default: false },
            'gamemode': { label: 'Gamemode', hideFromTable: true, default: 0 },
            'player_Idle_Timeout': { label: 'Player Idle Timeout', hideFromTable: true, default: 0 },
            'difficulty': { label: 'Difficulty', hideFromTable: true, default: 1 },
            'spawn_Monsters': { label: 'Spawn Monsters', hideFromTable: true, default: true },
            'op_Permission_Level': { label: 'Op Permission Level', hideFromTable: true, default: 4 },
            'announce_Player_Achievements': { label: 'Announce Player Achievements', hideFromTable: true, default: true },
            'pvp': { label: 'Pvp', hideFromTable: true, default: true },
            'snooper_Enabled': { label: 'Snooper Enabled', hideFromTable: true, default: true },
            'level_Type': { label: 'Level Type', hideFromTable: true, default: "DEFAULT" },
            'hardcore': { label: 'Hardcore', hideFromTable: true, default: false },
            'enable_Command_Block': { label: 'Enable Command Block', hideFromTable: true, default: false },
            'max_Players': { label: 'Max Players', hideFromTable: true, default: 20 },
            'network_Compression_Threshold': { label: 'Network Compression Threshold', hideFromTable: true, default: 256 },
            'resource_Pack_Sha1': { label: 'Resource Pack Sha1', hideFromTable: true, default: "" },
            'max_World_Size': { label: 'Max World Size', hideFromTable: true, default: 29999984 },
            'server_Ip': { label: 'Server Ip', hideFromTable: true, default: "" },
            'spawn_Npcs': { label: 'Spawn Npcs', hideFromTable: true, default: true },
            'allow_Flight': { label: 'Allow Flight', hideFromTable: true, default: false },
            'view_Distance': { label: 'View Distance', hideFromTable: true, default: 10 },
            'resource_Pack': { label: 'Resource Pack', hideFromTable: true, default: "" },
            'spawn_Animals': { label: 'Spawn Animals', hideFromTable: true, default: true },
            'white_List': { label: 'White List', hideFromTable: true, default: false },
            'generate_Structures': { label: 'Generate Structures', hideFromTable: true, default: true },
            'online_Mode': { label: 'Online Mode', hideFromTable: true, default: true },
            'max_Build_Height': { label: 'Max Build Height', hideFromTable: true, default: 256 },
            'level_Seed': { label: 'Level Seed', hideFromTable: true, default: "" },
            'prevent_Proxy_Connections': { label: 'Prevent Proxy Connections', hideFromTable: true, default: false },
            'motd': { label: 'Motd', hideFromTable: true, default: "" }
        }
    }
] as TabPageSettings[];

const Servers: React.FunctionComponent = () => {
    const { state: { servers }, dispatch } = getState();

    const onSave = useCallback((server: any, isNew: boolean) => {
        return isNew ? create(dispatch, server) : update(dispatch, server);
    }, [dispatch]);

    return (
        <>
            <Edit data={servers} columnMapping={column} onSave={onSave} supportEdit={true}/>
        </>
    );
};

export default Servers;