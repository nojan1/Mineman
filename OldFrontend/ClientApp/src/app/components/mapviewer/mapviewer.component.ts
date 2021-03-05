import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';

import { Entity, EntityType } from './mapwindow.component';
import { WorldService } from '../../services/world.service';

interface RenderEntity {
    x: number,
    y: number,
    offsets: number[],
    coordinatesIsRaw: boolean,
    image: HTMLImageElement,
    original: Entity,
    size: number
}

@Component({
    selector: 'mapviewer',
    templateUrl: './mapviewer.component.html',
    styleUrls: ['./mapviewer.component.css']
})
export class MapViewerComponent implements OnInit {
    private canvas: HTMLCanvasElement;
    private gkhead = new Image();
    private ctx;

    private lastDetection;

    private lastX: number;
    private lastY: number;
    private scalefactor: number;
    private _imageSrc: string;
    private totalZoomClicks = 0;

    private mapInfo;

    private renderEntities: RenderEntity[];

    @Output() entitySelected: EventEmitter<Entity> = new EventEmitter();
    @Input() serverId: number;

    @Input()
    set imageSrc(imageSrc: string) {
        if (!imageSrc)
            return;

        this._imageSrc = imageSrc;
        this.init();
    }

    @Input()
    set entities(entities: Entity[]) {
        if (!entities)
            return;

        this.renderEntities = entities.map(entity => {
            let entityImage = new Image();
            let offsets = null;

            if (entity.type == EntityType.Sign) {
                entityImage.src = "/images/sign-icon.png";
            } else if (entity.type == EntityType.Chest) {
                entityImage.src = "/images/chest-icon.png";
            } else if (entity.type == EntityType.Player) {
                entityImage.src = entity.data.skinUrl;
                offsets = [8, 8, 8, 8];
            }

            return {
                x: entity.x,
                y: entity.z,  //Yes we did just change coordinate plane
                coordinatesIsRaw: true,
                image: entityImage,
                original: entity,
                size: 5,
                offsets: offsets
            };
        });

        this.redraw();
    }

    constructor(private worldService: WorldService) { }

    public ngOnInit() {
        this.worldService.GetMapInfo(this.serverId)
            .subscribe(info => this.mapInfo = info);
    }

    private init() {
        if (!this.canvas)
            this.canvas = document.getElementsByTagName("canvas")[0];

        this.clearEventHandlers();
        this.setCanvasSize();
        this.gkhead.src = this._imageSrc;

        this.ctx = this.canvas.getContext('2d') as any;
        this.trackTransforms();

        this.ctx.translate(-(this.gkhead.width / 2), -(this.gkhead.height / 2));
        this.redraw();

        this.lastX = this.canvas.width / 2;
        this.lastY = this.canvas.height / 2;
        this.scalefactor = 1.1;

        var dragStart, dragged;

        this.canvas.addEventListener('mousedown', (evt) => {
            document.body.style.setProperty("mozUserSelect", "none");
            document.body.style.setProperty("webkitUserSelect", "none");
            document.body.style.setProperty("userSelect", "none");

            this.lastX = evt.offsetX || (evt.pageX - this.canvas.offsetLeft);
            this.lastY = evt.offsetY || (evt.pageY - this.canvas.offsetTop);
            dragStart = this.ctx.transformedPoint(this.lastX, this.lastY);
            dragged = false;
        }, false);

        this.canvas.addEventListener('mousemove', (evt) => {
            this.lastX = evt.offsetX || (evt.pageX - this.canvas.offsetLeft);
            this.lastY = evt.offsetY || (evt.pageY - this.canvas.offsetTop);
            var pt = this.ctx.transformedPoint(this.lastX, this.lastY);

            var searchX = pt.x - 15;
            var searchY = pt.y - 10;
            this.getRenderEntitiesAtPosition(searchX, searchY)
                .forEach(e => {
                    this.lastDetection = {
                        x: searchX,
                        y: searchY
                    };

                    console.log("Selected entity at x: " + e.x + " y: " + e.y);
                    this.entitySelected.emit(e.original)
                });

            dragged = true;
            if (dragStart) {
                this.ctx.translate(pt.x - dragStart.x, pt.y - dragStart.y);
                this.redraw();
            }
        }, false);

        this.canvas.addEventListener('mouseup', (evt) => {
            dragStart = null;
            if (!dragged) this.zoom(evt.shiftKey ? -1 : 1);
        }, false);

        this.canvas.addEventListener('DOMMouseScroll', (e) => this.handleScroll(e), false);
        this.canvas.addEventListener('mousewheel', (e) => this.handleScroll(e), false);
    }

    private zoom(clicks) {
        this.totalZoomClicks += clicks;

        var pt = this.ctx.transformedPoint(this.lastX, this.lastY);
        this.ctx.translate(pt.x, pt.y);
        var factor = Math.pow(this.scalefactor, clicks);
        this.ctx.scale(factor, factor);
        this.ctx.translate(-pt.x, -pt.y);
        this.redraw();
    }

