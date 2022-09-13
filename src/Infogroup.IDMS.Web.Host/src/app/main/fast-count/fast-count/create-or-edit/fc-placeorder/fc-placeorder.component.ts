import { HttpClient } from "@angular/common/http";
import { Component, Injector, Input, OnInit } from "@angular/core";
import { NgForm } from "@angular/forms";
import { NgbActiveModal, NgbNavChangeEvent } from "@ng-bootstrap/ng-bootstrap";
import { AppConsts } from "@shared/AppConsts";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
  CreateOrEditCampaignDto,
  CampaignGeneralDto,
  GetCampaignsOutputDto,
  CampaignsServiceProxy,
  EditCampaignsOutputDto,
  CampaignBillingDto,
  GetCampaignMaxPerForViewDto,
  CampaignLevelMaxPerDto,
  GetDecoyForViewDto,
  SegmentsServiceProxy,
  CreateOrEditSegmentDto,
  DropdownOutputDto,
  ExportLayoutsServiceProxy,
  GetExportLayoutSelectedFieldsDto,
  CampaignAttachmentDto,
  IDMSConfigurationsServiceProxy,
  IDMSConfigurationDto,
  SegmentSelectionDto,
  CampaignActionInputDto,
  CampaignStatus,
} from "@shared/service-proxies/service-proxies";
import { FileDownloadService } from "@shared/utils/file-download.service";
import { noop } from 'lodash';
import { FileUpload } from "primeng/fileupload";
import { finalize } from "rxjs/operators";

@Component({
  selector: "app-fc-placeorder",
  templateUrl: "./fc-placeorder.component.html",
})
export class FcPlaceorderComponent extends AppComponentBase implements OnInit {
  isLoading: boolean = true;
  isSaving: boolean = false;
  campaign: CreateOrEditCampaignDto = CreateOrEditCampaignDto.fromJS({
    generalData: CampaignGeneralDto.fromJS({
      mailer: new DropdownOutputDto(),
    }),
    campaignOutputDto: new EditCampaignsOutputDto(),
    getOutputData: GetCampaignsOutputDto.fromJS({
      shipToList: [],
      layoutlist: [],
    }),
    billingData: new CampaignBillingDto(),
    documentsData: [],
  });
  activeNav: number = 1;
  isEditLayout: boolean = false;
  requiredQuantityValue: string;
  allAvailableText = "All Available";
  segment: CreateOrEditSegmentDto = new CreateOrEditSegmentDto();
  mailers: DropdownOutputDto[] = [];
  exportLayoutAddFields: DropdownOutputDto[] = [];
  selectedOutputShipTo: string;
  shipToEmail: string = "";
  layoutExistingFields: GetExportLayoutSelectedFieldsDto[] = [];
  selectedAvailableFields: string[] = [];
  selectedTableValue: any;
  savedLayoutId: string;
  savedQuantityValue: number;
  idmsConfig: IDMSConfigurationDto[] = [];
  permissionForPlaceOrder: boolean;
  @Input() campaignId: number;
  @Input() databaseId: number;
  @Input() divisionId: number;
  @Input() segmentId: number;
  @Input() divisionalDatabase: boolean;
  @Input() buildId: number;
  @Input() fastCountConfig: any;
  @Input() selections: SegmentSelectionDto[];

  constructor(
    injector: Injector,
    public activeModal: NgbActiveModal,
    private _orderServiceProxy: CampaignsServiceProxy,
    private _segmentsServiceProxy: SegmentsServiceProxy,
    private _exportLayoutServiceProxy: ExportLayoutsServiceProxy,
    private _fileDownloadService: FileDownloadService,
    private _httpClient: HttpClient,
    private _idmsConfigProxy: IDMSConfigurationsServiceProxy,
    private _campaignServiceProxy: CampaignsServiceProxy
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.getDataForEdit();
    this.getRequiredConfiguration();
  }

  getRequiredConfiguration() {
    this._idmsConfigProxy
      .getMultipleConfigurationsValue(
        ["LVO_MAX_QUANTITY", "LVO_FORM_REQUIRED", "FAX_FORM_REQUIRED", "TeleMarketingFields"],
        this.databaseId
      )
      .subscribe((result) => (this.idmsConfig = result));
  }

