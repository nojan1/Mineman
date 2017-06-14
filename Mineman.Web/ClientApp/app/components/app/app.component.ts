import { Component, ViewContainerRef } from '@angular/core';
import { Overlay } from 'angular2-modal';
import { Modal } from 'angular2-modal/plugins/bootstrap';

import { ToastsManager } from 'ng2-toastr/ng2-toastr';

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
                private toastr: ToastsManager,
                public layoutService: LayoutService) {

        overlay.defaultViewContainer = vcRef;
        this.toastr.setRootViewContainerRef(vcRef);
    }
}
