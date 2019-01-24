import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { Observable, forkJoin, of } from 'rxjs';

import { ErrorService } from '../../services/error.service';
import { WorldService } from '../../services/world.service';
import { LayoutService } from '../../services/layout.service';
import { PlayerService, PlayerProfile } from '../../services/player.service';
import { catchError, map } from 'rxjs/operators';

export enum EntityType {
    Chest = 1,
    Sign = 2,
    Player = 3
}

export interface Entity {
    x: number,
    y: number,
    z: number,
    type: EntityType,
    data: any
}

@Component({
    selector: 'mapwindow',
    templateUrl: './mapwindow.component.html',
    styleUrls: ['./mapwindow.component.css']
})
export class MapWindowComponent implements OnInit {
    public serverId: number;

    public entities: Entity[];
    public selectedEntity: Entity;
    public worldInfo;

    constructor(private worldService: WorldService,
        private errorService: ErrorService,
        private layoutService: LayoutService,
        private route: ActivatedRoute,
        private playerService: PlayerService) {

    }

    public ngOnInit() {
        this.layoutService.fullscreen = true;
        this.layoutService.wideMode = true;

        this.route.params.subscribe(params => {
            this.serverId = +params["id"];

            this.worldService.GetInfo(this.serverId)
                .pipe(catchError(this.errorService.catchObservable))
                .subscribe(info => {
                    this.worldInfo = info;

                    this.entities = [];
                    info.Chests.forEach(x => {
                        this.entities.push({
                            x: Math.round(x.X),
                            y: Math.round(x.Y),
                            z: Math.round(x.Z),
                            type: EntityType.Chest,
                            data: x.Items
                        });
                    });

                    info.Signs.forEach(x => {
                        this.entities.push({
                            x: Math.round(x.X),
                            y: Math.round(x.Y),
                            z: Math.round(x.Z),
                            type: EntityType.Sign,
                            data: x.Text
                        });
                    });

                    forkJoin(info.Players.map(x =>
                        this.playerService.getProfile(x.UUID)
                            .pipe(catchError(err => of(null)))
                            .pipe(map((profile: PlayerProfile) => {
                                if (!profile)
                                    return null;

                                return {
                                    x: Math.round(x.X),
                                    y: Math.round(x.Y),
                                    z: Math.round(x.Z),
                                    type: EntityType.Player,
                                    data: {
                                        health: x.Health,
                                        name: profile.name,
                                        id: profile.id,
                                        skinUrl: profile.skinUrl
                                    }
                                };
                            }))
                        )
                    )
                    .subscribe((playerEntities: Entity[]) => {
                        this.entities = this.entities.concat(playerEntities.filter(x => x != null));
                    });
                },
                () => { },
                () => {

                });
        });
    }

    public selectEntity(event: Event, type: EntityType, object: any) {
        let newSelection: Entity = {
            type: type,
            data: null,
            x: Math.round(object.X),
            y: Math.round(object.Y),
            z: Math.round(object.Z)
        };

        if (type == EntityType.Chest) {
            newSelection.data = object.Items;
        } else {
            newSelection.data = object.Text;
        }

        this.selectedEntity = newSelection;
    }

    public onEntitySelected(entity: Entity) {
        this.selectedEntity = entity;
    }
}
