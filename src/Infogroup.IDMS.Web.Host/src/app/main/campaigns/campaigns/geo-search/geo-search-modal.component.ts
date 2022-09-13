import { Component, Injector, Input, OnInit } from "@angular/core";
import { AppComponentBase } from "@shared/common/app-component-base";
import { NgbActiveModal } from "@ng-bootstrap/ng-bootstrap";
import {
  SegmentSelectionDto,
  SegmentSelectionsServiceProxy,
  AddressDetailDto,
  BuildTableLayoutsServiceProxy,
  AdvanceSelectionFields,
  SaveGeoRadiusDto,
  AdvanceSelectionScreen
} from "@shared/service-proxies/service-proxies";
import { finalize } from "rxjs/operators";

@Component({
  selector: "geo-search",
  styleUrls: ["./geo-search-modal.component.css"],
  templateUrl: "./geo-search-modal.component.html"
})
export class GeoSearchModalComponent extends AppComponentBase
  implements OnInit {
  saving: boolean = false;
  @Input() segmentId: number;
  @Input() campaignId: number;
  @Input() buildId: number;
  @Input() databaseId: number;
  filterText: string = "";
  radiusText: string = "";
  fields: AdvanceSelectionFields;
  addresses: AddressDetailDto[] = [];
  selectedAddress: AddressDetailDto;
  radiusErrorMsg: string;

  constructor(
    injector: Injector,
    private _buildTableLayoutsServiceProxy: BuildTableLayoutsServiceProxy,
    private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
    private activeModal: NgbActiveModal
  ) {
    super(injector);
  }

  ngOnInit(): void {
    this.radiusErrorMsg = this.l("RadiusErrorMsg");
    let screenType = AdvanceSelectionScreen;
    this._buildTableLayoutsServiceProxy
      .getFieldDetails(this.databaseId, this.buildId, screenType.GeoRadius)
      .subscribe(result => {
        if (!result || !result.geoRadius) {
          this.message.error(this.l("FieldNotConfigured1", "Geo Radius"));
        }
        this.fields = result;
      });
  }
  getAddress(): void {
    this.primengTableHelper.showLoadingIndicator();
    this.addresses = [];
    let geoRadiusField = this.fields.geoRadius;
    this._segmentSelectionProxy
      .getAddressDetails(
        this.databaseId,
        this.segmentId,
        geoRadiusField.cTableName,
        this.filterText
      )
      .pipe(finalize(() => this.primengTableHelper.hideLoadingIndicator()))
      .subscribe(result => {
        this.addresses = result;
        if (this.addresses.length > 0) this.selectedAddress = this.addresses[0];
      });
  }

  save(): void {
    let selection: SegmentSelectionDto = SegmentSelectionDto.fromJS({
      segmentId: this.segmentId,
      cDescriptions: `${this.selectedAddress.description.trim()} Radius=${
        this.radiusText
      }`
    });
    switch (this.selectedAddress.matchLevel) {
      case 1:
      case 2:
        selection.cTableName = this.fields.geoRadius.cTableName;
        selection.cQuestionFieldName = this.fields.geoRadius.cQuestionFieldName;
        selection.cValues = `${this.selectedAddress.latitude}|${this.selectedAddress.longitude}:${this.radiusText}`;
        break;
      case 3:
        if (!this.fields.zipRadius) {
          this.message.error(this.l("FieldNotConfigured1", "Zip Radius"));
          return;
        }
        selection.cTableName = this.fields.zipRadius.cTableName;
        selection.cQuestionFieldName = this.fields.zipRadius.cQuestionFieldName;
        selection.cQuestionDescription = this.fields.zipRadius.cQuestionDescription;
        selection.cValues = `${this.selectedAddress.zipCode}-${this.radiusText}`;
        break;
      default:
        this.message.error(this.l("InvalidMatchLevelError"));
        return;
    }
    let segmentSelectionSaveDto: SaveGeoRadiusDto = SaveGeoRadiusDto.fromJS({
      campaignId: this.campaignId,
      databaseId: this.databaseId,
      matchLevel: this.selectedAddress.matchLevel,
      selection
    });
    this.saving = true;
    this._segmentSelectionProxy
      .saveGeoRadiusSelection(segmentSelectionSaveDto)
      .pipe(
        finalize(() => {
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l("SavedSuccessfully"));
        this.activeModal.close({ isSave: true });
      });
  }

  close(): void {
    this.activeModal.close({ isSave: false });
  }
}
