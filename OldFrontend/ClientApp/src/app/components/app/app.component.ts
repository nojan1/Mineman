import { Component, ViewContainerRef } from '@angular/core';
import { Overlay } from 'ngx-modialog';
import { Modal } from 'ngx-modialog/plugins/bootstrap';

import { LayoutService } from '../../services/layout.service';

@Component({
    selector: 'app',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(overlay: Overlay,
                vcRef: ViewContainerRef,
                public modal: Modal,
                public layoutService: LayoutService) {

        //overlay.defaultViewContainer = vcRef;
    }
}
