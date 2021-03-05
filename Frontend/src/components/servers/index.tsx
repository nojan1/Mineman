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
        hideOnAdd: true,
        columns:
        {
            'max_tick_time': { label: 'Max Tick Time', hideFromTable: true, default: 60000 },
            'generator_settings': { label: 'Generator Settings', hideFromTable: true },
            'allow_nether': { label: 'Allow Nether', hideFromTable: true, default: true },
            'force_gamemode': { label: 'Force Gamemode', hideFromTable: true, default: false },
            'gamemode': { label: 'Gamemode', hideFromTable: true, default: 0 },
            'player_idle_timeout': { label: 'Player Idle Timeout', hideFromTable: true, default: 0 },
            'difficulty': { label: 'Difficulty', hideFromTable: true, default: 1 },
            'spawn_monsters': { label: 'Spawn Monsters', hideFromTable: true, default: true },
            'op_permission_level': { label: 'Op Permission Level', hideFromTable: true, default: 4 },
            'announce_player_achievements': { label: 'Announce Player Achievements', hideFromTable: true, default: true },
            'pvp': { label: 'Pvp', hideFromTable: true, default: true },
            'snooper_enabled': { label: 'Snooper Enabled', hideFromTable: true, default: true },
            'level_type': { label: 'Level Type', hideFromTable: true, default: "DEFAULT" },
            'hardcore': { label: 'Hardcore', hideFromTable: true, default: false },
            'enable_command_block': { label: 'Enable Command Block', hideFromTable: true, default: false },
            'max_players': { label: 'Max Players', hideFromTable: true, default: 20 },
            'network_compression_threshold': { label: 'Network Compression Threshold', hideFromTable: true, default: 256 },
            'resource_pack_sha1': { label: 'Resource Pack Sha1', hideFromTable: true, default: "" },
            'max_world_size': { label: 'Max World Size', hideFromTable: true },
            'server_ip': { label: 'Server Ip', hideFromTable: true, default: "" },
            'spawn_npcs': { label: 'Spawn Npcs', hideFromTable: true, default: true },
            'allow_flight': { label: 'Allow Flight', hideFromTable: true, default: false },
            'view_distance': { label: 'View Distance', hideFromTable: true, default: 10 },
            'resource_Pack': { label: 'Resource Pack', hideFromTable: true, default: "" },
            'spawn_animals': { label: 'Spawn Animals', hideFromTable: true, default: true },
            'white_List': { label: 'White List', hideFromTable: true, default: false },
            'generate_structures': { label: 'Generate Structures', hideFromTable: true, default: true },
            'online_mode': { label: 'Online Mode', hideFromTable: true, default: true },
            'max_build_height': { label: 'Max Build Height', hideFromTable: true, default: 256 },
            'level_seed': { label: 'Level Seed', hideFromTable: true, default: "" },
            'prevent_proxy_connections': { label: 'Prevent Proxy Connections', hideFromTable: true, default: false },
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
        max_tick_time: x.max_tick_time,
        generator_settings: x.generator_settings,
        allow_nether: x.allow_nether,
        force_gamemode: x.force_gamemode,
        gamemode: x.gamemode,
        player_idle_timeout: x.player_idle_timeout,
        difficulty: x.difficulty,
        spawn_monsters: x.spawn_monsters,
        op_permission_level: x.op_permission_level,
        announce_player_achievements: x.announce_player_achievements,
        pvp: x.pvp,
        snooper_enabled: x.snooper_enabled,
        level_type: x.level_type,
        hardcore: x.hardcore,
        enable_command_block: x.enable_command_block,
        max_players: x.max_players,
        network_compression_threshold: x.network_compression_threshold,
        resource_pack_sha1: x.resource_pack_sha1,
        max_world_size: x.max_world_size,
        server_ip: x.server_ip,
        spawn_npcs: x.spawn_npcs,
        allow_flight: x.allow_flight,
        view_distance: x.view_distance,
        resource_pack: x.resource_pack,
        spawn_animals: x.spawn_animals,
        white_list: x.white_list,
        generate_structures: x.generate_structures,
        online_mode: x.online_mode,
        max_build_height: x.max_build_height,
        level_seed: x.level_seed,
        prevent_proxy_connections: x.prevent_proxy_connections,
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
                beforeEditTransform={(row) => ({...row, ...row.properties})}
            />
        </>
    );
};

export default Servers;