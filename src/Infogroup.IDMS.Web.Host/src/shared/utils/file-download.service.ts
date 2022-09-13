import { Injectable } from '@angular/core';
import { AppConsts } from '@shared/AppConsts';
import { FileDto } from '@shared/service-proxies/service-proxies';

@Injectable()
export class FileDownloadService {

    downloadTempFile(file: FileDto) {
        const url = AppConsts.remoteServiceBaseUrl + '/File/DownloadTempFile?fileType=' + file.fileType + '&fileToken=' + file.fileToken + '&fileName=' + file.fileName;
        location.href = url; //TODO: This causes reloading of same page in Firefox
    }
    downloadFile(file: FileDto) {
        const url = AppConsts.remoteServiceBaseUrl + '/File/DownloadFile?fileType=' + file.fileType + '&fileToken=' + file.fileToken + '&fileName=' + file.fileName + '&itShouldDelete=' + file.itShouldDelete;
        location.href = url; //TODO: This causes reloading of same page in Firefox
    }
    downloadDocumentAttachment(file: FileDto) {
        const url = AppConsts.remoteServiceBaseUrl + '/File/DownloadFileDocumentAttchment?fileType=' + file.fileType + '&fileToken=' + file.fileToken + '&fileName=' + file.fileName + '&itShouldDelete=' + file.itShouldDelete + '&downloadedFileName=' + file.downloadedFileName+'&isAWS=' + file.isAWS;
        location.href = url; //TODO: This causes reloading of same page in Firefox
    }
}
