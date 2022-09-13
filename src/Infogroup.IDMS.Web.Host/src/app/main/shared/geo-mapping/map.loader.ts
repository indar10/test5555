import { AppConsts } from "@shared/AppConsts";

declare global {
  interface Window {
    __onGoogleLoaded: any;
  }
}

export class MapLoaderService {
  private static promise: Promise<any>;
  public static load(): Promise<any> {
    const mapsUrl = `https://maps.googleapis.com/maps/api/js?libraries=geometry,drawing,places&key=${AppConsts.googleMapsApiKey}&callback=__onGoogleLoaded`;

    // First time 'load' is called?
    if (!this.promise) {
      // Make promise to load
      this.promise = new Promise((resolve) => {
        this.loadScript(mapsUrl);
        // Set callback for when google maps is loaded.
        window["__onGoogleLoaded"] = ($event: any) => {
          resolve("google maps api loaded");
        };
      });
    }
    // Always return promise. When 'load' is called many times, the promise is already resolved.
    return this.promise;
  }

  //this function will work cross-browser for loading scripts asynchronously
  static loadScript(src: any, callback?: any): void {
    var s: any, r: any, t: any;
    r = false;
    s = document.createElement("script");
    s.type = "text/javascript";
    s.src = src;
    s.onload = s.onreadystatechange = function () {
      if (!r && (!this.readyState || this.readyState == "complete")) {
        r = true;
        if (typeof callback === "function") callback();
      }
    };
    t = document.getElementsByTagName("script")[0];
    t.parentNode.insertBefore(s, t);
  }
}
