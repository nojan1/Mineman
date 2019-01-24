import { Injectable } from '@angular/core';

@Injectable()
export class LayoutService {
    constructor() { }

    public fullscreen: boolean = false;
    public wideMode: boolean = false;

}