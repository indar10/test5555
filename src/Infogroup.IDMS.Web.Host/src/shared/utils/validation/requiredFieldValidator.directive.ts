import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[requiredFieldValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: RequiredFieldValidator, multi: true }]
})

export class RequiredFieldValidator implements Validator {
    @Input() requiredFieldValidator: string;
    validationFlag: any;
    constructor() { }

    validate(control: AbstractControl): ValidationErrors {                
        if ((this.requiredFieldValidator == "tblSegment.IDMSNumber" || this.requiredFieldValidator == "tblSegment.cKeyCode1" || this.requiredFieldValidator == "tblSegment.cKeyCode2" || this.requiredFieldValidator == "tblSegment.distance" || this.requiredFieldValidator == "tblSegment.SpecialSIC" || this.requiredFieldValidator == "tblSegment.SPECIALSIC" || this.requiredFieldValidator == "tblSegment.SICDescription"))
            this.validationFlag = true;
        else {
            if (control.value == "")
                this.validationFlag = false;
            else
                this.validationFlag = true;
        }
        return this.validationFlag ? null : { requiredFieldValidator: { valid: false } };
    }
}