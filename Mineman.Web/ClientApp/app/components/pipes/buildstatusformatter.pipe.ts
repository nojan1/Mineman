import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'buildstatus' })
export class BuildStatusFormatter implements PipeTransform {
    transform(value: any): string {
        if (!value) {
            return "Not started";
        }

        if (value.buildSucceded) {
            return "Failed";
        } else {
            return "Completed";
        }
    }
}