import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[noSpaceValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: NoSpaceValidator, multi: true }]
})

export class NoSpaceValidator implements Validator {
    
    @Input() noSpaceValidator: string;
    validationFlag: any;
    constructor() { }

    validate(control: AbstractControl): ValidationErrors {  
        
        if ((control.value != null && control.value.toString().trim() == "" )|| !control.value)
            this.validationFlag = false;
            else
            this.validationFlag = true;
        return this.validationFlag ? null : { noSpaceValidator: { valid: false } };
    }
}