using System;
using System.Collections.Generic;
using System.Linq;
using Mineman.WorldParsing.Blocks;
using Mineman.WorldParsing.Entities;
using NBT;

namespace Mineman.WorldParsing
{
    public class NewChunk : Chunk
    {
        private readonly Dictionary<string, (int, byte)> _blockTranslationMap = new Dictionary<string, (int, byte)>
        {
            {"minecraft:air", (0, 0)},
            {"minecraft:cave_air", (0, 0)},
            {"minecraft:void_air", (0, 0)},
            {"minecraft:stone", (1, 0)},
            {"minecraft:granite", (1, 1)},
            {"minecraft:polished_granite", (1, 2)},
            {"minecraft:diorite", (1, 3)},
            {"minecraft:polished_diorite", (1, 4)},
            {"minecraft:andesite", (1, 5)},
            {"minecraft:polished_andesite", (1, 6)},
            {"minecraft:grass_block", (2, 0)},
            {"minecraft:dirt", (3, 0)},
            {"minecraft:coarse_dirt", (3, 1)},
            {"minecraft:podzol", (3, 2)},
            {"minecraft:cobblestone", (4, 0)},
            {"minecraft:infested_cobblestone", (4, 0)},
            {"minecraft:oak_planks", (5, 0)},
            {"minecraft:spruce_planks", (5, 1)},
            {"minecraft:birch_planks", (5, 2)},
            {"minecraft:jungle_planks", (5, 3)},
            {"minecraft:acacia_planks", (5, 4)},
            {"minecraft:dark_oak_planks", (5, 5)},
            {"minecraft:oak_sapling", (6, 0)},
            {"minecraft:spruce_sapling", (6, 1)},
            {"minecraft:birch_sapling", (6, 2)},
            {"minecraft:jungle_sapling", (6, 3)},
            {"minecraft:acacia_sapling", (6, 4)},
            {"minecraft:dark_oak_sapling", (6, 5)},
            {"minecraft:bedrock", (7, 0)},
            {"minecraft:water", (8, 0)},
            {"minecraft:lava", (10, 0)},
            {"minecraft:sand", (12, 0)},
            {"minecraft:red_sand", (12, 1)},
            {"minecraft:gravel", (13, 0)},
            {"minecraft:gold_ore", (14, 0)},
            {"minecraft:iron_ore", (15, 0)},
            {"minecraft:coal_ore", (16, 0)},
            {"minecraft:oak_log", (17, 0)},
            {"minecraft:spruce_log", (17, 1)},
            {"minecraft:birch_log", (17, 2)},
            {"minecraft:jungle_log", (17, 3)},
            {"minecraft:oak_leaves", (18, 0)},
            {"minecraft:spruce_leaves", (18, 1)},
            {"minecraft:birch_leaves", (18, 2)},
            {"minecraft:jungle_leaves", (18, 3)},
            {"minecraft:acacia_leaves", (18, 4)},
            {"minecraft:dark_oak_leaves", (18, 5)},
            {"minecraft:sponge", (19, 0)},
            {"minecraft:wet_sponge", (19, 1)},
            {"minecraft:glass", (20, 0)},
            {"minecraft:lapis_ore", (21, 0)},
            {"minecraft:lapis_block", (22, 0)},
            {"minecraft:dispenser", (23, 0)},
            {"minecraft:sandstone", (24, 0)},
            {"minecraft:cut_sandstone", (24, 2)},
            {"minecraft:chiseled_sandstone", (24, 3)},
            {"minecraft:note_block", (25, 0)},
            {"minecraft:white_bed", (26, 0)},
            {"minecraft:orange_bed", (26, 0)},
            {"minecraft:magenta_bed", (26, 0)},
            {"minecraft:light_blue_bed", (26, 0)},
            {"minecraft:yellow_bed", (26, 0)},
            {"minecraft:lime_bed", (26, 0)},
            {"minecraft:pink_bed", (26, 0)},
            {"minecraft:gray_bed", (26, 0)},
            {"minecraft:light_gray_bed", (26, 0)},
            {"minecraft:cyan_bed", (26, 0)},
            {"minecraft:purple_bed", (26, 0)},
            {"minecraft:blue_bed", (26, 0)},
            {"minecraft:brown_bed", (26, 0)},
            {"minecraft:green_bed", (26, 0)},
            {"minecraft:red_bed", (26, 0)},
            {"minecraft:black_bed", (26, 0)},
            {"minecraft:powered_rail", (27, 0)},
            {"minecraft:detector_rail", (28, 0)},
            {"minecraft:sticky_piston", (29, 0)},
            {"minecraft:cobweb", (30, 0)},
            {"minecraft:dead_bush", (31, 0)},
            {"minecraft:grass", (31, 1)},
            {"minecraft:fern", (31, 2)},
            {"minecraft:piston", (33, 0)},
            {"minecraft:piston_head", (34, 0)},
            {"minecraft:white_wool", (35, 0)},
            {"minecraft:orange_wool", (35, 1)},
            {"minecraft:magenta_wool", (35, 2)},
            {"minecraft:light_blue_wool", (35, 3)},
            {"minecraft:yellow_wool", (35, 4)},
            {"minecraft:lime_wool", (35, 5)},
            {"minecraft:pink_wool", (35, 6)},
            {"minecraft:gray_wool", (35, 7)},
            {"minecraft:light_gray_wool", (35, 8)},
            {"minecraft:cyan_wool", (35, 9)},
            {"minecraft:purple_wool", (35, 10)},
            {"minecraft:blue_wool", (35, 11)},
            {"minecraft:brown_wool", (35, 12)},
            {"minecraft:green_wool", (35, 13)},
            {"minecraft:red_wool", (35, 14)},
            {"minecraft:black_wool", (35, 15)},

            {"minecraft:poppy", (38, 0)},
            {"minecraft:blue_orchid", (38, 1)},
            {"minecraft:allium", (38, 2)},
            {"minecraft:azure_bluet", (38, 3)},
            {"minecraft:red_tulip", (38, 4)},
            {"minecraft:orange_tulip", (38, 5)},
            {"minecraft:white_tulip", (38, 6)},
            {"minecraft:pink_tulip", (38, 7)},
            {"minecraft:oxeye_daisy", (38, 8)},
            {"minecraft:dandelion", (38, 9)},
            {"minecraft:brown_mushroom", (39, 0)},
            {"minecraft:red_mushroom", (40, 0)},
            {"minecraft:gold_block", (41, 0)},
            {"minecraft:iron_block", (42, 0)},
            {"minecraft:stone_slab", (44, 0)},
            {"minecraft:sandstone_slab", (44, 1)},
            {"minecraft:oak_slab", (44, 2)},
            {"minecraft:cobblestone_slab", (44, 3)},
            {"minecraft:brick_slab", (44, 4)},
            {"minecraft:stone_brick_slab", (44, 5)},
            {"minecraft:nether_brick_slab", (44, 6)},
            {"minecraft:quartz_slab", (44, 7)},
            {"minecraft:bricks", (45, 0)},
            {"minecraft:tnt", (46, 0)},
            {"minecraft:bookshelf", (47, 0)},
            {"minecraft:mossy_cobblestone", (48, 0)},
            {"minecraft:obsidian", (49, 0)},
            {"minecraft:wall_torch", (50, 0)},
            {"minecraft:torch", (50, 5)},
            {"minecraft:fire", (51, 0)},
            {"minecraft:spawner", (52, 0)},
            {"minecraft:oak_stairs", (53, 0)},
            {"minecraft:chest", (54, 0)},
            {"minecraft:redstone_wire", (55, 0)},
            {"minecraft:diamond_ore", (56, 0)},
            {"minecraft:diamond_block", (57, 0)},
            {"minecraft:crafting_table", (58, 0)},
            {"minecraft:wheat", (59, 0)},
            {"minecraft:farmland", (60, 0)},
            {"minecraft:furnace", (61, 0)},
            {"minecraft:sign", (63, 0)},
            {"minecraft:oak_door", (64, 0)},
            {"minecraft:ladder", (65, 0)},
            {"minecraft:rail", (66, 0)},
            {"minecraft:stone_stairs", (67, 0)},
            {"minecraft:cobblestone_stairs", (67, 0)},
            {"minecraft:wall_sign", (68, 0)},
            {"minecraft:lever", (69, 0)},
            {"minecraft:stone_pressure_plate", (70, 0)},
            {"minecraft:iron_door", (71, 0)},
            {"minecraft:oak_pressure_plate", (72, 0)},
            {"minecraft:redstone_ore", (73, 0)},
            {"minecraft:redstone_wall_torch", (75, 0)},
            {"minecraft:redstone_torch", (75, 5)},
            {"minecraft:stone_button", (77, 0)},
            {"minecraft:snow", (78, 0)},
            {"minecraft:ice", (79, 0)},
            {"minecraft:snow_block", (80, 0)},
            {"minecraft:cactus", (81, 0)},
            {"minecraft:clay", (82, 0)},
            {"minecraft:sugar_cane", (83, 0)},
            {"minecraft:jukebox", (84, 0)},
            {"minecraft:oak_fence", (85, 0)},
            {"minecraft:pumpkin", (86, 0)},
            {"minecraft:netherrack", (87, 0)},
            {"minecraft:soul_sand", (88, 0)},
            {"minecraft:glowstone", (89, 0)},
            {"minecraft:nether_portal", (90, 0)},
            {"minecraft:jack_o_lantern", (91, 0)},
            {"minecraft:cake", (92, 0)},
            {"minecraft:repeater", (93, 0)},
            {"minecraft:oak_trapdoor", (96, 0)},
            {"minecraft:infested_stone", (97, 0)},
            {"minecraft:stone_bricks", (98, 0)},
            {"minecraft:infested_stone_bricks", (98, 0)},
            {"minecraft:mossy_stone_bricks", (98, 1)},
            {"minecraft:infested_mossy_stone_bricks", (98, 1)},
            {"minecraft:cracked_stone_bricks", (98, 2)},
            {"minecraft:infested_cracked_stone_bricks", (98, 2)},
            {"minecraft:chiseled_stone_bricks", (98, 3)},
            {"minecraft:infested_chiseled_stone_bricks", (98, 3)},
            {"minecraft:brown_mushroom_block", (99, 0)},
            {"minecraft:red_mushroom_block", (100, 0)},
            {"minecraft:mushroom_stem", (100, 10)},
            {"minecraft:iron_bars", (101, 0)},
            {"minecraft:glass_pane", (102, 0)},
            {"minecraft:melon", (103, 0)},
            {"minecraft:attached_pumpkin_stem", (104, 0)},
            {"minecraft:attached_melon_stem", (104, 0)},
            {"minecraft:pumpkin_stem", (105, 0)},
            {"minecraft:melon_stem", (105, 0)},
            {"minecraft:vine", (106, 0)},
            {"minecraft:oak_fence_gate", (107, 0)},
            {"minecraft:brick_stairs", (108, 0)},
            {"minecraft:stone_brick_stairs", (109, 0)},
            {"minecraft:mycelium", (110, 0)},
            {"minecraft:lily_pad", (111, 0)},
            {"minecraft:nether_bricks", (112, 0)},
            {"minecraft:nether_brick_fence", (113, 0)},
            {"minecraft:nether_brick_stairs", (114, 0)},
            {"minecraft:nether_wart", (115, 0)},
            {"minecraft:enchanting_table", (116, 0)},
            {"minecraft:brewing_stand", (117, 0)},
            {"minecraft:cauldron", (118, 0)},
            {"minecraft:end_portal", (119, 0)},
            {"minecraft:end_portal_frame", (120, 0)},
            {"minecraft:end_stone", (121, 0)},
            {"minecraft:dragon_egg", (122, 0)},
            {"minecraft:redstone_lamp", (123, 0)},
            {"minecraft:spruce_slab", (126, 1)},
            {"minecraft:birch_slab", (126, 2)},
            {"minecraft:jungle_slab", (126, 3)},
            {"minecraft:acacia_slab", (126, 4)},
            {"minecraft:dark_oak_slab", (126, 5)},
            {"minecraft:cocoa", (127, 0)},
            {"minecraft:sandstone_stairs", (128, 0)},
            {"minecraft:emerald_ore", (129, 0)},
            {"minecraft:ender_chest", (130, 0)},
            {"minecraft:tripwire", (131, 0)},
            {"minecraft:tripwire_hook", (132, 0)},
            {"minecraft:emerald_block", (133, 0)},
            {"minecraft:spruce_stairs", (134, 0)},
            {"minecraft:birch_stairs", (135, 0)},
            {"minecraft:jungle_stairs", (136, 0)},
            {"minecraft:command_block", (137, 0)},
            {"minecraft:beacon", (138, 0)},
            {"minecraft:cobblestone_wall", (139, 0)},
            {"minecraft:mossy_cobblestone_wall", (139, 1)},
            {"minecraft:flower_pot", (140, 0)},
            {"minecraft:potted_poppy", (140, 0)}, // Pots not rendering
            {"minecraft:potted_blue_orchid", (140, 0)},
            {"minecraft:potted_allium", (140, 0)},
            {"minecraft:potted_azure_bluet", (140, 0)},
            {"minecraft:potted_red_tulip", (140, 0)},
            {"minecraft:potted_orange_tulip", (140, 0)},
            {"minecraft:potted_white_tulip", (140, 0)},
            {"minecraft:potted_pink_tulip", (140, 0)},
            {"minecraft:potted_oxeye_daisy", (140, 0)},
            {"minecraft:potted_oak_sapling", (140, 0)},
            {"minecraft:potted_spruce_sapling", (140, 0)},
            {"minecraft:potted_birch_sapling", (140, 0)},
            {"minecraft:potted_jungle_sapling", (140, 0)},
            {"minecraft:potted_acacia_sapling", (140, 0)},
            {"minecraft:potted_dark_oak_sapling", (140, 0)},
            {"minecraft:potted_red_mushroom", (140, 0)},
            {"minecraft:potted_brown_mushroom", (140, 0)},
            {"minecraft:potted_fern", (140, 0)},
            {"minecraft:potted_dead_bush", (140, 0)},
            {"minecraft:potted_cactus", (140, 0)},
            {"minecraft:potted_bamboo", (140, 0)},
            {"minecraft:carrots", (141, 0)},
            {"minecraft:potatoes", (142, 0)},
            {"minecraft:oak_button", (143, 0)},
            {"minecraft:skeleton_wall_skull", (144, 0)}, // not rendering
            {"minecraft:wither_skeleton_wall_skull", (144, 1)}, // not rendering
            {"minecraft:zombie_wall_head", (144, 2)}, // not rendering
            {"minecraft:player_wall_head", (144, 3)}, // not rendering
            {"minecraft:creeper_wall_head", (144, 4)}, // not rendering
            {"minecraft:dragon_wall_head", (144, 5)}, // not rendering
            {"minecraft:anvil", (145, 0)},
            {"minecraft:chipped_anvil", (145, 4)},
            {"minecraft:damaged_anvil", (145, 8)},
            {"minecraft:trapped_chest", (146, 0)},
            {"minecraft:light_weighted_pressure_plate", (147, 0)},
            {"minecraft:heavy_weighted_pressure_plate", (148, 0)},
            {"minecraft:comparator", (149, 0)},
            {"minecraft:daylight_detector", (151, 0)},
            {"minecraft:redstone_block", (152, 0)},
            {"minecraft:nether_quartz_ore", (153, 0)},
            {"minecraft:hopper", (154, 0)},
            {"minecraft:quartz_block", (155, 0)},
            {"minecraft:smooth_quartz", (155, 0)}, // Only bottom texture is different
            {"minecraft:quartz_pillar", (155, 2)},
            {"minecraft:chiseled_quartz_block", (155, 1)},
            {"minecraft:quartz_stairs", (156, 0)},
            {"minecraft:activator_rail", (157, 0)},
            {"minecraft:dropper", (158, 0)},
            {"minecraft:white_terracotta", (159, 0)},
            {"minecraft:orange_terracotta", (159, 1)},
            {"minecraft:magenta_terracotta", (159, 2)},
            {"minecraft:light_blue_terracotta", (159, 3)},
            {"minecraft:yellow_terracotta", (159, 4)},
            {"minecraft:lime_terracotta", (159, 5)},
            {"minecraft:pink_terracotta", (159, 6)},
            {"minecraft:gray_terracotta", (159, 7)},
            {"minecraft:light_gray_terracotta", (159, 8)},
            {"minecraft:cyan_terracotta", (159, 9)},
            {"minecraft:purple_terracotta", (159, 10)},
            {"minecraft:blue_terracotta", (159, 11)},
            {"minecraft:brown_terracotta", (159, 12)},
            {"minecraft:green_terracotta", (159, 13)},
            {"minecraft:red_terracotta", (159, 14)},
            {"minecraft:black_terracotta", (159, 15)},
            {"minecraft:acacia_log", (162, 0)},
            {"minecraft:dark_oak_log", (162, 1)},
            {"minecraft:acacia_stairs", (163, 0)},
            {"minecraft:dark_oak_stairs", (164, 0)},
            {"minecraft:slime_block", (165, 0)},
            {"minecraft:iron_trapdoor", (167, 0)},
            {"minecraft:prismarine", (168, 0)},
            {"minecraft:dark_prismarine", (168, 2)},
            {"minecraft:prismarine_bricks", (168, 1)},
            {"minecraft:sea_lantern", (169, 0)},
            {"minecraft:hay_block", (170, 0)},
            {"minecraft:white_carpet", (171, 0)},
            {"minecraft:orange_carpet", (171, 1)},
            {"minecraft:magenta_carpet", (171, 2)},
            {"minecraft:light_blue_carpet", (171, 3)},
            {"minecraft:yellow_carpet", (171, 4)},
            {"minecraft:lime_carpet", (171, 5)},
            {"minecraft:pink_carpet", (171, 6)},
            {"minecraft:gray_carpet", (171, 7)},
            {"minecraft:light_gray_carpet", (171, 8)},
            {"minecraft:cyan_carpet", (171, 9)},
            {"minecraft:purple_carpet", (171, 10)},
            {"minecraft:blue_carpet", (171, 11)},
            {"minecraft:brown_carpet", (171, 12)},
            {"minecraft:green_carpet", (171, 13)},
            {"minecraft:red_carpet", (171, 14)},
            {"minecraft:black_carpet", (171, 15)},
            {"minecraft:terracotta", (172, 0)},
            {"minecraft:coal_block", (173, 0)},
            {"minecraft:packed_ice", (174, 0)},
            {"minecraft:sunflower", (175, 0)},
            {"minecraft:lilac", (175, 1)},
            {"minecraft:tall_grass", (175, 2)},
            {"minecraft:large_fern", (175, 3)},
            {"minecraft:rose_bush", (175, 4)},
            {"minecraft:peony", (175, 5)},
            {"minecraft:standing_banner", (176, 0)},
            {"minecraft:wall_banner", (177, 0)},
            {"minecraft:red_sandstone", (179, 0)},
            {"minecraft:cut_red_sandstone", (179, 2)},
            {"minecraft:chiseled_red_sandstone", (179, 3)},
            {"minecraft:red_sandstone_stairs", (180, 0)},
            {"minecraft:red_sandstone_slab", (182, 0)},
            {"minecraft:spruce_fence_gate", (183, 0)},
            {"minecraft:birch_fence_gate", (184, 0)},
            {"minecraft:jungle_fence_gate", (185, 0)},
            {"minecraft:dark_oak_fence_gate", (186, 0)},
            {"minecraft:acacia_fence_gate", (187, 0)},
            {"minecraft:spruce_fence", (188, 0)},
            {"minecraft:birch_fence", (189, 0)},
            {"minecraft:jungle_fence", (190, 0)},
            {"minecraft:dark_oak_fence", (191, 0)},
            {"minecraft:acacia_fence", (192, 0)},
            {"minecraft:spruce_door", (193, 0)},
            {"minecraft:birch_door", (194, 0)},
            {"minecraft:jungle_door", (195, 0)},
            {"minecraft:acacia_door", (196, 0)},
            {"minecraft:dark_oak_door", (197, 0)},
            {"minecraft:end_rod", (198, 0)}, // not rendering
            {"minecraft:chorus_plant", (199, 0)},
            {"minecraft:chorus_flower", (200, 0)},
            {"minecraft:purpur_block", (201, 0)},
            {"minecraft:purpur_pillar", (202, 0)},
            {"minecraft:purpur_stairs", (203, 0)},
            {"minecraft:purpur_slab", (205, 0)},
            {"minecraft:end_stone_bricks", (206, 0)},
            {"minecraft:beetroots", (207, 0)},
            {"minecraft:grass_path", (208, 0)},
            {"minecraft:repeating_command_block", (210, 0)},
            {"minecraft:chain_command_block", (211, 0)},
            {"minecraft:frosted_ice", (212, 0)},
            {"minecraft:magma_block", (213, 0)},
            {"minecraft:nether_wart_block", (214, 0)},
            {"minecraft:red_nether_bricks", (215, 0)},
            {"minecraft:bone_block", (216, 0)},
            {"minecraft:observer", (218, 0)},
            {"minecraft:white_shulker_box", (219, 0)},
            {"minecraft:orange_shulker_box", (220, 0)},
            {"minecraft:magenta_shulker_box", (221, 0)},
            {"minecraft:light_blue_shulker_box", (222, 0)},
            {"minecraft:yellow_shulker_box", (223, 0)},
            {"minecraft:lime_shulker_box", (224, 0)},
            {"minecraft:pink_shulker_box", (225, 0)},
            {"minecraft:gray_shulker_box", (226, 0)},
            {"minecraft:light_gray_shulker_box", (227, 0)},
            {"minecraft:cyan_shulker_box", (228, 0)},
            {"minecraft:shulker_box", (229, 0)}, // wrong color
            {"minecraft:purple_shulker_box", (229, 0)},
            {"minecraft:blue_shulker_box", (230, 0)},
            {"minecraft:brown_shulker_box", (231, 0)},
            {"minecraft:green_shulker_box", (232, 0)},
            {"minecraft:red_shulker_box", (233, 0)},
            {"minecraft:black_shulker_box", (234, 0)},
            {"minecraft:white_glazed_terracotta", (235, 0)},
            {"minecraft:orange_glazed_terracotta", (236, 0)},
            {"minecraft:magenta_glazed_terracotta", (237, 0)},
            {"minecraft:light_blue_glazed_terracotta", (238, 0)},
            {"minecraft:yellow_glazed_terracotta", (239, 0)},
            {"minecraft:lime_glazed_terracotta", (240, 0)},
            {"minecraft:pink_glazed_terracotta", (241, 0)},
            {"minecraft:gray_glazed_terracotta", (242, 0)},
            {"minecraft:light_gray_glazed_terracotta", (243, 0)},
            {"minecraft:cyan_glazed_terracotta", (244, 0)},
            {"minecraft:purple_glazed_terracotta", (245, 0)},
            {"minecraft:blue_glazed_terracotta", (246, 0)},
            {"minecraft:brown_glazed_terracotta", (247, 0)},
            {"minecraft:green_glazed_terracotta", (248, 0)},
            {"minecraft:red_glazed_terracotta", (249, 0)},
            {"minecraft:black_glazed_terracotta", (250, 0)},

            {"minecraft:structure_block", (255, 0)},

            {"minecraft:armor_stand", (416, 0)}, // not rendering

            // The following blocks are underwater and are not yet rendered.
            // To avoid spurious warnings, we"ll treat them as water for now.
            {"minecraft:brain_coral", (8, 0)},
            {"minecraft:brain_coral_fan", (8, 0)},
            {"minecraft:brain_coral_wall_fan", (8, 0)},
            {"minecraft:bubble_column", (8, 0)},
            {"minecraft:bubble_coral", (8, 0)},
            {"minecraft:bubble_coral_fan", (8, 0)},
            {"minecraft:bubble_coral_wall_fan", (8, 0)},
            {"minecraft:fire_coral", (8, 0)},
            {"minecraft:fire_coral_fan", (8, 0)},
            {"minecraft:fire_coral_wall_fan", (8, 0)},
            {"minecraft:horn_coral", (8, 0)},
            {"minecraft:horn_coral_fan", (8, 0)},
            {"minecraft:horn_coral_wall_fan", (8, 0)},
            {"minecraft:kelp", (8, 0)},
            {"minecraft:kelp_plant", (8, 0)},
            {"minecraft:sea_pickle", (8, 0)},
            {"minecraft:seagrass", (8, 0)},
            {"minecraft:tall_seagrass", (8, 0)},
            {"minecraft:tube_coral", (8, 0)},
            {"minecraft:tube_coral_fan", (8, 0)},
            {"minecraft:tube_coral_wall_fan", (8, 0)},

            // New blocks
            {"minecraft:carved_pumpkin", (11300, 0)},
            {"minecraft:spruce_pressure_plate", (11301, 0)},
            {"minecraft:birch_pressure_plate", (11302, 0)},
            {"minecraft:jungle_pressure_plate", (11303, 0)},
            {"minecraft:acacia_pressure_plate", (11304, 0)},
            {"minecraft:dark_oak_pressure_plate", (11305, 0)},
            {"minecraft:stripped_oak_log", (11306, 0)},
            {"minecraft:stripped_spruce_log", (11306, 1)},
            {"minecraft:stripped_birch_log", (11306, 2)},
            {"minecraft:stripped_jungle_log", (11306, 3)},
            {"minecraft:stripped_acacia_log", (11307, 0)},
            {"minecraft:stripped_dark_oak_log", (11307, 1)},
            {"minecraft:oak_wood", (11308, 0)},
            {"minecraft:spruce_wood", (11308, 1)},
            {"minecraft:birch_wood", (11308, 2)},
            {"minecraft:jungle_wood", (11308, 3)},
            {"minecraft:acacia_wood", (11309, 0)},
            {"minecraft:dark_oak_wood", (11309, 1)},
            {"minecraft:stripped_oak_wood", (11310, 0)},
            {"minecraft:stripped_spruce_wood", (11310, 1)},
            {"minecraft:stripped_birch_wood", (11310, 2)},
            {"minecraft:stripped_jungle_wood", (11310, 3)},
            {"minecraft:stripped_acacia_wood", (11311, 0)},
            {"minecraft:stripped_dark_oak_wood", (11311, 1)},
            {"minecraft:blue_ice", (11312, 0)},
            {"minecraft:smooth_stone", (11313, 0)},
            {"minecraft:smooth_sandstone", (11314, 0)},
            {"minecraft:smooth_red_sandstone", (11315, 0)},
            {"minecraft:brain_coral_block", (11316, 0)},
            {"minecraft:bubble_coral_block", (11317, 0)},
            {"minecraft:fire_coral_block", (11318, 0)},
            {"minecraft:horn_coral_block", (11319, 0)},
            {"minecraft:tube_coral_block", (11320, 0)},
            {"minecraft:dead_brain_coral_block", (11321, 0)},
            {"minecraft:dead_bubble_coral_block", (11322, 0)},
            {"minecraft:dead_fire_coral_block", (11323, 0)},
            {"minecraft:dead_horn_coral_block", (11324, 0)},
            {"minecraft:dead_tube_coral_block", (11325, 0)},
            {"minecraft:spruce_button", (11326, 0)},
            {"minecraft:birch_button", (11327, 0)},
            {"minecraft:jungle_button", (11328, 0)},
            {"minecraft:acacia_button", (11329, 0)},
            {"minecraft:dark_oak_button", (11330, 0)},
            {"minecraft:dried_kelp_block", (11331, 0)},
            {"minecraft:spruce_trapdoor", (11332, 0)},
            {"minecraft:birch_trapdoor", (11333, 0)},
            {"minecraft:jungle_trapdoor", (11334, 0)},
            {"minecraft:acacia_trapdoor", (11335, 0)},
            {"minecraft:dark_oak_trapdoor", (11336, 0)},
            {"minecraft:petrified_oak_slab", (126, 0)},
            {"minecraft:prismarine_stairs", (11337, 0)},
            {"minecraft:dark_prismarine_stairs", (11338, 0)},
            {"minecraft:prismarine_brick_stairs", (11339, 0)},
            {"minecraft:prismarine_slab", (11340, 0)},
            {"minecraft:dark_prismarine_slab", (11341, 0)},
            {"minecraft:prismarine_brick_slab", (11342, 0)},
            {"minecraft:andesite_slab", (11343, 0)},
            {"minecraft:diorite_slab", (11344, 0)},
            {"minecraft:granite_slab", (11345, 0)},
            {"minecraft:polished_andesite_slab", (11346, 0)},
            {"minecraft:polished_diorite_slab", (11347, 0)},
            {"minecraft:polished_granite_slab", (11348, 0)},
            {"minecraft:red_nether_brick_slab", (11349, 0)},
            {"minecraft:smooth_sandstone_slab", (11350, 0)},
            {"minecraft:cut_sandstone_slab", (11351, 0)},
            {"minecraft:smooth_red_sandstone_slab", (11352, 0)},
            {"minecraft:cut_red_sandstone_slab", (11353, 0)},
            {"minecraft:end_stone_brick_slab", (11354, 0)},
            {"minecraft:mossy_cobblestone_slab", (11355, 0)},
            {"minecraft:mossy_stone_brick_slab", (11356, 0)},
            {"minecraft:smooth_quartz_slab", (11357, 0)},
            {"minecraft:smooth_stone_slab", (11358, 0)},
            {"minecraft:fletching_table", (11359, 0)},
            {"minecraft:cartography_table", (11360, 0)},
            {"minecraft:smithing_table", (11361, 0)}
        };

