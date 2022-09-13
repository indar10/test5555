import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[exportWidthFieldValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: ExportWidthFieldValidator, multi: true }]
})

export class ExportWidthFieldValidator implements Validator {
    @Input() exportWidthFieldValidator: number;
    test: any;
    constructor() { }

    validate(control: AbstractControl): ValidationErrors {
        
        if (isNaN(control.value)) {
            this.test = false;
        }
        else {
            if (control.value == "" || parseInt(control.value) <= 0)
                this.test = false;
            else
                this.test = true;
        }
        
        return this.test ? null : { exportWidthFieldValidator: { valid: false } };
    }
}