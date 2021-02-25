import React, { useCallback } from 'react';
import { createServer, deleteServer, updateServer } from '../../actions/servers';
import { ServerModel } from '../../models/server';
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
            'mainPort': { label: 'Port', type: ColumnType.number, required: true, default: 25565 },
            'memoryAllocationMB': {
                label: 'Memory allocation in MB',
                hideFromTable: true,
                type: ColumnType.number,
                required: true,
                default: 1024
            },
            'image': {
                label: 'Server image',
                valueFormater: image => image?.name ?? '',
                component: ImageSelector,
                required: true
            },
            'world': {
                label: 'World',
                valueFormater: world => world?.displayName ?? '',
                component: WorldSelector,
                required: true
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

const buildServerAddModel = (x: any) => ({
    description: x.description,
    worldId: x.world?.id,
    imageId: x.image?.id,
    serverPort: x.mainPort,
    memoryAllocationMB: x.memoryAllocationMB,
    modIds: x.modIds ?? []
});

const buildServerUpdateModel = (x: any) => ({
    description: x.description,
    worldId: x.world?.id,
    imageId: x.image?.id,
    serverPort: x.mainPort,
    memoryAllocationMB: x.memoryAllocationMB,
    modIds: x.modIds ?? [],
    properties: {
        max_Tick_Time: x.max_Tick_Time,
        generator_Settings: x.generator_Settings,
        allow_Nether: x.allow_Nether,
        force_Gamemode: x.force_Gamemode,
        gamemode: x.gamemode,
        player_Idle_Timeout: x.player_Idle_Timeout,
        difficulty: x.difficulty,
        spawn_Monsters: x.spawn_Monsters,
        op_Permission_Level: x.op_Permission_Level,
        announce_Player_Achievements: x.announce_Player_Achievements,
        pvp: x.pvp,
        snooper_Enabled: x.snooper_Enabled,
        level_Type: x.level_Type,
        hardcore: x.hardcore,
        enable_Command_Block: x.enable_Command_Block,
        max_Players: x.max_Players,
        network_Compression_Threshold: x.network_Compression_Threshold,
        resource_Pack_Sha1: x.resource_Pack_Sha1,
        max_World_Size: x.max_World_Size,
        server_Ip: x.server_Ip,
        spawn_Npcs: x.spawn_Npcs,
        allow_Flight: x.allow_Flight,
        view_Distance: x.view_Distance,
        resource_Pack: x.resource_Pack,
        spawn_Animals: x.spawn_Animals,
        white_List: x.white_List,
        generate_Structures: x.generate_Structures,
        online_Mode: x.online_Mode,
        max_Build_Height: x.max_Build_Height,
        level_Seed: x.level_Seed,
        prevent_Proxy_Connections: x.prevent_Proxy_Connections,
        motd: x.motd
    }
});

const Servers: React.FunctionComponent = () => {
    const { state: { servers, images, worlds }, dispatch } = getState();

    const mergedServers = servers.map(s => ({
        ...s,
        image: images?.find(i => i.id == s.imageId),
        world: worlds?.find(w => w.id == s.worldId)
    }));

    const onSave = useCallback((server: any, isNew: boolean) =>
        isNew
            ? createServer(dispatch, buildServerAddModel(server))
            : updateServer(dispatch, server.id, buildServerUpdateModel(server))
        , [dispatch]);

    const onDelete = useCallback((server: ServerModel) => deleteServer(dispatch, server.id), [dispatch]);

    return (
        <>
            <Edit
                data={mergedServers}
                columnMapping={column}
                onSave={onSave}
                supportEdit={true}
                onDelete={onDelete}
                canDelete={() => true}
            />
        </>
    );
};

export default Servers;