        private readonly TagLongArray _blockStates;
        private readonly TagList _palette;
        private readonly byte[] _biomeIds;

        public NewChunk(int y, int z, int x, TagLongArray blockStates, TagList palette, byte[] biomeIds)
        {
            XWorld = x;
            ZWorld = z;
            YOrder = y;

            _blockStates = blockStates;
            _palette = palette;
            _biomeIds = biomeIds;
        }

        protected override IEnumerable<Block> GetBlocks()
        {
            var unpackedBlockstates = UnpackBlockstates().ToArray();

            for (var i = 0; i < unpackedBlockstates.Length; i++)
            {
                var paletteIndex = unpackedBlockstates[i];
                if (paletteIndex < 0 || paletteIndex > _palette.Value.Count - 1)
                    continue;

                var paletteItem = (TagCompound) _palette.Value.ElementAt(paletteIndex);

                var x = XWorld + (i % 16);
                var y = (YOrder * 16) + (i / 256);
                var z = ZWorld + ((i / 16) % 16);

                var name = paletteItem.GetStringValue("Name");
                var properties = paletteItem.GetCompound("Properties") ?? new TagCompound();

                (int id, byte data) = _blockTranslationMap.ContainsKey(name)
                    ? _blockTranslationMap[name]
                    : _blockTranslationMap["minecraft:air"];

                byte biomeId = _biomeIds[i % 256];

                yield return BlockFactory.CreateFromId(id, y, z, x, biomeId, data, 0, 0,
                    new BlockEntity(properties, name, x, y, z));
            }
        }

