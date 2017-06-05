import { Component, Input } from '@angular/core';


@Component({
    selector: 'loading-inline',
    templateUrl: './loading-inline.component.html',
    styleUrls: ['./loading-inline.component.css']
})
export class LoadingInlineComponent {
    @Input() isShown: boolean;
    @Input() text: string; 

    constructor() { }
}
