import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { UtilsModule } from '@shared/utils/utils.module';
import { CountoModule } from 'angular2-counto';
import { TableModule } from 'primeng/table';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppCommonModule } from '@app/shared/common/app-common.module';
import { DivisionalMailerComponent } from './divisional-mailer/divisional-mailer.component';
import { MaintenanceRoutingModule } from './maintenance-routing.module';
import { PaginatorModule } from 'primeng/paginator';
import { CheckboxModule } from 'primeng/checkbox';
import { ExportLayoutComponent } from './export-layout/export-layout.component';
import { BsDropdownModule, PopoverModule, ModalModule, TooltipModule, TabsModule } from 'ngx-bootstrap';
import { CreateorEditDivisionalMailerComponent } from './divisional-mailer/create-or-edit-divisional-mailer.component';
import { CreateExportLayoutModalComponent } from './export-layout/create-export-layout-modal.component';
import { SharedModule } from '../shared/shared.module';
import { CreateOrEditExportLayoutComponent } from '../shared/export-layout-add-fields/create-or-edit-export-layout.component';
import { RedisCacheComponent } from './redis-cache/redis-cache.component';
import { ExportLayoutFieldsComponent } from './export-layout/export-layout-fields/export-layout-fields.component';
import { DivisionShipTosComponent } from './divisionShipTos/divisionShipTos/divisionShipTos.component';
import { CreateOrEditDivisionShipToModalComponent } from './divisionShipTos/divisionShipTos/create-or-edit-divisionShipTo-modal.component';
import { MaintainanceContainerComponent } from './maintainance-tab-container/maintainance-container.component';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { EditorModule } from 'primeng/editor';
import { InputMaskModule } from 'primeng/inputmask'; import { FileUploadModule } from 'primeng/fileupload';
import { OwnersComponent } from './owners/owners.component';
import { CreateOrEditOwnerModalComponent } from './owners/create-or-edit-owner-modal.component';
import { MailersComponent } from './mailers/mailers/mailers.component';
import { CreateOrEditMailerModalComponent } from './mailers/mailers/create-or-edit-mailer-modal.component';
import { CreateOrEditOfferModalComponent } from './offers/offers/create-or-edit-offer-modal.component';
import { OffersComponent } from './offers/offers/offers.component';
import { ContactsComponent } from './contacts/contacts/contacts.component';
import { CreateOrEditContactModalComponent } from './contacts/contacts/create-or-edit-contact-modal.component';
import { OfferSamplesComponent } from './offerSamples/offerSamples/offerSamples.component';
import { CreateOrEditOfferSampleModalComponent } from './offerSamples/offerSamples/create-or-edit-offerSample-modal.component';
import { DatabasesComponent } from './databases/databases/databases.component';
import { CreateOrEditDatabaseModalComponent } from './databases/databases/create-or-edit-database-modal.component';
import { ManagersComponent } from './managers/managers/managers.component';
import { CreateOrEditManagerModalComponent } from './managers/managers/create-or-edit-manager-modal.component';
import { BrokersComponent } from './brokers/brokers.component';
import { CreateOrEditBrokerModalComponent } from './brokers/create-or-edit-broker-modal.component';
import { DecoysComponent } from './decoys/decoys.component';
import { CreateOrEditDecoyModalComponent } from './decoys/create-or-edit-decoy-modal.component';
import { DecoyMailersComponent } from './decoys/decoy-mailers.component';
import { GroupBrokersComponent } from './group-broker/group-broker-modal.component';
import { SecurityGroupsComponent } from './securityGroups/security-group-modal.component';
import { CreateOrEditSecurityGroupModalComponent } from './securityGroups/create-or-edit-security-group-modal.component';
import { SelectBrokerModalComponent } from './securityGroups/select-broker.modal.component';
import { GroupUserModalComponent } from './securityGroups/group-user-modal.component';
//import { CopyExportLayoutComponent } from './export-layout/copy-export-layout-modal.component';
import { ListAutomateComponent } from './list-automate/list-automate/list-automate.component';
import { CreateOrEditListautomateModalComponent} from './list-automate/list-automate/create-or-edit-listautomate-modal.component';
import { CopyExportLayoutComponent } from './export-layout/copy-export-layout-modal.component';
import { LookupsComponent } from './lookups/lookups.component';
import { CreateOrEditLookupModalComponent } from './lookups/create-or-edit-lookup-modal.component';;
import { IdmsConfigurationComponent } from './idms-configuration/idms-configuration.component';
import { CreateOrEditIdmsconfigurationComponent } from './idms-configuration/create-or-edit-idmsconfiguration.component';
import { CalendarModule } from 'primeng/calendar';
import { ExternalDatabaseLinksComponent } from './external-database-links/external-database-links.component';
import { CreateOrEditExternaDatabaseLinksComponent } from './external-database-links/create-or-edit-externa-database-links/create-or-edit-externa-database-links.component';
import { ProcessQueueComponent } from './process-queue/process-queue.component'
import { CreateOrEditProcessQueueComponent } from './process-queue/create-or-edit-process-queue/create-or-edit-process-queue.component';
import { CreateOrEditProcessQueueDatabaseComponent } from './process-queue/create-or-edit-process-queue-database/create-or-edit-process-queue-database.component';
import { ProcessQueueDatabasesComponent } from './process-queue/process-queue-databases/process-queue-databases.component'
import { ListboxModule } from 'primeng/primeng';
@NgModule({
    imports: [
        CalendarModule,
        FileUploadModule,
        AutoCompleteModule,
        EditorModule,
        InputMaskModule,
        CommonModule,
        FormsModule,
        AppCommonModule,
        UtilsModule,
        CountoModule,
        PaginatorModule,
        NgbModule,
        CheckboxModule,
        MaintenanceRoutingModule,
        BsDropdownModule.forRoot(),
        PopoverModule.forRoot(),
        SharedModule,
        FileUploadModule,
        TableModule,
        ModalModule.forRoot(),
        TooltipModule.forRoot(),
        TabsModule.forRoot(),
        ListboxModule,        
    ],
    declarations: [
        DatabasesComponent,
        CreateOrEditDatabaseModalComponent,
        OfferSamplesComponent,
        CreateOrEditOfferSampleModalComponent,
        OffersComponent,
        CreateOrEditOfferModalComponent,
        MailersComponent,
        CreateOrEditMailerModalComponent,
        DivisionShipTosComponent,
        CreateOrEditDivisionShipToModalComponent,
        DivisionalMailerComponent,
        OwnersComponent,
        ContactsComponent,
        RedisCacheComponent,
        CreateorEditDivisionalMailerComponent,
        CreateOrEditOwnerModalComponent,
        CreateOrEditContactModalComponent,
        ExportLayoutComponent,
        CreateExportLayoutModalComponent,
        ExportLayoutFieldsComponent,
        MaintainanceContainerComponent,
        BrokersComponent,
        CreateOrEditBrokerModalComponent,        
        ManagersComponent,
        CreateOrEditManagerModalComponent,
        CreateOrEditDatabaseModalComponent,
        DecoysComponent,
        CreateOrEditDecoyModalComponent,
        DecoyMailersComponent,
        CreateOrEditDatabaseModalComponent,
        GroupBrokersComponent,
        SecurityGroupsComponent,
        CreateOrEditSecurityGroupModalComponent,
        SelectBrokerModalComponent,
        GroupUserModalComponent,
        CopyExportLayoutComponent,
        ListAutomateComponent,
        CreateOrEditListautomateModalComponent,
        LookupsComponent,
        CreateOrEditLookupModalComponent,
        IdmsConfigurationComponent,
        CreateOrEditIdmsconfigurationComponent, 
        ExternalDatabaseLinksComponent,
        CreateOrEditExternaDatabaseLinksComponent  ,
        CreateOrEditIdmsconfigurationComponent ,
        ProcessQueueComponent ,
        CreateOrEditProcessQueueComponent ,
        CreateOrEditProcessQueueDatabaseComponent,
        ProcessQueueDatabasesComponent,
    ],
    entryComponents: [
        CreateOrEditContactModalComponent,
        CreateorEditDivisionalMailerComponent,
        CreateExportLayoutModalComponent,
        CreateOrEditExportLayoutComponent,
        CreateOrEditOwnerModalComponent,
        CreateOrEditDivisionShipToModalComponent,
        CreateOrEditOfferModalComponent,
        CreateOrEditOfferSampleModalComponent,
        CreateOrEditDatabaseModalComponent,        
        CreateOrEditManagerModalComponent,
        CreateOrEditMailerModalComponent,        
        CreateOrEditBrokerModalComponent,
        CreateOrEditDecoyModalComponent,
        CreateOrEditBrokerModalComponent,
        CreateOrEditSecurityGroupModalComponent,
        SelectBrokerModalComponent,
        GroupUserModalComponent,
        CopyExportLayoutComponent,
        CreateOrEditListautomateModalComponent,
        CreateOrEditLookupModalComponent,
        CreateOrEditIdmsconfigurationComponent,
        CreateOrEditExternaDatabaseLinksComponent,
        CreateOrEditIdmsconfigurationComponent,
        CreateOrEditProcessQueueComponent,
        ProcessQueueDatabasesComponent,
        CreateOrEditProcessQueueDatabaseComponent
        
    ]
})
export class MaintenanceModule { }