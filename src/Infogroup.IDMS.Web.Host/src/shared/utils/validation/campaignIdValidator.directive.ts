import { Directive, Input } from '@angular/core';
import { AbstractControl, NG_VALIDATORS, ValidationErrors, Validator } from "@angular/forms";

@Directive({
    selector: '[campaignIdValidator]',
    providers: [{ provide: NG_VALIDATORS, useExisting: CampaignIdValidatorDirective, multi: true }]
})

export class CampaignIdValidatorDirective implements Validator {
    @Input() campaignIdValidator: string;

    constructor() { }

    validate(control: AbstractControl): ValidationErrors {  
        
        if (control.value == "") {
            return;
        }
        var trigger = control.value,
            regexp = new RegExp('^' + this.campaignIdValidator + '$'),
            test = regexp.test(trigger);

        return test ? null : { campaignIdValidator: { valid: false } }
    }
}