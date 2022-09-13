import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";
import { parse } from 'url';

@Directive({
    selector: '[maxLen]',
    providers: [{ provide: NG_VALIDATORS, useExisting: MaxLengthValidatorDirective, multi: true }]
})

export class MaxLengthValidatorDirective implements Validator {
    @Input() maxLen: number;

    constructor() { }

    validate(control: AbstractControl): ValidationErrors {
        if (control.value == null || control.value == undefined)
            control.setValue("");
        return control.value.length <= this.maxLen ? null : { maxLen: { valid: false } };
    }
}