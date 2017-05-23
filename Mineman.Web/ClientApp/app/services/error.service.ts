import { Injectable } from '@angular/core';

import { Modal } from 'angular2-modal/plugins/bootstrap';
import { Observable } from 'rxjs';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ErrorService {
    constructor(private modal: Modal) { }

    public catchObservable<T>(error: any, caught: Observable<T>) {
        //this.modal.alert()
        //    .title("Error")
        //    .body(error)
        //    .open();

        alert(error);

        return Observable.empty();
    }

    public catchObservableSilent<T>(error: any, caught: Observable<T>) {
        console.error(error);

        return Observable.empty();
    }
}