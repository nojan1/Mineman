import { Injectable } from '@angular/core';

import { Observable } from 'rxjs';

@Injectable()
export class ErrorService {
    constructor() { }

    public catchObservable<T>(error: any, caught: Observable<T>) {
        //this.modal.alert()
        //    .title("Error")
        //    .body(error)
        //    .open();

        alert(error);

        return Observable.throw(error);
    }

    public catchObservableSilent<T>(error: any, caught: Observable<T>) {
        console.error(error);

        return Observable.throw(error);
    }
}