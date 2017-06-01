import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'logformatting' })
export class LogFormattingPipe implements PipeTransform {
    transform(value: string): string {
        let withLineBreaks = (value + '').replace(/([^>\r\n]?)(\r\n|\n\r|\r|\n)/g, '$1<br/>$2');

        return withLineBreaks;
    }
}