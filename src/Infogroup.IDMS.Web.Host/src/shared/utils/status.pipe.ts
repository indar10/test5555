import {Pipe, PipeTransform} from '@angular/core';

@Pipe({name: 'statusTransform'})
export class statusTransformPipe implements PipeTransform {
    transform(value) {
        
        switch(value){
            case 10: value = '10: Count Created';
                break;
            case 20: value = '20: Count Submitted';
                break;
            case 30: value = '30: Count Running';
                break;
            case 40: value = '40: Count Completed';
                break;
            case 50: value = '50: Count Failed';
                break;
            case 60: value = '60: Ready to Output';
                break;
            case 70: value = '70: Output Submitted';
                break;
            case 80: value = '80: Output Running';
                break;
            case 90: value = '90: Output Completed';
                break;
            case 100: value = '100: Output Failed';
                break;
            case 110: value = '110: Approved for Shipping';
                break;
            case 120: value = '120: Waiting to Ship';
                break;
            case 130: value = '130: Shipped';
                break;
            case 140: value = '140: Shipment Failed';
                break;
            case 150: value = '150: Cancelled';
                break;
            default: value = '';
                break;
        }
        return value;
    }
}