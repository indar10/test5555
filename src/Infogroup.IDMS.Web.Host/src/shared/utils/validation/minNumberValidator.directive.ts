import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[minNumber]',
    providers: [{ provide: NG_VALIDATORS, useExisting: MinNumberValidator, multi: true }]
})

export class MinNumberValidator implements Validator {
    @Input() minNumber: number;

    constructor() { }

    validate(control: AbstractControl): ValidationErrors {
        if (isNaN(control.value) || control.value == "") {
            return;
        }
        return parseInt(control.value) >= this.minNumber ? null : { minNumber: { valid: false } };
    }
}