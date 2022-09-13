import { Injectable } from '@angular/core';
import { AppLocalizationService } from '@app/shared/common/localization/app-localization.service';
import * as moment from 'moment';

@Injectable()
export class IDMSDateTimeService {

    constructor(private _appLocalizationService: AppLocalizationService) {

    }

    createDateRangePickerOptions(): any {
        let ranges : Date[]=[];
        ranges[this._appLocalizationService.l('Today')] = [moment().startOf('day').toDate(), moment().endOf('day').toDate()];
        ranges[this._appLocalizationService.l('Yesterday')] = [moment().subtract(1, 'days').startOf('day').toDate(), moment().subtract(1, 'days').endOf('day').toDate()];
        ranges[this._appLocalizationService.l('Last7Days')] = [moment().subtract(6, 'days').startOf('day').toDate(), moment().endOf('day').toDate()];
        ranges[this._appLocalizationService.l('Last30Days')] = [moment().subtract(29, 'days').startOf('day').toDate(), moment().endOf('day').toDate()];
        ranges[this._appLocalizationService.l('ThisMonth')] = [moment().startOf('month').toDate(), moment().endOf('month').toDate()];
        ranges[this._appLocalizationService.l('LastMonth')] = [moment().subtract(1, 'month').startOf('month').toDate(), moment().subtract(1, 'month').endOf('month').toDate()];
        return ranges;
    }
}