  getDataForEdit() {
    this.isLoading = true;
    this._orderServiceProxy
      .getCampaignForEdit(this.campaignId, this.databaseId, this.divisionId)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((result) => {
        this.campaign = result;
        if (!this.campaign.campaignOutputDto) {
          this.campaign.campaignOutputDto = new EditCampaignsOutputDto();
        }
        this.campaign.campaignOutputDto.layout = result.getOutputData.layout
          ? result.getOutputData.layout
          : this.fastCountConfig["default-output-layout"]
            ? String(this.fastCountConfig["default-output-layout"])
            : "";
        this.savedLayoutId = `${result.getOutputData.layoutId}`;
        const isNotCampaign = this.savedLayoutId == "" || +this.savedLayoutId <= 0;
        this.getSelectedFields(!isNotCampaign);

        this.permissionForPlaceOrder = this.isGranted("Pages.PlaceOrder");

        //IDMS-2347-Place order process - "Ship to"
        if (this.permissionForPlaceOrder) {
          if (this.campaign.getOutputData.shipTo) {
            this.selectedOutputShipTo = this.campaign.getOutputData.shipTo;
          } else {
            this.selectedOutputShipTo = this.appSession.user.emailAddress;
          }
        }
      });
    this._segmentsServiceProxy
      .getSegmentForEdit(this.segmentId)
      .subscribe((result) => {
        this.segment = result;
        this.savedQuantityValue = this.segment.iRequiredQty;
        if (this.segment.iRequiredQty === 0) {
          this.requiredQuantityValue = this.allAvailableText;
        } else {
          this.requiredQuantityValue = `${result.iRequiredQty}`;
        }
      });
  }

  getSelectedFields(isCampaignLayout: boolean) {
    this.isLoading = true;
    let campaignId;
    let layoutId;
    let buildId;
    if (isCampaignLayout) {
      campaignId = this.campaignId;
      layoutId = 0;
      buildId = 0;
    } else {
      layoutId = +this.campaign.campaignOutputDto.layout;
      buildId = this.buildId;
    }
    this._exportLayoutServiceProxy
      .getExportLayoutSelectedFields(
        campaignId,
        isCampaignLayout,
        layoutId,
        buildId
      )
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((result) => (this.layoutExistingFields = result));
  }

  onNavChange(event: NgbNavChangeEvent) {
    if (event.nextId == 2 && !this.layoutExistingFields.length) {
      const isNotCampaign = this.savedLayoutId == "" || +this.savedLayoutId <= 0;
      this.getSelectedFields(!isNotCampaign);
    }
  }

  getLayoutDescription() {
    if (
      this.campaign.campaignOutputDto.layout &&
      this.campaign.getOutputData.layoutlist.length
    ) {
      const layout = this.campaign.getOutputData.layoutlist.find(
        (item) => item.value == this.campaign.campaignOutputDto.layout
      );
      return layout.label || "";
    }
    return "";
  }

  onRequiredQuantityKeyDown(event) {
    return event.key === "." ? false : true;
  }

  searchMailers(event) {
    this._orderServiceProxy
      .getSearchResultForMailer(
        event.query,
        this.databaseId,
        this.divisionalDatabase
      )
      .subscribe((result) => (this.mailers = result));
  }

  shipToKeyDown(event): void {
    if (event.key === "Enter") {
      if (!isNaN(this.selectedOutputShipTo as any)) {
        this.campaign.campaignOutputDto.companyId = parseInt(
          this.selectedOutputShipTo
        );
        this.getEmailAddress();
      }
    }
  }

  getEmailAddress() {
    this._orderServiceProxy
      .getDetailsByCompanyIdAndOrderId(
        this.campaignId,
        +this.selectedOutputShipTo
      )
      .subscribe((result: any) => {
        this.campaign.campaignOutputDto.ftpSite = result.ftpSite;
        this.campaign.campaignOutputDto.userName = result.userName;
        if (!this.shipToEmail) {
          this.shipToEmail = result.emailAddress;
        } else if (result.emailAddress !== "") {
          this.shipToEmail += ";" + result.emailAddress;
        }
        this.selectedOutputShipTo = this.shipToEmail;
      });
  }

  onShipToDropdownChange(event): void {
    if (event.originalEvent instanceof PointerEvent) {
      if (
        !isNaN(this.selectedOutputShipTo as any) &&
        this.selectedOutputShipTo.trim() !== ""
      ) {
        this.campaign.campaignOutputDto.companyId = parseInt(
          this.selectedOutputShipTo
        );
        this.getEmailAddress();
      } else {
        this.selectedOutputShipTo = event.value;
      }
    }
  }

  selectNext(tabNo: number) {
    this.activeNav = tabNo;
    if (!this.layoutExistingFields.length) {
      const isNotCampaign = this.savedLayoutId == "" || +this.savedLayoutId <= 0;
      this.getSelectedFields(!isNotCampaign);
    }
  }

