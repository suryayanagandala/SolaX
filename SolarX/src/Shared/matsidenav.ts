import { Injectable } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';

@Injectable({
  providedIn: 'root',
})
export class MatsidenavService {
  private sidenav!:MatSidenav;

  setSideNav(sidenav:MatSidenav){
    this.sidenav=sidenav;
  }

  openSideNav(){
    if (this.sidenav) {
      this.sidenav.open();
    } else {
      console.warn('Sidenav not set yet');
    }
  }

  closeSideNav(){
    this.sidenav.close();
  }
}
