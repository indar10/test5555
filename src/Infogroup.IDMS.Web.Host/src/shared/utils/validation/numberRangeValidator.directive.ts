import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[numberRangeValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: NumberRangeValidator, multi: true }]
})

export class NumberRangeValidator implements Validator {
    @Input() numberRangeValidator: number;
    validationFlag: any;
    constructor() { }

    validate(control: AbstractControl): ValidationErrors {    
        
        if (isNaN(control.value)) {
            this.validationFlag = false;
        }
        else {
            if (control.value == "" || parseInt(control.value) <= 0 || parseInt(control.value) > this.numberRangeValidator)
                this.validationFlag = false;
            else
                this.validationFlag = true;
        }
        return this.validationFlag ? null : { numberRangeValidator: { valid: false } };
    }
}