  editLayout() {
    this.isLoading = true;
    if (this.savedLayoutId != this.campaign.campaignOutputDto.layout) {
      this._orderServiceProxy
        .copyOrderExportLayout(
          this.campaignId,
          +this.campaign.campaignOutputDto.layout,
          this.appSession.idmsUser.idmsUserName,
          this.campaign.currentStatus
        )
        .subscribe(() => {
          this.getEditLayoutData();
          this.savedLayoutId = this.campaign.campaignOutputDto.layout;
        });
    } else {
      this.getEditLayoutData();
    }
  }

  getEditLayoutData() {
    this._exportLayoutServiceProxy
      .getTableDescriptionDropDownValues(this.campaignId, 0, true, 0)
      .subscribe((result) => {
        this.selectedTableValue = result.find((x) =>
          x.label.includes("(tblMain)")
        ).value;
        this.getLayoutDataField();
      });
  }

  addFields() {
    this.isLoading = true;
    const selectedAddFields = [];
    this.selectedAvailableFields.forEach((item) => {
      var elItem = this.exportLayoutAddFields.find(
        (elItem) => elItem.value == item
      );
      selectedAddFields.push(elItem.label);
    });
    this.selectedAvailableFields = [];
    this._exportLayoutServiceProxy
      .addNewSelectedFields(
        selectedAddFields.join(","),
        this.selectedTableValue,
        this.campaignId,
        0,
        true,
        0,
        0,
        this.campaign.currentStatus
      )
      .subscribe(() => {
        this.notify.info(this.l("Added Successfully"));
        this.getLayoutDataField();
        this.getSelectedFields(true);
      });
  }

  deleteField(fieldId: any) {
    this.isLoading = true;
    this._exportLayoutServiceProxy
      .deleteExportLayoutRecord([fieldId], this.campaignId, true, 0, 0)
      .subscribe(() => {
        this.notify.info(this.l("Deleted Successfully"));
        this.getLayoutDataField();
        this.getSelectedFields(true);
      });
  }

  goBackToLayout() {
    if (this.isEditLayout && this.activeNav != 3) {
      this.isEditLayout = false;
      this.selectedAvailableFields = [];
    } else {
      this.activeNav -= 1;
    }
  }

  onLayoutDropdownChange(): void {
    this.getSelectedFields(false);
  }

  save(placeOrderForm: NgForm) {
    if (!this.validate()) {
      return;
    }
    this.isSaving = true;
    this.saveSegmentIfRequired();
    if (this.savedLayoutId != this.campaign.campaignOutputDto.layout) {
      this._orderServiceProxy
        .copyOrderExportLayout(
          this.campaignId,
          +this.campaign.campaignOutputDto.layout,
          this.appSession.idmsUser.idmsUserName,
          this.campaign.currentStatus
        )
        .subscribe(noop);
    }
    if (placeOrderForm.form.pristine) {
      this.validateForTelemarketing();
    } else {
      this._orderServiceProxy
        .createOrEdit(this.getCampaignData(), this.campaign.currentStatus)
        .subscribe(() => this.validateForTelemarketing());
    }
  }

  validateForTelemarketing() {
    if (this.campaign.generalData.cChannelType == "T") {
      this.isSaving = true;
      this._orderServiceProxy.validateCampaignForSAN(this.campaignId)
        .pipe(finalize(() => (this.isSaving = false)))
        .subscribe(result => {
          if (result) this.executeCampaign();
        })
    } else {
      this.executeCampaign();
    }
  }

  executeCampaign() {
    this.isSaving = true;
    const input = CampaignActionInputDto.fromJS({
      campaignId: this.campaignId,
      databaseId: this.databaseId,
      buildId: this.buildId,
      currentBuild: this.buildId,
      cNotes: "",
      campaignStatus: this.campaign.currentStatus
    });
    switch (this.campaign.currentStatus) {
      case CampaignStatus.OrderCreated:
      case CampaignStatus.OrderFailed: {
        input.cNotes = "AUTO_OUTPUT";
        this._campaignServiceProxy
            .campaignActions(input)
            .pipe(finalize(() => (this.isSaving = false)))
            .subscribe((result) => {
              if (result.success) {
                this.notify.info(this.l("SavedSuccessfully"));
                this.close(true);
              }
            });
      }
        break;
      case CampaignStatus.OutputFailed:
      case CampaignStatus.OutputCompleted: {
        input.cNotes = "AUTO_SHIP";
        input.campaignStatus = CampaignStatus.OutputSubmitted;
        this._campaignServiceProxy.updateOrderStatus(input).subscribe(() => {
          this.notify.info(this.l("SavedSuccessfully"));
          this.isSaving = false;
          this.close(true);
        }, () => {
          this.isSaving = false;
        });
      }
        break;
      case CampaignStatus.Cancelled:
      case CampaignStatus.ShippingFailed: {
        input.cNotes = "";
        input.campaignStatus = CampaignStatus.ApprovedforShipping;
        this._campaignServiceProxy.updateOrderStatus(input).subscribe(() => {
          this.notify.info(this.l("SavedSuccessfully"));
          this.isSaving = false;
          this.close(true);
        }, () => {
          this.isSaving = false;
        });
      }
        break;
      default: {
        this.isSaving = false;
        this.close(true);
        break;
      }
    }
  }

