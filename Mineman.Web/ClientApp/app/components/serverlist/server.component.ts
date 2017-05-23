﻿import { Component, OnInit, Input } from '@angular/core';

import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'server',
    templateUrl: './server.component.html',
    styleUrls: ['./server.component.css']
})
export class ServerComponent implements OnInit {
    @Input() serverWithInfo: any;

    constructor(public authService: AuthService) { }

    ngOnInit() {

    }
}
