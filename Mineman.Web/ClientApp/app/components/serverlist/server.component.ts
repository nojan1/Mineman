import { Component, OnInit, Input } from '@angular/core';

@Component({
    selector: 'server',
    templateUrl: './server.component.html',
    styleUrls: ['./server.component.css']
})
export class ServerComponent implements OnInit {
    @Input() serverWithInfo: any;

    ngOnInit() {

    }
}
