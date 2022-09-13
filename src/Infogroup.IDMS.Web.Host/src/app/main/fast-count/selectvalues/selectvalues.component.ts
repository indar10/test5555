
import {Component, Injector} from '@angular/core';
import {AppComponentBase} from '@shared/common/app-component-base';
import {SegmentSelectionsServiceProxy, SegmentSelectionSaveDto} from '@shared/service-proxies/service-proxies';

interface Role {
    name: string;
    value: string;
}

interface OptionValueList {
    id: number;
    cDescription: string;
    cValue: string;
}


@Component({
    selector: 'app-selectvalues',
    templateUrl: './selectvalues.component.html',
    styleUrls: ['./selectvalues.component.css']
})

export class SelectValuesComponent extends AppComponentBase {

    public index: number;
    public selfRef: SelectValuesComponent;
    public fieldName: string;
    buildLolId: number;
    countries: any[];
    public selectedDBAttribute: {
        label: string,
        field: string,
        type: string,
        input: string,
        multiple: boolean,
        id: string
    };
    dto: SegmentSelectionSaveDto = new SegmentSelectionSaveDto();
    optionValues: OptionValueList[];
    selectedOptions: OptionValueList[];
    selectedRoles: Role[];

    constructor(injector: Injector, private _segmentSelectionProxy: SegmentSelectionsServiceProxy) {
        super(injector);
    }

    // tslint:disable-next-line:use-lifecycle-interface
    ngOnInit() {
        this.getFieldsValues();
    }

    getFieldsValues() {
        this._segmentSelectionProxy.getFieldValues(this.selectedDBAttribute.id, this.buildLolId)
            .subscribe(result => {
                let valueList = result;
                this.optionValues = valueList;
            });
    }

}
