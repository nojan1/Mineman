import { Component, OnInit, Input } from '@angular/core';

import { AuthService } from '../../services/auth.service';

@Component({
    selector: 'propertieseditor',
    templateUrl: './propertieseditor.component.html'
})
export class PropertiesEditorComponent implements OnInit {
    @Input() userProperties: string[];
    @Input() propertyValues: any;

    constructor(public authService: AuthService) { }

    ngOnInit() {

    }
}
