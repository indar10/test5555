import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[emailValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: MultipleEmailValidator, multi: true }]
})

export class MultipleEmailValidator implements Validator {
    @Input() emailValidator: string;

    constructor() { }

    validate(control: AbstractControl): ValidationErrors { 
        if (control.value == "" || control.value == undefined ) {
            return;
        }
        var trigger = control.value,
            regexp = new RegExp('^' + this.emailValidator+'$'),
            test = regexp.test(trigger);

        return test ? null : { emailValidator: { valid: false } };
    }
}