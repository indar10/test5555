import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[valueValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: SegmentRequiredQuantityValidator, multi: true }]
})

export class SegmentRequiredQuantityValidator implements Validator {
    @Input() valueValidator: string;

    constructor() { }

    validate(control: AbstractControl): ValidationErrors {        
        if (control.value == "" || control.value == undefined || !isNaN(control.value)) {
            return;
        }
        var trigger = control.value.toUpperCase(),
            regexp = new RegExp( this.valueValidator),
            test = regexp.test(trigger);

        return test ? null : { valueValidator: { valid: false } };
    }
}