import { Injectable, EventEmitter } from '@angular/core';

export interface LoadingInstance {
    message: string;
    handle: number;
    progress: number;
    hasProgress: boolean;
}

@Injectable()
export class LoadingService {
    constructor() { }

    public instanceChanged = new EventEmitter<LoadingInstance>();

    //public currentInstance: LoadingInstance;

    private instanceBuffer : LoadingInstance[] = [];

    public setLoading(message: string): number {
        var handle = this.getHandle();
        var instance: LoadingInstance = {
            handle: handle,
            hasProgress: false,
            message: message,
            progress: -1
        };

        this.instanceBuffer.push(instance);
        this.updateCurrentInstance();

        return handle;
    }

    public updateProgress(handle: number, progress: number) {
        var instance = this.instanceBuffer.find(v => v.handle == handle);
        instance.hasProgress = true;
        instance.progress = progress;

        this.updateCurrentInstance();
    }

    public clearLoading(handle: number) {
        var index = this.instanceBuffer.findIndex(v => v.handle == handle);
        if (index != -1) {
            this.instanceBuffer.splice(index, 1);
            this.updateCurrentInstance();
        }
    }

    private getHandle(): number {
        var handle = this.instanceBuffer.length + 1;
        while (this.instanceBuffer.some(x => x.handle == handle)) {
            handle++;
        }

        return handle;
    }

    private updateCurrentInstance() {
        if (this.instanceBuffer.length == 0) {
            this.instanceChanged.emit(null);
            //this.currentInstance = null;
        } else {
            //this.currentInstance = this.instanceBuffer[0];
            this.instanceChanged.emit(this.instanceBuffer[0]);
        }
    }
}