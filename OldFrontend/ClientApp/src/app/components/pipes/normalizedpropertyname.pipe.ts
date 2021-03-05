import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'normalizedPropertyName' })
export class NormalizedPropertyNamePipe implements PipeTransform {
    transform(value: string): string {
        return value.replace(/__/g, ".")
            .replace(/_/g, "-")
            .toLowerCase();
    }
}