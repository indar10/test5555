import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[maxNumber]',
    providers: [{ provide: NG_VALIDATORS, useExisting: MaxNumberValidatorDirective, multi: true }]
})

export class MaxNumberValidatorDirective implements Validator {
    @Input() maxNumber: number;

    constructor() { }

    validate(control: AbstractControl): ValidationErrors {
        if (isNaN(control.value) || control.value == "" || control.value == null) {
            return;
        }
        return parseInt(control.value) <= this.maxNumber ? null : { maxNumber: { valid: false } };
    }
}