    private handleScroll(evt) {
        var delta = evt.wheelDelta ? evt.wheelDelta / 40 : evt.detail ? -evt.detail : 0;
        if (delta) this.zoom(delta);
        return evt.preventDefault() && false;
    }

    private redraw() {
        if (!this.canvas || !this.gkhead)
            throw Error("Resources not initalized");

        var p1 = this.ctx.transformedPoint(0, 0);
        var p2 = this.ctx.transformedPoint(this.canvas.width, this.canvas.height);

        this.ctx.clearRect(p1.x, p1.y, p2.x - p1.x, p2.y - p1.y);

        this.ctx.save();
        this.ctx.setTransform(1, 0, 0, 1, 0, 0);
        this.ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
        this.ctx.restore();

        this.ctx.drawImage(this.gkhead, 0, 0);

        if (this.renderEntities && this.mapInfo) {
            this.renderEntities.forEach(entity => {
                let maxZoom = 21;
                let minZoom = 6;

                let size = 5;

                if (this.totalZoomClicks > maxZoom) {
                    size /= Math.pow(this.scalefactor, this.totalZoomClicks - maxZoom);
                }
                else if (this.totalZoomClicks < minZoom) {
                    size *= Math.pow(this.scalefactor, minZoom - this.totalZoomClicks);
                }

                if (entity.coordinatesIsRaw) {
                    entity.x += this.mapInfo.OffsetX;
                    entity.y += this.mapInfo.OffsetZ; //Yes we did just change coordinate plane

                    entity.coordinatesIsRaw = false;
                }

                entity.size = size;

                if (entity.offsets) {
                    this.ctx.drawImage(entity.image, entity.offsets[0], entity.offsets[1], entity.offsets[2], entity.offsets[3], entity.x - (size / 2), entity.y - (size / 2), size, size);
                } else {
                    this.ctx.drawImage(entity.image, entity.x - (size / 2), entity.y - (size / 2), size, size);
                }
            });
        }

        if (this.lastDetection) {
            this.ctx.fillStyle = "#FF0000";
            this.ctx.fillRect(this.lastDetection.x - 2, this.lastDetection.y - 2, 4, 4);
        }
    }

    private trackTransforms() {
        var svg = document.createElementNS("http://www.w3.org/2000/svg", 'svg');
        var xform = svg.createSVGMatrix();
        this.ctx.getTransform = function () { return xform; };

        var savedTransforms = [];
        var save = this.ctx.save;
        this.ctx.save = () => {
            savedTransforms.push(xform.translate(0, 0));
            return save.call(this.ctx);
        };

        var restore = this.ctx.restore;
        this.ctx.restore = () => {
            xform = savedTransforms.pop();
            return restore.call(this.ctx);
        };

        var scale = this.ctx.scale;
        this.ctx.scale = (sx, sy) => {
            xform = (xform as any).scaleNonUniform(sx, sy);
            return scale.call(this.ctx, sx, sy);
        };

        var rotate = this.ctx.rotate;
        this.ctx.rotate = (radians) => {
            xform = xform.rotate(radians * 180 / Math.PI);
            return rotate.call(this.ctx, radians);
        };

        var translate = this.ctx.translate;
        this.ctx.translate = (dx, dy) => {
            xform = xform.translate(dx, dy);
            return translate.call(this.ctx, dx, dy);
        };

        var transform = this.ctx.transform;
        this.ctx.transform = (a, b, c, d, e, f) => {
            var m2 = svg.createSVGMatrix();
            m2.a = a; m2.b = b; m2.c = c; m2.d = d; m2.e = e; m2.f = f;
            xform = xform.multiply(m2);
            return transform.call(this.ctx, a, b, c, d, e, f);
        };

        var setTransform = this.ctx.setTransform;
        this.ctx.setTransform = (a, b, c, d, e, f) => {
            xform.a = a;
            xform.b = b;
            xform.c = c;
            xform.d = d;
            xform.e = e;
            xform.f = f;
            return setTransform.call(this.ctx, a, b, c, d, e, f);
        };

        var pt = svg.createSVGPoint();
        this.ctx.transformedPoint = (x, y) => {
            pt.x = x; pt.y = y;
            return pt.matrixTransform(xform.inverse());
        }
    }

    private clearEventHandlers() {
        if (!this.canvas)
            return;

        this.canvas.removeEventListener("mousedown", null);
        this.canvas.removeEventListener("mousemove", null);
        this.canvas.removeEventListener("mouseup", null);
        this.canvas.removeEventListener("DOMMouseScroll", null);
        this.canvas.removeEventListener("mousewheel", null);
        window.removeEventListener("resize", null);
    }

    private setCanvasSize() {
        this.canvas.width = this.canvas.parentElement.clientWidth;
        this.canvas.height = this.canvas.parentElement.clientHeight - 30;
    }

    private getRenderEntitiesAtPosition(x: number, y: number) {
        if (!this.renderEntities)
            return [];

        return this.renderEntities.filter(e =>
                x >= e.x &&
                x <= e.x + e.size &&
                y >= e.y &&
                y <= e.y + e.size &&
                e.coordinatesIsRaw == false);
    }
}
