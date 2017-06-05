import { Injectable } from '@angular/core';

@Injectable()
export class LoadingService {
    constructor() { }

    public loadingMessages = [];

    public setLoading(message: string): number {
        this.loadingMessages.push(message);

        return -1;
    }

    public clearLoading(id: number) {
        this.loadingMessages = [];
    }
}