  validate() {
    if (
      this.validateForLVO() &&
      this.campaign.documentsData.some(
        (item) => item.code == "LVO" && !item.realFileName
      )
    ) {
      this.message.error(
        "Please upload LVO (Large Volume Order) Form to proceed."
      );
      return false;
    }
    if (
      this.validateForFAX() &&
      this.campaign.documentsData.some(
        (item) => item.code == "FWF" && !item.realFileName
      )
    ) {
      this.message.error(
        "Please upload Fax Waiver Form to proceed."
      );
      return false;
    }
    if (this.campaign.generalData.cChannelType == "T" && !this.campaign.billingData.sanNumber && this.campaign.documentsData.some(
      (item) => item.code == "DNCWF" && !item.realFileName
    )) {
      this.message.error(
        "For Tele-marketing channel, either add SAN number or upload Do not call waiver form."
      );
      return false;
    }
    if (this.idmsConfig[3] && this.idmsConfig[3].cValue && this.campaign.generalData.cChannelType != "T") { // idmsConfig-3: TeleMarketingFields
      const cnfItems = this.idmsConfig[3].cValue.toLowerCase().split(',');
      if (this.layoutExistingFields.length) {
        if (this.layoutExistingFields.some(item => cnfItems.includes(item.fieldName.toLowerCase()))) {
          this.message.error(
            "Order Channel must be Tele-marketing."
          );
          return false;
        }
      } else {
        this.message.error(
          "Export layout fields not loaded"
        );
        return false; 
      }
    }
    return true;
  }

  validateForLVO() {
    const LVOQty = this.idmsConfig[0].iValue; // idmsConfig-0: LVO_MAX_QUANTITY
    const LVORequired = this.idmsConfig[1].cValue == "1"; // idmsConfig-1: LVO_FORM_REQUIRED
    if (LVORequired && LVOQty > 0 && this.segment.iProvidedQty > LVOQty) {
      return true;
    }
    return false;
  }

  validateForFAX() {
    const FAXRequired = this.idmsConfig[2].cValue == "1"; // idmsConfig-2: FAX_FORM_REQUIRED
    if (FAXRequired && this.campaign.generalData.cChannelType == "T") {
      return true;
    }
    return false;
  }

  saveSegmentIfRequired() {
    if (
      this.requiredQuantityValue.toUpperCase() ===
      this.allAvailableText.toUpperCase()
    ) {
      this.segment.iRequiredQty = 0;
    } else {
      this.segment.iRequiredQty = +this.requiredQuantityValue;
    }
    if (this.savedQuantityValue != this.segment.iRequiredQty) {
      this.segment.orderId = this.campaignId;
      this._segmentsServiceProxy
        .createOrEdit(this.segment, this.campaign.currentStatus)
        .subscribe();
    }
  }