        private IEnumerable<int> UnpackBlockstates()
        {
            var totalBits = _blockStates.Value.Length * 64;
            var bitsPerValue = totalBits / 4096;

            if (bitsPerValue < 4 || bitsPerValue > 12)
                yield break;

            for (int currentIndex = 0; currentIndex < totalBits; currentIndex += bitsPerValue)
            {
                int endIndex = currentIndex + bitsPerValue - 1;

                int lowerIndex = currentIndex / 64;
                int upperIndex = endIndex / 64;

                var normalizedIndex = currentIndex - (lowerIndex * 64);
                var normalizedEndIndex = endIndex - (upperIndex * 64);

                /*
                 *      0         1          2
                 * [........][.........][.........]
                 *  
                 */

                if (lowerIndex == upperIndex)
                {
                    //Same long
                    var rightHandAnd = ((ulong) Math.Pow(2, bitsPerValue + 1)) - 1;

                    var value = (int) ((((ulong) _blockStates.Value[lowerIndex]) >> normalizedIndex) & rightHandAnd);
                    yield return value;
                }
                else
                {
                    //Spread over 2 longs
                    var lengthInLowerHalf = 64 - normalizedIndex;
                    var lowerHalfRightHandAnd = ((ulong) Math.Pow(2, lengthInLowerHalf + 1)) - 1;
                    var lowerHalfNumShifts = 64 - lengthInLowerHalf;
                    var lowerHalf = (((ulong) _blockStates.Value[lowerIndex]) >> lowerHalfNumShifts) &
                                    lowerHalfRightHandAnd;

                    var upperHalf = 0ul;
                    var lengthInUpperHalf = normalizedEndIndex;

                    if (lengthInUpperHalf > 0)
                    {
                        var upperHalfRightHandAnd = ((ulong) Math.Pow(2, lengthInUpperHalf + 1)) - 1;
                        upperHalf = ((ulong) _blockStates.Value[upperIndex]) & upperHalfRightHandAnd;
                    }

                    var value = (int) (lowerHalf + upperHalf);
                    yield return value;
                }
            }
        }
    }
}