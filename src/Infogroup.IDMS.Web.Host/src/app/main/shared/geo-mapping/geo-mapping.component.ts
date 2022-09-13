import { AfterViewInit, Component, Injector, Input } from "@angular/core";
import {
  NgbActiveModal,
  NgbModal,
  NgbModalRef,
} from "@ng-bootstrap/ng-bootstrap";
import { AppComponentBase } from "@shared/common/app-component-base";
import {
  AdvanceSelectionFields,
  AdvanceSelectionScreen,
  BuildTableLayoutsServiceProxy,
  SaveGeoRadiusDto,
  SegmentSelectionDto,
  SegmentSelectionsServiceProxy,
} from "@shared/service-proxies/service-proxies";
import { MapLoaderService } from "./map.loader";
import { finalize } from "rxjs/operators";
import { NgForm } from "@angular/forms";
import { MapConstants } from "@shared/costants/map-constants";
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: "app-geo-mapping",
  templateUrl: "./geo-mapping.component.html",
  styleUrls: ["./geo-mapping.component.css"],
})
export class GeoMappingComponent
  extends AppComponentBase
  implements AfterViewInit {
  @Input() segmentId: number;
  @Input() campaignId: number;
  @Input() buildId: number;
  @Input() databaseId: number;
  @Input() pageType: string;
  @Input() popupData: any;
  map: google.maps.Map<HTMLElement>;
  drawingManager: google.maps.drawing.DrawingManager;
  drawingMode: google.maps.drawing.OverlayType;
  shapeDrawn: any;
  radius: string | null;
  name: string;
  saving: boolean;
  latitude?: number;
  longitude?: number;
  marker: google.maps.Marker;
  fields: AdvanceSelectionFields;
  isDisabled: boolean;
  selectedTab: number = 2;

  constructor(
    injector: Injector,
    private activeModal: NgbActiveModal,
    private _segmentSelectionProxy: SegmentSelectionsServiceProxy,
    private _buildTableLayoutsServiceProxy: BuildTableLayoutsServiceProxy,
    public activatedRoute: ActivatedRoute,
  ) {
    super(injector);
    this.radius = "";
    if (window.location.href.includes("?")) {
      this.activatedRoute.queryParams.subscribe((params) => {
        this.isDisabled = params["Edit"] == 0;
      });
    }
  }

  ngOnInit(): void {
    this.primengTableHelper.showLoadingIndicator();
    this._buildTableLayoutsServiceProxy
      .getFieldDetails(
        this.databaseId,
        this.buildId,
        AdvanceSelectionScreen.GeoMapping
      )
      .pipe(
        finalize(() => {
          this.primengTableHelper.hideLoadingIndicator();
        })
      )
      .subscribe((result) => {
        if (!result || !result.geoMapping) {
          this.message.error(this.l("FieldNotConfigured1", "Geo Mapping"));
        }
        this.fields = result;
      });
    if (this.pageType === "FastCount" && !this.popupData) {
      this.name = MapConstants.DEFAULT_MAP_NAME;
    }
  }

  ngAfterViewInit() {
    MapLoaderService.load().then(() => {
      this.setupMapEvents();
    });
  }

  setupMapEvents() {
    this.map = new google.maps.Map(document.getElementById("map"), {
      center: {
        lat: MapConstants.DEFAULT_US_LAT,
        lng: MapConstants.DEFAULT_US_LNG,
      },
      zoom: 3,
      scrollwheel: true,
      zoomControl: true,
      zoomControlOptions: {
        position: google.maps.ControlPosition.LEFT_BOTTOM,
      },
      streetViewControl: false,
      fullscreenControlOptions: {
        position: google.maps.ControlPosition.LEFT_TOP
      },
      mapTypeControl: false,
    });

    // #region Autocomplete search
    const input = document.getElementById("mSearch") as HTMLInputElement;
    const options: google.maps.places.AutocompleteOptions = {
      fields: ["geometry"],
      strictBounds: true,
      componentRestrictions: {
        country: MapConstants.DEFAULT_COUNTRY,
      },
    };

    const autocomplete = new google.maps.places.Autocomplete(input, options);
    autocomplete.bindTo("bounds", this.map);

    autocomplete.addListener("place_changed", () => {
      this.onAutoComplete(autocomplete);
    });
    // #endregion

    // #region Drawing Manager for polygon and circle
    this.drawingManager = new google.maps.drawing.DrawingManager({
      drawingControl: false,
      drawingControlOptions: {
        position: google.maps.ControlPosition.TOP_CENTER,
        drawingModes: [
          google.maps.drawing.OverlayType.POLYGON,
          google.maps.drawing.OverlayType.CIRCLE,
        ],
      },
      polygonOptions: {
        draggable: true,
        editable: true,
      },
      circleOptions: {
        draggable: true,
        editable: true,
      },
      map: this.map,
    });

    if (this.popupData != null) {
      this.drawingMode = null;
      let shape;
      let mapType;
      const parsedData = JSON.parse(`${this.popupData}`.toLowerCase());
      const data: string = parsedData.value;
      this.name = parsedData.name;
      switch (parsedData.type) {
        case google.maps.drawing.OverlayType.CIRCLE:
          this.latitude = Number(data.split("|")[0]);
          this.longitude = Number(data.split("|")[1].split(":")[0]);
          const radius = data.split("|")[1].split(":")[1];
          this.radius = `${radius}`;
          shape = this.drawCircleOnMap(Number(radius));
          mapType = google.maps.drawing.OverlayType.CIRCLE;
          this.selectedTab = 2;
          break;
        case google.maps.drawing.OverlayType.POLYGON:
          const paths = data.split("|");
          shape = new google.maps.Polygon({
            map: this.map,
            draggable: true,
            editable: true,
            paths: paths.map(
              (path) =>
                new google.maps.LatLng(
                  Number(path.split(":")[0]),
                  -Number(path.split(":")[1])
                )
            ),
          });
          mapType = google.maps.drawing.OverlayType.POLYGON;
          this.selectedTab = 1;
          break;
      }
      this.shapeDrawn = {
        overlay: shape,
        type: mapType,
      };
    } else {
      this.drawingMode = google.maps.drawing.OverlayType.CIRCLE;
    }
    this.drawingManager.setDrawingMode(this.drawingMode);

    google.maps.event.addListener(
      this.drawingManager,
      "overlaycomplete",
      (event: any) => {
        if (this.shapeDrawn != null) this.shapeDrawn.overlay.setMap(null);
        this.shapeDrawn = event;
        if (event.type == "circle") {
          this.updateValues();
          this.addCircleShapeEvents(this.shapeDrawn.overlay);
        }
        this.drawingManager.setDrawingMode(null);
      }
    );
    // #endregion

    google.maps.event.addListener(
      this.drawingManager,
      "drawingmode_changed",
      () => {
        this.drawingMode = this.drawingManager.getDrawingMode();
        if (!this.drawingMode) return;
        if (
          this.shapeDrawn != null &&
          this.shapeDrawn.type != this.drawingMode
        ) {
          this.shapeDrawn.overlay.setMap(null);
          this.shapeDrawn = null;
          this.radius = null;
        }
        this.addMarker();
      }
    );
  }

  onAutoComplete(autocomplete: google.maps.places.Autocomplete): void {
    const place = autocomplete.getPlace();
    if (!place.geometry || !place.geometry.location) {
      this.message.error(`No details available for input: '${place.name}'`);
      return;
    }

    // If the place has a geometry, then present it on a map.
    if (place.geometry.viewport) {
      this.map.fitBounds(place.geometry.viewport);
    } else {
      this.map.setCenter(place.geometry.location);
      this.map.setZoom(17);
    }

    this.latitude = Math.abs(place.geometry.location.lat());
    this.longitude = Math.abs(place.geometry.location.lng());
    this.addMarker();
  }

  save(): void {
    const data = this.getMappingValue();
    if (this.pageType === "FastCount") {
      this.activeModal.close({
        isSave: true,
        popupData: JSON.stringify(data),
      });
      return;
    }
    const selection: SegmentSelectionDto = SegmentSelectionDto.fromJS({
      segmentId: this.segmentId,
      cTableName: this.fields.geoMapping.cTableName,
      cQuestionFieldName: this.fields.geoMapping.cQuestionFieldName,
      cDescriptions: this.name,
      cValues: JSON.stringify(data),
    });
    const segmentSelectionSaveDto: SaveGeoRadiusDto = SaveGeoRadiusDto.fromJS({
      campaignId: this.campaignId,
      databaseId: this.databaseId,
      selection,
    });
    this.saving = true;
    this._segmentSelectionProxy
      .saveGeoMappingSelection(segmentSelectionSaveDto)
      .pipe(
        finalize(() => {
          this.primengTableHelper.hideLoadingIndicator();
          this.saving = false;
        })
      )
      .subscribe(() => {
        this.notify.info(this.l("SavedSuccessfully"));
        this.close(true);
      });
  }

  getMappingValue() {
    let value: string = "";
    switch (this.shapeDrawn.type) {
      case google.maps.drawing.OverlayType.CIRCLE:
        const circle: google.maps.Circle = this.shapeDrawn.overlay;
        const circleValues = {
          lat: Math.abs(circle.getCenter().lat()).toFixed(5),
          lng: Math.abs(circle.getCenter().lng()).toFixed(5),
          radius: MapConstants.toMiles(circle.getRadius()), //convert meters to miles
        };
        value = `${circleValues.lat}|${circleValues.lng}:${circleValues.radius}`;
        break;

      case google.maps.drawing.OverlayType.POLYGON:
        const paths: string[] = [];
        (this.shapeDrawn.overlay as google.maps.Polygon)
          .getPath()
          .forEach((elem: google.maps.LatLng) =>
            paths.push(
              `${Math.abs(elem.lat()).toFixed(5)}:${Math.abs(
                elem.lng()
              ).toFixed(5)}`
            )
          );
        if (paths[0] !== paths[paths.length - 1]) paths.push(paths[0]);
        value = paths.join("|");
        break;

      default:
        value = "";
    }
    return {
      value,
      type: this.shapeDrawn.type,
      name: this.name
    };
  }

  close(isSave: boolean = false): void {
    this.activeModal.close({ isSave });
  }

  setRadius(value: number) {
    if (!value) return;
    if (
      this.shapeDrawn != null &&
      this.shapeDrawn.type === google.maps.drawing.OverlayType.CIRCLE
    ) {
      (this.shapeDrawn.overlay as google.maps.Circle).setRadius(
        +MapConstants.toMeters(value)
      );
    } else {
      this.drawCircleOnMap(value);
    }
  }

  drawCircleOnMap(value: number) {
    let center: google.maps.LatLngLiteral;
    if (this.latitude && this.longitude) {
      center = {
        lat: Number(this.latitude),
        lng: -Number(this.longitude),
      };
    } else {
      center = {
        lat: Number(MapConstants.DEFAULT_US_LAT),
        lng: Number(MapConstants.DEFAULT_US_LNG),
      };
      this.latitude = Number(MapConstants.DEFAULT_US_LAT);
      this.longitude = -Number(MapConstants.DEFAULT_US_LNG);
    }
    const circle = new google.maps.Circle({
      center,
      draggable: true,
      editable: true,
      radius: +MapConstants.toMeters(value),
      map: this.map,
    });
    this.map.setCenter(center);
    this.shapeDrawn = {
      overlay: circle,
      type: google.maps.drawing.OverlayType.CIRCLE,
    };
    this.drawingManager.setDrawingMode(null);
    this.addCircleShapeEvents(circle);
    return circle;
  }

  addCircleShapeEvents(circle: google.maps.Circle): void {
    circle.addListener("radius_changed", () => {
      this.updateValues();
    });
    circle.addListener("center_changed", () => {
      this.updateValues();
    });
  }

  updateValues(): void {
    const circle: google.maps.Circle = this.shapeDrawn.overlay;
    this.radius = MapConstants.toMiles(circle.getRadius());
    this.latitude = Math.abs(circle.getCenter().lat());
    this.longitude = Math.abs(circle.getCenter().lng());
  }

  addMarker(): void {
    if (!this.latitude || !this.longitude) return;
    const center: google.maps.LatLngLiteral = {
      lat: Number(this.latitude),
      lng: -Number(this.longitude),
    };
    if (this.marker) {
      this.marker.setPosition(center);
    } else {
      this.marker = new google.maps.Marker({
        map: this.map,
        position: center,
      });
    }
    this.map.setCenter(center);
  }

  clearMarker(coordinatesForm: NgForm): void {
    if (this.marker) {
      this.marker.setMap(null);
      this.marker = null;
    }
    this.latitude = null;
    this.longitude = null;
    coordinatesForm.resetForm();
  }

  changeTab(tabIndex: number) {
    this.selectedTab = tabIndex;
    let drawingMode = null;
    switch (tabIndex) {
      case 1: drawingMode = google.maps.drawing.OverlayType.POLYGON; break;
      case 2: drawingMode = google.maps.drawing.OverlayType.CIRCLE; break;
    }
    this.drawingMode = drawingMode;
    this.drawingManager.setDrawingMode(drawingMode)
  }
}