  getCampaignData(): CreateOrEditCampaignDto {
    const campaignDto = CreateOrEditCampaignDto.fromJS({
      id: this.campaign.id,
      generalData: CampaignGeneralDto.fromJS(this.campaign.generalData),
      listOfXTabRecords: [],
      listOfMultidimensionalRecords: [],
      campaignOutputDto: EditCampaignsOutputDto.fromJS({
        ...this.campaign.getOutputData,
        companyId: this.campaign.campaignOutputDto.companyId,
        layoutId: +this.campaign.campaignOutputDto.layout,
        layout: this.campaign.campaignOutputDto.layout,
        shipTo: this.selectedOutputShipTo,
      }),
      billingData: CampaignBillingDto.fromJS(this.campaign.billingData),
      maxPerData: GetCampaignMaxPerForViewDto.fromJS({
        campaignId: this.campaign.id,
        getSegmentLevelMaxPerData: [],
        getCampaignLevelMaxPerData: CampaignLevelMaxPerDto.fromJS({
          cMaximumQuantity:
            this.campaign.maxPerData.getCampaignLevelMaxPerData
              .cMaximumQuantity,
          cMinimumQuantity:
            this.campaign.maxPerData.getCampaignLevelMaxPerData
              .cMinimumQuantity,
          cMaxPerFieldOrderLevel:
            this.campaign.maxPerData.getCampaignLevelMaxPerData
              .cMaxPerFieldOrderLevel,
        }),
      }),
      decoyData: GetDecoyForViewDto.fromJS({
        listOfDecoys: [],
        isDecoyKeyMethod: this.campaign.decoyData.isDecoyKeyMethod,
        decoyKey: this.campaign.decoyData.decoyKey,
        decoyKey1: this.campaign.decoyData.decoyKey1,
      }),
    });
    if (campaignDto.generalData.cChannelType != "T") {
      campaignDto.billingData.sanNumber = "";
    }
    campaignDto.campaignOutputDto.shipTo = this.selectedOutputShipTo;
    const layoutDescription = this.getLayoutDescription();
    if (layoutDescription) {
      campaignDto.campaignOutputDto.layoutDescription = layoutDescription
        .split(":")[1]
        .trim();
    }
    if (
      !campaignDto.generalData.broker ||
      !campaignDto.generalData.broker.value
    ) {
      campaignDto.generalData.broker = DropdownOutputDto.fromJS({
        label: this.fastCountConfig["broker"]["label"],
        value: this.fastCountConfig["broker"]["value"],
      });
      campaignDto.generalData.brokerDescription =
        this.fastCountConfig["broker"]["label"];
    }
    return campaignDto;
  }

  close(saving: boolean = false): void {
    this.activeModal.close({ saving });
  }

  downloadAttachment(code: string) {
    this._orderServiceProxy
      .downloadFile(this.campaignId, code, this.databaseId)
      .subscribe((result) => {
        this._fileDownloadService.downloadDocumentAttachment(result);
      });
  }

  uploadFile(
    event,
    fileUploadControl: FileUpload,
    record: CampaignAttachmentDto
  ): void {
    this.isLoading = true;
    if (event.files[0].size > this.campaign.documentFileSize * 1024 * 1024) {
      this.message.error(
        this.l(
          "File_Document_Warn_SizeLimit",
          this.campaign.documentFileSize.toString()
        )
      );
      fileUploadControl.clear();
      return;
    }

    const uploadUrl =
      AppConsts.remoteServiceBaseUrl +
      "/File/UploadAttachmentFile?campaignId=" +
      this.campaignId;

    const formData: FormData = new FormData();
    formData.append("file", event.files[0], event.files[0].name);
    formData.append("code", record.code);

    this._httpClient.post<any>(uploadUrl, formData).subscribe((response) => {
      if (response.success) {
        record.cFileName = response.result.cFileName;
        record.realFileName = response.result.realFileName;
        this.notify.success("Upload Successful");
        this._orderServiceProxy
          .uploadFile(
            record.cFileName,
            record.realFileName,
            record.code,
            record.id,
            this.campaignId
          )
          .subscribe(() => {
            this._orderServiceProxy
              .fetchAttachmentsData(this.campaignId)
              .pipe(finalize(() => (this.isLoading = false)))
              .subscribe(
                (result) => (this.campaign.documentsData = result),
                () => {
                  this.notify.error("Upload Unsuccessful");
                }
              );
          });
      } else if (response.error != null) {
        this.notify.error("Upload Unsuccessful");
      }
    });
  }

  onUploadError(): void {
    this.notify.error("Upload Unsuccessful");
  }

  deleteOrderAttachment(id: number) {
    if (id > 0) {
      this.message.confirm(this.l(""), (isConfirmed) => {
        if (isConfirmed) {
          this.isLoading = true;
          this._orderServiceProxy.deleteCampaignAttachment(id).subscribe(() => {
            this._orderServiceProxy
              .fetchAttachmentsData(this.campaignId)
              .pipe(finalize(() => (this.isLoading = false)))
              .subscribe((result) => (this.campaign.documentsData = result));
          });
        }
      });
    }
  }

  getLayoutDataField() {
    this._exportLayoutServiceProxy
      .getExportLayoutAddField(
        this.selectedTableValue,
        this.campaignId,
        true,
        0,
        0,
        0
      )
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe((result) => {
        this.exportLayoutAddFields = [];
        for (const fieldName in this.fastCountConfig["selection-fields"]) {
          const value = this.fastCountConfig["selection-fields"][fieldName];
          if (result.some(item => item.value === value.field)) {
            const option = result.find(item => item.value === value.field);
            option.label = value["fc-description"];
            this.exportLayoutAddFields.push(option);
          }
        }
        this.isEditLayout = true;
      });
  }

}
