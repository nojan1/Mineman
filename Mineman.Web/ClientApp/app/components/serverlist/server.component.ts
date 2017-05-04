import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'server',
    templateUrl: './server.component.html'
})
export class ServerComponent implements OnInit {
    @Input() server: server.serverAddModel;

    ngOnInit() {

    }
}
